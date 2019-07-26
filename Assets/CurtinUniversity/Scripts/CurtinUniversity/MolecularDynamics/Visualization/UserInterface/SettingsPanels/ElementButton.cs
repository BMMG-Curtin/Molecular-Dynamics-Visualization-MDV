using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class ElementButton : MonoBehaviour {

        [HideInInspector]
        public string ElementName;

        private bool elementEnabled = true;
        private Color enabledColour;
        private Color disabledColour;
        private ColorBlock colors;

        SetElementDelegate elementCallback;

        void Start() {
            colors = GetComponent<Button>().colors;

            // save existing colors for use
            enabledColour = colors.normalColor;
            disabledColour = colors.disabledColor;

            // set all button colors to enabled color 
            colors.normalColor = enabledColour;
            colors.highlightedColor = enabledColour;
            colors.pressedColor = enabledColour;
        }

        public void SetCallback(SetElementDelegate callback) {
            elementCallback = callback;
        }

        public void ElementClick() {

            if (elementCallback != null) {
                elementEnabled = !elementEnabled;
                elementCallback(ElementName, elementEnabled);
                if (elementEnabled) {
                    colors.normalColor = enabledColour;
                    colors.highlightedColor = enabledColour;
                    colors.pressedColor = enabledColour;
                }
                else {
                    colors.normalColor = disabledColour;
                    colors.highlightedColor = disabledColour;
                    colors.pressedColor = disabledColour;
                }
                GetComponent<Button>().colors = colors;
            }
        }
    }
}
