using System;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Model {

    public static class ResidueHelper {

        public static StandardResidue FindResidueType(string name) {

            if (StandardAminoAcid(name)) {
                return StandardResidue.AminoAcid;
            }

            if (StandardDNA(name)) {
                return StandardResidue.DNA;
            }

            if (StandardRNA(name)) {
                return StandardResidue.RNA;
            }

            return StandardResidue.None;
        }

        public static bool StandardAminoAcid(string name) {

            if (name != null && name.Trim() != "") {
                foreach (string AminoAcid in Enum.GetNames(typeof(StandardAminoAcid))) {
                    if (name.Equals(AminoAcid)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool StandardDNA(string name) {

            if (name != null && name.Trim() != "") {
                foreach (string AminoAcid in Enum.GetNames(typeof(StandardDeoxyribonucleotide))) {
                    if (name.Equals(AminoAcid)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool StandardRNA(string name) {

            if (name != null && name.Trim() != "") {
                foreach (string AminoAcid in Enum.GetNames(typeof(StandardRibonucleotide))) {
                    if (name.Equals(AminoAcid)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
