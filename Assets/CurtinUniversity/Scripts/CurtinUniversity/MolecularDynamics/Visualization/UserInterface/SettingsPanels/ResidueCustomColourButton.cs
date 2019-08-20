using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueCustomColourButton : MonoBehaviour {

        private SetCustomColourButtonColourDelegate setColour;
        private Color colour;

        public void SetColorCallback(SetCustomColourButtonColourDelegate setColourCallback, Color callbackColour) {
            setColour = setColourCallback;
            colour = callbackColour;
        }

        public void ButtonClick() {
            setColour(colour);
        }
    }
}
