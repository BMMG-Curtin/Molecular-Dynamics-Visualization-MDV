using System;

namespace CurtinUniversity.MolecularDynamics.Model {

    public static class PDBDumpTest {

        private static string inputFilePath = @"D:\Molecule Examples\PDBTestFiles20161203\";
        private static string inputFileName = @"4osd.pdb";
        private static string tmpFilePath = @"D:\tmp\";

        public static void Run() {

            Console.WriteLine("Parsing PDB File");
            PrimaryStructure primaryStructure = PDBStructureParser.GetPrimaryStructure(inputFilePath + inputFileName);

            // dump structure to PDB file. Can check this file against the original input file for equivalence
            string tmpFileName = tmpFilePath + @"tempStructure.pdb";
            PDBStructureCreator pdbCreator = new PDBStructureCreator(primaryStructure, null);
            pdbCreator.CreatePDBFile(tmpFileName, true, true);
            // FileUtil.DeleteFile(tmpFileName);
        }
    }
}
