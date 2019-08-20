using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueCustomSettingsPanel : MonoBehaviour {

        [SerializeField]
        private GameObject customSettingsPanel;

        [SerializeField]
        private Text PanelTitle;

        [SerializeField]
        private Toggle ColourAtomsToggle;

        [SerializeField]
        private Toggle CPKToggle;

        [SerializeField]
        private Toggle VDWToggle;

        [SerializeField]
        private Toggle ColourBondsToggle;

        [SerializeField]
        private Toggle LargeBondsToggle;

        [SerializeField]
        private Toggle ColourSecondaryStructureToggle;

        [SerializeField]
        private Button ResidueColourButton;

        [SerializeField]
        private Button ModifyAtomSettingsButton;

        private List<int> residueIDs;
        private string residueName;
        private HashSet<string> atomNames;
        private ResidueUpdateType residueUpdateType;
        private ResidueRenderSettings renderSettings;
        private ResidueRenderSettingsUpdated settingsUpdatedCallback;
        private ClosedResidueSettingsPanel onClose;

        //public void Start() {

        //    //addColourSelectButton(Settings.ResidueColour1);
        //    //addColourSelectButton(Settings.ResidueColour2);
        //    //addColourSelectButton(Settings.ResidueColour3);
        //    //addColourSelectButton(Settings.ResidueColour4);
        //    //addColourSelectButton(Settings.ResidueColour5);
        //    //addColourSelectButton(Settings.ResidueColour6);
        //    //addColourSelectButton(Settings.ResidueColour7);
        //    //addColourSelectButton(Settings.ResidueColour8);
        //    //addColourSelectButton(Settings.ResidueColour9);
        //    //addColourSelectButton(Settings.ResidueColour10);
        //}

        public void Initialise(List<int> residueIDs, string residueName, List<string> atomNames, ResidueUpdateType residueUpdateType, ResidueRenderSettings renderSettings, ResidueRenderSettingsUpdated settingsUpdatedCallback, ClosedResidueSettingsPanel onClose) {

            this.residueIDs = residueIDs;
            this.residueName = residueName;
            this.renderSettings = renderSettings;
            this.residueUpdateType = residueUpdateType;
            this.settingsUpdatedCallback = settingsUpdatedCallback;
            this.onClose = onClose;

            LoadSettings();
            customSettingsPanel.SetActive(true);
        }

        public void LoadSettings() {

            if (residueUpdateType == ResidueUpdateType.ID) {
                PanelTitle.text = "Residue " + renderSettings.ResidueID + " - Custom Settings";
            }
            else if (residueUpdateType == ResidueUpdateType.Name) {
                PanelTitle.text = "Residue " + residueName + " - Custom Settings";
            }
            else {
                PanelTitle.text = "Custom Residue Settings";
            }

            ColourAtomsToggle.isOn = renderSettings.ColourAtoms;

            CPKToggle.isOn = false;
            VDWToggle.isOn = false;

            if (renderSettings.AtomRepresentation == MolecularRepresentation.CPK) {
                CPKToggle.isOn = true;
            }
            else if(renderSettings.AtomRepresentation == MolecularRepresentation.VDW) {
                VDWToggle.isOn = true;
            }

            ColourBondsToggle.isOn = renderSettings.ColourBonds;
            LargeBondsToggle.isOn = renderSettings.LargeBonds;
            ColourSecondaryStructureToggle.isOn = renderSettings.ColourSecondaryStructure;

            setResidueColourButtonColour(renderSettings.ResidueColour);

            if (residueUpdateType == ResidueUpdateType.ID || residueUpdateType == ResidueUpdateType.Name) {
                ModifyAtomSettingsButton.interactable = false;
            }

            // create atom option buttons

            //List<string> atomNames = new List<string>();
            //foreach(KeyValuePair<int, Atom> atom in residue.Atoms) {
            //    if(!atomNames.Contains(atom.Value.Name)) {
            //        atomNames.Add(atom.Value.Name);
            //    }
            //}

            //foreach(string atomName in atomNames) {

            //    GameObject atomOptionsButton = GameObject.Instantiate(AtomNameButtonPrefab);
            //    atomOptionsButton.transform.SetParent(AtomNameListContentPanel.transform);
            //    AtomNameButton buttonScript = atomOptionsButton.GetComponent<AtomNameButton>();

            //    if(renderSettings.AtomDisplayOptions.ContainsKey(atomName)) {
            //        renderSettings.AtomDisplayOptions.Add(atomName, new AtomRenderSettings(atomName, Settings.ResidueColourDefault));
            //    }

            //    buttonScript.Initialise(renderSettings.AtomDisplayOptions[atomName]);
            //}
        }

        public void CloseSettingsPanel() {

            customSettingsPanel.SetActive(false);
            onClose();
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

        private void setResidueColourButtonColour(Color colour) {

            ColorBlock buttonColours = ResidueColourButton.colors;
            buttonColours.normalColor = colour;
            buttonColours.highlightedColor = colour;
            buttonColours.pressedColor = colour;
            buttonColours.colorMultiplier = 1f;
            ResidueColourButton.colors = buttonColours;
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
