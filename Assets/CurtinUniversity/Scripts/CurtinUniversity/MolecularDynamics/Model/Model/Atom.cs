using System;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class Atom {

        public int Index { get; private set; } // index number in the model. Should be unique
        public int ID { get; private set; } // number from the structure file. Doesn't always match the index in the file. Shouldnt be matched to trajectory atom index
        public string Name { get; private set; }
        public Element Element { get; private set; }
        public float AtomicRadius { get; private set; } // in nanometres
        public float VDWRadius { get; private set; } // in nanometres
        public Vector3 Position { get; private set; } // in nanometres

        // parent information stored here so parent residue can be found easily without re-searching residue dictionary
        public int ResidueIndex { get; set; }
        public int ResidueID { get; set; }
        public string ResidueName { get; set; }
        public StandardResidue ResidueType { get; set; }
        public int ChainIndex { get; set; }
        public string ChainID { get; set; }

        public Atom(int index, int id, string name, Element element, Vector3 position) {

            Index = index;
            ID = id;
            Name = name;
            Element = element;
            Position = position;

            float atomicRadius;
            if (!ChemicalElement.AtomicRadius.TryGetValue(Element, out atomicRadius)) {
                ChemicalElement.AtomicRadius.TryGetValue(Element.Other, out atomicRadius);
            }
            AtomicRadius = atomicRadius;

            float vdwRadius;
            if (!ChemicalElement.VDWRadius.TryGetValue(Element, out vdwRadius)) {
                ChemicalElement.VDWRadius.TryGetValue(Element.Other, out vdwRadius);
            }
            VDWRadius = vdwRadius;
        }

        public override string ToString() {
            return "Atom [" + ID + "][" + Name + "][" + Element + "][" + Position + "]";
        }

        public string ToStringExtended() {

            return
                "Atom:" +
                "Index: [" + Index + "]\n" +
                "ID: [" + ID + "]\n" +
                "Name: [" + Name + "]\n" +
                "Element: [" + Element + "]\n" +
                "Position: [" + Position + "]\n" +
                "ResidueIndex: [" + ResidueIndex + "]\n" +
                "ResidueID: [" + ResidueID + "]\n" +
                "ResidueName: [" + ResidueName + "]\n" +
                "ResidueType: [" + ResidueType + "]\n" +
                "ChainIndex: [" + ChainIndex + "]\n" +
                "ChainID: [" + ChainID + "]\n";
        }
    }
}
