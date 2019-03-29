using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResiduesPanel : MonoBehaviour {

        public GameObject ResidueButtonContent;
        public GameObject ResidueButtonPrefab;

        public ScrollRect ScrollView;
        public GameObject ResidueDisplayOptions;
        public GameObject ResidueFilterPanel;
        public Text ResiduesEnableAllButtonText;

        [HideInInspector]
        public HashSet<string> EnabledResidueNames { get { return enabledResidueNames; } }

        [HideInInspector]
        public HashSet<string> CustomDisplayResidues { get { return customDisplayResidues; } }

        [HideInInspector]
        public Dictionary<string, ResidueDisplayOptions> ResidueOptions { get { return residueOptions; } }

        [HideInInspector]
        public HashSet<int> EnabledResideNumbers { get { return ResidueFilterPanel.GetComponent<ResidueFilterPanel>().EnabledResiduesNumbers; } }

        [HideInInspector]
        public bool FilterByNumber { get { return ResidueFilterPanel.GetComponent<ResidueFilterPanel>().EnableFilter.isOn; } }

        public string UpdateAllResiduesKey { get { return "__UPDATEALLRESIDUES__"; } }

        private SceneManager sceneManager;
        private List<string> modelResidues;
        private HashSet<string> enabledResidueNames;
        private HashSet<string> customDisplayResidues;
        private Dictionary<string, ResidueDisplayOptions> residueOptions;
        private Dictionary<string, ResidueButton> residueButtons;

        private int scrollStepCount = 5;
        private int buttonsPerLine = 7; // used to calculate scroll speed

        private bool residuesEnableAllButtonStatus = true;

        void Start() {

            sceneManager = SceneManager.instance;

            if (modelResidues == null) {
                modelResidues = new List<string>();
                Initialise();
            }

            CloseResidueDisplayOptions();
            CloseResidueFilter();
        }

        public void SetModelResidues(HashSet<string> residues) {
            modelResidues = new List<string>();
            foreach (string residue in residues) {
                modelResidues.Add(residue);
            }
            modelResidues.Sort();
            Initialise();
        }

        public bool HasHiddenResidues {
            get {
                if (modelResidues.Count > enabledResidueNames.Count) {
                    return true;
                }
                return false;
            }
        }

        public bool HasCustomDisplayResidues {
            get {
                if (customDisplayResidues.Count > 0) {
                    return true;
                }
                return false;
            }
        }

        public void Initialise() {

            Utility.Cleanup.DestroyGameObjects(ResidueButtonContent);
            ScrollPanelToTop();

            if (modelResidues != null) {
                enabledResidueNames = new HashSet<string>(modelResidues);
            }
            else {
                enabledResidueNames = new HashSet<string>();
            }

            customDisplayResidues = new HashSet<string>();

            ResidueFilterPanel.GetComponent<ResidueFilterPanel>().Initialise();

            if (residueOptions == null) {
                residueOptions = new Dictionary<string, ResidueDisplayOptions>();
            }
            else {
                foreach (KeyValuePair<string, ResidueDisplayOptions> options in residueOptions) {
                    customDisplayResidues.Add(options.Key);
                }
            }

            residueButtons = new Dictionary<string, ResidueButton>();

            foreach (string residue in modelResidues) {

                ResidueDisplayOptions displayOptions;

                if (residueOptions.ContainsKey(residue)) {
                    displayOptions = residueOptions[residue];
                }
                else {
                    displayOptions = new ResidueDisplayOptions(residue, Settings.ResidueColourDefault);
                    residueOptions.Add(residue, displayOptions);
                }

                GameObject button;

                button = (GameObject)Instantiate(ResidueButtonPrefab, Vector3.zero, Quaternion.identity);
                button.GetComponent<Image>().color = new Color(1, 1, 1);

                ResidueButton buttonScript = button.GetComponent<ResidueButton>();
                if(buttonScript == null) {
                    Debug.Log("Button script null in residue panel init");
                }

                buttonScript.SetCallback(displayOptions, new SaveResidueButtonOptionsDelegate(SaveResidueDisplayOptions), new OpenResidueDisplayOptionsDelegate(OpenResidueDisplayOptions));
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
            enabledResidueNames = new HashSet<string>();

            foreach (KeyValuePair<string, ResidueDisplayOptions> options in residueOptions) {

                options.Value.Enabled = residuesEnableAllButtonStatus;
                SaveResidueDisplayOptions(options.Value, true, false);
            }

            if (residuesEnableAllButtonStatus == false) {
                ResiduesEnableAllButtonText.text = "Show All";
            }
            else {
                ResiduesEnableAllButtonText.text = "Hide All";
            }

            StartCoroutine(sceneManager.ReloadModelView(true, false));
        }

        public void OpenResidueDisplayOptions(string residueName) {

            if (residueOptions.ContainsKey(residueName)) {

                ResidueDisplayOptions.SetActive(true);
                ResidueDisplayOptionsPanel displayOptionsPanel = ResidueDisplayOptions.GetComponent<ResidueDisplayOptionsPanel>();
                displayOptionsPanel.Initialise(residueOptions[residueName]);
            }
        }

        public void OpenResidueDisplayOptionsForAllResidues() {

            ResidueDisplayOptions displayOptions = new ResidueDisplayOptions(UpdateAllResiduesKey, Settings.ResidueColourDefault);
            ResidueDisplayOptions.SetActive(true);

            ResidueDisplayOptionsPanel displayOptionsPanel = ResidueDisplayOptions.GetComponent<ResidueDisplayOptionsPanel>();
            displayOptionsPanel.Initialise(displayOptions);
        }


        public void SaveResidueDisplayOptions(ResidueDisplayOptions options, bool updateButton, bool updateModel = true) {

            if (options.ResidueName == UpdateAllResiduesKey) {

                foreach (KeyValuePair<string, ResidueDisplayOptions> oldOptions in residueOptions) {

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
                    if (!enabledResidueNames.Contains(options.ResidueName)) {
                        enabledResidueNames.Add(options.ResidueName);
                    }
                }
                else {
                    enabledResidueNames.Remove(options.ResidueName);
                }

                if (!options.IsDefault()) {
                    if (!customDisplayResidues.Contains(options.ResidueName)) {
                        customDisplayResidues.Add(options.ResidueName);
                    }
                }
                else {
                    customDisplayResidues.Remove(options.ResidueName);
                }
            }

            if (updateModel) {
                StartCoroutine(sceneManager.ReloadModelView(true, true));
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

            foreach (KeyValuePair<string, ResidueDisplayOptions> options in residueOptions) {

                options.Value.SetDefaultOptions();
                SaveResidueDisplayOptions(options.Value, true, false);
            }

            ResidueFilterPanel.GetComponent<ResidueFilterPanel>().SetDefaultOptions();

            StartCoroutine(sceneManager.ReloadModelView(true, true));
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
