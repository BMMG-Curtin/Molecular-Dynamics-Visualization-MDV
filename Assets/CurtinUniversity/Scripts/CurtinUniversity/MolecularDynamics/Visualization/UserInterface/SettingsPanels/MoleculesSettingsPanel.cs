using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using FullSerializer;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculesSettingsPanel : MonoBehaviour {

        [SerializeField]
        private GameObject loadFileDialog;

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
        private Button lockUnlockMoleculeButton;

        [SerializeField]
        private TextMeshProUGUI lockUnlockMoleculeButtonText;

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
        private MoleculeList molecules;
        private Dictionary<int, MoleculeSettingsPanelListItem> moleculeListItems;

        [SerializeField]
        private TrajectoryControls trajectoryControls;

        private HashSet<int> hiddenMolecules;
        private HashSet<int> movingMolecules;

        public void Awake() {

            moleculeListItems = new Dictionary<int, MoleculeSettingsPanelListItem>();
            hiddenMolecules = new HashSet<int>();
            movingMolecules = new HashSet<int>();

            molecules.SelectedMoleculeID = null;
            updateSelectedMoleculeInterfaceSettings();

            trajectoryControls.transform.gameObject.SetActive(false);
        }

        public void OnLoadMoleculeButton() {

            loadFileDialog.SetActive(true);
            LoadFileDialog dialog = loadFileDialog.GetComponent<LoadFileDialog>();
            List<string> validFileExtensions = new List<string>(Settings.StructureFileExtensions);
            dialog.Initialise(validFileExtensions, onLoadMoleculeFileSubmitted);
        }

        public void LoadMolecule(string filePath, MoleculeRenderSettings? settings) {

            if(filePath == null || filePath.Length == 0) {
                return;
            }

            MoleculeSettings molecule = molecules.Add(filePath);

            if (settings != null) {
                molecule.RenderSettings = (MoleculeRenderSettings)settings;
            }

            UserInterfaceEvents.RaiseLoadMolecule(molecule.ID, filePath, molecule.RenderSettings);
        }

        public void MoleculeLoaded(int id, string name, string description, int atomCount, int residueCount) {

            if (molecules.Contains(id)) {

                //console.ShowMessage("Loaded file: " + name);

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
                item.Initialise(id, name, onMoleculeListItemClick, onMoleculeListItemDoubleClick);

                moleculeListItems.Add(id, item);
                numberMoleculeListItems();

                if(molecules.GetSelected() == null) {
                    setMoleculeSelected(id);
                }

                updateSelectedMoleculeInterfaceSettings();
            }
        }

        public void OnLoadSettingsButton() {


        }

        public void OnSaveSettingsButton() {

            MoleculeSettings molecule = molecules.GetSelected();

            if (molecule == null) {
                return;
            }

            fsSerializer serializer = new fsSerializer();
            fsData data;

            serializer.TrySerialize<MoleculeRenderSettings>(molecule.RenderSettings, out data).AssertSuccessWithoutWarnings();
            
            // string json = fsJsonPrinter.PrettyJson(data);
            string json = fsJsonPrinter.CompressedJson(data);
            Debug.Log("Json:\n" + json);
            Debug.Log("Json Length:\n" + json.Length);
        }

        public void OnLoadTrajectoryButton() {

            loadFileDialog.SetActive(true);
            LoadFileDialog dialog = loadFileDialog.GetComponent<LoadFileDialog>();
            List<string> validFileExtensions = new List<string>(Settings.TrajectoryFileExtensions);
            dialog.Initialise(validFileExtensions, onLoadTrajectoryFileSubmitted);
        }

        public void TrajectoryLoaded(int id, int frameCount) {

            if (molecules.Contains(id)) {

                console.ShowMessage("Loaded trajectory. Frame count: " + frameCount);

                MoleculeSettings settings = molecules.Get(id);
                settings.HasTrajectory = true;
                settings.TrajectoryFrameCount = frameCount;
                settings.CurrentTrajectoryFrameNumber = null;

                loadTrajectoryButtonText.text = "Update Trajectory";

                updateSelectedMoleculeInterfaceSettings();
            }
        }

        public void OnMoveReleaseMoleculeButton() {

            MoleculeSettings molecule = molecules.GetSelected();

            if (molecule == null) {
                return;
            }

            if (movingMolecules.Contains(molecule.ID)) {

                UserInterfaceEvents.RaiseEnableMoveMolecule(molecule.ID);
                movingMolecules.Remove(molecule.ID);
            }
            else {

                UserInterfaceEvents.RaiseDisableMoveMolecule(molecule.ID);
                movingMolecules.Add(molecule.ID);
            }

            updateSelectedMoleculeInterfaceSettings();
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

                hiddenMolecules.Remove(molecule.ID);
            }
            else {

                molecule.Hidden = true;
                trajectoryControls.StopAnimation();
                UserInterfaceEvents.RaiseHideMolecule(molecule.ID);
                hiddenMolecules.Add(molecule.ID);

                // stop moving if we hide the molecule
                if (movingMolecules.Contains(molecule.ID)) {

                    UserInterfaceEvents.RaiseEnableMoveMolecule(molecule.ID);
                    movingMolecules.Remove(molecule.ID);
                }
            }

            updateSelectedMoleculeInterfaceSettings();
        }

        public void OnRemoveMoleculeButton() {

            if(molecules.SelectedMoleculeID == null) {
                return;
            }

            int moleculeID = (int)molecules.SelectedMoleculeID;

            UserInterfaceEvents.RaiseRemoveMolecule(moleculeID);

            molecules.Remove(moleculeID);

            if(moleculeListItems.ContainsKey(moleculeID)) {
                GameObject.Destroy(moleculeListItems[moleculeID].gameObject);
                moleculeListItems.Remove(moleculeID);
            }

            numberMoleculeListItems();

            // set a new molecule as selected if one available
            int? selected = molecules.SetFirstMoleculeSelected();
            if(selected != null) {

                foreach (KeyValuePair<int, MoleculeSettingsPanelListItem> item in moleculeListItems) {

                    if (item.Key == molecules.SelectedMoleculeID) {
                        item.Value.SetHighlighted(true);
                    }
                    else {
                        item.Value.SetHighlighted(false);
                    }
                }
            }

            updateSelectedMoleculeInterfaceSettings();
        }

        private void onLoadMoleculeFileSubmitted(string fileName, string filePath) {

            console.ShowMessage("Selected file: " + fileName + ", [" + filePath + "]");

            MoleculeSettings molecule = molecules.Add(filePath);

            UserInterfaceEvents.RaiseLoadMolecule(molecule.ID, filePath, molecule.RenderSettings);
        }

        private void onLoadTrajectoryFileSubmitted(string fileName, string filePath) {

            //console.ShowMessage("Selected file: " + fileName + ", [" + filePath + "]");

            if (molecules.SelectedMoleculeID != null) {
                UserInterfaceEvents.RaiseLoadTrajectory((int)molecules.SelectedMoleculeID, filePath);
            }
        }

        // This is purely cosmetic. These numbers have no reference value. 
        // All molecules are referenced by moleculeID
        private void numberMoleculeListItems() {

            int displayID = 0;
            foreach (Transform transform in moleculeListContent.transform) {
                if (transform.gameObject.activeSelf) {
                    transform.gameObject.GetComponent<MoleculeSettingsPanelListItem>().DisplayID = ++displayID;
                }
            }
        }

        private void onMoleculeListItemClick(int moleculeID) {
            setMoleculeSelected(moleculeID);
        }

        private void onMoleculeListItemDoubleClick(int moleculeID) {
            // do nothing at present
        }

        private void setMoleculeSelected(int moleculeID) {

            molecules.SelectedMoleculeID = moleculeID;

            foreach(KeyValuePair<int, MoleculeSettingsPanelListItem> item in moleculeListItems) {

                if(item.Key == molecules.SelectedMoleculeID) {
                    item.Value.SetHighlighted(true);
                }
                else {
                    item.Value.SetHighlighted(false);
                }
            }

            updateSelectedMoleculeInterfaceSettings();
        }

        private void updateSelectedMoleculeInterfaceSettings() {

            if (molecules.SelectedMoleculeID == null) {

                loadSettingsButton.interactable = false;
                loadSettingsButton.gameObject.SetActive(false);
                saveSettingsButton.interactable = false;
                saveSettingsButton.gameObject.SetActive(false);
                loadTrajectoryButton.interactable = false;
                loadTrajectoryButton.gameObject.SetActive(false);
                lockUnlockMoleculeButton.interactable = false;
                lockUnlockMoleculeButton.gameObject.SetActive(false);
                showHideMoleculeButton.interactable = false;
                showHideMoleculeButton.gameObject.SetActive(false);
                removeMoleculeButton.interactable = false;
                removeMoleculeButton.gameObject.SetActive(false);
                moleculeInfoPanel.SetActive(false);
            }
            else {

                MoleculeSettings molecule = molecules.GetSelected();

                loadSettingsButton.interactable = true;
                loadSettingsButton.gameObject.SetActive(true);
                saveSettingsButton.interactable = true;
                saveSettingsButton.gameObject.SetActive(true);
                loadTrajectoryButton.interactable = true;
                loadTrajectoryButton.gameObject.SetActive(true);
                lockUnlockMoleculeButton.interactable = true;
                lockUnlockMoleculeButton.gameObject.SetActive(true);
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

                if (movingMolecules.Contains((int)molecule.ID)) {
                    lockUnlockMoleculeButtonText.text = "Release Molecule";
                }
                else {
                    lockUnlockMoleculeButtonText.text = "Move Molecule";
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
