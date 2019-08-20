using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueIDButton : MonoBehaviour {

        [SerializeField]
        private Text buttonIDText;

        [SerializeField]
        private Color32 EnabledColour;

        [SerializeField]
        private Color32 HighlightedColour;

        [SerializeField]
        private Color32 DisabledColour;

        private ColorBlock buttonColours;

        private int residueID;
        bool residueIDEnabled = false;
        bool residueIDCustomSettings = false;

        private ToggleResidueIDDelegate toggleResidueCallback;
        private OpenResidueCustomSettingsDelegate openCustomSettingsCallback;

        private void Awake() {
            buttonColours = GetComponent<Button>().colors;
        }

        public void Initialise(int residueID, bool residueIDEnabled, bool residueIDCustomSettings, ToggleResidueIDDelegate toggleResidueCallback, OpenResidueCustomSettingsDelegate openCustomSettingsCallback) {

            this.residueID = residueID;
            this.residueIDEnabled = residueIDEnabled;
            this.residueIDCustomSettings = residueIDCustomSettings;
            this.toggleResidueCallback = toggleResidueCallback;
            this.openCustomSettingsCallback = openCustomSettingsCallback;

            buttonIDText.text = residueID.ToString();
            updateButtonColors();
        }

        public void SetResidueEnabled(bool enabled) {

            residueIDEnabled = enabled;
            updateButtonColors();
        }

        public void SetResidueCustomSettings(bool customSettings) {

            residueIDCustomSettings = customSettings;
            updateButtonColors();
        }

        public void ResidueClick() {

            if (InputManager.Instance.ShiftPressed) {
                openCustomSettingsCallback?.Invoke(residueID);
            }
            else {

                if (toggleResidueCallback != null) {

                    residueIDEnabled = !residueIDEnabled;
                    updateButtonColors();
                    toggleResidueCallback(residueID);
                }
            }
        }

        private void updateButtonColors() {

            if (residueIDEnabled) {
                if (residueIDCustomSettings) {
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
