using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueButton : MonoBehaviour {

        public Color32 EnabledColour;
        public Color32 HighlightedColour;
        public Color32 DisabledColour;

        [HideInInspector]
        public string ResidueName;

        private SceneManager sceneManager;
        private ColorBlock buttonColours;
        private ResidueDisplayOptions residueOptions;

        SaveResidueButtonOptionsDelegate SaveOptionsCallback;
        OpenResidueDisplayOptionsDelegate OpenDisplayOptionsCallback;

        void Start() {

            sceneManager = SceneManager.instance;
            buttonColours = GetComponent<Button>().colors;
        }

        public void SetCallback(ResidueDisplayOptions options, SaveResidueButtonOptionsDelegate saveOptionsCallback, OpenResidueDisplayOptionsDelegate openDisplayCallback) {

            SaveOptionsCallback = saveOptionsCallback;
            OpenDisplayOptionsCallback = openDisplayCallback;
            UpdateResidueOptions(options);
        }

        public void UpdateResidueOptions(ResidueDisplayOptions options) {

            this.residueOptions = options;
            UpdateButtonColors();
        }

        public void ResidueClick() {

            if (sceneManager.InputManager.ShiftPressed) {

                if (OpenDisplayOptionsCallback != null) {
                    OpenDisplayOptionsCallback(ResidueName);
                }
            }
            else {

                if (SaveOptionsCallback != null) {

                    residueOptions.Enabled = !residueOptions.Enabled;
                    UpdateButtonColors();
                }
            }

            SaveOptionsCallback(residueOptions, false);
        }

        private void UpdateButtonColors() {

            if (residueOptions.Enabled) {
                if (residueOptions.IsDefault()) {
                    SetButtonColours(EnabledColour);
                }
                else {
                    SetButtonColours(HighlightedColour);
                }
            }
            else {
                SetButtonColours(DisabledColour);
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
