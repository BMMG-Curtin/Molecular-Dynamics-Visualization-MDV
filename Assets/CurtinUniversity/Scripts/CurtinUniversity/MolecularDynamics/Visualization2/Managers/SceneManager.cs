using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Visualization;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class SceneManager : MonoBehaviour {

        [SerializeField]
        private UserInterface userInterface;

        [SerializeField]
        private Molecules molecules;

        [SerializeField]
        private Scene scene;

        private void Start() {

            Config.LoadConfig();
            Settings.Load();

            scene.Settings = new SceneSettings();
            loadDefaultModel();
        }

        private void loadDefaultModel() {

            if (Config.GetString("LoadMoleculeOnStart") == "True") {

                string filename = Config.GetString("LoadMoleculeFileName");
                if (filename == null || filename.Trim() == "") {
                    return;
                }

                userInterface.ConsoleSetSilent(false);
                string filePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + filename;
                MoleculeRenderSettings settings = new MoleculeRenderSettings();
                molecules.LoadMolecule(filePath, settings, onConsoleMessage, onMoleculeLoaded);
                userInterface.ConsoleSetSilent(false);
            }
        }

        private void onMoleculeLoaded() {

            Debug.Log("Molecule file loaded!");
            StartCoroutine(scene.Lighting.LightToDefaults(1f));
        }

        private void onConsoleMessage(string message, bool error) {

            if (error) {
                userInterface.Console.ShowError(message);
            }
            else {
                userInterface.Console.ShowMessage(message);
            }
        }
    }
}
