using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueAtomNameButton : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI atomNameText;

        [SerializeField]
        private TextMeshProUGUI representationText;

        [SerializeField]
        private Image backgroundImage;

        public AtomRenderSettings AtomSettings { get; private set; }
        private ResidueCustomColourSelect colourSelectPanel;

        private List<string> representations;
        private int selectedRepresentationIndex;

        private float buttonAlpha;

        private void Awake() {

            representations = Enum.GetNames(typeof(MolecularRepresentation)).ToList();
            buttonAlpha = backgroundImage.color.a;
        }

        public void Initialise(AtomRenderSettings settings, ResidueCustomColourSelect colourSelectPanel) {

            this.AtomSettings = settings;
            this.colourSelectPanel = colourSelectPanel;

            updateAtomSettingsDisplay();
        }

        private void updateAtomSettingsDisplay() {

            atomNameText.text = AtomSettings.AtomName.ToString();

            for(int i=0; i < representations.Count; i++) {
                if(AtomSettings.Representation.ToString() == representations[i]) {
                    selectedRepresentationIndex = i;
                    break;
                }
            }

            representationText.text = AtomSettings.Representation.ToString();

            backgroundImage.color = AtomSettings.AtomColour;
            resetAlpha();
        }

        public void OnNameButtonClick() {

            colourSelectPanel.gameObject.SetActive(true);
            colourSelectPanel.Initialise(setAtomColor);
        }

        private void setAtomColor(Color? color) {

            if (color == null) {
                AtomSettings.SetDefaultColour();
            }
            else {

                AtomSettings.CustomColour = true;
                AtomSettings.AtomColour = (Color)color;
            }

            updateAtomSettingsDisplay();
        }

        private void resetAlpha() {

            Color imageColor = backgroundImage.color;
            imageColor.a = buttonAlpha;
            backgroundImage.color = imageColor;
        }

        public void OnRepresentationButtonClick() {

            selectedRepresentationIndex++;

            if (selectedRepresentationIndex >= representations.Count) {
                selectedRepresentationIndex = 0;
            }

            if(Enum.TryParse(representations[selectedRepresentationIndex], out MolecularRepresentation rep)) {

                AtomSettings.Representation = rep;
                representationText.text = representations[selectedRepresentationIndex];
            }
        }
    }
}
