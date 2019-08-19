using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueNameButton : MonoBehaviour {

        [SerializeField]
        private Text buttonNameText;

        [SerializeField]
        public Color32 EnabledColour;
        public Color32 HighlightedColour;
        public Color32 DisabledColour;

        private ColorBlock buttonColours;

        private string residueName;
        bool residueEnabled = true;
        bool residueIDsModified = true;
        private ToggleResidueNameDelegate toggleResidueCallback;
        private OpenResidueIDsDelegate openResidueIDsCallback;

        void Start() {
            buttonColours = GetComponent<Button>().colors;
        }

        public void Initialise(string residueName, bool residueEnabled, bool residueIDsModified, ToggleResidueNameDelegate saveEnabledCallback, OpenResidueIDsDelegate openDisplayCallback) {

            this.residueName = residueName;
            this.toggleResidueCallback = saveEnabledCallback;
            this.openResidueIDsCallback = openDisplayCallback;
            this.residueEnabled = residueEnabled;
            this.residueIDsModified = residueIDsModified;

            buttonNameText.text = residueName.Trim();
            updateButtonColors();
        }

        public void SetResidueEnabled(bool enabled) {

            residueEnabled = enabled;
            updateButtonColors();
        }

        public void SetResidueModified(bool modified) {

            residueIDsModified = modified;
            updateButtonColors();
        }

        public void ResidueClick() {

            if (InputManager.Instance.ShiftPressed) {
                openResidueIDsCallback?.Invoke(residueName);
            }
            else {

                if (toggleResidueCallback != null) {

                    residueEnabled = !residueEnabled;
                    updateButtonColors();
                    toggleResidueCallback(residueName);
                }
            }
        }

        private void updateButtonColors() {

            if (residueEnabled) {
                if (residueIDsModified) {
                    setButtonColours(HighlightedColour);
                }
                else {
                    setButtonColours(EnabledColour);
                }
            }
            else {
                setButtonColours(DisabledColour);
            }
        }

        private void setButtonColours(Color32 color) {

            buttonColours.normalColor = color;
            buttonColours.highlightedColor = color;
            buttonColours.pressedColor = color;
            buttonColours.colorMultiplier = 1f;
            GetComponent<Button>().colors = buttonColours;
        }
    }
}
