using System;
using System.Diagnostics;

namespace CurtinUniversity.MolecularDynamics.Model {

    public static class StrideTest {

        private static string exePath = @"D:\Stride\stride_WIN32.exe";
        // private static string inputFilePath = @"D:\Molecule Examples\PDB Test Files 20161121\";
        // private static string inputFilePath = @"D:\Molecule Examples\PDB Test Files 20170121\";
        private static string inputFilePath = @"D:\Molecule Examples\Problem Files 20170127\";
        

        // private static string inputFileName = @"5tql_BB.pdb";
        // private static string inputFileName = @"5tql_BB_test.pdb";
        // private static string inputFileName = @"lipoprotein_DS_test.pdb";
        // private static string inputFileName = @"lipid-droplet_CG.pdb";
        private static string inputFileName = @"lipoprotein_original.pdb";

        public static void Run() {

            Console.WriteLine("Parsing PBD File");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            PrimaryStructure model = PDBStructureParser.GetPrimaryStructure(inputFilePath + inputFileName);
            stopWatch.Stop();

            Console.WriteLine("Structure Parsing Complete [" + stopWatch.ElapsedMilliseconds + " ms]");
            Console.WriteLine("Atom Count: " + model.Atoms().Count);

            Console.WriteLine("Running Stride Analysis");

            stopWatch = new Stopwatch();
            stopWatch.Start();

            StrideAnalysis stride = new StrideAnalysis(exePath);
            SecondaryStructure structure = stride.GetSecondaryStructure(inputFilePath + inputFileName);
            // Console.WriteLine("Secondary structure:\n\n" + structure);

            stopWatch.Stop();
            Console.WriteLine("Processing complete [" + stopWatch.ElapsedMilliseconds + " ms]");

            Console.WriteLine("End Testing Stride");

            foreach(Chain chain in model.Chains()) {
                Console.WriteLine("Chain ID: " + chain.ID);

                if (chain.ResidueType != StandardResidue.AminoAcid) {
                    Console.WriteLine("Not a protein chain");
                    continue;
                }

                foreach (Residue residue in chain.MainChainResidues) {


                    if(residue.CarbonylOxygen == null) {
                        Console.WriteLine("Residue ID: " + residue.ID + " has no oxygen");
                    }

                    SecondaryStructureInfomation structureInfo = structure.GetStructureInformation(residue.Index);

                    if(structureInfo == null) {
                        Console.WriteLine("Couldn't find structure info for residue index: " + residue.Index);
                    }
                    else {
                        Console.WriteLine("Residue [" + residue.ID + "][" + residue.Name + "] has structure [" + structureInfo.ToString() + "] and Alpha Carbon: " + residue.AlphaCarbon);
                    }
                }
            }

        }
    }
}

    
