using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

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

            // setup UI and Molecule events
            UserInterfaceEvents.OnLoadMolecule += onLoadMolecule;
            UserInterfaceEvents.OnLoadTrajectory += molecules.LoadMoleculeTrajectory;
            UserInterfaceEvents.OnRemoveMolecule += molecules.RemoveMolecule;
            UserInterfaceEvents.OnEnableMoveMolecule += molecules.EnableMoveMolecule;
            UserInterfaceEvents.OnDisableMoveMolecule += molecules.DisableMoveMolecule;
            UserInterfaceEvents.OnShowMolecule += molecules.ShowMolecule;
            UserInterfaceEvents.OnHideMolecule += molecules.HideMolecule;
            UserInterfaceEvents.OnMoleculeRenderSettingsUpdated += molecules.UpdateMoleculeRenderSettings;
            UserInterfaceEvents.OnSceneSettingsUpdated += onSceneSettingsUpdated;

            MoleculeEvents.OnMoleculeLoaded += onMoleculeLoaded;
            MoleculeEvents.OnMoleculeLoaded += userInterface.MoleculeLoaded;
            MoleculeEvents.OnTrajectoryLoaded += userInterface.MoleculeTrajectoryLoaded;
            MoleculeEvents.OnRenderMessage += onMoleculeRenderMessage;

            SceneSettings sceneSettings = SceneSettings.Default();
            userInterface.SetSceneSettings(sceneSettings);
            scene.Settings = sceneSettings;
            scene.Lighting.SetLighting(Vector3.zero, 20f, 20f);
            scene.Lighting.Brightness = 1f;

            loadDefaultModel();
        }

        private void loadDefaultModel() {

            if(!Settings.LoadMoleculeOnStart || Settings.LoadMoleculeFileName == null || Settings.LoadMoleculeFileName.Trim() == "") {
                    return;
            }

            userInterface.ConsoleSetSilent(false);
            string filePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + Settings.LoadMoleculeFileName;

            MoleculeRenderSettings settings = MoleculeRenderSettings.Default();
            settings.ShowAtoms = false;
            settings.ShowSimulationBox = false;
            userInterface.LoadMolecule(filePath, settings);

            userInterface.ConsoleSetSilent(false);
        }

        private void onLoadMolecule(int moleculeID, string filePath, MoleculeRenderSettings settings) {
            StartCoroutine(loadMolecule(moleculeID, filePath, settings));
        }

        private IEnumerator loadMolecule(int moleculeID, string filePath, MoleculeRenderSettings settings) {

            yield return StartCoroutine(scene.Lighting.DimToBlack(0.5f));
            molecules.LoadMolecule(moleculeID, filePath, settings);
        }

        private void onMoleculeLoaded(int id, string name, string desc, HashSet<string> elements, Dictionary<string, HashSet<int>> residues, int atomCount, int residueCount) {
            StartCoroutine(scene.Lighting.LightToDefaults(0.5f));
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
