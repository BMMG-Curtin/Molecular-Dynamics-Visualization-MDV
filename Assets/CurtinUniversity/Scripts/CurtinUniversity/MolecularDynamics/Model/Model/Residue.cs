using System;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class Residue {

        public int Index { get; private set; } // index number in the model. Should be unique
        public int ID { get; private set; } // number from the structure file. Not unique across model
        public string Name { get; private set; }

        public StandardResidue ResidueType { get; private set; }

        public SortedDictionary<int, Atom> Atoms { get; set; }
        public Atom AmineNitrogen { get; set; }
        public Atom AlphaCarbon { get; set; }
        public Atom CarbonylCarbon { get; set; }
        public Atom CarbonylOxygen { get; set; }

        public Residue(int index, int id, string name) {

            Index = index;
            ID = id;
            Name = name.Trim().ToUpper();

            ResidueType = ResidueHelper.FindResidueType(name);

            Atoms = new SortedDictionary<int, Atom>();

            if (ResidueType == StandardResidue.AminoAcid) {

                AmineNitrogen = null;
                AlphaCarbon = null;
                CarbonylCarbon = null;
                CarbonylOxygen = null;
            }
        }

        public override string ToString() {

            string output = "Residue [" + ID + "][" + Name + "]\n";

            List<int> keyList = new List<int>(Atoms.Keys);
            keyList.Sort();

            foreach (int key in keyList) {
                output += "\t" + Atoms[key] + "\n";
            }

            return output;
        }
    }
}
