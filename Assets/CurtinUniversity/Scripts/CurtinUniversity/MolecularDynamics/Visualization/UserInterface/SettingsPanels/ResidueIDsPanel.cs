using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void ToggleResidueIDDelegate(int residueID);
    public delegate void OpenCustomResidueRenderSettingsDelegate(int residueID);
    public delegate void ClosedCustomRenderSettings();

    public class ResidueIDsPanel : MonoBehaviour {

        [SerializeField]
        private ResidueCustomRenderSettingsPanel CustomRenderSettingsPanel;

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
        private MoleculeRenderSettings renderSettings;
        private PrimaryStructure primaryStructure;
        private ResidueRenderSettingsUpdated settingsUpdatedCallback;
        private OnCloseResidueIDsPanel onClose;

        List<int> residueIDs;
        private Dictionary<int, ResidueIDButton> residueIDButtons;

        private bool allResiduesEnabled = true;

        public void Initialise(string residueName, MoleculeRenderSettings settings, PrimaryStructure primaryStructure, ResidueRenderSettingsUpdated settingsUpdatedCallback, OnCloseResidueIDsPanel onClose) {

            this.residueName = residueName;
            this.renderSettings = settings;
            this.primaryStructure = primaryStructure;
            this.settingsUpdatedCallback = settingsUpdatedCallback;
            this.onClose = onClose;

            residueIDs = primaryStructure.GetResidueIDs(new List<String>() { residueName }).ToList();
            residueIDs.Sort();

            renderResidueButtons();

            allResiduesEnabled = true;
            foreach(int residueID in residueIDs) {
                if(!renderSettings.EnabledResidueIDs.Contains(residueID)) {
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

                if (allResiduesEnabled && !renderSettings.EnabledResidueIDs.Contains(residueID)) {
                    renderSettings.EnabledResidueIDs.Add(residueID);
                }

                if (!allResiduesEnabled && renderSettings.EnabledResidueIDs.Contains(residueID)) {
                    renderSettings.EnabledResidueIDs.Remove(residueID);
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

                bool residueEnabled = renderSettings.EnabledResidueIDs.Contains(residueID);
                bool residueModified = renderSettings.CustomResidueRenderSettings.ContainsKey(residueID);

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

                    if (!renderSettings.EnabledResidueIDs.Contains(residueID)) {
                        renderSettings.EnabledResidueIDs.Add(residueID);
                    }

                    if(renderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                        renderSettings.CustomResidueRenderSettings.Remove(residueID);
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

            if(renderSettings.EnabledResidueIDs.Contains(residueID)) {
                renderSettings.EnabledResidueIDs.Remove(residueID);
            }
            else {
                renderSettings.EnabledResidueIDs.Add(residueID);
            }

            updateCustomResidueNameStatus();
            settingsUpdatedCallback();
        }

        private void updateCustomResidueNameStatus() {

            bool custom = false;

            foreach(int residueID in residueIDs) {

                if(!renderSettings.EnabledResidueIDs.Contains(residueID)) {
                    custom = true;
                    break;
                }

                if (renderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                    custom = true;
                    break;
                }
            }

            if(custom && !renderSettings.CustomResidueNames.Contains(residueName)) {
                renderSettings.CustomResidueNames.Add(residueName);
            }

            if (!custom && renderSettings.CustomResidueNames.Contains(residueName)) {
                renderSettings.CustomResidueNames.Remove(residueName);
            }
        }

        private void openCustomResidueRenderSettings(int residueID) {

        }
    }
}
