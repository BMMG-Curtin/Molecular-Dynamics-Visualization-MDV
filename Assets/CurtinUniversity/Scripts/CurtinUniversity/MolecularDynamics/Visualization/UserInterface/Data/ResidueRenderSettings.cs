using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueRenderSettings {

        public int ResidueID { get; set; }
        public bool LargeBonds { get; set; }
        public Color32? BondColour { get; set; }
        public Color32? SecondaryStructureColour { get; set; }
        public Dictionary<string, AtomRenderSettings> AtomDisplayOptions;

        private Color defaultColour;

        public ResidueRenderSettings(int residueID, Color defaultColour) {

            ResidueID = residueID;
            this.defaultColour = defaultColour;
            SetDefaultOptions();
        }

        public void SetDefaultOptions() {

            LargeBonds = false;
            BondColour = null;
            SecondaryStructureColour = null;
            AtomDisplayOptions = new Dictionary<string, AtomRenderSettings>();
        }

        public bool IsDefault() {

            if (LargeBonds == false &&
                BondColour == null &&
                SecondaryStructureColour == null &&
                AtomDisplayOptions != null && 
                AtomDisplayOptions.Count == 0) {

                return true;
            }

            return false;
        }

        public override string ToString() {

            string output =
                "ResidueID: " + ResidueID + "\n" +
                "LargeBonds: " + LargeBonds + "\n" +
                "BondColour: " + BondColour + "\n" +
                "SecondaryStructureColour: " + BondColour + "\n" +
                "AtomDisplayOptions: " + "\n";

            foreach (AtomRenderSettings atomOptions in AtomDisplayOptions.Values) {
                output += atomOptions.ToString() + "\n";
            }

            return output;
        }
    }
}
