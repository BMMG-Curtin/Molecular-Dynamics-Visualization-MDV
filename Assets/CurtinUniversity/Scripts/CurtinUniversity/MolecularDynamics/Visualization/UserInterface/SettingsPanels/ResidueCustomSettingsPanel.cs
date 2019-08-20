using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void SetCustomColourButtonColourDelegate(Color color);

    public class ResidueCustomSettingsPanel : MonoBehaviour {

        [SerializeField]
        private GameObject customSettingsPanel;

        [SerializeField]
        private ResidueCustomColourSelect colourSelectPanel;

        [SerializeField]
        private ConfirmDialog confirmDialog;

        [SerializeField]
        private GameObject atomNameButtonPrefab;

        [SerializeField]
        private GameObject atomListContentPanel;

        [SerializeField]
        private Text panelTitle;

        [SerializeField]
        private Toggle colourAtomsToggle;

        [SerializeField]
        private Toggle cpkToggle;

        [SerializeField]
        private Toggle vdwToggle;

        [SerializeField]
        private Toggle colourBondsToggle;

        [SerializeField]
        private Toggle largeBondsToggle;

        [SerializeField]
        private Toggle colourSecondaryStructureToggle;

        [SerializeField]
        private Button residueColourButton;

        private List<int> residueIDs;
        private string residueName;
        private List<string> atomNames;
        private ResidueUpdateType residueUpdateType;
        private ResidueRenderSettings renderSettings;
        private SaveCustomResidueSettingsDelegate saveSettingsCallback;
        private CloseCustomResidueSettingsDelegate onCloseCallback;

        private ResidueRenderSettings savedRenderSettings;

        private void OnDisable() {
            colourSelectPanel.gameObject.SetActive(false);
        }

        public void Initialise(List<int> residueIDs, string residueName, List<string> atomNames, ResidueUpdateType residueUpdateType, ResidueRenderSettings renderSettings, SaveCustomResidueSettingsDelegate saveSettings, CloseCustomResidueSettingsDelegate onClose) {

            this.residueIDs = residueIDs;
            this.residueName = residueName;
            this.atomNames = atomNames;
            this.renderSettings = renderSettings;
            this.residueUpdateType = residueUpdateType;
            this.saveSettingsCallback = saveSettings;
            this.onCloseCallback = onClose;

            this.savedRenderSettings = renderSettings.Clone();

            loadSettings();
            customSettingsPanel.SetActive(true);
        }

        public void OpenColourSelectPanel() {

            colourSelectPanel.gameObject.SetActive(true);
            colourSelectPanel.Initialise(setResidueColour);
        }

        public void RestoreDefaults() {

            renderSettings.SetDefaultOptions();
            loadSettings();
        }

        public void SaveRenderSettings() {

            savePanelToRenderSettings();

            Debug.Log("Save button click");

            if (!renderSettings.Equals(savedRenderSettings)) {

                Debug.Log("Save button - residues settings changed. Calling save settings");

                saveSettingsCallback(residueIDs, renderSettings);
                savedRenderSettings = renderSettings.Clone();
            }
        }

        public void OnCloseButton() {

            savePanelToRenderSettings();

            if (!renderSettings.Equals(savedRenderSettings)) {
                confirmDialog.gameObject.SetActive(true);
                confirmDialog.Initialise("Residue settings have changed.\nWould you like to save?", saveRenderSettingsAndClose);
            }
            else {
                customSettingsPanel.SetActive(false);
            }

            onCloseCallback();
        }

        public void loadSettings() {

            if (residueUpdateType == ResidueUpdateType.ID) {
                panelTitle.text = "Residue " + renderSettings.ResidueID + " - Custom Settings";
            }
            else if (residueUpdateType == ResidueUpdateType.Name) {
                panelTitle.text = "Residue " + residueName + " - Custom Settings";
            }
            else {
                panelTitle.text = "Custom Residue Settings";
            }

            colourAtomsToggle.isOn = renderSettings.ColourAtoms;

            cpkToggle.isOn = false;
            vdwToggle.isOn = false;

            if (renderSettings.AtomRepresentation == MolecularRepresentation.CPK) {
                cpkToggle.isOn = true;
            }
            else if (renderSettings.AtomRepresentation == MolecularRepresentation.VDW) {
                vdwToggle.isOn = true;
            }

            colourBondsToggle.isOn = renderSettings.ColourBonds;
            largeBondsToggle.isOn = renderSettings.LargeBonds;
            colourSecondaryStructureToggle.isOn = renderSettings.ColourSecondaryStructure;

            setResidueColourButtonColour(renderSettings.ResidueColour);

            // create atom option buttons
            UnityCleanup.DestroyGameObjects(atomListContentPanel);

            foreach (string atomName in atomNames) {

                GameObject atomOptionsButton = GameObject.Instantiate(atomNameButtonPrefab);
                atomOptionsButton.transform.SetParent(atomListContentPanel.transform, false);
                ResidueAtomNameButton buttonScript = atomOptionsButton.GetComponent<ResidueAtomNameButton>();

                AtomRenderSettings atomSettingsCopy = new AtomRenderSettings(atomName, Settings.ResidueColourDefault);
                if (renderSettings.AtomSettings.ContainsKey(atomName)) {
                    atomSettingsCopy = renderSettings.AtomSettings[atomName].Clone();
                }

                buttonScript.Initialise(atomSettingsCopy);
            }
        }

        private void savePanelToRenderSettings() {

            renderSettings.ColourAtoms = colourAtomsToggle.isOn;

            if (cpkToggle.isOn) {
                renderSettings.AtomRepresentation = MolecularRepresentation.CPK;
            }
            else if (vdwToggle.isOn) {
                renderSettings.AtomRepresentation = MolecularRepresentation.VDW;
            }
            else {
                renderSettings.AtomRepresentation = MolecularRepresentation.None;
            }

            renderSettings.ColourBonds = colourBondsToggle.isOn;
            renderSettings.LargeBonds = largeBondsToggle.isOn;
            renderSettings.ColourSecondaryStructure = colourSecondaryStructureToggle.isOn;
        }

        private void saveRenderSettingsAndClose(bool confirmedSave) {

            if (confirmedSave) {
                saveSettingsCallback(residueIDs, renderSettings);
            }

            customSettingsPanel.SetActive(false);
        }

        private void setResidueColour(Color color) {

            renderSettings.ResidueColour = color;
            setResidueColourButtonColour(renderSettings.ResidueColour);
        }

        private void setResidueColourButtonColour(Color colour) {

            ColorBlock buttonColours = residueColourButton.colors;
            buttonColours.normalColor = colour;
            buttonColours.highlightedColor = colour;
            buttonColours.pressedColor = colour;
            buttonColours.colorMultiplier = 1f;
            residueColourButton.colors = buttonColours;
        }
    }
}
