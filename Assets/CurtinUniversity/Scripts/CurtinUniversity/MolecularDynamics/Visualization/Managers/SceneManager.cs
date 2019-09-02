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
        private UserInterface userInterface;

        [SerializeField]
        private MoleculeManager molecules;

        [SerializeField]
        private GameObject sceneCamera;

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
            UserInterfaceEvents.OnSaveMoleculeRenderSettings += saveMoleculeSettings;
            UserInterfaceEvents.OnLoadMoleculeRenderSettings += loadMoleculeSettings;

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
            molecules.UpdateMeshQuality(settings.AutoMeshQuality, settings.MeshQuality);
            molecules.SetAutoRotateSpeed(settings.AutoRotateSpeed);
        }

        private void saveMoleculeSettings(int moleculeID, string filePath, MoleculeRenderSettings renderSettings) {

            SettingsFile settingsFile = new SettingsFile();
            settingsFile.RenderSettings = renderSettings;
            settingsFile.MoleculeTransform = molecules.GetMoleculeTransform(moleculeID);
            settingsFile.CameraTransform = new SerializableTransform(sceneCamera.transform);

            try {

                fsSerializer serializer = new fsSerializer();
                fsData data;
                serializer.TrySerialize<SettingsFile>(settingsFile, out data).AssertSuccessWithoutWarnings();
                string json = fsJsonPrinter.CompressedJson(data);

                if (!filePath.EndsWith(Settings.MDVSettingsFileExtension)) {
                    filePath += Settings.MDVSettingsFileExtension;
                }

                File.WriteAllText(filePath, json);
                userInterface.ShowConsoleMessage("Saved molecule settings to: " + filePath);
            }
            catch (Exception e) {

                userInterface.ShowConsoleError("Error saving molecule settings: " + e.Message);
                return;
            }
        }

        private void loadMoleculeSettings(int moleculeID, string filePath, LoadMoleculeRenderSettingsDelegate loadRenderSettingsCallback) {

            try {

                string json = File.ReadAllText(filePath);
                fsData data = fsJsonParser.Parse(json);
                object settings = null;
                (new fsSerializer()).TryDeserialize(data, typeof(SettingsFile), ref settings).AssertSuccessWithoutWarnings();
                SettingsFile settingsFile = (SettingsFile)settings;

                loadRenderSettingsCallback(moleculeID, settingsFile.RenderSettings);
                molecules.SetMoleculeTransform(moleculeID, settingsFile.MoleculeTransform);
                sceneCamera.transform.position = settingsFile.CameraTransform.Position;
                sceneCamera.transform.rotation = settingsFile.CameraTransform.Rotation;
                sceneCamera.transform.localScale= settingsFile.CameraTransform.Scale;
                userInterface.ShowConsoleMessage("Loaded new settings for molecule from " + filePath);
            }
            catch (Exception e) {
                userInterface.ShowConsoleError("Error loading settings: " + e.Message);
            }
        }
    }
}
