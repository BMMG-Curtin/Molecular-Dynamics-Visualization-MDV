using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void SetCustomColourButtonColourDelegate(Color? color);
    public delegate void AtomButtonClickDelegate();

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

        private Dictionary<string, ResidueAtomNameButton> atomButtons;

        private ResidueRenderSettings savedRenderSettings;

        private bool autoSaveSettingUpdates = false;

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

            autoSaveSettingUpdates = true;
        }

        public void OpenColourSelectPanel() {

            colourSelectPanel.gameObject.SetActive(true);
            colourSelectPanel.Initialise(setResidueColour);
        }

        public void RestoreDefaults() {

            renderSettings.SetDefaultOptions();
            loadSettings();
            AutoSaveSettings();
        }

        public void AutoSaveSettings() {

            if(autoSaveSettingUpdates) {
                SaveRenderSettings();
            }
        }

        public void SaveRenderSettings() {

            savePanelToRenderSettings();

            if (!renderSettings.Equals(savedRenderSettings) || residueUpdateType != ResidueUpdateType.ID) {

                savedRenderSettings = renderSettings.Clone();
                saveSettingsCallback(residueIDs, renderSettings, residueUpdateType);
            }
        }

        public void OnCloseButton() {

            if (!autoSaveSettingUpdates) {
                savePanelToRenderSettings();
            }

            if (!renderSettings.Equals(savedRenderSettings)) {
                confirmDialog.gameObject.SetActive(true);
                confirmDialog.Initialise("Residue settings have changed.\nWould you like to save?", saveRenderSettingsAndClose);
            }
            else {
                autoSaveSettingUpdates = false;
                customSettingsPanel.SetActive(false);
            }

            onCloseCallback();
        }

        public void loadSettings() {

            if (residueUpdateType == ResidueUpdateType.ID) {
                if (residueIDs != null && residueIDs.Count > 0) {
                    panelTitle.text = "Residue " + residueIDs[0] + " - Custom Settings";
                }
                else {
                    panelTitle.text = "Residue - Custom Settings";
                }
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
            atomButtons = new Dictionary<string, ResidueAtomNameButton>();
            UnityCleanup.DestroyGameObjects(atomListContentPanel);

            if (atomNames != null) {

                foreach (string atomName in atomNames) {

                    GameObject atomOptionsButton = GameObject.Instantiate(atomNameButtonPrefab);
                    atomOptionsButton.transform.SetParent(atomListContentPanel.transform, false);
                    ResidueAtomNameButton buttonScript = atomOptionsButton.GetComponent<ResidueAtomNameButton>();

                    AtomRenderSettings atomSettingsCopy = new AtomRenderSettings(atomName, Settings.ResidueColourDefault);
                    if (renderSettings.AtomSettings.ContainsKey(atomName)) {
                        atomSettingsCopy = renderSettings.AtomSettings[atomName].Clone();
                    }

                    buttonScript.Initialise(atomSettingsCopy, colourSelectPanel, AutoSaveSettings);

                    if (!atomButtons.ContainsKey(atomName)) {
                        atomButtons.Add(atomName, buttonScript);
                    }
                    else {
                        Debug.Log("Duplicate atom name in custom residue settings");
                    }
                }
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

            renderSettings.AtomSettings = new Dictionary<string, AtomRenderSettings>();
            foreach(ResidueAtomNameButton atomButton in atomButtons.Values) {
                if(!atomButton.AtomSettings.IsDefault()) {
                    renderSettings.AtomSettings.Add(atomButton.AtomSettings.AtomName, atomButton.AtomSettings);
                }
            }
        }

        private void saveRenderSettingsAndClose(bool confirmedSave, object data = null) {

            if (confirmedSave) {
                saveSettingsCallback(residueIDs, renderSettings, residueUpdateType);
            }

            autoSaveSettingUpdates = false;
            customSettingsPanel.SetActive(false);
        }

        private void setResidueColour(Color? color) {

            if(color == null) {
                renderSettings.SetDefaultColour();
            }
            else {
                renderSettings.ResidueColour = (Color)color;
            }
            setResidueColourButtonColour(renderSettings.ResidueColour);

            AutoSaveSettings();
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
