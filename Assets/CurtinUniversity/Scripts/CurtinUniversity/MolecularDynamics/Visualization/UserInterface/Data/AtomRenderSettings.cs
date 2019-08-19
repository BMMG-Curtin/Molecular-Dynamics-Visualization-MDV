using System;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class AtomRenderSettings {

        public string AtomName { get; set; }
        public bool Enabled { get; set; }
        public MolecularRepresentation Representation { get; set; }
        public bool CustomColour { get; set; }
        public Color32 AtomColour { get; set; }

        private Color defaultColour;

        public AtomRenderSettings(string atomName, Color defaultColour) {

            AtomName = atomName;
            this.defaultColour = defaultColour;
            SetDefaultOptions();
        }

        public void SetDefaultOptions() {

            Enabled = true;
            Representation = MolecularRepresentation.None;
            CustomColour = false;
            AtomColour = defaultColour;
        }

        public bool IsDefault() {

            if (Enabled == true &&
                Representation == MolecularRepresentation.None &&
                CustomColour == false) {

                return true;
            }

            return false;
        }
    
        public override string ToString() {

            return
                "AtomID: " + AtomName + "\n" +
                "Enabled: " + Enabled + "\n" +
                "Representation:" + Representation + "\n" +
                "CustomColour: " + CustomColour + "\n" +
                "AtomColour: " + AtomColour.ToString();
        }
    }
}
