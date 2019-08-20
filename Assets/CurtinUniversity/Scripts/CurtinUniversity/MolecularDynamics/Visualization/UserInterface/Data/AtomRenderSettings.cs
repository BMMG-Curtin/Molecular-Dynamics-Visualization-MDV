using System;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class AtomRenderSettings {

        public string AtomName { get; set; }
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

            Representation = MolecularRepresentation.None;
            CustomColour = false;
            AtomColour = defaultColour;
        }

        public bool IsDefault() {

            if (Representation == MolecularRepresentation.None &&
                CustomColour == false &&
                AtomColour.Equals(defaultColour)) {

                return true;
            }

            return false;
        }

        public AtomRenderSettings Clone() {

            AtomRenderSettings clone = new AtomRenderSettings(AtomName, defaultColour);
            clone.Representation = Representation;
            clone.CustomColour = CustomColour;
            clone.AtomColour = AtomColour;

            return clone;
        }

        public override string ToString() {

            return
                "AtomID: " + AtomName + "\n" +
                "Representation:" + Representation + "\n" +
                "CustomColour: " + CustomColour + "\n" +
                "AtomColour: " + AtomColour.ToString();
        }
    }
}
