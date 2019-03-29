using System;
using System.Diagnostics;

using CurtinUniversity.MolecularDynamics.Model.Definitions;
using CurtinUniversity.MolecularDynamics.Model.FileParser;
using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.Model.Examples {

    public static class MainChainFinder {

        // private static string inputFilePath = @"D:\Molecule Examples\PDB Test Files 20161121\";
        private static string inputFilePath = @"D:\Molecule Examples\";

        // private static string inputFileFileName = @"5tql_BB.pdb";
        // private static string inputFileFileName = @"protein+water.gro";
        private static string inputFileFileName = @"membrane_protein.gro";
        // private static string inputFileFileName = @"lipoprotein_DS.pdb";
        //private static string inputFileFileName = @"lipoprotein.pdb";

        //private static string inputFilePath = @"D:\Molecule Examples\PDB Example Files\";
        //private static string inputFileFileName = @"Zika_Virus_5ire.pdb";

        public static void Run() {

            Console.WriteLine("Parsing PBD File");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            // PrimaryStructure model = PDBStructureParser.GetStructure(inputFilePath + inputFileFileName);
            PrimaryStructure model = GROStructureParser.GetStructure(inputFilePath + inputFileFileName);
            stopWatch.Stop();

            Console.WriteLine("Structure Parsing Complete [" + stopWatch.ElapsedMilliseconds + " ms]");
            Console.WriteLine("Atom Count: " + model.Atoms().Count);

            // Console.WriteLine(model.ToString());

            foreach (Chain chain in model.Chains()) {
                Console.WriteLine("Chain " + chain.ID + ": ");//  + residue.ToString());
                if (chain.ResidueType == StandardResidue.AminoAcid) {
                    foreach (Residue residue in chain.MainChainResidues) {
                        Console.WriteLine(residue);
                    }
                    //foreach (Atom mainChainAtom in chain.Value.MainChainAtoms) {
                    //    Console.WriteLine(mainChainAtom);
                    //}
                }
                else {
                    Console.WriteLine("Chain " + chain.ID + " is not a protein chain");
                    //foreach (KeyValuePair<int, Residue> residue in chain.Value.Residues) {
                    //    Console.WriteLine(residue.Value.ResidueType.ToString());
                    //}
                }
            }
        }
    }
}
