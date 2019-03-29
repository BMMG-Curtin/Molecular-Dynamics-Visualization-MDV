using System;
using System.Collections.Generic;

using CurtinUniversity.MolecularDynamics.Model.Definitions;

namespace CurtinUniversity.MolecularDynamics.Model.Model {

    public class Chain {

        // Returns StandardResidue.None if the residue type for every residue is not a standard residue or they are not all the same
        // Note, this method iterates through all residues on every call (no caching) as the chain could have been updated between calls
        public StandardResidue ResidueType {

            get {
                bool firstResidue = true;
                StandardResidue firstResidueType = StandardResidue.None;

                foreach (Residue residue in Residues) {

                    if (firstResidue) {
                        firstResidueType = residue.ResidueType;
                        if(firstResidueType == StandardResidue.None) {
                            return StandardResidue.None;
                        }
                        firstResidue = false;
                    }
                    else {
                        if (residue.ResidueType != firstResidueType) {
                            return StandardResidue.None;
                        }
                    }
                }

                return firstResidueType;
            }
        }

        public int Index { get; private set; }
        public string ID { get; private set; }

        public List<Residue> Residues { get; private set; }
        public List<Residue> MainChainResidues { get; private set; }
        public List<Atom> MainChainAtoms { get; private set; }

        public Chain(int index, string id) {

            Index = index;
            ID = id;

            Residues = new List<Residue>();
            MainChainResidues = new List<Residue>();
            MainChainAtoms = new List<Atom>();
        }

        public void AddResidue(int residueIndex, Residue residue) {

            Residues.Add(residue);

            if (residue.ResidueType == StandardResidue.AminoAcid && residue.AmineNitrogen != null && residue.AlphaCarbon != null && residue.CarbonylCarbon != null) {
                MainChainResidues.Add(residue);
                MainChainAtoms.Add(residue.AmineNitrogen);
                MainChainAtoms.Add(residue.AlphaCarbon);
                MainChainAtoms.Add(residue.CarbonylCarbon);
            }
        }

        public override string ToString() {

            string output = "Chain [" + ID + "]\n";

            foreach (Residue residue in Residues) {
                output += "\t" + residue + "\n";
            }

            return output;
        }
    }
}
