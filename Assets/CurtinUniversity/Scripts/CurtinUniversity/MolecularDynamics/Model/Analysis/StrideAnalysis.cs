using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using CurtinUniversity.MolecularDynamics.Model.Definitions;
using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.Model.Analysis {

    // Note that the analysis matches residue structure to the residue indexes supplied in the input file. 
    // Please be sure that the residue indexes in the file match the residue indexes in the primary structure you are matching against. 
    // This will always be the case if the input file was created by the PBDStructureCreator class

    public class StrideAnalysis {

        private string exePath;

        public StrideAnalysis(string exePath) {
            this.exePath = exePath;
        }

        public SecondaryStructure GetSecondaryStructure(string inputFilePath) {

            string strideOutput = processFile(inputFilePath);
            return parseStrideOutput(strideOutput);
        }

        private string processFile(string inputFilePath) { 
            
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.ErrorDialog = false;

            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardError = true;

            processStartInfo.Arguments = "\"" + inputFilePath + "\"";
            processStartInfo.FileName = exePath;

            string output = "";

            try {
                using (Process process = Process.Start(processStartInfo)) {

                    // Console.WriteLine("Running process: " + process.ToString());
                    // Console.WriteLine("Args: " + processStartInfo.Arguments);
                    output = process.StandardOutput.ReadToEnd();
                    if (output == null || output.Trim() == "") {
                        string error = process.StandardError.ReadToEnd();
                        if (error != null && error.Trim() != "") {
                            throw new StrideException("Stride Error: " + error);
                            // throw new StrideException("Stride processing error.");
                        }
                        else {
                            //throw new StrideException("Stride executed but returned no output or error");
                            throw new StrideException("Stride processing error.");
                        }
                    }
                }
            }
            catch(Win32Exception) {
                throw new StrideException("Stride executable not found.");
            }

            Console.WriteLine("Found output:\n" + output);

            return output;
        }

        private SecondaryStructure parseStrideOutput(string strideOutput) {

            SecondaryStructure ss = new SecondaryStructure();

            using (StringReader reader = new StringReader(strideOutput)) {

                string line;
                int residueIndex = 0;
                while ((line = reader.ReadLine()) != null) {

                    // Console.WriteLine("Processing line: [" + line + "]");

                    if (!line.StartsWith("ASG ")) {
                        continue;
                    }

                    if (int.TryParse(line.Substring(10, 5), out residueIndex)) {

                        string structureCode = line.Substring(24, 1);

                        SecondaryStructureType structure;
                        switch (structureCode) {
                            case "G":
                                structure = SecondaryStructureType.ThreeHelix;
                                break;
                            case "H":
                                structure = SecondaryStructureType.AlphaHelix;
                                break;
                            case "I":
                                structure = SecondaryStructureType.FiveHelix;
                                break;
                            case "T":
                                structure = SecondaryStructureType.Turn;
                                break;
                            case "E":
                                structure = SecondaryStructureType.BetaSheet;
                                break;
                            case "B":
                                structure = SecondaryStructureType.BetaBridge;
                                break;
                            case "S":
                                structure = SecondaryStructureType.Bend;
                                break;
                            default:
                                structure = SecondaryStructureType.Coil;
                                break;
                        }

                        string phi = line.Substring(42, 7);
                        string psi = line.Substring(52, 7);

                        SecondaryStructureInfomation information = new SecondaryStructureInfomation();
                        information.type = structure;
                        information.phi = float.Parse(phi);
                        information.psi = float.Parse(psi);

                        ss.AddStructureInformation(residueIndex, information);

                        Console.WriteLine("Added structure information for residue index: " + residueIndex);
                    }
                }
            }

            return ss;
        }
    }
}
