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

        private string testStructureFile = @"D:\MolecularModels\MDV\Demo Files\Final Systems\AB.gro";
        private string testTrajectoryFile = @"D:\MolecularModels\MDV\Demo Files\Final Systems\AB_traj.xtc";

        private void Awake() {
            Settings.Load();
        }

        private void Start() {

            // setup UI and Molecule Render events
            UserInterfaceEvents.OnSceneSettingsUpdated += onSceneSettingsUpdated;
            UserInterfaceEvents.OnLoadMolecule += molecules.LoadMolecule;

            MoleculeEvents.OnMoleculeLoaded += onMoleculeLoaded;
            MoleculeEvents.OnMoleculeLoaded += userInterface.MoleculeLoaded;
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

                // need to load settings via UI

                //MoleculeSettings settings = new MoleculeSettings(-99, filePath);
                //molecules.LoadMolecule(-99, filePath, settings.RenderSettings);
                userInterface.ConsoleSetSilent(false);
            }
        }

        private void onMoleculeLoaded(int id, string name, string desc) {
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
