using System;
using System.Collections;
using System.IO;

using UnityEngine;

using FullSerializer;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    // This is the 'main' class for the application. It should be set at a higher priority to load before 
    // everything else in the application
    public class SceneManager : MonoBehaviour {

        [SerializeField]
        private Scene scene;

        [SerializeField]
        private CameraController sceneCamera;

        [SerializeField]
        private UserInterface userInterface;

        [SerializeField]
        private MoleculeManager molecules;

        private bool loadingSettingsFile = false;

        private void Awake() {
            Settings.Load();
        }

        // The visualisation components are saparated into two groups, the User Interface and the Molecule Rendering
        // The communication between the two groups is via the event assignments below
        private void Start() {

            // setup UI and Molecule events
            UserInterfaceEvents.OnLoadMolecule += molecules.LoadMolecule;
            UserInterfaceEvents.OnLoadTrajectory += molecules.LoadTrajectory;
            UserInterfaceEvents.OnRemoveMolecule += molecules.RemoveMolecule;
            UserInterfaceEvents.OnMoleculeSelected += molecules.SetMoleculeSelected;
            UserInterfaceEvents.OnResetMoleculeTransform += molecules.LoadDefaultMoleculeTransform;
            UserInterfaceEvents.OnEnableMoleculeMovement += molecules.EnableMoleculeInput;
            UserInterfaceEvents.OnShowMolecule += molecules.ShowMolecule;
            UserInterfaceEvents.OnHideMolecule += molecules.HideMolecule;
            UserInterfaceEvents.OnMoleculeRenderSettingsUpdated += molecules.UpdateMoleculeRenderSettings;

            UserInterfaceEvents.OnGeneralSettingsUpdated += onGeneralSettingsUpdated;
            UserInterfaceEvents.OnSaveMoleculeSettings += saveSettingsFile;
            UserInterfaceEvents.OnLoadMoleculeSettings += loadSettingsFile;
            UserInterfaceEvents.OnMoveCameraToMolecule += lookAtMolecule;

            UserInterfaceEvents.OnStartMonitoringMoleculeInteractions += molecules.StartMonitoringInteractions;
            UserInterfaceEvents.OnStopMonitoringMoleculeInteractions += molecules.StopMonitoringInteractions;
            UserInterfaceEvents.OnUpdateMolecularInteractionSettings += molecules.UpdateMolecularInteractionSettings;

            MoleculeEvents.OnMoleculeLoaded += userInterface.MoleculeLoaded;
            MoleculeEvents.OnMoleculeLoadFailed += userInterface.MoleculeLoadFailed;
            MoleculeEvents.OnTrajectoryLoaded += userInterface.MoleculeTrajectoryLoaded;

            MoleculeEvents.OnShowMessage += ShowConsoleMessage;
            MoleculeEvents.OnInteractionsMessage += ShowConsoleMessage;
            MoleculeEvents.OnInteractionsInformation += userInterface.ShowInteractionsInformation;

            userInterface.SetSceneSettings(GeneralSettings.Default());

            StartCoroutine(loadDefaultModel());
        }

        public void ShowConsoleMessage(string message, bool error) {

            if (error) {
                userInterface.ShowConsoleError(message);
            }
            else {
                userInterface.ShowConsoleMessage(message);
            }
        }

        private IEnumerator loadDefaultModel() {

            if(!Settings.LoadMoleculeOnStart) {
                yield break;
            }

            if (!string.IsNullOrEmpty(Settings.LoadMoleculeFileName)) {
                string filePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + Settings.LoadMoleculeFileName;
                userInterface.LoadMolecule(filePath);
            }
        }

        private void onGeneralSettingsUpdated(GeneralSettings settings) {

            scene.Settings = settings;
            molecules.UpdateGeneralSettings(settings);
            sceneCamera.EnableSpaceNavigatorInput(settings.SpaceNavigatorCameraControlEnabled);
        }

        // Settings file management methods below. These don't fit into the UserInterface or Molecule groups 
        // so have left separately here. Could be moved into it's own class but the code but the code is minor 
        // at the moment so have left here.

        private void saveSettingsFile(MoleculeSettings moleculeSettings, string saveFilePath) {

            SettingsFile settingsFile = new SettingsFile();
            settingsFile.StructureFilePath = moleculeSettings.FilePath;
            settingsFile.TrajectoryFilePath = moleculeSettings.TrajectoryFilePath;
            settingsFile.RenderSettings = moleculeSettings.RenderSettings;
            settingsFile.MoleculeTransform = molecules.GetMoleculeTransform(moleculeSettings.ID);
            settingsFile.CameraTransform = new SerializableTransform(sceneCamera.transform);

            try {

                fsSerializer serializer = new fsSerializer();
                fsData data;
                serializer.TrySerialize<SettingsFile>(settingsFile, out data).AssertSuccessWithoutWarnings();
                string json = fsJsonPrinter.CompressedJson(data);

                if (!saveFilePath.EndsWith(Settings.SettingsFileExtension)) {
                    saveFilePath += Settings.SettingsFileExtension;
                }

                File.WriteAllText(saveFilePath, json);
                userInterface.ShowConsoleMessage("Saved molecule settings to: " + saveFilePath);
            }
            catch (Exception e) {

                userInterface.ShowConsoleError("Error saving molecule settings: " + e.Message);
                return;
            }
        }

        private void loadSettingsFile(int moleculeID, string filePath, bool loadStructure, bool loadTrajectory, bool loadRenderSettings, bool loadMoleculeTransform, bool loadCameraTransform, LoadMoleculeRenderSettingsDelegate loadMoleculeRenderSettingsCallback) {

            if (!loadingSettingsFile) {

                loadingSettingsFile = true;

                SettingsFile settingsFile = null;
                try {

                    string json = File.ReadAllText(filePath);
                    fsData data = fsJsonParser.Parse(json);
                    object settings = null;
                    (new fsSerializer()).TryDeserialize(data, typeof(SettingsFile), ref settings).AssertSuccessWithoutWarnings();
                    settingsFile = (SettingsFile)settings;
                }
                catch (Exception e) {

                    userInterface.ShowConsoleError("Error loading settings: " + e.Message);
                    loadingSettingsFile = false;

                    if(loadStructure) {
                        userInterface.MoleculeLoadFailed(moleculeID);
                    }

                    return;
                }

                if (loadStructure) {

                    if (string.IsNullOrEmpty(settingsFile.StructureFilePath)) {
                        settingsFile.StructureFilePath = null;
                    }
                    else if (!File.Exists(settingsFile.StructureFilePath)) {

                        userInterface.ShowConsoleMessage("Can't load molecule from settings file. File not found: " + settingsFile.StructureFilePath);
                        userInterface.MoleculeLoadFailed(moleculeID);
                        loadingSettingsFile = false;
                        return;
                    }
                }
                else {
                    settingsFile.StructureFilePath = null;
                }

                if (settingsFile.StructureFilePath == null) {

                    loadTrajectory = false;
                    settingsFile.TrajectoryFilePath = null;
                }

                if (loadTrajectory) {

                    if (string.IsNullOrEmpty(settingsFile.TrajectoryFilePath)) {
                        settingsFile.TrajectoryFilePath = null;
                    }
                    else if (!File.Exists(settingsFile.TrajectoryFilePath)) {
                        userInterface.ShowConsoleMessage("Can't load trajectory for molecule. File not found: " + settingsFile.TrajectoryFilePath);
                        settingsFile.TrajectoryFilePath = null;
                    }
                }

                if (!loadMoleculeTransform) {
                    settingsFile.MoleculeTransform = null;
                }

                if (!loadCameraTransform) {
                    settingsFile.CameraTransform = null;
                }

                if (!loadRenderSettings) {
                    settingsFile.RenderSettings = null;
                }

                StartCoroutine(loadSettings(settingsFile, moleculeID, filePath, loadMoleculeRenderSettingsCallback));
            }
        }

        private IEnumerator loadSettings(SettingsFile settingsFile, int moleculeID, string filePath, LoadMoleculeRenderSettingsDelegate loadMoleculeRenderSettingsCallback) {

            if (settingsFile.StructureFilePath != null) {

                if (settingsFile.TrajectoryFilePath != null) {
                    yield return molecules.LoadMolecule(moleculeID, settingsFile.StructureFilePath, settingsFile.TrajectoryFilePath, settingsFile.RenderSettings);
                }
                else {
                    yield return molecules.LoadMoleculeStructure(moleculeID, settingsFile.StructureFilePath, settingsFile.RenderSettings);
                }
            }

            if (settingsFile.MoleculeTransform != null) {

                molecules.SetMoleculeTransform(moleculeID, settingsFile.MoleculeTransform);
                molecules.SaveCurrentMoleculeTransformAsDefault(moleculeID);
            }

            if (settingsFile.CameraTransform != null) {
                lookAtMolecule(moleculeID);
            }

            if (settingsFile.RenderSettings != null) {
                loadMoleculeRenderSettingsCallback(moleculeID, settingsFile.RenderSettings);
            }

            userInterface.ShowConsoleMessage("Loaded new settings for molecule from " + filePath);

            loadingSettingsFile = false;
        }

        private void lookAtMolecule(int moleculeID) {

            SerializableTransform moleculetransform = molecules.GetMoleculeTransform(moleculeID);
            BoundingBox moleculeBox = molecules.GetMoleculeBoundingBox(moleculeID);

            if(moleculetransform == null || moleculeBox == null) {
                return;
            }

            float size = Mathf.Max(moleculeBox.Width, moleculeBox.Height, moleculeBox.Depth);

            Vector3 lookAt = moleculetransform.Position;
            Vector3 cameraPosition = lookAt;
            float cameraDistance = Mathf.Max(size * 1.5f, 0.5f);
            cameraPosition.z -= cameraDistance;

            sceneCamera.MoveTo(cameraPosition, lookAt);
        }
    }
}
