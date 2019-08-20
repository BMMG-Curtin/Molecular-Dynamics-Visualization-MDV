using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void ToggleResidueIDDelegate(int residueID);
    public delegate void OpenResidueCustomSettingsDelegate(int residueID);
    public delegate void SaveCustomResidueSettingsDelegate(List<int> residueIDs, ResidueRenderSettings customResidueSettings);
    public delegate void CloseCustomResidueSettingsDelegate();

    public class ResidueIDsPanel : MonoBehaviour {

        [SerializeField]
        private ResidueCustomSettingsPanel CustomSettingsPanel;

        [SerializeField]
        private GameObject residueIDsPanel;

        [SerializeField]
        private ScrollRect residueIDsScrollView;

        [SerializeField]
        private GameObject residueButtons;

        [SerializeField]
        private GameObject residueButtonPrefab;

        [SerializeField]
        private Text toggleResiduesButtonText;

        [SerializeField]
        private ConfirmDialog ConfirmDialog;

        private string residueName;
        private MoleculeRenderSettings moleculeRenderSettings;
        private PrimaryStructure primaryStructure;
        private ResidueRenderSettingsUpdated settingsUpdatedCallback;
        private ClosedResidueSettingsPanel onClose;

        List<int> residueIDs;
        private Dictionary<int, ResidueIDButton> residueIDButtons;

        private bool allResiduesEnabled = true;

        public void Initialise(string residueName, MoleculeRenderSettings settings, PrimaryStructure primaryStructure, ResidueRenderSettingsUpdated settingsUpdatedCallback, ClosedResidueSettingsPanel onClose) {

            this.residueName = residueName;
            this.moleculeRenderSettings = settings;
            this.primaryStructure = primaryStructure;
            this.settingsUpdatedCallback = settingsUpdatedCallback;
            this.onClose = onClose;

            residueIDs = primaryStructure.GetResidueIDs(new List<String>() { residueName }).ToList();
            residueIDs.Sort();

            renderResidueButtons();

            allResiduesEnabled = true;
            foreach(int residueID in residueIDs) {
                if(!moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                    allResiduesEnabled = false;
                    break;
                }
            }

            updateToggleResidueButtonText();

            residueIDsPanel.SetActive(true);
        }

        public void ToggleAllResidues() {

            allResiduesEnabled = !allResiduesEnabled;

            foreach (int residueID in residueIDs) {

                if (allResiduesEnabled && !moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                    moleculeRenderSettings.EnabledResidueIDs.Add(residueID);
                }

                if (!allResiduesEnabled && moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                    moleculeRenderSettings.EnabledResidueIDs.Remove(residueID);
                }
            }

            updateToggleResidueButtonText();

            foreach(ResidueIDButton button in residueIDButtons.Values) {
                button.SetResidueEnabled(allResiduesEnabled);
            }

            updateCustomResidueNameStatus();
            settingsUpdatedCallback();
        }

        public void CloseResidueIDsPanel() {

            residueIDsPanel.SetActive(false);
            onClose();
        }

        public void ResetAllResidues() {

            ConfirmDialog.gameObject.SetActive(true);
            ConfirmDialog.Initialise("Would you like to delete custom settings for\nresidue " + residueName + "?", onConfirmReset);
        }

        private void renderResidueButtons() {

            UnityCleanup.DestroyGameObjects(residueButtons);
            residueIDsScrollView.verticalNormalizedPosition = 1;

            residueIDButtons = new Dictionary<int, ResidueIDButton>();

            foreach (int residueID in residueIDs) {

                bool residueEnabled = moleculeRenderSettings.EnabledResidueIDs.Contains(residueID);
                bool residueModified = moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residueID);

                GameObject button = (GameObject)Instantiate(residueButtonPrefab, Vector3.zero, Quaternion.identity);
                button.GetComponent<Image>().color = new Color(1, 1, 1);

                ResidueIDButton buttonScript = button.GetComponent<ResidueIDButton>();
                buttonScript.Initialise(residueID, residueEnabled, residueModified, toggleResidue, openCustomResidueRenderSettings);

                residueIDButtons.Add(residueID, buttonScript);

                RectTransform rect = button.GetComponent<RectTransform>();
                rect.SetParent(residueButtons.GetComponent<RectTransform>(), false);
            }
        }

        private void updateToggleResidueButtonText() {

            if (allResiduesEnabled) {
                toggleResiduesButtonText.text = "Hide All";
            }
            else {
                toggleResiduesButtonText.text = "Show All";
            }
        }

        private void onConfirmReset(bool confirmed) {

            if (confirmed) {

                foreach(int residueID in residueIDs) {

                    if (!moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                        moleculeRenderSettings.EnabledResidueIDs.Add(residueID);
                    }

                    if(moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                        moleculeRenderSettings.CustomResidueRenderSettings.Remove(residueID);
                    }
                }

                allResiduesEnabled = true;
                updateToggleResidueButtonText();
                renderResidueButtons();

                updateCustomResidueNameStatus();
                settingsUpdatedCallback();
            }
        }

        private void toggleResidue(int residueID) {

            if(moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                moleculeRenderSettings.EnabledResidueIDs.Remove(residueID);
            }
            else {
                moleculeRenderSettings.EnabledResidueIDs.Add(residueID);
            }

            updateCustomResidueNameStatus();
            settingsUpdatedCallback();
        }

        private void updateCustomResidueNameStatus() {

            bool custom = false;

            foreach(int residueID in residueIDs) {

                if(!moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                    custom = true;
                    break;
                }

                if (moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                    custom = true;
                    break;
                }
            }

            if(custom && !moleculeRenderSettings.CustomResidueNames.Contains(residueName)) {
                moleculeRenderSettings.CustomResidueNames.Add(residueName);
            }

            if (!custom && moleculeRenderSettings.CustomResidueNames.Contains(residueName)) {
                moleculeRenderSettings.CustomResidueNames.Remove(residueName);
            }
        }

        private void openCustomResidueRenderSettings(int residueID) {

            List<Residue> residues = primaryStructure.GetResiduesByID(residueID);
            if(residues.Count == 0) {
                return;
            }

            List<int> residueIDs = new List<int>() { residueID };
            List<string> atomNames = new List<string>();

            foreach (Atom atom in residues[0].Atoms.Values) {
                atomNames.Add(atom.Name);
            }

            ResidueRenderSettings residueSettingsCopy = new ResidueRenderSettings(residueID, Settings.ResidueColourDefault);
            if(moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                residueSettingsCopy = moleculeRenderSettings.CustomResidueRenderSettings[residueID].Clone();
            }

            CustomSettingsPanel.Initialise(residueIDs, residueName, atomNames, ResidueUpdateType.ID, residueSettingsCopy, saveCustomResidueSettings, onCloseCustomResidueSettings);

            //residueIDsPanel.gameObject.SetActive(false);
        }

        private void saveCustomResidueSettings(List<int> residueIDs, ResidueRenderSettings customResidueSettings) {

            if (!customResidueSettings.IsDefault()) {

                Debug.Log("Saving residue settings - not default ");

                foreach (int residueID in residueIDs) {

                    if (moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                        moleculeRenderSettings.CustomResidueRenderSettings[residueID] = customResidueSettings;
                    }
                    else {
                        moleculeRenderSettings.CustomResidueRenderSettings.Add(residueID, customResidueSettings);
                    }
                }
            }
            else {

                Debug.Log("Removing residue settings - are default");

                foreach (int residueID in residueIDs) {
                    if (moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                        moleculeRenderSettings.CustomResidueRenderSettings.Remove(residueID);
                    }
                }
            }

            updateCustomResidueNameStatus();
            renderResidueButtons();
            settingsUpdatedCallback();
        }

        private void onCloseCustomResidueSettings() {

            //residueIDsPanel.gameObject.SetActive(true);
        }
    }
}
