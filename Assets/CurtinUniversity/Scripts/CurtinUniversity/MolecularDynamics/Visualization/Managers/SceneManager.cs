using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using FullSerializer;

namespace CurtinUniversity.MolecularDynamics.Visualization {

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

        private void Start() {

            // setup UI and Molecule events
            UserInterfaceEvents.OnLoadMolecule += molecules.LoadMolecule;
            UserInterfaceEvents.OnLoadTrajectory += molecules.LoadMoleculeTrajectory;
            UserInterfaceEvents.OnRemoveMolecule += molecules.RemoveMolecule;
            UserInterfaceEvents.OnMoleculeSelected += molecules.SetMoleculeSelected;
            UserInterfaceEvents.OnShowMolecule += molecules.ShowMolecule;
            UserInterfaceEvents.OnHideMolecule += molecules.HideMolecule;
            UserInterfaceEvents.OnMoleculeRenderSettingsUpdated += molecules.UpdateMoleculeRenderSettings;

            UserInterfaceEvents.OnGeneralSettingsUpdated += onGeneralSettingsUpdated;
            UserInterfaceEvents.OnSaveMoleculeSettings += saveSettingsFile;
            UserInterfaceEvents.OnLoadMoleculeSettings += loadSettingsFile;

            MoleculeEvents.OnMoleculeLoaded += userInterface.MoleculeLoaded;
            MoleculeEvents.OnTrajectoryLoaded += userInterface.MoleculeTrajectoryLoaded;
            MoleculeEvents.OnRenderMessage += ShowConsolerMessage;

            userInterface.SetSceneSettings(GeneralSettings.Default());
            loadDefaultModel();
        }

        public void ShowConsolerMessage(string message, bool error) {

            if (error) {
                userInterface.ShowConsoleError(message);
            }
            else {
                userInterface.ShowConsoleMessage(message);
            }
        }

        private void loadDefaultModel() {

            if(!Settings.LoadMoleculeOnStart || Settings.LoadMoleculeFileName == null || Settings.LoadMoleculeFileName.Trim() == "") {
                return;
            }

            string filePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + Settings.LoadMoleculeFileName;

            MoleculeRenderSettings settings = MoleculeRenderSettings.Default();
            userInterface.LoadMolecule(filePath, settings);
        }

        private void onGeneralSettingsUpdated(GeneralSettings settings) {

            scene.Settings = settings;
            //molecules.UpdateMeshQuality(settings.AutoMeshQuality, settings.MeshQuality);
            //molecules.SetAutoRotateSpeed(settings.AutoRotateSpeed);
            //molecules.SetSpaceNavigatorControlEnabled(settings.SpaceNavigatorMoleculeControlEnabled);
            molecules.UpdateGeneralSettings(settings);

            CameraInputSource inputSource = settings.SpaceNavigatorCameraControlEnabled ? CameraInputSource.SpaceNavigator : CameraInputSource.Mouse;
            sceneCamera.SetInputSource(inputSource);
        }

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
                    return;
                }

                if (loadStructure) {

                    if (string.IsNullOrEmpty(settingsFile.StructureFilePath)) {
                        settingsFile.StructureFilePath = null;
                    }
                    else if (!File.Exists(settingsFile.StructureFilePath)) {
                        userInterface.ShowConsoleMessage("Can't load molecule from settings file. File not found: " + settingsFile.StructureFilePath);
                        settingsFile.StructureFilePath = null;
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
            }

            if (settingsFile.CameraTransform != null) {
                sceneCamera.transform.position = settingsFile.CameraTransform.Position;
                sceneCamera.transform.rotation = settingsFile.CameraTransform.Rotation;
                sceneCamera.transform.localScale = settingsFile.CameraTransform.Scale;
            }

            if (settingsFile.RenderSettings != null) {
                loadMoleculeRenderSettingsCallback(moleculeID, settingsFile.RenderSettings);
            }

            userInterface.ShowConsoleMessage("Loaded new settings for molecule from " + filePath);

            loadingSettingsFile = false;
        }
    }
}
