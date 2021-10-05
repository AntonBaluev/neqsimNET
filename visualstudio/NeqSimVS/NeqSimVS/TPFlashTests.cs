using System;
using System.Linq;
using neqsim.thermo.component;

// Improting neqsim modules
using neqsim.thermo.system;
using neqsim.thermodynamicOperations;
using NUnit.Framework;

namespace NeqSimVS
{
    [TestFixture]
    public class TPFlashTests
    {
        [Test]              
        public void Test()
        {
            var fluid = GetTestFluid();

            var components = fluid.getPhase(0).getComponents()
                .Where(item => item != null).ToList();

            foreach (var component in components)
            {
                PrintComponent(component);
            }

            ThermodynamicOperations ops = new ThermodynamicOperations(fluid);
            ops.TPflash();            

            PrintFluid(fluid);
        }

        public SystemInterface GetTestFluid() {
            SystemInterface fluid = new SystemSrkEos(298.15, 10.0);
            fluid.addComponent("water", 0.01);
            fluid.addComponent("nitrogen", 0.02);
            fluid.addComponent("CO2", 0.03);
            fluid.addComponent("H2S", 0.01);
            fluid.addComponent("methane", 0.80);
            fluid.addComponent("ethane", 0.04);
            fluid.addComponent("propane", 0.03);
            fluid.addComponent("i-butane", 0.02);
            fluid.addComponent("n-butane", 0.01);
            fluid.addComponent("i-pentane", 0.01);
            fluid.addComponent("n-pentane", 0.01);
            fluid.addComponent("n-hexane", 0.01);
            fluid.createDatabase(true);
            fluid.setMixingRule(2);
            fluid.useVolumeCorrection(true);
            fluid.setMultiPhaseCheck(true);
            fluid.setTemperature(45.0, "C");
            fluid.setPressure(10.0, "bara");
            return fluid;
        }

        public void PrintFluid(SystemInterface fluid)
        {
            var table = fluid.createTable("test table");
            var table2 = new string[table.Length,table[0].Length];
            foreach (var row in table)
            {
                foreach (var cell in row)
                {
                    var toPrint = cell == "" ? "---" : cell;
                    Console.Write($"{toPrint.PadRight(20)}");
                }
                Console.WriteLine();
            }
        }

        public void PrintComponent(ComponentInterface component)
        {
            var methods = component.GetType().GetMethods();

            var filteredMethods = methods
                .Where(m => m.GetParameters().Count() == 0)
                .Where(m => m.Name.StartsWith("get", StringComparison.CurrentCultureIgnoreCase) == true)
                .OrderBy(m => m.Name);

            Console.WriteLine($"{component.getName()}**********");
            foreach (var method in filteredMethods)
            {
                Console.WriteLine($"{method.Name}:  {method.Invoke(component, null)}");
            }
            Console.WriteLine("**********");
            Console.WriteLine();
        }

        public double[] GetFluidProperties(SystemInterface fluid)
        {
            int k = 0;
            double[] fluidProperties = new double[67];
            fluidProperties[k++] = (double)fluid.getNumberOfPhases(); // Mix Number of Phases
            fluidProperties[k++] = fluid.getPressure("Pa"); // Mix Pressure [Pa]
            fluidProperties[k++] = fluid.getTemperature("K"); // Mix Temperature [K]
            fluidProperties[k++] = fluid.getMoleFractionsSum() * 100; // Mix Mole Percent
            fluidProperties[k++] = 100.0; // Mix Weight Percent
            fluidProperties[k++] = 1.0 / fluid.getDensity("mol/m3"); // Mix Molar Volume [m3/mol]
            fluidProperties[k++] = 100.0; // Mix Volume Percent
            fluidProperties[k++] = fluid.getDensity("kg/m3"); // Mix Density [kg/m3]
            fluidProperties[k++] = fluid.getZ(); // Mix Z Factor
            fluidProperties[k++] = fluid.getMolarMass() * 1000; // Mix Molecular Weight [g/mol]
            // fluidProperties[k++] = fluid.getEnthalpy()/fluid.getNumberOfMoles(); //
            // Mix Enthalpy [J/mol]
            fluidProperties[k++] = fluid.getEnthalpy("J/mol");
            // fluidProperties[k++] = fluid.getEntropy()/fluid.getNumberOfMoles(); // Mix
            // Entropy [J/molK]
            fluidProperties[k++] = fluid.getEntropy("J/molK");
            fluidProperties[k++] = fluid.getCp("J/molK"); // Mix Heat Capacity-Cp [J/molK]
            fluidProperties[k++] = fluid.getCv("J/molK");// Mix Heat Capacity-Cv [J/molK]
            // fluidProperties[k++] = fluid.Cp()/fluid.getCv();// Mix Kappa (Cp/Cv)
            fluidProperties[k++] = fluid.getGamma();// Mix Kappa (Cp/Cv)
            fluidProperties[k++] = Double.NaN; // Mix JT Coefficient [K/Pa]
            fluidProperties[k++] = Double.NaN; // Mix Velocity of Sound [m/s]
            fluidProperties[k++] = fluid.getViscosity("kg/msec"); // Mix Viscosity [Pa s] or [kg/(m*s)]
            fluidProperties[k++] = fluid.getThermalConductivity("W/mK"); // Mix Thermal Conductivity [W/mK]

            String[] phaseName = { "gas", "oil", "aqueous" };
            for (int j = 0; j < 3; j++)
            {
                if (fluid.hasPhaseType(phaseName[j]))
                {
                    int phaseNumber = fluid.getPhaseNumberOfPhase(phaseName[j]);
                    fluidProperties[k++] = fluid.getMoleFraction(phaseNumber) * 100; // Phase Mole Percent
                    fluidProperties[k++] = fluid.getWtFraction(phaseNumber) * 100; // Phase Weight Percent
                    fluidProperties[k++] = 1.0 / fluid.getPhase(phaseNumber).getDensity("mol/m3"); // Phase Molar
                                                                                                   // Volume
                                                                                                   // [m3/mol]
                    fluidProperties[k++] = fluid.getCorrectedVolumeFraction(phaseNumber) * 100;// Phase Volume
                                                                                               // Percent
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getDensity("kg/m3"); // Phase Density [kg/m3]
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getZ(); // Phase Z Factor
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getMolarMass() * 1000; // Phase Molecular
                                                                                              // Weight [g/mol]
                                                                                              // fluidProperties[k++] = fluid.getPhase(phaseNumber).getEnthalpy() /
                                                                                              // fluid.getPhase(phaseNumber).getNumberOfMolesInPhase(); // Phase Enthalpy
                                                                                              // [J/mol]
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getEnthalpy("J/mol"); // Phase Enthalpy
                                                                                             // [J/mol]
                                                                                             // fluidProperties[k++] = fluid.getPhase(phaseNumber).getEntropy() /
                                                                                             // fluid.getPhase(phaseNumber).getNumberOfMolesInPhase(); // Phase Entropy
                                                                                             // [J/molK]
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getEntropy("J/molK"); // Phase Entropy
                                                                                             // [J/molK]
                                                                                             // fluidProperties[k++] = fluid.getPhase(phaseNumber).getCp() /
                                                                                             // fluid.getPhase(phaseNumber).getNumberOfMolesInPhase(); // Phase Heat
                                                                                             // Capacity-Cp [J/molK]
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getCp("J/molK"); // Phase Heat Capacity-Cp
                                                                                        // [J/molK]
                                                                                        // fluidProperties[k++] = fluid.getPhase(phaseNumber).getCv() /
                                                                                        // fluid.getPhase(phaseNumber).getNumberOfMolesInPhase(); // Phase Heat
                                                                                        // Capacity-Cv [J/molK]
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getCv("J/molK"); // Phase Heat Capacity-Cv
                                                                                        // [J/molK]
                                                                                        // fluidProperties[k++] = fluid.getPhase(phaseNumber).getCp() /
                                                                                        // fluid.getPhase(phaseNumber).getCv(); // Phase Kappa (Cp/Cv)
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getKappa(); // Phase Kappa (Cp/Cv)
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getJouleThomsonCoefficient() / 1e5; // Phase
                                                                                                           // JT
                                                                                                           // Coefficient
                                                                                                           // [K/Pa]
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getSoundSpeed(); // Phase Velocity of Sound
                                                                                        // [m/s]
                                                                                        // fluidProperties[k++] =
                                                                                        // fluid.getPhase(phaseNumber).getPhysicalProperties().getViscosity();// Phase
                                                                                        // Viscosity [Pa s]
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getViscosity("kg/msec");// Phase Viscosity [Pa
                                                                                               // s] or [kg/msec]
                                                                                               // fluidProperties[k++] =
                                                                                               // fluid.getPhase(phaseNumber).getPhysicalProperties().getConductivity(); //
                                                                                               // Phase Thermal Conductivity [W/mK]
                    fluidProperties[k++] = fluid.getPhase(phaseNumber).getConductivity("W/mK"); // Phase Thermal
                                                                                                // Conductivity
                                                                                                // [W/mK]
                                                                                                // Phase Surface Tension(N/m) ** NOT USED
                }
                else
                {
                    fluidProperties[k++] = Double.NaN; // Phase Mole Percent
                    fluidProperties[k++] = Double.NaN; // Phase Weight Percent
                    fluidProperties[k++] = Double.NaN; // Phase Molar Volume [m3/mol]
                    fluidProperties[k++] = Double.NaN; // Phase Volume Percent
                    fluidProperties[k++] = Double.NaN; // Phase Density [kg/m3]
                    fluidProperties[k++] = Double.NaN; // Phase Z Factor
                    fluidProperties[k++] = Double.NaN; // Phase Molecular Weight [g/mol]
                    fluidProperties[k++] = Double.NaN; // Phase Enthalpy [J/mol]
                    fluidProperties[k++] = Double.NaN; // Phase Entropy [J/molK]
                    fluidProperties[k++] = Double.NaN; // Phase Heat Capacity-Cp [J/molK]
                    fluidProperties[k++] = Double.NaN; // Phase Heat Capacity-Cv [J/molK]
                    fluidProperties[k++] = Double.NaN; // Phase Kappa (Cp/Cv)
                    fluidProperties[k++] = Double.NaN; // Phase JT Coefficient K/Pa]
                    fluidProperties[k++] = Double.NaN; // Phase Velocity of Sound [m/s]
                    fluidProperties[k++] = Double.NaN;// Phase Viscosity [Pa s]
                    fluidProperties[k++] = Double.NaN; // Phase Thermal Conductivity [W/mK]

                }
            }

            return fluidProperties;
        }
    }
}
