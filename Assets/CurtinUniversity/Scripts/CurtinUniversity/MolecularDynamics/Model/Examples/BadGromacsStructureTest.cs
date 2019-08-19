using System;
using System.Diagnostics;

namespace CurtinUniversity.MolecularDynamics.Model {

    class BadGromacsStructureTest {

        private static string strideExePath = @"D:\Stride\stride_WIN32.exe";
        private static string tmpFilePath = @"D:\tmp\";

        // private static string filepath = @"D:\Molecule Examples\Problem Files 20170127\HIVE\";
        private static string filepath = @"D:\Molecule Examples\";

        private static string structureFile = @"bad.gro";

        public static void Run() {

            Console.WriteLine("Parsing GRO File");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            PrimaryStructure primaryStructure = GROStructureParser.GetStructure(filepath + structureFile);
            stopWatch.Stop();

            Console.WriteLine("Structure Parsing Complete [" + stopWatch.ElapsedMilliseconds + " ms]");

            stopWatch.Reset();
            stopWatch.Start();
            SecondaryStructure secondaryStructure = SecondaryStructure.CreateFromPrimaryStructure(primaryStructure, strideExePath, tmpFilePath);
            Console.WriteLine("Main Secondary Structure Parsing Complete [" + stopWatch.ElapsedMilliseconds + " ms]");


            foreach (Chain chain in primaryStructure.Chains()) {

                Console.WriteLine("\n---------------------\n");
                Console.WriteLine("Chain ID: " + chain.ID);
                Console.WriteLine("Chain Residue Count: " + chain.Residues.Count);
                foreach (Residue residue in chain.Residues) {
                    Console.WriteLine("Residue index: " + residue.Index + ", ResidueID: " + residue.ID + ", Residue Name: " + residue.Name);
                    SecondaryStructureInfomation information = secondaryStructure.GetStructureInformation(residue.Index);
                    if (information == null) {
                        Console.WriteLine("information null: " + residue.Index);
                    }
                }
            }


        }
    }
}
