using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using FullSerializer;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void LoadMoleculeRenderSettingsDelegate(int moleculeID, MoleculeRenderSettings renderSettings);

    public class MoleculesSettingsPanel : MonoBehaviour {

        [SerializeField]
        private GameObject loadFileDialog;

        [SerializeField]
        private GameObject saveFileDialog;

        [SerializeField]
        private ConfirmDialog confirmDialog;

        [SerializeField]
        private MessageConsole console;

        [SerializeField]
        private GameObject moleculeListItemPrefab;

        [SerializeField]
        private GameObject moleculeListContent;

        [SerializeField]
        private Button loadSettingsButton;

        [SerializeField]
        private Button saveSettingsButton;

        [SerializeField]
        private Button loadTrajectoryButton;

        [SerializeField]
        private TextMeshProUGUI loadTrajectoryButtonText;

        [SerializeField]
        private Button resetMoleculePositionButton;

        [SerializeField]
        private Button showHideMoleculeButton;

        [SerializeField]
        private TextMeshProUGUI showHideMoleculeButtonText;

        [SerializeField]
        private Button removeMoleculeButton;

        [SerializeField]
        private GameObject moleculeInfoPanel;

        [SerializeField]
        private TextMeshProUGUI moleculePathText;

        [SerializeField]
        private TextMeshProUGUI moleculeHeaderText;

        [SerializeField]
        private TextMeshProUGUI moleculeAtomCountText;

        [SerializeField]
        private TextMeshProUGUI moleculeResidueCountText;

        [SerializeField]
        private InteractionsSettingsPanel interactionsPanel;

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private TrajectoryControls trajectoryControls;

        private Dictionary<int, MoleculeSettingsPanelListItem> moleculeListItems;

        private HashSet<int> hiddenMolecules;

        private string savedSettings;

        public void Awake() {

            moleculeListItems = new Dictionary<int, MoleculeSettingsPanelListItem>();
            hiddenMolecules = new HashSet<int>();

            molecules.SelectedMoleculeID = null;
            updateSelectedMoleculeInterfaceSettings();

            trajectoryControls.transform.gameObject.SetActive(false);
        }

        public void OnEnable() {
            SetMoleculeSelected(molecules.SelectedMoleculeID);
        }

        public void OnLoadMoleculeButton() {

            loadFileDialog.SetActive(true);
            LoadFileDialog dialog = loadFileDialog.GetComponent<LoadFileDialog>();
            List<string> validFileExtensions = new List<string>(Settings.StructureFileExtensions);
            validFileExtensions.Add(Settings.SettingsFileExtension);
            dialog.Initialise(validFileExtensions, onLoadMoleculeFileSubmitted);
        }

        public void LoadMolecule(string filePath) {

            if(filePath == null || filePath.Length == 0) {
                return;
            }

            onLoadMoleculeFileSubmitted(filePath);
        }

        public void MoleculeLoaded(int id, string name, string description, int atomCount, int residueCount) {

            if (molecules.Contains(id)) {

                console.ShowMessage("Loaded file: " + name);

                MoleculeSettings settings = molecules.Get(id);
                settings.Name = name;
                settings.Description = description;
                settings.AtomCount = atomCount;
                settings.ResidueCount = residueCount;
                settings.Loaded = true;

                GameObject listItem = GameObject.Instantiate(moleculeListItemPrefab);
                listItem.transform.position = Vector3.zero;
                listItem.transform.rotation = Quaternion.identity;
                listItem.transform.localScale = Vector3.one;
                listItem.SetActive(true);
                listItem.transform.SetParent(moleculeListContent.transform, false);

                MoleculeSettingsPanelListItem item = listItem.GetComponent<MoleculeSettingsPanelListItem>();

                item.Initialise(id, settings.FileName, onMoleculeListItemClick, onMoleculeListItemDoubleClick);

                moleculeListItems.Add(id, item);
                numberMoleculeListItems();

                SetMoleculeSelected(id);
                onMoleculeListItemDoubleClick(id);

                updateSelectedMoleculeInterfaceSettings();
            }
        }

        public void MoleculeLoadFailed(int id) {
            if (molecules.Contains(id)) {
                molecules.Remove(id);
            }
        }

        public void OnLoadRenderSettingsButton() {

            loadFileDialog.SetActive(true);
            LoadFileDialog dialog = loadFileDialog.GetComponent<LoadFileDialog>();
            List<string> validFileExtensions = new List<string>() { Settings.SettingsFileExtension };
            dialog.Initialise(validFileExtensions, onLoadRenderSettingsFileSubmitted);
        }

        public void SetMoleculeSelected(int? moleculeID) {

            molecules.SelectedMoleculeID = moleculeID;

            foreach (KeyValuePair<int, MoleculeSettingsPanelListItem> item in moleculeListItems) {

                item.Value.SetHighlighted(false);
                UserInterfaceEvents.RaiseMoleculeSelected(item.Key, false);
            }

            if (moleculeID != null) {

                foreach (KeyValuePair<int, MoleculeSettingsPanelListItem> item in moleculeListItems) {

                    if (item.Key == molecules.SelectedMoleculeID) {

                        item.Value.SetHighlighted(true);

                        // if molecule is hidden then dont raise event. When molecule is unhidden then an event will be raised instead
                        MoleculeSettings molecule = molecules.Get((int)moleculeID);
                        if (molecule != null && !molecule.Hidden) {
                            UserInterfaceEvents.RaiseMoleculeSelected(item.Key, true);
                        }
                    }
                }
            }

            updateSelectedMoleculeInterfaceSettings();
        }

        private void onLoadRenderSettingsFileSubmitted(string fullPath) { 

            MoleculeSettings molecule = molecules.GetSelected();

            if (molecule == null) {
                return;
            }

            if (!File.Exists(fullPath)) {
                console.ShowError("Cannot load settings, file not found at " + fullPath);
                return;
            }

            confirmDialog.gameObject.SetActive(true);
            confirmDialog.Initialise("Load new camera position from settings file also?", onConfirmLoadTransformsFromSettings, fullPath);
        }

        private void onConfirmLoadTransformsFromSettings(bool confirmed, object data = null) {

            MoleculeSettings molecule = molecules.GetSelected();

            if (molecule == null || data == null) {
                return;
            }

            try {
                UserInterfaceEvents.RaiseLoadMoleculeSettings(molecule.ID, (string)data, false, true, true, true, confirmed, loadRenderSettings);
            }
            catch (InvalidCastException) {
                // do nothing
            }
        }

        private void loadRenderSettings(int moleculeID, MoleculeRenderSettings settings) {

            MoleculeSettings molecule = molecules.Get(moleculeID);

            if (molecule == null) {
                return;
            }

            molecule.RenderSettings = (MoleculeRenderSettings)settings;
            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
        }

        public void OnSaveSettingsButton() {

            MoleculeSettings molecule = molecules.GetSelected();

            if (molecule == null) {
                return;
            }

            saveFileDialog.SetActive(true);
            SaveFileDialog dialog = saveFileDialog.GetComponent<SaveFileDialog>();
            List<string> validFileExtensions = new List<string>() { Settings.SettingsFileExtension };
            dialog.Initialise(validFileExtensions, onSaveSettingsFileSubmitted);
        }

        private void onSaveSettingsFileSubmitted(string fullPath) { 

            MoleculeSettings molecule = molecules.GetSelected();

            if (molecule == null) {
                return;
            }

            UserInterfaceEvents.RaiseSaveMoleculeSettings(molecule, fullPath);
        }

        public void OnLoadTrajectoryButton() {

            loadFileDialog.SetActive(true);
            LoadFileDialog dialog = loadFileDialog.GetComponent<LoadFileDialog>();
            List<string> validFileExtensions = new List<string>(Settings.TrajectoryFileExtensions);
            dialog.Initialise(validFileExtensions, onLoadTrajectoryFileSubmitted);
        }

        public void TrajectoryLoaded(int id, string filePath, int frameCount) {

            if (molecules.Contains(id)) {

                console.ShowMessage("Loaded trajectory. Frame count: " + frameCount);

                MoleculeSettings settings = molecules.Get(id);
                settings.HasTrajectory = true;
                settings.TrajectoryFilePath = filePath;
                settings.TrajectoryFrameCount = frameCount;
                settings.CurrentTrajectoryFrameNumber = null;

                loadTrajectoryButtonText.text = "Update Trajectory";

                updateSelectedMoleculeInterfaceSettings();
            }
        }

        public void OnResetPositionButton() {

            MoleculeSettings molecule = molecules.GetSelected();

            if (molecule == null) {
                return;
            }

            UserInterfaceEvents.RaiseResetMoleculeTransform(molecule.ID);
        }

        public void OnShowHideMoleculeButton() {

            MoleculeSettings molecule = molecules.GetSelected();

            if (molecule == null) {
                return;
            }

            if (hiddenMolecules.Contains(molecule.ID)) {

                molecule.Hidden = false;
                UserInterfaceEvents.RaiseShowMolecule(molecule.ID);
                if(molecule.PendingRerender) {
                    UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
                }

                UserInterfaceEvents.RaiseMoleculeSelected(molecule.ID, true);
                hiddenMolecules.Remove(molecule.ID);
            }
            else {

                molecule.Hidden = true;
                trajectoryControls.StopAnimation();
                UserInterfaceEvents.RaiseHideMolecule(molecule.ID);
                hiddenMolecules.Add(molecule.ID);

                if (interactionsPanel.MonitoringEnabled) {

                    interactionsPanel.StopInteractions();
                    console.ShowError("Stopped monitoring molecular interactions, hiding molecule");
                }

                UserInterfaceEvents.RaiseMoleculeSelected(molecule.ID, false);
            }

            updateSelectedMoleculeInterfaceSettings();
        }

        public void OnRemoveMoleculeButton() {

            if(molecules.SelectedMoleculeID == null) {
                return;
            }

            if (interactionsPanel.MonitoringEnabled) {

                interactionsPanel.StopInteractions();
                console.ShowError("Stopped monitoring molecular interactions, removed molecule");
            }

            int moleculeID = (int)molecules.SelectedMoleculeID;

            UserInterfaceEvents.RaiseRemoveMolecule(moleculeID);

            if(moleculeListItems.ContainsKey(moleculeID)) {
                GameObject.Destroy(moleculeListItems[moleculeID].gameObject);
                moleculeListItems.Remove(moleculeID);
            }

            molecules.Remove(moleculeID, true);
            SetMoleculeSelected(molecules.SelectedMoleculeID);
            numberMoleculeListItems();
            updateSelectedMoleculeInterfaceSettings();
        }

        private void onLoadMoleculeFileSubmitted(string filePath) {

            MoleculeSettings molecule = molecules.Add(filePath);

            if (interactionsPanel.MonitoringEnabled) {

                interactionsPanel.StopInteractions();
                console.ShowError("Stopped monitoring molecular interactions, loading molecule");
            }

            if (filePath.EndsWith(Settings.SettingsFileExtension)) {
                UserInterfaceEvents.RaiseLoadMoleculeSettings(molecule.ID, filePath, true, true, true, true, true, loadRenderSettings);
            }
            else {
                UserInterfaceEvents.RaiseLoadMolecule(molecule.ID, filePath, molecule.RenderSettings);
            }
        }

        private void onLoadTrajectoryFileSubmitted(string filePath) {

            if (molecules.SelectedMoleculeID != null) {

                if (interactionsPanel.MonitoringEnabled) {

                    interactionsPanel.StopInteractions();
                    console.ShowError("Stopped monitoring molecular interactions, loading trajectory");
                }

                UserInterfaceEvents.RaiseLoadTrajectory((int)molecules.SelectedMoleculeID, filePath);
            }
        }

        // This is purely cosmetic. These numbers have no reference value. 
        // All molecules are referenced by moleculeID
        private void numberMoleculeListItems() {

            int displayID = 0;
            foreach(MoleculeSettingsPanelListItem item in moleculeListItems.Values) { 
                item.DisplayID = ++displayID;
            }
        }

        private void onMoleculeListItemClick(int moleculeID) {
            SetMoleculeSelected((int)moleculeID);
        }

        private void onMoleculeListItemDoubleClick(int moleculeID) {
            UserInterfaceEvents.RaiseMoveCameraToMolecule(moleculeID);
        }

        private void updateSelectedMoleculeInterfaceSettings() {

            if (molecules.SelectedMoleculeID == null) {

                loadSettingsButton.interactable = false;
                loadSettingsButton.gameObject.SetActive(false);
                saveSettingsButton.interactable = false;
                saveSettingsButton.gameObject.SetActive(false);
                loadTrajectoryButton.interactable = false;
                loadTrajectoryButton.gameObject.SetActive(false);
                resetMoleculePositionButton.interactable = false;
                resetMoleculePositionButton.gameObject.SetActive(false);
                showHideMoleculeButton.interactable = false;
                showHideMoleculeButton.gameObject.SetActive(false);
                removeMoleculeButton.interactable = false;
                removeMoleculeButton.gameObject.SetActive(false);
                moleculeInfoPanel.SetActive(false);

                trajectoryControls.transform.gameObject.SetActive(false);
            }
            else {

                MoleculeSettings molecule = molecules.GetSelected();

                loadSettingsButton.interactable = true;
                loadSettingsButton.gameObject.SetActive(true);
                saveSettingsButton.interactable = true;
                saveSettingsButton.gameObject.SetActive(true);
                loadTrajectoryButton.interactable = true;
                loadTrajectoryButton.gameObject.SetActive(true);
                resetMoleculePositionButton.interactable = true;
                resetMoleculePositionButton.gameObject.SetActive(true);
                showHideMoleculeButton.interactable = true;
                showHideMoleculeButton.gameObject.SetActive(true);
                removeMoleculeButton.interactable = true;
                removeMoleculeButton.gameObject.SetActive(true);

                if (molecule.HasTrajectory) {

                    loadTrajectoryButtonText.text = "Update Trajectory";
                    trajectoryControls.transform.gameObject.SetActive(true);
                    trajectoryControls.UpdateFrameNumberInfo();
                }
                else {

                    loadTrajectoryButtonText.text = "Load Trajectory";
                    trajectoryControls.transform.gameObject.SetActive(false);
                }

                if (hiddenMolecules.Contains((int)molecule.ID)) {
                    showHideMoleculeButtonText.text = "Show Molecule";
                }
                else {
                    showHideMoleculeButtonText.text = "Hide Molecule";
                }

                moleculeInfoPanel.SetActive(true);

                moleculePathText.text = molecule.FilePath;
                moleculeHeaderText.text = molecule.Description;
                moleculeAtomCountText.text = molecule.AtomCount.ToString();
                moleculeResidueCountText.text = molecule.ResidueCount.ToString();
            }
        }
    }
}
