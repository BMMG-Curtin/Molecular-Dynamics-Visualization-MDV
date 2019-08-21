using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueUpdateRangePanel : MonoBehaviour {

        [SerializeField]
        private GameObject residueUpdateRangePanel;

        [SerializeField]
        private TMP_InputField ResidueRangeString;

        [SerializeField]
        private TextMeshProUGUI ErrorText;

        [SerializeField]
        private TextMeshProUGUI ValidatedText;

        [SerializeField]
        private Text toggleResiduesButtonText;

        [SerializeField]
        private ConfirmDialog confirmDialog;

        private MoleculeRenderSettings moleculeRenderSettings;
        private PrimaryStructure primaryStructure;
        private SaveCustomResidueSettingsDelegate saveCustomResidueSettings;
        private ResidueRenderSettingsUpdated settingsUpdatedCallback;
        private UpdatedResidueIDsDelegate residueIDsUpdated;
        private OpenUpdateAllResiduesPanel openUpdateAllPanel;
        private ClosedResidueSettingsPanel onClose;

        private HashSet<int> savedResiduesNumbers;
        private const int MAX_RESIDUE_NUMBER = 100000;

        private bool allResiduesEnabled = true;

        private void Awake() {

            savedResiduesNumbers = new HashSet<int>();

            ResidueRangeString.text = "";
            ErrorText.text = "";
            ValidatedText.text = "";
        }

        public void Initialise(MoleculeRenderSettings moleculeRenderSettings, PrimaryStructure primaryStructure, SaveCustomResidueSettingsDelegate saveCustomResidueSettings, ResidueRenderSettingsUpdated settingsUpdatedCallback, UpdatedResidueIDsDelegate residueIDsUpdated, OpenUpdateAllResiduesPanel openUpdateAllPanel, ClosedResidueSettingsPanel onClose) {

            this.moleculeRenderSettings = moleculeRenderSettings;
            this.primaryStructure = primaryStructure;
            this.saveCustomResidueSettings = saveCustomResidueSettings;
            this.settingsUpdatedCallback = settingsUpdatedCallback;
            this.residueIDsUpdated = residueIDsUpdated;
            this.openUpdateAllPanel = openUpdateAllPanel;
            this.onClose = onClose;

            updateToggleResidueButtonText();
            residueUpdateRangePanel.SetActive(true);
        }

        public void OnEnable() {
            ErrorText.text = "";
        }

        public void OnToggleButton() {

            if(savedResiduesNumbers == null || savedResiduesNumbers.Count == 0) {
                return;
            }

            allResiduesEnabled = !allResiduesEnabled;

            foreach (int residueID in savedResiduesNumbers) {

                if (allResiduesEnabled && !moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                    moleculeRenderSettings.EnabledResidueIDs.Add(residueID);
                }

                if (!allResiduesEnabled && moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                    moleculeRenderSettings.EnabledResidueIDs.Remove(residueID);
                }
            }

            updateToggleResidueButtonText();
            residueIDsUpdated(savedResiduesNumbers.ToList());
            settingsUpdatedCallback();
        }

        public void OnUpdateAllButton() {

            if (savedResiduesNumbers == null || savedResiduesNumbers.Count == 0) {
                return;
            }

            openUpdateAllPanel(savedResiduesNumbers.ToList());
        }

        public void OnResetAllButton() {

            if (savedResiduesNumbers == null || savedResiduesNumbers.Count == 0) {
                return;
            }

            confirmDialog.gameObject.SetActive(true);
            confirmDialog.Initialise("Would you like to delete custom settings for\n your selected residues?", onConfirmReset);
        }

        public void OnCloseButton() {
            residueUpdateRangePanel.SetActive(false);
        }

        public void SaveResidueRangeInputString() {

            ErrorText.text = "";
            ValidatedText.text = "";

            string inputText = cleanFilterString(ResidueRangeString.text);
            string[] items = inputText.Split(',');

            if (items == null || items.Length == 0) {

                ResidueRangeString.text = "";
                ErrorText.text = "No valid numbers or ranges found in input";
                return;
            }

            List<string> filterStringItems = new List<string>();
            savedResiduesNumbers = new HashSet<int>();

            foreach (string item in items) {

                if (item == null || item.Trim() == "") {
                    continue;
                }

                if (item.Contains("-")) {

                    string[] numbers = item.Split('-');

                    if (numbers != null && numbers.Length >= 2) {

                        int number1 = -1;
                        int number2 = -1;

                        if (int.TryParse(numbers[0], out number1) && int.TryParse(numbers[1], out number2)) {

                            if (number1 >= 0 && number1 <= MAX_RESIDUE_NUMBER && number2 >= 0 && number2 <= MAX_RESIDUE_NUMBER) {

                                if (number1 <= number2) {
                                    for (int i = number1; i <= number2; i++) {
                                        savedResiduesNumbers.Add(i);
                                    }
                                }
                                else {
                                    for (int i = number2; i <= number1; i++) {
                                        savedResiduesNumbers.Add(i);
                                    }
                                }

                                filterStringItems.Add(item);
                            }
                        }
                    }
                }
                else {

                    int residueNumber = -1;

                    if (int.TryParse(item, out residueNumber)) {

                        if (residueNumber >= 0 && residueNumber <= MAX_RESIDUE_NUMBER) {

                            filterStringItems.Add(residueNumber.ToString());
                            savedResiduesNumbers.Add(residueNumber);
                        }
                    }
                }
            }

            if (filterStringItems.Count > 0) {
                ResidueRangeString.text = string.Join(",", filterStringItems.ToArray());
                ValidatedText.text = "Residue number range validated";
            }
            else {
                ResidueRangeString.text = "";
                ErrorText.text = "No valid numbers or ranges found in input";
            }

            //string residueNumbers = "";
            //foreach (int number in savedResiduesNumbers) {
            //    residueNumbers += ", " + number;
            //}

            //Debug.Log("Numbers saved: " + residueNumbers);
        }

        private string cleanFilterString(string inputText) {

            StringBuilder cleanText = new StringBuilder();

            foreach (char character in inputText) {
                if (character != ' ' && character == ',' || character == '-' || char.IsDigit(character)) {
                    cleanText.Append(character);
                }
            }

            return cleanText.ToString();
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

                foreach (int residueID in savedResiduesNumbers) {

                    if (!moleculeRenderSettings.EnabledResidueIDs.Contains(residueID)) {
                        moleculeRenderSettings.EnabledResidueIDs.Add(residueID);
                    }

                    if (moleculeRenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
                        moleculeRenderSettings.CustomResidueRenderSettings.Remove(residueID);
                    }
                }

                allResiduesEnabled = true;

                updateToggleResidueButtonText();
                residueIDsUpdated(savedResiduesNumbers.ToList());
                settingsUpdatedCallback();
            }
        }
    }
}
