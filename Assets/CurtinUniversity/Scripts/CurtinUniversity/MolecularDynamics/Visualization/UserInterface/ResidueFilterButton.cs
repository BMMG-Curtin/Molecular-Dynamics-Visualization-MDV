using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueFilterButton : MonoBehaviour {

        public Color32 EnabledColour;
        public Color32 HighlightedColour;

        private ColorBlock buttonColours;

        void Start() {
            buttonColours = GetComponent<Button>().colors;
        }

        public void Highlight(bool enableHighlight) {

            if (enableHighlight) {
                SetButtonColours(HighlightedColour);
            }
            else {
                SetButtonColours(EnabledColour);
            }
        }

        private void SetButtonColours(Color32 color) {

            buttonColours.normalColor = color;
            buttonColours.highlightedColor = color;
            buttonColours.pressedColor = color;
            buttonColours.colorMultiplier = 1f;
            GetComponent<Button>().colors = buttonColours;
        }
    }
}
