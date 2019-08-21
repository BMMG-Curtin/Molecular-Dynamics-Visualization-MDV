using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueNameButton : MonoBehaviour {

        [SerializeField]
        private Text buttonNameText;

        [SerializeField]
        private Color32 EnabledColour;

        [SerializeField]
        private Color32 HighlightedColour;

        [SerializeField]
        private Color32 DisabledColour;

        private ColorBlock buttonColours;

        private string residueName;
        bool residueNameEnabled = true;
        bool residueIDsModified = true;

        private ToggleResidueNameDelegate toggleResidueCallback;
        private OpenResidueIDsDelegate openResidueIDsCallback;

        private void Awake() {
            buttonColours = GetComponent<Button>().colors;
        }

        public void Initialise(string residueName, bool residueEnabled, bool residueIDsModified, ToggleResidueNameDelegate toggleResidueCallback, OpenResidueIDsDelegate openDisplayCallback) {

            this.residueName = residueName;
            this.toggleResidueCallback = toggleResidueCallback;
            this.openResidueIDsCallback = openDisplayCallback;
            this.residueNameEnabled = residueEnabled;
            this.residueIDsModified = residueIDsModified;

            buttonNameText.text = residueName.Trim();
            updateButtonColors();
        }

        public void SetResidueEnabled(bool enabled) {

            residueNameEnabled = enabled;
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

                    residueNameEnabled = !residueNameEnabled;
                    updateButtonColors();
                    toggleResidueCallback(residueName);
                }
            }
        }

        private void updateButtonColors() {

            if (residueNameEnabled) {
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
