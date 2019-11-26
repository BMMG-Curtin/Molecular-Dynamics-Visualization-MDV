using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// ElementButton is used in ElementSettingsPanel to show and hide elements in the selected molecule
    /// </summary>
    public class ElementButton : MonoBehaviour {

        [HideInInspector]
        public string ElementName;

        private bool elementEnabled = true;
        private Color enabledColour;
        private Color disabledColour;
        private ColorBlock colors;

        private SetElementDelegate elementCallback;

        private void Awake() {

            colors = GetComponent<Button>().colors;

            // save existing colors for use
            enabledColour = colors.normalColor;
            disabledColour = colors.disabledColor;

            // set all button colors to enabled color 
            colors.normalColor = enabledColour;
            colors.highlightedColor = enabledColour;
            colors.pressedColor = enabledColour;
        }

        public void Initialise(bool elementEnabled, SetElementDelegate callback) {

            elementCallback = callback;
            this.elementEnabled = elementEnabled;
            updateButtonColors();
        }

        public void ElementClick() {

            if (elementCallback != null) {

                elementEnabled = !elementEnabled;

                elementCallback(ElementName, elementEnabled);
                updateButtonColors();
            }
        }

        private void updateButtonColors() {

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
