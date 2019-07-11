using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

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
        private Button removeMoleculeButton;

        [SerializeField]
        private Button showHideMoleculeButton;

        [SerializeField]
        private TextMeshProUGUI showHideMoleculeButtonText;

        [SerializeField]
        private MoleculeList molecules;
        private Dictionary<int, MoleculeSettingsPanelListItem> moleculeListItems;

        private HashSet<int> hiddenMolecules;

        public void Awake() {

            moleculeListItems = new Dictionary<int, MoleculeSettingsPanelListItem>();
            hiddenMolecules = new HashSet<int>();

            molecules.SelectedMoleculeID = null;
            removeMoleculeButton.interactable = false;
            removeMoleculeButton.gameObject.SetActive(false);

            showHideMoleculeButton.interactable = false;
            showHideMoleculeButton.gameObject.SetActive(false);
            showHideMoleculeButtonText.text = "Hide Molecule";
        }

        public void OnLoadMoleculeButton() {

            loadFileDialog.SetActive(true);
            LoadFileDialog dialog = loadFileDialog.GetComponent<LoadFileDialog>();
            List<string> validFileExtensions = new List<string>() { ".gro", ".pdb" };
            dialog.Initialise(validFileExtensions, onLoadMoleculeFileSubmitted);
        }

        public void MoleculeLoaded(int id, string name, string description) {

            if (molecules.Contains(id)) {

                console.ShowMessage("Loaded file: " + name + ", [" + description + "]");

                MoleculeSettings settings = molecules.Get(id);
                settings.Name = name;
                settings.Description = description;
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

                removeMoleculeButton.gameObject.SetActive(true);
                removeMoleculeButton.interactable = true;
                showHideMoleculeButton.gameObject.SetActive(true);
                showHideMoleculeButton.interactable = true;
            }
        }

        public void OnShowHideMoleculeButton() {

            if (molecules.SelectedMoleculeID == null) {
                return;
            }

            int moleculeID = (int)molecules.SelectedMoleculeID;

            if (hiddenMolecules.Contains(moleculeID)) {

                molecules.GetSelected().Hidden = false;
                UserInterfaceEvents.RaiseShowMolecule(moleculeID);
                if(molecules.GetSelected().PendingRerender) {
                    UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(moleculeID, molecules.GetSelected().RenderSettings);
                }

                hiddenMolecules.Remove(moleculeID);
                showHideMoleculeButtonText.text = "Hide Molecule";
            }
            else {

                molecules.GetSelected().Hidden = true;
                UserInterfaceEvents.RaiseHideMolecule(moleculeID);
                hiddenMolecules.Add(moleculeID);
                showHideMoleculeButtonText.text = "Show Molecule";
            }
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
            else {
                removeMoleculeButton.gameObject.SetActive(false);
                removeMoleculeButton.interactable = false;
                showHideMoleculeButton.gameObject.SetActive(false);
                showHideMoleculeButton.interactable = false;
            }
        }

        private void onLoadMoleculeFileSubmitted(string fileName, string filePath) {

            console.ShowMessage("Selected file: " + fileName + ", [" + filePath + "]");

            MoleculeSettings molecule = molecules.Add(filePath);

            UserInterfaceEvents.RaiseLoadMolecule(molecule.ID, filePath, molecule.RenderSettings);
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

            if(molecules.SelectedMoleculeID != null) {

                if (hiddenMolecules.Contains((int)molecules.SelectedMoleculeID)) {
                    showHideMoleculeButtonText.text = "Show Molecule";
                }
                else {
                    showHideMoleculeButtonText.text = "Hide Molecule";
                }
            }
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
        }
    }
}
