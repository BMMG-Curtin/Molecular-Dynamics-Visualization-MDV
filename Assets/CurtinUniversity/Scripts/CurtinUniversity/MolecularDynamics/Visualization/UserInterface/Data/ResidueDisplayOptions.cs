using System;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueDisplayOptions {

        public string ResidueName { get; set; }
        public bool Enabled { get; set; }
        public bool LargeBonds { get; set; }
        public bool ColourAtoms { get; set; }
        public bool ColourBonds { get; set; }
        public Color32 CustomColour { get; set; }
        public bool ColourSecondaryStructure { get; set; }

        private Color defaultColour;

        public ResidueDisplayOptions(string residueName, Color defaultColour) {

            ResidueName = residueName;
            this.defaultColour = defaultColour;

            SetDefaultOptions();
        }

        public void SetDefaultOptions() {

            Enabled = true;
            CustomColour = defaultColour;
            LargeBonds = false;
            ColourAtoms = false;
            ColourBonds = false;
            ColourSecondaryStructure = false;
        }

        public bool IsDefault() {

            if (Enabled == true &&
                LargeBonds == false &&
                ColourAtoms == false &&
                ColourBonds == false &&
                ColourSecondaryStructure == false) {

                return true;
            }

            return false;
        }

        public void Clone(ResidueDisplayOptions newOptions) {

            ResidueName = newOptions.ResidueName;
            Enabled = newOptions.Enabled;
            CustomColour = newOptions.CustomColour;
            LargeBonds = newOptions.LargeBonds;
            ColourAtoms = newOptions.ColourAtoms;
            ColourBonds = newOptions.ColourBonds;
            ColourSecondaryStructure = newOptions.ColourSecondaryStructure;
        }

        public override string ToString() {

            return
                "ResidueName: " + ResidueName + "\n" +
                "Enabled: " + Enabled + "\n" +
                "CustomColour: " + CustomColour + "\n" +
                "LargeBonds: " + LargeBonds + "\n" +
                "ColourAtoms: " + ColourAtoms + "\n" +
                "ColourBonds: " + ColourBonds + "\n" +
                "ColourSecondaryStructure: " + ColourSecondaryStructure;
        }

        public override bool Equals(object obj) {

            if(obj.GetType() == typeof(ResidueDisplayOptions)) {
                return Equals((ResidueDisplayOptions)obj);
            }

            return false;
        }

        public bool Equals(ResidueDisplayOptions otherOptions) {

            return 
                otherOptions.ResidueName == ResidueName &&
                otherOptions.Enabled == Enabled &&
                otherOptions.LargeBonds == LargeBonds &&
                otherOptions.ColourAtoms == ColourAtoms &&
                otherOptions.ColourBonds == ColourBonds &&
                otherOptions.CustomColour.Equals(CustomColour) &&
                otherOptions.ColourSecondaryStructure == ColourSecondaryStructure;
        }
    }
}
