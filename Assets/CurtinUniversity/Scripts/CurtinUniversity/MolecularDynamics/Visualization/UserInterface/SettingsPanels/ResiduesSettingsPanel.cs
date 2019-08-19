using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    //public delegate void SetCustomColourButtonColour(Color color);
    //public delegate void OpenResidueIDsDelegate(string residueName);
    //public delegate void OpenResidueDisplayOptionsDelegate(ResidueUpdateType residueUpdateType, int residueID);
    //public delegate void SaveResidueNameEnabledDelegate(string residueName, bool enabled, bool updateMolecule = true);
    //public delegate void SaveResidueButtonOptionsDelegate(ResidueUpdateType residueUpdateType, ResidueRenderSettings options, bool updateModel);

    public delegate void ResidueRenderSettingsUpdated();

    public enum ResidueUpdateType {
        ID,
        Name,
        All
    }

    public class ResiduesSettingsPanel : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private TextMeshProUGUI selectedMoleculeText;

        [SerializeField]
        public ResidueNamesPanel residueNamesPanel;

        [SerializeField]
        public GameObject residueNamesPanelGO;

        [SerializeField]
        public GameObject residueIDsPanelGO;

        [SerializeField]
        public GameObject customRenderSettingsPanelGO;


        //[SerializeField]
        //public GameObject residueIDsPanel;


        //[SerializeField]
        //public ScrollRect residueIDsScrollView;

        //[SerializeField]
        //public GameObject residueIDsButtonContent;

        //[SerializeField]
        //public GameObject residueIDButtonPrefab;

        //[SerializeField]
        //public GameObject residueDisplayOptionsPanel;


        private Dictionary<int, PrimaryStructure> primaryStructures;
        //private Dictionary<int, List<string>> moleculeResidueNames;
        //private Dictionary<int, List<int>> moleculeResidueIDs;
        //private Dictionary<int, Dictionary<string, List<int>>> moleculeResidues;

        //private Dictionary<string, ResidueNameButton> residueNameButtons;
        //private Dictionary<int, ResidueIDButton> residueIDButtons;

        //private bool showAllResidueNamesButtonStatus = true;
        //private bool showAllResidueIDsButtonStatus = true;

        private MoleculeSettings selectedMolecule;
        //private string currentlyDisplayedResidue;

        private void Awake() {
            enableResidueSettingsPanels(false);
        }

        private void OnEnable() {

            selectedMolecule = molecules.GetSelected();

            if (selectedMolecule != null) {

                selectedMoleculeText.text = "Modifying settings for molecule  - " + selectedMolecule.Name;

                if (primaryStructures.ContainsKey(selectedMolecule.ID)) {

                    PrimaryStructure primaryStructure = primaryStructures[selectedMolecule.ID];
                    initialiseResidueRenderSettings(primaryStructure);
                    showResidueNamesPanel(selectedMolecule.RenderSettings, primaryStructure);
                }
            }
            else {
                selectedMoleculeText.text = "< no molecule selected >";
            }
        }

        private void OnDisable() {
            enableResidueSettingsPanels(false);
        }

        public void SetPrimaryStructure(int moleculeID, PrimaryStructure primaryStructure) {

            if (primaryStructures == null) {
                primaryStructures = new Dictionary<int, PrimaryStructure>();
            }

            if (!primaryStructures.ContainsKey(moleculeID)) {
                primaryStructures.Add(moleculeID, primaryStructure);
            }
            else {
                primaryStructures[moleculeID] = primaryStructure;
            }
        }

        private void initialiseResidueRenderSettings(PrimaryStructure primaryStructure) {

            // initialise the residue render settings for the molecule
            if (selectedMolecule.RenderSettings.EnabledResidueNames == null) {
                selectedMolecule.RenderSettings.EnabledResidueNames = new HashSet<string>(primaryStructure.ResidueNames);
            }

            if (selectedMolecule.RenderSettings.CustomResidueNames == null) {
                selectedMolecule.RenderSettings.CustomResidueNames = new HashSet<string>();
            }

            if (selectedMolecule.RenderSettings.EnabledResidueIDs == null) {
                selectedMolecule.RenderSettings.EnabledResidueIDs = new HashSet<int>(primaryStructure.ResidueIDs);
            }

            if (selectedMolecule.RenderSettings.CustomResidueRenderSettings == null) {
                selectedMolecule.RenderSettings.CustomResidueRenderSettings = new Dictionary<int, ResidueRenderSettings>();
            }
        }

        private void showResidueNamesPanel(MoleculeRenderSettings renderSettings, PrimaryStructure primaryStructure) {

            residueNamesPanel.Initialise(renderSettings, primaryStructure, updateMoleculeRender);
            residueNamesPanel.gameObject.SetActive(true);
        }

        private void updateMoleculeRender() {

            if (selectedMolecule.Hidden) {
                selectedMolecule.PendingRerender = true;
            }
            else {
                UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
            }
        }

        private void enableResidueSettingsPanels(bool enable) {

            residueNamesPanelGO.gameObject.SetActive(enable);
            residueIDsPanelGO.gameObject.SetActive(enable);
            customRenderSettingsPanelGO.gameObject.SetActive(enable);
        }

        //public void SetModelResidues(int moleculeID, PrimaryStructure primaryStructure) {

        //    if (!primaryStructures.ContainsKey(moleculeID)) {
        //        primaryStructures.Add(moleculeID, primaryStructure);
        //    }
        //    else {
        //        primaryStructures[moleculeID] = primaryStructure;
        //    }

        //    Dictionary<string, HashSet<int>> residues = primaryStructure.GetResidueIDsByName();

        //    if (moleculeResidues == null || moleculeResidueNames == null) {

        //        moleculeResidues = new Dictionary<int, Dictionary<string, List<int>>>();
        //        moleculeResidueIDs = new Dictionary<int, List<int>>();
        //        moleculeResidueNames = new Dictionary<int, List<string>>();
        //    }

        //    // set the molecule residue names
        //    moleculeResidueNames.Add(moleculeID, new List<string>());

        //    foreach (string residueName in residues.Keys) {
        //        moleculeResidueNames[moleculeID].Add(residueName);
        //    }

        //    moleculeResidueNames[moleculeID].Sort();

        //    // set the molecule residue IDs
        //    // while IDs should be unique they might not be, so using a HashSet to aggregate before adding to list
        //    HashSet<int> residueIDsSet = new HashSet<int>();

        //    foreach (HashSet<int> ids in residues.Values) {
        //        foreach (int id in ids) {
        //            residueIDsSet.Add(id);
        //        }
        //    }

        //    List<int> residueIDsList = residueIDsSet.ToList();
        //    residueIDsList.Sort();
        //    moleculeResidueIDs.Add(moleculeID, residueIDsList);

        //    // set the molecule residue IDs by name
        //    moleculeResidues.Add(moleculeID, new Dictionary<string, List<int>>());

        //    foreach (KeyValuePair<string, HashSet<int>> residue in residues) {

        //        List<int> residueIDs = residue.Value.ToList();
        //        residueIDs.Sort();
        //        moleculeResidues[moleculeID].Add(residue.Key, residueIDs);
        //    }
        //}



        //public void ToggleAllResidueNames() {

        //    showAllResidueNamesButtonStatus = !showAllResidueNamesButtonStatus;

        //    foreach (string name in moleculeResidueNames[selectedMolecule.ID]) {
        //        saveResidueNameEnabled(name, showAllResidueNamesButtonStatus, false);
        //        residueNameButtons[name].SetResidueEnabled(showAllResidueNamesButtonStatus);
        //    }

        //    residuesEnableAllButtonText.text = showAllResidueNamesButtonStatus == false ? "Show All": "Hide All";

        //    updateMoleculeRender();
        //}

        //public void ToggleAllResidueIDs() {

        //    //residuesEnableAllButtonStatus = !residuesEnableAllButtonStatus;
        //    //selectedMolecule.RenderSettings.EnabledResidueNames = new HashSet<string>();

        //    //foreach (KeyValuePair<int, ResidueDisplayOptions> options in selectedMolecule.RenderSettings.CustomResidues) {

        //    //    options.Value.Enabled = residuesEnableAllButtonStatus;
        //    //    SaveResidueDisplayOptions(options.Value, true, false);
        //    //}

        //    //if (residuesEnableAllButtonStatus == false) {
        //    //    ResiduesEnableAllButtonText.text = "Show All";
        //    //}
        //    //else {
        //    //    ResiduesEnableAllButtonText.text = "Hide All";
        //    //}

        //    updateMoleculeRender();
        //}

        //// residue name button callbacks

        //private void saveResidueNameEnabled(string residueName, bool enabled, bool updateMolecule = true) {

        //    HashSet<string> enabledResidues = selectedMolecule.RenderSettings.EnabledResidueNames;

        //    if (enabledResidues != null) {
        //        if(enabled) {
        //            enabledResidues.Add(residueName);
        //        }
        //        else {
        //            if (enabledResidues.Contains(residueName)) {
        //                enabledResidues.Remove(residueName);
        //            }
        //        }
        //    }

        //    if (updateMolecule) {
        //        updateMoleculeRender();
        //    }
        //}

        //private void OpenResidueIDsPanel(string residueName) {

        //    if (!moleculeResidues.ContainsKey(selectedMolecule.ID)) {
        //        return;
        //    }

        //    Dictionary<string, List<int>> residues = moleculeResidues[selectedMolecule.ID];

        //    if (!residues.ContainsKey(residueName)) {
        //        return;
        //    }

        //    currentlyDisplayedResidue = residueName;

        //    List<int> residueIDs = residues[residueName];

        //    residueNamesPanel.SetActive(false);

        //    UnityCleanup.DestroyGameObjects(residueIDsButtonContent);
        //    residueIDsScrollView.verticalNormalizedPosition = 1;

        //    // render all the residue id buttons for the given residue
        //    residueIDButtons = new Dictionary<int, ResidueIDButton>();

        //    foreach (int residueID in residueIDs) {

        //        ResidueRenderSettings options;
        //        if(selectedMolecule.RenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
        //            options = selectedMolecule.RenderSettings.CustomResidueRenderSettings[residueID];
        //        }
        //        else {
        //            options = new ResidueRenderSettings(residueID, Settings.ResidueColourDefault);
        //        }

        //        GameObject button = (GameObject)Instantiate(residueIDButtonPrefab, Vector3.zero, Quaternion.identity);
        //        button.GetComponent<Image>().color = new Color(1, 1, 1);

        //        ResidueIDButton buttonScript = button.GetComponent<ResidueIDButton>();
        //        buttonScript.Initialise(options, SaveResidueDisplayOptions, OpenResidueDisplayOptions);

        //        residueIDButtons.Add(residueID, buttonScript);

        //        RectTransform rect = button.GetComponent<RectTransform>();
        //        rect.SetParent(residueIDsButtonContent.GetComponent<RectTransform>(), false);
        //    }

        //    residueIDsPanel.SetActive(true);
        //}

        //public void CloseResidueIDsPanel() {

        //    residueIDsPanel.SetActive(false);
        //    residueNamesPanel.SetActive(true);
        //    currentlyDisplayedResidue = null;
        //}

        //// residue id button callbacks

        //private void OpenResidueDisplayOptions(ResidueUpdateType residueUpdateType, int residueID) {

        //    if (!primaryStructures.ContainsKey(selectedMolecule.ID)) {
        //        return;
        //    }

        //    PrimaryStructure primaryStructure = primaryStructures[selectedMolecule.ID];
        //    List<Residue> residues = primaryStructure.GetResidues(residueID);

        //    if(residues == null || residues.Count == 0) {
        //        return;
        //    }

        //    residueDisplayOptionsPanel.SetActive(true);
        //    ResidueDisplayOptionsPanel displayOptionsPanel = residueDisplayOptionsPanel.GetComponent<ResidueDisplayOptionsPanel>();

        //    if (!selectedMolecule.RenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
        //        selectedMolecule.RenderSettings.CustomResidueRenderSettings.Add(residueID, new ResidueRenderSettings(residueID, Settings.ResidueColourDefault));
        //    }

        //    displayOptionsPanel.Initialise(residues[0], selectedMolecule.RenderSettings.CustomResidueRenderSettings[residueID], residueUpdateType);
        //}

        //public void SaveResidueDisplayOptions(ResidueUpdateType residueUpdateType, ResidueRenderSettings options, bool updateMolecule = true) {

        //    if (!options.IsDefault()) {
        //        if (!selectedMolecule.RenderSettings.CustomResidueRenderSettings.ContainsKey(options.ResidueID)) {
        //            selectedMolecule.RenderSettings.CustomResidueRenderSettings.Add(options.ResidueID, options);
        //        }
        //    }
        //    else {
        //        selectedMolecule.RenderSettings.CustomResidueRenderSettings.Remove(options.ResidueID);
        //    }

        //    // update residue Name button state
        //    updateCustomResidueNames();
        //    displayCustomResidueNames();

        //    if (updateMolecule) {
        //        updateMoleculeRender();
        //    }
        //}

        //public void CloseResidueDisplayOptions() {
        //    residueDisplayOptionsPanel.SetActive(false);
        //}

        //private void updateCustomResidueNames() {

        //    selectedMolecule.RenderSettings.CustomResidueNames = new HashSet<string>();
        //    Dictionary<string, List<int>> residues = moleculeResidues[selectedMolecule.ID];

        //    foreach (KeyValuePair<string, List<int>> residue in residues) {

        //        foreach (int residueID in residue.Value) {
        //            if (selectedMolecule.RenderSettings.CustomResidueRenderSettings.ContainsKey(residueID)) {
        //                selectedMolecule.RenderSettings.CustomResidueNames.Add(residue.Key);
        //            }
        //        }
        //    }
        //}

        //private void displayCustomResidueNames() { 

        //    foreach (KeyValuePair<string, ResidueNameButton> item in residueNameButtons) {
        //        bool residueModified = selectedMolecule.RenderSettings.CustomResidueNames.Contains(item.Key);
        //        item.Value.SetResidueModified(residueModified);
        //    }
        //}

        //public void ResetAllDisplayOptions() {

        //    selectedMolecule.RenderSettings.EnabledResidueNames = null;
        //    selectedMolecule.RenderSettings.EnabledResidueIDs = null;
        //    selectedMolecule.RenderSettings.CustomResidueNames = null;
        //    selectedMolecule.RenderSettings.CustomResidueRenderSettings = null;

        //    initialise();
        //    updateMoleculeRender();
        //}

    }
}
