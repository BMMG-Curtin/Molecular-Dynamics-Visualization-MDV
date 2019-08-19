using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void ToggleResidueNameDelegate(string residueName);
    public delegate void OpenResidueIDsDelegate(string residueName);

    public class ResidueNamesPanel : MonoBehaviour {

        [SerializeField]
        private ResidueIDsPanel residueIDsPanel;

        [SerializeField]
        private ScrollRect residueNamesScrollView;

        [SerializeField]
        private GameObject residueButtons;

        [SerializeField]
        private GameObject residueButtonPrefab;

        [SerializeField]
        private Text toggleResiduesButtonText;

        [SerializeField]
        private ConfirmDialog ConfirmDialog;

        private MoleculeRenderSettings renderSettings;
        private PrimaryStructure primaryStructure;
        private ResidueRenderSettingsUpdated settingsUpdatedCallback;

        private Dictionary<string, ResidueNameButton> residueNameButtons;

        private bool allResiduesEnabled = true;

        public void Initialise(MoleculeRenderSettings settings, PrimaryStructure primaryStructure, ResidueRenderSettingsUpdated settingsUpdatedCallback) {

            this.renderSettings = settings;
            this.primaryStructure = primaryStructure;
            this.settingsUpdatedCallback = settingsUpdatedCallback;

            renderResidueButtons();

            allResiduesEnabled = false;
            if (residueNameButtons.Count == renderSettings.EnabledResidueNames.Count) {
                allResiduesEnabled = true;
            }

            updateToggleResidueButtonText();
        }

        public void OnDisable() {
            residueIDsPanel.gameObject.SetActive(false);
        }

        public void ToggleAllResidues() {

            if(allResiduesEnabled) {
                renderSettings.EnabledResidueNames = new HashSet<string>();
            }
            else {
                renderSettings.EnabledResidueNames = new HashSet<string>(primaryStructure.ResidueNames);
            }

            allResiduesEnabled = !allResiduesEnabled;
            updateToggleResidueButtonText();

            foreach(ResidueNameButton button in residueNameButtons.Values) {
                button.SetResidueEnabled(allResiduesEnabled);
            }

            settingsUpdatedCallback();
        }

        public void ResetAllResidues() {

            ConfirmDialog.gameObject.SetActive(true);
            ConfirmDialog.Initialise("This will delete all custom residue settings.\nWould you like to reset all residues?", onConfirmReset);
        }

        private void renderResidueButtons() {

            UnityCleanup.DestroyGameObjects(residueButtons);
            residueNamesScrollView.verticalNormalizedPosition = 1;

            List<string> residueNamesList = primaryStructure.ResidueNames.ToList();
            residueNamesList.Sort();

            residueNameButtons = new Dictionary<string, ResidueNameButton>();

            foreach (string residueName in residueNamesList) {

                bool residueEnabled = renderSettings.EnabledResidueNames.Contains(residueName);
                bool residueModified = renderSettings.CustomResidueNames.Contains(residueName);

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

        private void onConfirmReset(bool confirmed) {

            if (confirmed) {

                renderSettings.EnabledResidueNames = new HashSet<string>(primaryStructure.ResidueNames);
                renderSettings.CustomResidueNames = new HashSet<string>();
                renderSettings.EnabledResidueIDs = new HashSet<int>(primaryStructure.ResidueIDs);
                renderSettings.CustomResidueRenderSettings = new Dictionary<int, ResidueRenderSettings>();

                allResiduesEnabled = true;
                updateToggleResidueButtonText();
                renderResidueButtons();
            }
        }

        private void toggleResidue(string residueName) {

            if(renderSettings.EnabledResidueNames.Contains(residueName)) {
                renderSettings.EnabledResidueNames.Remove(residueName);
            }
            else {
                renderSettings.EnabledResidueNames.Add(residueName);
            }

            settingsUpdatedCallback();
        }

        private void openResidueIDs(string residueName) {

            residueIDsPanel.gameObject.SetActive(true);
            residueIDsPanel.Initialise(residueName, renderSettings, primaryStructure, settingsUpdatedCallback);
        }
    }
}
