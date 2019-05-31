using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public struct Molecule

    public class MoleculesSettings : MonoBehaviour {

        public GameObject LoadFileDialog;
        public MessageConsole Console;

        public void OnLoadMoleculeButton() {

            LoadFileDialog.SetActive(true);
            FileBrowserDialog dialog = LoadFileDialog.GetComponent<FileBrowserDialog>();
            List<string> validFileExtensions = new List<string>() { ".gro", ".pdb" };
            dialog.Initialise(validFileExtensions, onLoadMoleculeFileSubmitted);
        }

        private void onLoadMoleculeFileSubmitted(string fileName, string filePath) {

            Console.ShowMessage("Selected file: " + fileName + ", [" + filePath + "]");

            int id = 1;
            MoleculeRenderSettings settings = MoleculeRenderSettings.Default();
            UserInterfaceEvents.RaiseOnLoadMolecule(id, filePath, settings);


        }

        public void OnRemoveMolecule(int id) {
            UserInterfaceEvents.RaiseOnRemoveMolecule(id);
        }
    }
}
