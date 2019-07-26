using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueDisplayOptionsPanel : MonoBehaviour {

        public ResiduesSettingsPanel ResiduesPanel;
        public GameObject ColorSelectPanel;
        public GameObject ColorSelectPanelButtonList;
        public GameObject ColorSelectButtonPrefab;

        public Text PanelTitle;
        public Toggle EnabledToggle;
        public Toggle LargeBondsToggle;
        public Toggle ColourAtomsToggle;
        public Toggle ColourBondsToggle;
        public Toggle ColourSecondaryStructureToggle;
        public Button CustomColourButton;

        private ResidueDisplayOptions displayOptions;

        public void Start() {

            addColourSelectButton(VisualizationP3.Settings.ResidueColour1);
            addColourSelectButton(VisualizationP3.Settings.ResidueColour2);
            addColourSelectButton(VisualizationP3.Settings.ResidueColour3);
            addColourSelectButton(VisualizationP3.Settings.ResidueColour4);
            addColourSelectButton(VisualizationP3.Settings.ResidueColour5);
            addColourSelectButton(VisualizationP3.Settings.ResidueColour6);
            addColourSelectButton(VisualizationP3.Settings.ResidueColour7);
            addColourSelectButton(VisualizationP3.Settings.ResidueColour8);
            addColourSelectButton(VisualizationP3.Settings.ResidueColour9);
            addColourSelectButton(VisualizationP3.Settings.ResidueColour10);
        }

        public void Initialise(ResidueDisplayOptions displayOptions) {

            this.displayOptions = displayOptions;
            LoadSettings();
        }

        public void LoadSettings() {

            if (displayOptions.ResidueName == ResiduesPanel.UpdateAllResiduesKey) {
                PanelTitle.text = "Updating All Residues";
            }
            else {
                PanelTitle.text = displayOptions.ResidueName + " Residue Display Options";
            }

            EnabledToggle.isOn = displayOptions.Enabled;
            LargeBondsToggle.isOn = displayOptions.LargeBonds;
            ColourAtomsToggle.isOn = displayOptions.ColourAtoms;
            ColourBondsToggle.isOn = displayOptions.ColourBonds;
            ColourSecondaryStructureToggle.isOn = displayOptions.ColourSecondaryStructure;
            setCustomColourButtonColour(displayOptions.CustomColour);
        }

        public void SaveSettings() {

            displayOptions.Enabled = EnabledToggle.isOn;
            displayOptions.LargeBonds = LargeBondsToggle.isOn;
            displayOptions.ColourAtoms = ColourAtomsToggle.isOn;
            displayOptions.ColourBonds = ColourBondsToggle.isOn;
            displayOptions.ColourSecondaryStructure = ColourSecondaryStructureToggle.isOn;

            ResiduesPanel.SaveResidueDisplayOptions(displayOptions, true);
        }

        public void RestoreDefaults() {

            displayOptions.SetDefaultOptions();
            LoadSettings();
            ResiduesPanel.SaveResidueDisplayOptions(displayOptions, true);
        }

        public void OpenColourSelectPanel() {
            ColorSelectPanel.SetActive(true);
        }

        private void SaveAndCloseColourSelectPanel(Color colour) {

            setCustomColourButtonColour(colour);
            ColorSelectPanel.SetActive(false);
            displayOptions.CustomColour = colour;
            ResiduesPanel.SaveResidueDisplayOptions(displayOptions, true);
        }

        private void setCustomColourButtonColour(Color colour) {

            ColorBlock buttonColours = CustomColourButton.colors;
            buttonColours.normalColor = colour;
            buttonColours.highlightedColor = colour;
            buttonColours.pressedColor = colour;
            buttonColours.colorMultiplier = 1f;
            CustomColourButton.colors = buttonColours;
        }

        private void addColourSelectButton(Color colour) {

            GameObject button = (GameObject)Instantiate(ColorSelectButtonPrefab, Vector3.zero, Quaternion.identity);

            ResidueCustomColourButton buttonScript = button.GetComponent<ResidueCustomColourButton>();
            buttonScript.SetSetColorCallback(SaveAndCloseColourSelectPanel, colour);

            ColorBlock colors = button.GetComponent<Button>().colors;
            colors.normalColor = colour;
            colors.highlightedColor = colour;
            colors.pressedColor = colour;
            button.GetComponent<Button>().colors = colors;

            RectTransform rect = button.GetComponent<RectTransform>();
            rect.SetParent(ColorSelectPanelButtonList.GetComponent<RectTransform>(), false);
            rect.localPosition = new Vector3(0, 0, 0);
            rect.localRotation = Quaternion.Euler(0, 0, 0);
            rect.localScale = Vector3.one;
        }
    }
}
