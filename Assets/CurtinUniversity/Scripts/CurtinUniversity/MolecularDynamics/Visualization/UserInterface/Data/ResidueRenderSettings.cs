using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueRenderSettings {

        public int ResidueID { get; set; }
        public bool ColourAtoms { get; set; }
        public MolecularRepresentation? AtomRepresentation { get; set; }
        public bool ColourBonds{ get; set; }
        public bool LargeBonds { get; set; }
        public bool ColourSecondaryStructure{ get; set; }
        public Color ResidueColour;

        public Dictionary<string, AtomRenderSettings> AtomSettings;

        private Color defaultColour;

        public ResidueRenderSettings(int residueID, Color defaultColour) {

            ResidueID = residueID;
            this.defaultColour = defaultColour;
            SetDefaultOptions();
        }

        public void SetDefaultOptions() {

            ColourAtoms = false;
            AtomRepresentation = null;
            ColourBonds = false;
            LargeBonds = false;
            ColourSecondaryStructure = false;
            ResidueColour = defaultColour;
            AtomSettings = new Dictionary<string, AtomRenderSettings>();
        }

        public bool IsDefault() {

            if (ColourAtoms == false && 
                AtomRepresentation == null && 
                ColourBonds == false &&
                LargeBonds == false &&
                ColourSecondaryStructure == false &&
                ResidueColour == defaultColour && 
                AtomSettings != null && 
                AtomSettings.Count == 0) {

                return true;
            }

            return false;
        }

        public ResidueRenderSettings Clone() {

            ResidueRenderSettings clone = new ResidueRenderSettings(ResidueID, defaultColour);
            clone.ColourAtoms = ColourAtoms;
            clone.AtomRepresentation = AtomRepresentation;
            clone.ColourBonds = ColourBonds;
            clone.LargeBonds = LargeBonds;
            clone.ColourSecondaryStructure = ColourSecondaryStructure;
            clone.ResidueColour = ResidueColour;

            clone.AtomSettings = new Dictionary<string, AtomRenderSettings>();
            foreach(KeyValuePair<string, AtomRenderSettings> atom in AtomSettings) {
                clone.AtomSettings.Add(atom.Key, atom.Value.Clone());
            }

            return clone;
        }

        public override string ToString() {

            string output =
                "ResidueID: " + ResidueID + "\n" +
                "ColourAtoms: " + ColourAtoms + "\n" +
                "AtomRepresentation: " + AtomRepresentation + "\n" +
                "ColourBonds: " + ColourBonds + "\n" +
                "LargeBonds: " + LargeBonds + "\n" +
                "ColourSecondaryStructure: " + ColourSecondaryStructure + "\n" +
                "ResidueColour: " + ResidueColour + "\n" +
                "AtomDisplayOptions: " + "\n";

            foreach (AtomRenderSettings atomOptions in AtomSettings.Values) {
                output += atomOptions.ToString() + "\n";
            }

            return output;
        }
    }
}
