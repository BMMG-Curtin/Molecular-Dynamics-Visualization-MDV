using System;
using System.Diagnostics;

namespace CurtinUniversity.MolecularDynamics.Model {

    class BadPDBStructureTest {

        private static string strideExePath = @"D:\Stride\stride_WIN32.exe";
        private static string tmpFilePath = @"D:\tmp\";

        private static string filepath = @"D:\Molecule Examples\PDBTestFiles20161203\";
        private static string structureFile = @"5h4y.pdb";

        public static void Run() {

            Console.WriteLine("Parsing PDB File");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            PrimaryStructure primaryStructure = PDBStructureParser.GetPrimaryStructure(filepath + structureFile);
            stopWatch.Stop();

            Console.WriteLine("Structure Parsing Complete [" + stopWatch.ElapsedMilliseconds + " ms]");

            stopWatch.Reset();
            stopWatch.Start();
            SecondaryStructure.CreateFromPrimaryStructure(primaryStructure, strideExePath, tmpFilePath);
            Console.WriteLine("Main Secondary Structure Parsing Complete [" + stopWatch.ElapsedMilliseconds + " ms]");


            foreach (Chain chain in primaryStructure.Chains()) {

                foreach (Atom atom in chain.MainChainAtoms) {
                    // if no frame number use the base structure coordinates.
                    if (atom == null) {
                        Console.WriteLine("Main chain atom is null");
                    }
                    else {
                        Console.WriteLine("Atom found: " + atom.ToString());
                    }
                }


                //Console.WriteLine("\n---------------------\n");
                //Console.WriteLine("Chain ID: " + chain.ID);
                //Console.WriteLine("Chain Residue Count: " + chain.Residues.Count);
                //if (chain.ResidueType == MolecularDynamics.Definitions.StandardResidue.AminoAcid) {
                //    foreach (Residue residue in chain.Residues) {
                //        Console.WriteLine("Residue index: " + residue.Index + ", ResidueID: " + residue.ID + ", Residue Name: " + residue.Name);
                //        SecondaryStructureInfomation information = secondaryStructure.GetStructureInformation(residue.Index);
                //        if (information == null) {
                //            Console.WriteLine("information null: " + residue.Index);
                //        }
                //    }
                //}
                //else {
                //    Console.WriteLine("Not an amino acid residue");
                //}
            }
        }
    }
}
