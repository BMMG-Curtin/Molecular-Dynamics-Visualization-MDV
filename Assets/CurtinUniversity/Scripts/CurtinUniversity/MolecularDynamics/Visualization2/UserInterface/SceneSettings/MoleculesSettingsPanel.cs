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

        private Dictionary<int, MoleculeSettings> molecules;
        private Dictionary<int, MoleculeListItem> moleculeListItems;

        private int? selectedMoleculeID;

        public void Awake() {
            molecules = new Dictionary<int, MoleculeSettings>();
            moleculeListItems = new Dictionary<int, MoleculeListItem>();
            selectedMoleculeID = null;
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

        public void OnRemoveMolecule(int id) {
            UserInterfaceEvents.RaiseRemoveMolecule(id);
        }

        private void onLoadMoleculeFileSubmitted(string fileName, string filePath) {

            console.ShowMessage("Selected file: " + fileName + ", [" + filePath + "]");

            int moleculeID = molecules.Count + 1;
            MoleculeSettings settings = new MoleculeSettings(moleculeID, filePath);
            molecules.Add(moleculeID, settings);

            UserInterfaceEvents.RaiseLoadMolecule(moleculeID, filePath, settings.RenderSettings);
        }

        // This is purely cosmetic. These numbers have no reference value. 
        // All molecules are references by moleculeID
        private void numberMoleculeListItems() {

            int displayID = 0;
            foreach (Transform transform in moleculeListContent.transform) {
                if (transform.gameObject.activeSelf) {
                    transform.gameObject.GetComponent<MoleculeListItem>().DisplayID = ++displayID;
                }
            }
        }

        private void onMoleculeListItemClick(int moleculeID) {

            Debug.Log("Single click molecule id: " + moleculeID);
            setMoleculeSelected(moleculeID);

        }

        private void onMoleculeListItemDoubleClick(int moleculeID) {

            Debug.Log("Double click molecule id: " + moleculeID);
        }

        private void setMoleculeSelected(int moleculeID) {

            Debug.Log("Setting selected: " + moleculeID);

            selectedMoleculeID = moleculeID;

            foreach(KeyValuePair<int, MoleculeListItem> item in moleculeListItems) {

                Debug.Log("Checking molecule ID: " + item.Key);

                if(item.Key == selectedMoleculeID) {
                    Debug.Log("Setting highlighted: " + item.Key);
                    item.Value.SetHighlighted(true);
                }
                else {
                    Debug.Log("Setting not highlighted: " + item.Key);
                    item.Value.SetHighlighted(false);
                }
            }
        }
    }
}
