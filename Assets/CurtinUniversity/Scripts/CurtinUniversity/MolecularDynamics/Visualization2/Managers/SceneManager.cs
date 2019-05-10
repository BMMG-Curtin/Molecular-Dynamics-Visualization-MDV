using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class SceneManager : MonoBehaviour {

        [SerializeField]
        private Scene scene;

        [SerializeField]
        private UserInterface userInterface;

        [SerializeField]
        private MoleculeManager molecules;

        private void Awake() {

            Settings.Load();
        }

        private void Start() {

            // setup UI and Molecule Render events
            UserInterfaceEvents.OnSceneSettingsUpdated += onSceneSettingsUpdated;
            MoleculeEvents.OnMoleculeLoaded += onMoleculeLoaded;
            MoleculeEvents.OnRenderMessage += onMoleculeRenderMessage;

            SceneSettings sceneSettings = SceneSettings.Default();
            scene.Settings = sceneSettings;
            userInterface.SetSceneSettings(sceneSettings);

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
                molecules.LoadMolecule(filePath, settings);
                userInterface.ConsoleSetSilent(false);
            }
        }

        private void onMoleculeLoaded() {
            StartCoroutine(scene.Lighting.LightToDefaults(1f));
        }

        private void onMoleculeRenderMessage(string message, bool error) {

            if(error) {
                userInterface.ShowConsoleError(message);
            }
            else {
                userInterface.ShowConsoleMessage(message);
            }
        }

        private void onSceneSettingsUpdated(SceneSettings settings) {
            scene.Settings = settings;
        }
    }
}
