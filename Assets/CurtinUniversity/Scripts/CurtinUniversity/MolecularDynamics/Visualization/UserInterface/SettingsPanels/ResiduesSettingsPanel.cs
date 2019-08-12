using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void SetCustomColourButtonColour(Color color);
    public delegate void OpenResidueDisplayOptionsDelegate(string residueName);
    //public delegate void SaveResidueButtonOptionsDelegate(ResidueDisplayOptions options, bool updateButton, bool updateModel = true);
    public delegate void SaveResidueButtonOptionsDelegate(ResidueDisplayOptions options, bool updateButton, bool updateModel);

    public class ResiduesSettingsPanel : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private TextMeshProUGUI selectedMoleculeText;

        public GameObject ResidueButtonContent;
        public GameObject ResidueButtonPrefab;

        public ScrollRect ScrollView;
        public GameObject ResidueDisplayOptions;
        public GameObject ResidueFilterPanel;
        public Text ResiduesEnableAllButtonText;

        public string UpdateAllResiduesKey { get { return "__UPDATEALLRESIDUES__"; } }

        private Dictionary<int, List<string>> modelResidues;
        private Dictionary<string, ResidueButton> residueButtons;

        private int scrollStepCount = 5;
        private int buttonsPerLine = 7; // used to calculate scroll speed

        private bool residuesEnableAllButtonStatus = true;

        private MoleculeSettings selectedMolecule;

        private void Awake() {

            if (modelResidues == null) {
                modelResidues = new Dictionary<int, List<string>>();
            }

            CloseResidueFilter();
            CloseResidueDisplayOptions();
        }

        public void OnEnable() {

            selectedMolecule = molecules.GetSelected();
            initialise();

            if (selectedMolecule != null) {
                selectedMoleculeText.text = "Modifying settings for molecule  - " + selectedMolecule.Name;
            }
            else {
                selectedMoleculeText.text = "< no molecule selected >";
            }
        }

        public void SetModelResidues(int moleculeID, HashSet<string> residues) {

            if (modelResidues == null) {
                modelResidues = new Dictionary<int, List<string>>();
            }

            modelResidues.Add(moleculeID, new List<string>());
            foreach (string residue in residues) {
                modelResidues[moleculeID].Add(residue);
            }

            modelResidues[moleculeID].Sort();
        }

        private void initialise() {

            Utility.Cleanup.DestroyGameObjects(ResidueButtonContent);
            ScrollPanelToTop();

            if (selectedMolecule == null || modelResidues == null || !modelResidues.ContainsKey(selectedMolecule.ID)) {
                return;
            }

            if (selectedMolecule.RenderSettings.EnabledResidueNames == null) {
                if (modelResidues.ContainsKey(selectedMolecule.ID)) {
                    selectedMolecule.RenderSettings.EnabledResidueNames = new HashSet<string>(modelResidues[selectedMolecule.ID]);
                }
                else {
                    selectedMolecule.RenderSettings.EnabledResidueNames = new HashSet<string>();
                }
            }

            if (selectedMolecule.RenderSettings.CustomDisplayResidues == null) {
                selectedMolecule.RenderSettings.CustomDisplayResidues = new HashSet<string>();
            }

            ResidueFilterPanel.GetComponent<ResidueFilterPanel>().Initialise();

            if (selectedMolecule.RenderSettings.ResidueOptions == null) {
                selectedMolecule.RenderSettings.ResidueOptions = new Dictionary<string, ResidueDisplayOptions>();
            }
            else {
                foreach (KeyValuePair<string, ResidueDisplayOptions> options in selectedMolecule.RenderSettings.ResidueOptions) {
                    selectedMolecule.RenderSettings.CustomDisplayResidues.Add(options.Key);
                }
            }

            residueButtons = new Dictionary<string, ResidueButton>();

            foreach (string residue in modelResidues[selectedMolecule.ID]) {

                ResidueDisplayOptions displayOptions;

                if (selectedMolecule.RenderSettings.ResidueOptions.ContainsKey(residue)) {
                    displayOptions = selectedMolecule.RenderSettings.ResidueOptions[residue];
                }
                else {
                    displayOptions = new ResidueDisplayOptions(residue, Settings.ResidueColourDefault);
                    selectedMolecule.RenderSettings.ResidueOptions.Add(residue, displayOptions);
                }

                GameObject button;

                button = (GameObject)Instantiate(ResidueButtonPrefab, Vector3.zero, Quaternion.identity);
                button.GetComponent<Image>().color = new Color(1, 1, 1);

                ResidueButton buttonScript = button.GetComponent<ResidueButton>();
                if(buttonScript == null) {
                    Debug.Log("Button script null in residue panel init");
                }

                buttonScript.Initialise(displayOptions, SaveResidueDisplayOptions, OpenResidueDisplayOptions);
                buttonScript.ResidueName = residue;
                button.GetComponentInChildren<Text>().text = residue.Trim();

                residueButtons.Add(residue, buttonScript);

                RectTransform rect = button.GetComponent<RectTransform>();
                rect.SetParent(ResidueButtonContent.GetComponent<RectTransform>());
                rect.localPosition = new Vector3(0, 0, 0);
                rect.localRotation = Quaternion.Euler(0, 0, 0);
                rect.localScale = Vector3.one;
            }
        }

        public void ToggleAllResidues() {

            residuesEnableAllButtonStatus = !residuesEnableAllButtonStatus;
            selectedMolecule.RenderSettings.EnabledResidueNames = new HashSet<string>();

            foreach (KeyValuePair<string, ResidueDisplayOptions> options in selectedMolecule.RenderSettings.ResidueOptions) {

                options.Value.Enabled = residuesEnableAllButtonStatus;
                SaveResidueDisplayOptions(options.Value, true, false);
            }

            if (residuesEnableAllButtonStatus == false) {
                ResiduesEnableAllButtonText.text = "Show All";
            }
            else {
                ResiduesEnableAllButtonText.text = "Hide All";
            }

            selectedMolecule.RenderSettings.FilterResiduesByNumber = ResidueFilterPanel.GetComponent<ResidueFilterPanel>().EnableFilter.isOn;
            selectedMolecule.RenderSettings.EnabledResidueNumbers = ResidueFilterPanel.GetComponent<ResidueFilterPanel>().EnabledResiduesNumbers;

            if (selectedMolecule.Hidden) {
                selectedMolecule.PendingRerender = true;
            }
            else {
                UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
            }
        }

        public void OpenResidueDisplayOptions(string residueName) {

            if (selectedMolecule.RenderSettings.ResidueOptions.ContainsKey(residueName)) {

                ResidueDisplayOptions.SetActive(true);
                ResidueDisplayOptionsPanel displayOptionsPanel = ResidueDisplayOptions.GetComponent<ResidueDisplayOptionsPanel>();
                displayOptionsPanel.Initialise(selectedMolecule.RenderSettings.ResidueOptions[residueName]);
            }
        }

        public void OpenResidueDisplayOptionsForAllResidues() {

            ResidueDisplayOptions displayOptions = new ResidueDisplayOptions(UpdateAllResiduesKey, Visualization.Settings.ResidueColourDefault);
            ResidueDisplayOptions.SetActive(true);

            ResidueDisplayOptionsPanel displayOptionsPanel = ResidueDisplayOptions.GetComponent<ResidueDisplayOptionsPanel>();
            displayOptionsPanel.Initialise(displayOptions);
        }

        public void SaveResidueDisplayOptions(ResidueDisplayOptions options, bool updateButton, bool updateModel = true) {

            if (options.ResidueName == UpdateAllResiduesKey) {

                foreach (KeyValuePair<string, ResidueDisplayOptions> oldOptions in selectedMolecule.RenderSettings.ResidueOptions) {

                    string residueName = oldOptions.Value.ResidueName;
                    oldOptions.Value.Clone(options);
                    oldOptions.Value.ResidueName = residueName;
                    SaveResidueDisplayOptions(oldOptions.Value, true, false);

                    if (options.Enabled) {
                        ResiduesEnableAllButtonText.text = "Hide All";
                        residuesEnableAllButtonStatus = true;
                    }
                    else {
                        ResiduesEnableAllButtonText.text = "Show All";
                        residuesEnableAllButtonStatus = false;
                    }
                }
            }
            else {

                // update residue button state
                if (updateButton) {
                    residueButtons[options.ResidueName].UpdateResidueOptions(options);
                }

                // update state lists
                if (options.Enabled) {
                    if (!selectedMolecule.RenderSettings.EnabledResidueNames.Contains(options.ResidueName)) {
                        selectedMolecule.RenderSettings.EnabledResidueNames.Add(options.ResidueName);
                    }
                }
                else {
                    selectedMolecule.RenderSettings.EnabledResidueNames.Remove(options.ResidueName);
                }

                if (!options.IsDefault()) {
                    if (!selectedMolecule.RenderSettings.CustomDisplayResidues.Contains(options.ResidueName)) {
                        selectedMolecule.RenderSettings.CustomDisplayResidues.Add(options.ResidueName);
                    }
                }
                else {
                    selectedMolecule.RenderSettings.CustomDisplayResidues.Remove(options.ResidueName);
                }
            }

            if (updateModel) {

                selectedMolecule.RenderSettings.FilterResiduesByNumber = ResidueFilterPanel.GetComponent<ResidueFilterPanel>().EnableFilter.isOn;
                selectedMolecule.RenderSettings.EnabledResidueNumbers = ResidueFilterPanel.GetComponent<ResidueFilterPanel>().EnabledResiduesNumbers;

                if (selectedMolecule.Hidden) {
                    selectedMolecule.PendingRerender = true;
                }
                else {
                    UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
                }
            }
        }

        public void CloseResidueDisplayOptions() {
            ResidueDisplayOptions.SetActive(false);
        }

        public void OpenResidueFilter() {
            ResidueFilterPanel.SetActive(true);
        }

        public void CloseResidueFilter() {
            ResidueFilterPanel.SetActive(false);
        }

        public void ResetAllResidueDisplayOptions() {

            foreach (KeyValuePair<string, ResidueDisplayOptions> options in selectedMolecule.RenderSettings.ResidueOptions) {

                options.Value.SetDefaultOptions();

                selectedMolecule.RenderSettings.FilterResiduesByNumber = ResidueFilterPanel.GetComponent<ResidueFilterPanel>().EnableFilter.isOn;
                selectedMolecule.RenderSettings.EnabledResidueNumbers = ResidueFilterPanel.GetComponent<ResidueFilterPanel>().EnabledResiduesNumbers;
                SaveResidueDisplayOptions(options.Value, true, false);
            }

            ResidueFilterPanel.GetComponent<ResidueFilterPanel>().SetDefaultOptions();

            if (selectedMolecule.Hidden) {
                selectedMolecule.PendingRerender = true;
            }
            else {
                UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
            }
        }

        public void ScrollPanelToTop() {
            ScrollView.verticalNormalizedPosition = 1;
        }

        public void ScrollPanelUp() {

            int currentLineCount = modelResidues.Count / buttonsPerLine;

            float scrollAmount = (1.0f / currentLineCount) * scrollStepCount;
            ScrollView.verticalNormalizedPosition += scrollAmount;
            if (ScrollView.verticalNormalizedPosition > 1) {
                ScrollView.verticalNormalizedPosition = 1;
            }
        }

        public void ScrollPanelDown() {

            int currentLineCount = modelResidues.Count / buttonsPerLine;

            float scrollAmount = (1.0f / currentLineCount) * scrollStepCount;
            ScrollView.verticalNormalizedPosition -= scrollAmount;

            if (ScrollView.verticalNormalizedPosition < 0) {
                ScrollView.verticalNormalizedPosition = 0;
            }
        }
    }
}
