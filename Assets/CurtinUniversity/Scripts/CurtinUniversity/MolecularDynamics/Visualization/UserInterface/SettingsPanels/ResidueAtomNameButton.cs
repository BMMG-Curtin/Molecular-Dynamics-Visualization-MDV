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

        private AtomRenderSettings atomSettings;
        private ResidueCustomColourSelect colourSelectPanel;


        private List<string> representations;
        private int selectedRepresentationIndex;

        private float buttonAlpha;

        private void Awake() {

            representations = Enum.GetNames(typeof(MolecularRepresentation)).ToList();
            buttonAlpha = backgroundImage.color.a;
        }

        public void Initialise(AtomRenderSettings options, ResidueCustomColourSelect colourSelectPanel) {

            this.atomSettings = options;
            this.colourSelectPanel = colourSelectPanel;

            updateAtomOptions();
        }

        private void updateAtomOptions() {

            atomNameText.text = atomSettings.AtomName.ToString();

            for(int i=0; i < representations.Count; i++) {
                if(atomSettings.Representation.ToString() == representations[i]) {
                    selectedRepresentationIndex = i;
                    break;
                }
            }

            representationText.text = atomSettings.Representation.ToString();

            backgroundImage.color = atomSettings.AtomColour;
            resetAlpha();
        }

        public void OnNameButtonClick() {

            colourSelectPanel.gameObject.SetActive(true);
            colourSelectPanel.Initialise(setAtomColor);
        }

        private void setAtomColor(Color color) {

            atomSettings.AtomColour = color;

            color.a = buttonAlpha;
            backgroundImage.color = color;
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

                atomSettings.Representation = rep;
                representationText.text = representations[selectedRepresentationIndex];
            }
        }
    }
}
