using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

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

        private Dictionary<int, MoleculeSettings> molecules;
        private Dictionary<int, MoleculeListItem> moleculeListItems;

        private HashSet<int> hiddenMolecules;

        private int? selectedMoleculeID;

        public void Awake() {
            molecules = new Dictionary<int, MoleculeSettings>();
            moleculeListItems = new Dictionary<int, MoleculeListItem>();
            hiddenMolecules = new HashSet<int>();

            selectedMoleculeID = null;
            removeMoleculeButton.interactable = false;
            removeMoleculeButton.gameObject.SetActive(false);
        }

        public void OnLoadMoleculeButton() {

            loadFileDialog.SetActive(true);
            LoadFileDialog dialog = loadFileDialog.GetComponent<LoadFileDialog>();
            List<string> validFileExtensions = new List<string>() { ".gro", ".pdb" };
            dialog.Initialise(validFileExtensions, onLoadMoleculeFileSubmitted);
        }

        public void MoleculeLoaded(int id, string name, string description) {

            if (molecules.ContainsKey(id)) {

                console.ShowMessage("Loaded file: " + name + ", [" + description + "]");

                MoleculeSettings settings = molecules[id];
                settings.Name = name;
                settings.Description = description;
                settings.Loaded = true;

                GameObject listItem = GameObject.Instantiate(moleculeListItemPrefab);
                listItem.transform.position = Vector3.zero;
                listItem.transform.rotation = Quaternion.identity;
                listItem.transform.localScale = Vector3.one;
                listItem.SetActive(true);
                listItem.transform.SetParent(moleculeListContent.transform, false);

                MoleculeListItem item = listItem.GetComponent<MoleculeListItem>();
                item.Initialise(id, name, onMoleculeListItemClick, onMoleculeListItemDoubleClick);

                moleculeListItems.Add(id, item);
                numberMoleculeListItems();
            }
        }

        public void OnShowHideMoleculeButton() {

            if (selectedMoleculeID == null) {
                return;
            }

            int moleculeID = (int)selectedMoleculeID;

            if (hiddenMolecules.Contains(moleculeID)) {
                UserInterfaceEvents.RaiseShowMolecule(moleculeID, true);
                hiddenMolecules.Remove(moleculeID);
            }
            else {
                UserInterfaceEvents.RaiseShowMolecule(moleculeID, false);
                hiddenMolecules.Remove(moleculeID);
            }
        }

        public void OnRemoveMoleculeButton() {

            if(selectedMoleculeID == null) {
                return;
            }

            int moleculeID = (int)selectedMoleculeID;

            UserInterfaceEvents.RaiseRemoveMolecule(moleculeID);

            if (molecules.ContainsKey(moleculeID)) {
                molecules.Remove(moleculeID);
            }

            if(moleculeListItems.ContainsKey(moleculeID)) {
                GameObject.Destroy(moleculeListItems[moleculeID].gameObject);
                moleculeListItems.Remove(moleculeID);
            }

            numberMoleculeListItems();
        }

        private void onLoadMoleculeFileSubmitted(string fileName, string filePath) {

            console.ShowMessage("Selected file: " + fileName + ", [" + filePath + "]");

            int moleculeID = molecules.Count + 1;
            MoleculeSettings settings = new MoleculeSettings(moleculeID, filePath);
            molecules.Add(moleculeID, settings);

            UserInterfaceEvents.RaiseLoadMolecule(moleculeID, filePath, settings.RenderSettings);
        }

        // This is purely cosmetic. These numbers have no reference value. 
        // All molecules are referenced by moleculeID
        private void numberMoleculeListItems() {

            int displayID = 0;
            foreach (Transform transform in moleculeListContent.transform) {
                if (transform.gameObject.activeSelf) {
                    transform.gameObject.GetComponent<MoleculeListItem>().DisplayID = ++displayID;
                }
            }
        }

        private void onMoleculeListItemClick(int moleculeID) {

            // if list item for moleculeID is already selected then deselect it
            if(selectedMoleculeID != null && moleculeID == selectedMoleculeID) {
                moleculeListItems[(int)selectedMoleculeID].SetHighlighted(false);
                selectedMoleculeID = null;
            }
            else {
                setMoleculeSelected(moleculeID);
            }

            if(selectedMoleculeID != null) {
                removeMoleculeButton.gameObject.SetActive(true);
                removeMoleculeButton.interactable = true;
            }
            else {
                removeMoleculeButton.gameObject.SetActive(false);
                removeMoleculeButton.interactable = false;
            }
        }

        private void onMoleculeListItemDoubleClick(int moleculeID) {
            // do nothing 
            // Debug.Log("Todo: handle double click molecule list item");
        }

        private void setMoleculeSelected(int moleculeID) {

            selectedMoleculeID = moleculeID;

            foreach(KeyValuePair<int, MoleculeListItem> item in moleculeListItems) {

                if(item.Key == selectedMoleculeID) {
                    item.Value.SetHighlighted(true);
                }
                else {
                    item.Value.SetHighlighted(false);
                }
            }
        }
    }
}
