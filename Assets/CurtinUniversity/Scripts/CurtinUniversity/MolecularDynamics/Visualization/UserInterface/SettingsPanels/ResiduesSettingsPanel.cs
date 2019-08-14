using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void SetCustomColourButtonColour(Color color);
    public delegate void OpenResidueIDsDelegate(string residueName);
    public delegate void OpenResidueDisplayOptionsDelegate(int residueID);
    public delegate void SaveResidueNameEnabledDelegate(string residueName, bool enabled);
    public delegate void SaveResidueButtonOptionsDelegate(ResidueDisplayOptions options, bool updateButton, bool updateModel);

    public class ResiduesSettingsPanel : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private TextMeshProUGUI selectedMoleculeText;

        [SerializeField]
        public GameObject residueNamesPanel;

        [SerializeField]
        public GameObject residueIDsPanel;

        [SerializeField]
        public ScrollRect residueNamesScrollView;

        [SerializeField]
        public ScrollRect residueIDsScrollView;

        [SerializeField]
        public GameObject residueNamesButtonContent;

        [SerializeField]
        public GameObject residueIDsButtonContent;

        [SerializeField]
        public GameObject residueNameButtonPrefab;

        [SerializeField]
        public GameObject residueIDButtonPrefab;

        [SerializeField]
        public GameObject residueDisplayOptionsPanel;

        [SerializeField]
        public Text residuesEnableAllButtonText;

        private Dictionary<int, List<string>> moleculeResidueNames;
        private Dictionary<int, List<int>> moleculeResidueIDs;
        private Dictionary<int, Dictionary<string, List<int>>> moleculeResidues;

        private Dictionary<string, ResidueNameButton> residueNameButtons;
        private Dictionary<int, ResidueIDButton> residueIDButtons;

        private int scrollStepCount = 5;
        private int buttonsPerLine = 7; // used to calculate scroll speed

        private bool residuesEnableAllButtonStatus = true;

        private MoleculeSettings selectedMolecule;

        private void Awake() {

            if (moleculeResidues == null || moleculeResidueNames == null) {

                moleculeResidueNames = new Dictionary<int, List<string>>();
                moleculeResidueIDs = new Dictionary<int, List<int>>();
                moleculeResidues = new Dictionary<int, Dictionary<string, List<int>>>();
            }

            residueIDsPanel.SetActive(false);
            residueNamesPanel.SetActive(false);
            residueDisplayOptionsPanel.SetActive(false);
        }

        private void OnEnable() {

            selectedMolecule = molecules.GetSelected();
            initialise();

            if (selectedMolecule != null) {
                selectedMoleculeText.text = "Modifying settings for molecule  - " + selectedMolecule.Name;
            }
            else {
                selectedMoleculeText.text = "< no molecule selected >";
            }
        }

        private void OnDisable() {

            residueIDsPanel.SetActive(false);
            residueNamesPanel.SetActive(false);
            residueDisplayOptionsPanel.SetActive(false);
        }

        public void SetModelResidues(int moleculeID, Dictionary<string, HashSet<int>> residues) {

            if (moleculeResidues == null || moleculeResidueNames == null) {

                moleculeResidues = new Dictionary<int, Dictionary<string, List<int>>>();
                moleculeResidueIDs = new Dictionary<int, List<int>>();
                moleculeResidueNames = new Dictionary<int, List<string>>();
            }

            // set the molecule residue names
            moleculeResidueNames.Add(moleculeID, new List<string>());

            foreach (string residueName in residues.Keys) {
                moleculeResidueNames[moleculeID].Add(residueName);
            }

            moleculeResidueNames[moleculeID].Sort();

            // set the molecule residue IDs
            // while IDs should be unique they might not be, so using a HashSet to aggregate before adding to list
            HashSet<int> residueIDsSet = new HashSet<int>();

            foreach (HashSet<int> ids in residues.Values) {
                foreach (int id in ids) {
                    residueIDsSet.Add(id);
                }
            }

            List<int> residueIDsList = residueIDsSet.ToList();
            residueIDsList.Sort();
            moleculeResidueIDs.Add(moleculeID, residueIDsList);

            // set the molecule residue IDs by name, and also the molecule residue IDs list at the same time
            moleculeResidues.Add(moleculeID, new Dictionary<string, List<int>>());

            foreach (KeyValuePair<string, HashSet<int>> residue in residues) {

                List<int> residueIDs = residue.Value.ToList();
                residueIDs.Sort();
                moleculeResidues[moleculeID].Add(residue.Key, residueIDs);
            }
        }

        private void initialise() {

            Utility.Cleanup.DestroyGameObjects(residueNamesButtonContent);
            residueNamesScrollView.verticalNormalizedPosition = 1;

            if (selectedMolecule == null || moleculeResidues == null || !moleculeResidues.ContainsKey(selectedMolecule.ID)) {
                return;
            }

            // initialise the residue render settings for the molecule

            if (selectedMolecule.RenderSettings.EnabledResidueNames == null) {
                if (moleculeResidueNames.ContainsKey(selectedMolecule.ID)) {
                    selectedMolecule.RenderSettings.EnabledResidueNames = new HashSet<string>(moleculeResidueNames[selectedMolecule.ID]);
                }
                else {
                    selectedMolecule.RenderSettings.EnabledResidueNames = new HashSet<string>();
                }
            }

            if (selectedMolecule.RenderSettings.EnabledResidueIDs == null) {
                if (moleculeResidueIDs.ContainsKey(selectedMolecule.ID)) {
                    selectedMolecule.RenderSettings.EnabledResidueIDs = new HashSet<int>(moleculeResidueIDs[selectedMolecule.ID]);
                }
                else {
                    selectedMolecule.RenderSettings.EnabledResidueIDs = new HashSet<int>();
                }
            }

            if (selectedMolecule.RenderSettings.CustomResidueNames == null) {
                selectedMolecule.RenderSettings.CustomResidueNames = new HashSet<string>();
            }

            if (selectedMolecule.RenderSettings.CustomResidues == null) {
                selectedMolecule.RenderSettings.CustomResidues = new Dictionary<int, ResidueDisplayOptions>();
            }


            // render all the residue name buttons
            residueNameButtons = new Dictionary<string, ResidueNameButton>();

            foreach (string residueName in moleculeResidueNames[selectedMolecule.ID]) {

                bool residueEnabled = selectedMolecule.RenderSettings.EnabledResidueNames.Contains(residueName);
                bool residueModified = selectedMolecule.RenderSettings.CustomResidueNames.Contains(residueName);

                GameObject button = (GameObject)Instantiate(residueNameButtonPrefab, Vector3.zero, Quaternion.identity);
                button.GetComponent<Image>().color = new Color(1, 1, 1);

                ResidueNameButton buttonScript = button.GetComponent<ResidueNameButton>();
                buttonScript.Initialise(residueName, residueEnabled, residueModified, saveResidueNameEnabled, OpenResidueIDsPanel);

                residueNameButtons.Add(residueName, buttonScript);

                RectTransform rect = button.GetComponent<RectTransform>();
                rect.SetParent(residueNamesButtonContent.GetComponent<RectTransform>(), false);
            }

            residueNamesPanel.SetActive(true);
        }

        //public void ToggleAllResidues() {

        //    residuesEnableAllButtonStatus = !residuesEnableAllButtonStatus;
        //    selectedMolecule.RenderSettings.EnabledResidueNames = new HashSet<string>();

        //    foreach (KeyValuePair<string, ResidueDisplayOptions> options in selectedMolecule.RenderSettings.ResidueOptions) {

        //        options.Value.Enabled = residuesEnableAllButtonStatus;
        //        SaveResidueDisplayOptions(options.Value, true, false);
        //    }

        //    if (residuesEnableAllButtonStatus == false) {
        //        ResiduesEnableAllButtonText.text = "Show All";
        //    }
        //    else {
        //        ResiduesEnableAllButtonText.text = "Hide All";
        //    }

        //    if (selectedMolecule.Hidden) {
        //        selectedMolecule.PendingRerender = true;
        //    }
        //    else {
        //        UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
        //    }
        //}

        // residue name button callbacks

        private void saveResidueNameEnabled(string residueName, bool enabled) {

            HashSet<string> enabledResidues = selectedMolecule.RenderSettings.EnabledResidueNames;

            if (enabledResidues != null) {
                if(enabled) {
                    enabledResidues.Add(residueName);
                }
                else {
                    if (enabledResidues.Contains(residueName)) {
                        enabledResidues.Remove(residueName);
                    }
                }
            }

            if (selectedMolecule.Hidden) {
                selectedMolecule.PendingRerender = true;
            }
            else {
                UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
            }
        }

        private void OpenResidueIDsPanel(string residueName) {

            if (!moleculeResidues.ContainsKey(selectedMolecule.ID)) {
                return;
            }

            Dictionary<string, List<int>> residues = moleculeResidues[selectedMolecule.ID];

            if (!residues.ContainsKey(residueName)) {
                return;
            }

            List<int> residueIDs = residues[residueName];

            residueNamesPanel.SetActive(false);

            Utility.Cleanup.DestroyGameObjects(residueIDsButtonContent);
            residueIDsScrollView.verticalNormalizedPosition = 1;

            // render all the residue id buttons for the given residue
            residueIDButtons = new Dictionary<int, ResidueIDButton>();

            foreach (int residueID in residueIDs) {

                ResidueDisplayOptions options;
                if(selectedMolecule.RenderSettings.CustomResidues.ContainsKey(residueID)) {
                    options = selectedMolecule.RenderSettings.CustomResidues[residueID];
                }
                else {
                    options = new ResidueDisplayOptions(residueID, Settings.ResidueColourDefault);
                }

                GameObject button = (GameObject)Instantiate(residueIDButtonPrefab, Vector3.zero, Quaternion.identity);
                button.GetComponent<Image>().color = new Color(1, 1, 1);

                ResidueIDButton buttonScript = button.GetComponent<ResidueIDButton>();

                buttonScript.Initialise(options, SaveResidueDisplayOptions, OpenResidueDisplayOptions);

                residueIDButtons.Add(residueID, buttonScript);

                RectTransform rect = button.GetComponent<RectTransform>();
                rect.SetParent(residueIDsButtonContent.GetComponent<RectTransform>(), false);
            }

            residueIDsPanel.SetActive(true);
        }

        // residue id button callbacks

        public void OpenResidueDisplayOptions(int residueID) {

            residueDisplayOptionsPanel.SetActive(true);
            ResidueDisplayOptionsPanel displayOptionsPanel = residueDisplayOptionsPanel.GetComponent<ResidueDisplayOptionsPanel>();

            if (!selectedMolecule.RenderSettings.CustomResidues.ContainsKey(residueID)) {
                selectedMolecule.RenderSettings.CustomResidues.Add(residueID, new ResidueDisplayOptions(residueID, Settings.ResidueColourDefault));
            }

            displayOptionsPanel.Initialise(selectedMolecule.RenderSettings.CustomResidues[residueID]);
        }

        //public void OpenResidueDisplayOptionsForAllResidues() {

        //    ResidueDisplayOptions displayOptions = new ResidueDisplayOptions(UpdateAllResiduesKey, Visualization.Settings.ResidueColourDefault);
        //    ResidueDisplayOptions.SetActive(true);

        //    ResidueDisplayOptionsPanel displayOptionsPanel = ResidueDisplayOptions.GetComponent<ResidueDisplayOptionsPanel>();
        //    displayOptionsPanel.Initialise(displayOptions);
        //}

        public void SaveResidueDisplayOptions(ResidueDisplayOptions options, bool updateButton, bool updateModel = true) {

            // update residue button state
            if (updateButton) {
                residueIDButtons[options.ResidueID].UpdateResidueOptions(options);
            }

            // update state lists
            if (options.Enabled) {
                if (!selectedMolecule.RenderSettings.EnabledResidueIDs.Contains(options.ResidueID)) {
                    selectedMolecule.RenderSettings.EnabledResidueIDs.Add(options.ResidueID);
                }
            }
            else {
                selectedMolecule.RenderSettings.EnabledResidueIDs.Remove(options.ResidueID);
            }

            if (!options.IsDefault()) {
                if (!selectedMolecule.RenderSettings.CustomResidues.ContainsKey(options.ResidueID)) {
                    selectedMolecule.RenderSettings.CustomResidues.Add(options.ResidueID, options);
                }
            }
            else {
                selectedMolecule.RenderSettings.CustomResidues.Remove(options.ResidueID);
            }

            if (updateModel) {

                if (selectedMolecule.Hidden) {
                    selectedMolecule.PendingRerender = true;
                }
                else {
                    UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
                }
            }
        }

        //public void ResetAllResidueDisplayOptions() {

        //    foreach (KeyValuePair<string, ResidueDisplayOptions> options in selectedMolecule.RenderSettings.ResidueOptions) {

        //        options.Value.SetDefaultOptions();
        //        SaveResidueDisplayOptions(options.Value, true, false);
        //    }

        //    if (selectedMolecule.Hidden) {
        //        selectedMolecule.PendingRerender = true;
        //    }
        //    else {
        //        UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
        //    }
        //}

        public void CloseResidueDisplayOptions() {
            residueDisplayOptionsPanel.SetActive(false);
        }

        //public void ScrollPanelUp() {

        //    int currentLineCount = moleculeResidues.Count / buttonsPerLine;

        //    float scrollAmount = (1.0f / currentLineCount) * scrollStepCount;
        //    ScrollView.verticalNormalizedPosition += scrollAmount;
        //    if (ScrollView.verticalNormalizedPosition > 1) {
        //        ScrollView.verticalNormalizedPosition = 1;
        //    }
        //}

        //public void ScrollPanelDown() {

        //    int currentLineCount = moleculeResidues.Count / buttonsPerLine;

        //    float scrollAmount = (1.0f / currentLineCount) * scrollStepCount;
        //    ScrollView.verticalNormalizedPosition -= scrollAmount;

        //    if (ScrollView.verticalNormalizedPosition < 0) {
        //        ScrollView.verticalNormalizedPosition = 0;
        //    }
        //}
    }
}
