using System;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class SecondaryStructureInfomation {

        public SecondaryStructureType type;
        public float phi;
        public float psi;

        public override string ToString() {
            return "Type: " + type + ", PHI: " + phi + ", PSI: " + psi;
        }
    }

    public class SecondaryStructure {

        private Dictionary<int, SecondaryStructureInfomation> secondaryStructure;

        public SecondaryStructure() {
            secondaryStructure = new Dictionary<int, SecondaryStructureInfomation>();
        }

        public void AddStructureInformation(int residueIndex, SecondaryStructureInfomation structure) {

            if (secondaryStructure.ContainsKey(residueIndex)) {
                secondaryStructure[residueIndex] = structure;
            }
            else {
                secondaryStructure.Add(residueIndex, structure);
            }
        }

        public SecondaryStructureInfomation GetStructureInformation(int residueIndex) {

            try {
                return secondaryStructure[residueIndex];
            }
            catch (KeyNotFoundException) {
                return null;
            }
        }

        public override string ToString() {

            string output = "";

            foreach(KeyValuePair<int, SecondaryStructureInfomation> residue in secondaryStructure) {
                output += "Residue[" + residue.Key + "] has structure [" + residue.Value.ToString() + "]\n";
            }

            return output;
        }

        public static SecondaryStructure CreateFromPrimaryStructure(PrimaryStructure primaryStructure, string strideExePath, string tmpFilePath) {

            string tmpFileName = tmpFilePath + @"tempStructure.pdb";
            PDBStructureCreator pdbCreator = new PDBStructureCreator(primaryStructure,null);
            //pdbCreator.CreatePDBFile(tmpFileName, true, true, true);
            pdbCreator.CreatePDBFile(tmpFileName);

            SecondaryStructure secondaryStructure = null;

            try {
                StrideAnalysis stride = new StrideAnalysis(strideExePath);
                secondaryStructure = stride.GetSecondaryStructure(tmpFileName);
            }
            finally {
                FileUtil.DeleteFile(tmpFileName);
            }

            return secondaryStructure;
        }
    }
}
