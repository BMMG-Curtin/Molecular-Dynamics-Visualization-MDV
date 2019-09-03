using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void ToggleResidueNameDelegate(string residueName);
    public delegate void OpenResidueIDsDelegate(string residueName);
    public delegate void SaveCustomResidueSettingsDelegate(List<int> residueIDs, ResidueRenderSettings customResidueSettings, ResidueUpdateType updateType);
    public delegate void UpdatedResidueIDsDelegate(List<int> residueIDs);
    public delegate void OpenUpdateAllResiduesPanel(List<int> residueIDs);
    public delegate void ClosedResidueSettingsPanel();

    public class ResidueNamesPanel : MonoBehaviour {

        [SerializeField]
        private ResidueCustomSettingsPanel customSettingsPanel;

        [SerializeField]
        private GameObject residueNamesPanel;

        [SerializeField]
        private ResidueIDsPanel residueIDsPanel;

        [SerializeField]
        private ResidueUpdateRangePanel residueUpdateRangePanel;

        [SerializeField]
        private ScrollRect residueNamesScrollView;

        [SerializeField]
        private GameObject residueButtons;

        [SerializeField]
        private GameObject residueButtonPrefab;

        [SerializeField]
        private Text toggleResiduesButtonText;

        [SerializeField]
        private ConfirmDialog confirmDialog;

        private MoleculeRenderSettings moleculeRenderSettings;
        private PrimaryStructure primaryStructure;
        private ResidueRenderSettingsUpdated settingsUpdatedCallback;

        private List<string> residueNames;
        private Dictionary<string, ResidueNameButton> residueNameButtons;

        private bool allResiduesEnabled = true;

        public void Initialise(MoleculeRenderSettings moleculeRenderSettings, PrimaryStructure primaryStructure, ResidueRenderSettingsUpdated settingsUpdatedCallback) {

            this.moleculeRenderSettings = moleculeRenderSettings;
            this.primaryStructure = primaryStructure;
            this.settingsUpdatedCallback = settingsUpdatedCallback;

            residueNames = primaryStructure.ResidueNames.ToList();
            residueNames.Sort();

            renderResidueNameButtons();

            allResiduesEnabled = false;
            if (residueNameButtons.Count == this.moleculeRenderSettings.EnabledResidueNames.Count) {
                allResiduesEnabled = true;
            }

            updateToggleResidueButtonText();

            residueNamesPanel.SetActive(true);
        }

        public void OnToggleAllResiduesButton() {

            allResiduesEnabled = !allResiduesEnabled;

            if (allResiduesEnabled) {
                moleculeRenderSettings.EnabledResidueNames = new HashSet<string>(primaryStructure.ResidueNames);
            }
            else {
                moleculeRenderSettings.EnabledResidueNames = new HashSet<string>();
            }

            updateToggleResidueButtonText();

            foreach(ResidueNameButton button in residueNameButtons.Values) {
                button.SetResidueEnabled(allResiduesEnabled);
            }

            settingsUpdatedCallback();
        }

        public void OnUpdateAllResiduesButton() {
            openUpdateAllResiduesPanel(primaryStructure.ResidueIDs.ToList());
        }

        public void OnUpdateResidueRangeButton() {
            residueUpdateRangePanel.Initialise(moleculeRenderSettings, primaryStructure, saveCustomResidueSettings, settingsUpdatedCallback, updateCustomResidueNameStatus, openUpdateAllResiduesPanel, onCloseResidueIDsPanel);
        }

        public void OnResetAllResiduesButton() {

            confirmDialog.gameObject.SetActive(true);
            confirmDialog.Initialise("This will delete all custom residue settings.\nWould you like to reset all residues?", onConfirmReset);
        }

        private void renderResidueNameButtons() {

            UnityCleanup.DestroyGameObjects(residueButtons);
            residueNamesScrollView.verticalNormalizedPosition = 1;

            residueNameButtons = new Dictionary<string, ResidueNameButton>();

            foreach (string residueName in residueNames) {

                bool residueEnabled = moleculeRenderSettings.EnabledResidueNames.Contains(residueName);
                bool residueModified = moleculeRenderSettings.CustomResidueNames.Contains(residueName);

                GameObject button = (GameObject)Instantiate(residueButtonPrefab, Vector3.zero, Quaternion.identity);
                button.GetComponent<Image>().color = new Color(1, 1, 1);

                ResidueNameButton buttonScript = button.GetComponent<ResidueNameButton>();
                buttonScript.Initialise(residueName, residueEnabled, residueModified, toggleResidue, openResidueIDs);

                residueNameButtons.Add(residueName, buttonScript);

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

        private void openUpdateAllResiduesPanel(List<int> residueIDs) {

            if (residueIDs == null || residueIDs.Count == 0) {
                return;
            }

            List<Residue> residues = primaryStructure.GetResiduesByID(residueIDs);

            if (residues == null || residues.Count == 0) {
                return;
            }

            Residue selectedResidue = residues[0];

            bool atomNamesAllTheSame = true;
            HashSet<string> selectedResidueAtomNames = new HashSet<string>();
            foreach(Atom atom in selectedResidue.Atoms.Values) {
                selectedResidueAtomNames.Add(atom.Name);
            }

            foreach (Residue residue in residues) {

                HashSet<string> residueAtomNames = new HashSet<string>();
                foreach (Atom atom in residue.Atoms.Values) {
                    residueAtomNames.Add(atom.Name);
                }

                if(!residueAtomNames.SetEquals(selectedResidueAtomNames)) {
                    atomNamesAllTheSame = false;
                }
            }

            List<string> atomNames = null;

            if (atomNamesAllTheSame) {

                atomNames = new List<string>();

                foreach (Atom atom in selectedResidue.Atoms.Values) {
                    atomNames.Add(atom.Name);
                }
            }

            bool residueSettingsAllTheSame = true;
            foreach (Residue residue in residues) {

                if (!moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residue.ID) ||
                    !moleculeRenderSettings.CustomResidueRenderSettings[residue.ID].Equals(moleculeRenderSettings.CustomResidueRenderSettings[selectedResidue.ID])) {
                    residueSettingsAllTheSame = false;
                    break;
                }
            }
            ResidueRenderSettings panelResidueSettings = new ResidueRenderSettings();

            if (residueSettingsAllTheSame) {
                if (moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(selectedResidue.ID)) {
                    panelResidueSettings = moleculeRenderSettings.CustomResidueRenderSettings[selectedResidue.ID].Clone();
                }
            }

            customSettingsPanel.Initialise(residueIDs, null, atomNames, ResidueUpdateType.All, panelResidueSettings, saveCustomResidueSettings, onCloseCustomResidueSettings);
        }

        private void onConfirmReset(bool confirmed, object data = null) {

            if (confirmed) {

                moleculeRenderSettings.EnabledResidueNames = new HashSet<string>(primaryStructure.ResidueNames);
                moleculeRenderSettings.CustomResidueNames = new HashSet<string>();
                moleculeRenderSettings.EnabledResidueIDs = new HashSet<int>(primaryStructure.ResidueIDs);
                moleculeRenderSettings.CustomResidueRenderSettings = new Dictionary<int, ResidueRenderSettings>();

                allResiduesEnabled = true;
                updateToggleResidueButtonText();
                renderResidueNameButtons();

                settingsUpdatedCallback();
            }
        }

        private void toggleResidue(string residueName) {

            if(moleculeRenderSettings.EnabledResidueNames.Contains(residueName)) {
                moleculeRenderSettings.EnabledResidueNames.Remove(residueName);
            }
            else {
                moleculeRenderSettings.EnabledResidueNames.Add(residueName);
            }

            settingsUpdatedCallback();
        }

        private void saveCustomResidueSettings(List<int> residueIDs, ResidueRenderSettings customResidueSettings, ResidueUpdateType updateType) {

            if (!customResidueSettings.IsDefault()) {

                //Debug.Log("Saving residue settings - not default ");

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

                //Debug.Log("Removing residue settings - are default");

                foreach (int residueID in residueIDs) {
                    if (moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                        moleculeRenderSettings.CustomResidueRenderSettings.Remove(residueID);
                    }
                }
            }

            updateCustomResidueNameStatus(residueIDs);

            //if (updateType == ResidueUpdateType.All) {
            //    renderResidueNameButtons();
            //}

            settingsUpdatedCallback();
        }

        private void updateCustomResidueNameStatus(List<int> residueIDs) {

            if (residueIDs == null || residueIDs.Count == 0) {
                return;
            }

            bool custom = false;

            HashSet<string> residueNames = primaryStructure.ResidueNamesForIDs(residueIDs);

            if (residueNames == null || residueNames.Count == 0) {
                return;
            }

            foreach (int residueID in residueIDs) {

                if (!moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                    custom = true;
                    break;
                }

                if (moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                    custom = true;
                    break;
                }
            }

            foreach (string residueName in residueNames) {

                if (custom && !moleculeRenderSettings.CustomResidueNames.Contains(residueName)) {
                    moleculeRenderSettings.CustomResidueNames.Add(residueName);
                }

                if (!custom && moleculeRenderSettings.CustomResidueNames.Contains(residueName)) {
                    moleculeRenderSettings.CustomResidueNames.Remove(residueName);
                }
            }

            renderResidueNameButtons();
        }

        private void openResidueIDs(string residueName) {

            residueNamesPanel.SetActive(false);
            residueIDsPanel.Initialise(residueName, moleculeRenderSettings, primaryStructure, saveCustomResidueSettings, settingsUpdatedCallback, onCloseResidueIDsPanel);
        }

        private void onCloseResidueIDsPanel() {

            renderResidueNameButtons();
            residueNamesPanel.SetActive(true);
        }

        private void onCloseCustomResidueSettings() {

            //residueIDsPanel.gameObject.SetActive(true);
        }
    }
}
