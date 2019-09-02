using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

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
            UserInterfaceEvents.OnLoadMolecule += molecules.LoadMolecule;
            UserInterfaceEvents.OnLoadTrajectory += molecules.LoadMoleculeTrajectory;
            UserInterfaceEvents.OnRemoveMolecule += molecules.RemoveMolecule;
            UserInterfaceEvents.OnEnableMoveMolecule += molecules.EnableMoveMolecule;
            UserInterfaceEvents.OnDisableMoveMolecule += molecules.DisableMoveMolecule;
            UserInterfaceEvents.OnShowMolecule += molecules.ShowMolecule;
            UserInterfaceEvents.OnHideMolecule += molecules.HideMolecule;
            UserInterfaceEvents.OnMoleculeRenderSettingsUpdated += molecules.UpdateMoleculeRenderSettings;
            UserInterfaceEvents.OnGeneralSettingsUpdated += onGeneralSettingsUpdated;

            //MoleculeEvents.OnMoleculeLoaded += onMoleculeLoaded;
            MoleculeEvents.OnMoleculeLoaded += userInterface.MoleculeLoaded;
            MoleculeEvents.OnTrajectoryLoaded += userInterface.MoleculeTrajectoryLoaded;
            MoleculeEvents.OnRenderMessage += onMoleculeRenderMessage;

            userInterface.SetSceneSettings(GeneralSettings.Default());
            loadDefaultModel();
        }

        private void loadDefaultModel() {

            if(!Settings.LoadMoleculeOnStart || Settings.LoadMoleculeFileName == null || Settings.LoadMoleculeFileName.Trim() == "") {
                return;
            }

            string filePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + Settings.LoadMoleculeFileName;

            MoleculeRenderSettings settings = MoleculeRenderSettings.Default();
            userInterface.LoadMolecule(filePath, settings);
        }

        private void onMoleculeRenderMessage(string message, bool error) {

            if(error) {
                userInterface.ShowConsoleError(message);
            }
            else {
                userInterface.ShowConsoleMessage(message);
            }
        }

        private void onGeneralSettingsUpdated(GeneralSettings settings) {

            scene.Settings = settings;
            molecules.UpdateMeshQuality(settings.AutoMeshQuality, settings.MeshQuality);
            molecules.SetAutoRotateSpeed(settings.AutoRotateSpeed);
        }
    }
}
