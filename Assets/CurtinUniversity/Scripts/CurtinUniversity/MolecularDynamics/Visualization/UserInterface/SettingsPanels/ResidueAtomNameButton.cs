using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void AtomSubButtonClickDelegate();

    public class ResidueAtomNameButton : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI atomNameText;

        [SerializeField]
        private TextMeshProUGUI representationText;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private ResidueAtomSubButton nameSubButton;

        [SerializeField]
        private ResidueAtomSubButton representationSubButton;

        public AtomRenderSettings AtomSettings { get; private set; }
        private ResidueCustomColourSelect colourSelectPanel;
        private AtomButtonClickDelegate onClick;

        private List<string> representations;
        private int selectedRepresentationIndex;

        private float buttonAlpha;

        private void Awake() {

            representations = Enum.GetNames(typeof(MolecularRepresentation)).ToList();
            buttonAlpha = backgroundImage.color.a;

            // we are using subbuttons to capture clicks via IPointerDownHandler to 
            // stop the OnClick component eventhandlers consuming the mouse wheel input
            nameSubButton.Initialise(nameButtonClick);
            representationSubButton.Initialise(representationButtonClick);
        }

        public void Initialise(AtomRenderSettings settings, ResidueCustomColourSelect colourSelectPanel, AtomButtonClickDelegate onClick) {

            this.AtomSettings = settings;
            this.colourSelectPanel = colourSelectPanel;
            this.onClick = onClick;

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

        private void nameButtonClick() {

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

            backgroundImage.color = AtomSettings.AtomColour;
            resetAlpha();

            onClick();
        }

        private void resetAlpha() {

            Color imageColor = backgroundImage.color;
            imageColor.a = buttonAlpha;
            backgroundImage.color = imageColor;
        }

        private void representationButtonClick() {

            selectedRepresentationIndex++;

            if (selectedRepresentationIndex >= representations.Count) {
                selectedRepresentationIndex = 0;
            }

            try {
                AtomSettings.Representation = (MolecularRepresentation)Enum.Parse(typeof(MolecularRepresentation), representations[selectedRepresentationIndex]);
                representationText.text = representations[selectedRepresentationIndex];
            }
            catch (Exception) {
                // ingore
            }

            onClick();
        }
    }
}
