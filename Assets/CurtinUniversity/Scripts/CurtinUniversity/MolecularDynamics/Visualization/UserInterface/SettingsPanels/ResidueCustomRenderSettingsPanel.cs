using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueCustomRenderSettingsPanel : MonoBehaviour {

        [SerializeField]
        private GameObject customRenderSettingsPanel;

        public Text PanelTitle;
        public Toggle LargeBondsToggle;
        public Toggle BondsColourToggle;
        public Button BondColourButton;
        public Toggle SecondaryStructureColourToggle;
        public Button SecondaryStrucureColourButton;

        public GameObject AtomNameListContentPanel;
        public GameObject AtomNameButtonPrefab;

        private ResidueRenderSettings renderSettings;
        private ResidueUpdateType residueUpdateType;

        private Residue residue;

        public void Start() {

            //addColourSelectButton(Settings.ResidueColour1);
            //addColourSelectButton(Settings.ResidueColour2);
            //addColourSelectButton(Settings.ResidueColour3);
            //addColourSelectButton(Settings.ResidueColour4);
            //addColourSelectButton(Settings.ResidueColour5);
            //addColourSelectButton(Settings.ResidueColour6);
            //addColourSelectButton(Settings.ResidueColour7);
            //addColourSelectButton(Settings.ResidueColour8);
            //addColourSelectButton(Settings.ResidueColour9);
            //addColourSelectButton(Settings.ResidueColour10);
        }

        public void Initialise(Residue residue, ResidueRenderSettings renderSettings, ResidueUpdateType residueUpdateType) {

            this.residueUpdateType = residueUpdateType;
            this.residue = residue;
            this.renderSettings = renderSettings;

            LoadSettings();
        }

        public void LoadSettings() {

            if (residueUpdateType == ResidueUpdateType.ID) {
                PanelTitle.text = "Residue " + renderSettings.ResidueID + " Display Options";
            }
            else if (residueUpdateType == ResidueUpdateType.Name) {
                PanelTitle.text = "Residue " + renderSettings.ResidueID + " Display Options";
            }

            LargeBondsToggle.isOn = renderSettings.LargeBonds;

            if(renderSettings.BondColour == null) {
                BondsColourToggle.isOn = false;
            }
            else {
                BondsColourToggle.isOn = true;
                setBondColourButtonColour((Color)renderSettings.BondColour);
            }

            if(renderSettings.BondColour == null) {
                SecondaryStructureColourToggle.isOn = false;
            }
            else {
                SecondaryStructureColourToggle.isOn = true;
                setSecondaryStructureColourButtonColour((Color)renderSettings.SecondaryStructureColour);
            }

            // create atom option buttons

            List<string> atomNames = new List<string>();
            foreach(KeyValuePair<int, Atom> atom in residue.Atoms) {
                if(!atomNames.Contains(atom.Value.Name)) {
                    atomNames.Add(atom.Value.Name);
                }
            }

            foreach(string atomName in atomNames) {

                GameObject atomOptionsButton = GameObject.Instantiate(AtomNameButtonPrefab);
                atomOptionsButton.transform.SetParent(AtomNameListContentPanel.transform);
                AtomNameButton buttonScript = atomOptionsButton.GetComponent<AtomNameButton>();

                if(renderSettings.AtomDisplayOptions.ContainsKey(atomName)) {
                    renderSettings.AtomDisplayOptions.Add(atomName, new AtomRenderSettings(atomName, Settings.ResidueColourDefault));
                }

                buttonScript.Initialise(renderSettings.AtomDisplayOptions[atomName]);
            }
        }

        public void SaveSettings() {

            //renderSettings.LargeBonds = LargeBondsToggle.isOn;

            //if (BondsColourToggle.isOn) {
            //    renderSettings.BondColour = ColourBondsToggle.isOn;
            //}
            //else {
            //    renderSettings.BondColour = null;
            //}

            //if (Colo.isOn) {
            //    renderSettings.BondColour = ColourBondsToggle.isOn;
            //}
            //else {
            //    renderSettings.BondColour = null;
            //}


            //renderSettings.ColourSecondaryStructure = ColourSecondaryStructureToggle.isOn;

            //ResiduesPanel.SaveResidueDisplayOptions(residueUpdateType, renderSettings, true);
        }

        public void RestoreDefaults() {

            renderSettings.SetDefaultOptions();
            LoadSettings();
            //ResiduesPanel.SaveResidueDisplayOptions(residueUpdateType, renderSettings, true);
        }

        //public void OpenColourSelectPanel() {
        //    ColorSelectPanel.SetActive(true);
        //}

        //private void SaveAndCloseColourSelectPanel(Color colour) {

        //    setCustomColourButtonColour(colour);
        //    ColorSelectPanel.SetActive(false);
        //    renderSettings.Colour = colour;
        //    ResiduesPanel.SaveResidueDisplayOptions(residueUpdateType, renderSettings, true);
        //}

        private void setBondColourButtonColour(Color colour) {

            ColorBlock buttonColours = BondColourButton.colors;
            buttonColours.normalColor = colour;
            buttonColours.highlightedColor = colour;
            buttonColours.pressedColor = colour;
            buttonColours.colorMultiplier = 1f;
            BondColourButton.colors = buttonColours;
        }

        private void setSecondaryStructureColourButtonColour(Color colour) {

            ColorBlock buttonColours = SecondaryStrucureColourButton.colors;
            buttonColours.normalColor = colour;
            buttonColours.highlightedColor = colour;
            buttonColours.pressedColor = colour;
            buttonColours.colorMultiplier = 1f;
            SecondaryStrucureColourButton.colors = buttonColours;
        }

        //private void addColourSelectButton(Color colour) {

        //    GameObject button = (GameObject)Instantiate(ColorSelectButtonPrefab, Vector3.zero, Quaternion.identity);

        //    ResidueCustomColourButton buttonScript = button.GetComponent<ResidueCustomColourButton>();
        //    buttonScript.SetSetColorCallback(SaveAndCloseColourSelectPanel, colour);

        //    ColorBlock colors = button.GetComponent<Button>().colors;
        //    colors.normalColor = colour;
        //    colors.highlightedColor = colour;
        //    colors.pressedColor = colour;
        //    button.GetComponent<Button>().colors = colors;

        //    RectTransform rect = button.GetComponent<RectTransform>();
        //    rect.SetParent(ColorSelectPanelButtonList.GetComponent<RectTransform>(), false);
        //    rect.localPosition = new Vector3(0, 0, 0);
        //    rect.localRotation = Quaternion.Euler(0, 0, 0);
        //    rect.localScale = Vector3.one;
        //}
    }
}
