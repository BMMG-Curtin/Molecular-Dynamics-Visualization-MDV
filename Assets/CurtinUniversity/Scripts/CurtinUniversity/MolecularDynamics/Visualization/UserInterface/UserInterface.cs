﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class UserInterface : MonoBehaviour {

        // inspector properties
        [SerializeField]
        private GameObject UserInterfaceCanvas;

        [SerializeField]
        private GameObject SettingsGO;

        [SerializeField]
        private GameObject ConsoleGO;

        [SerializeField]
        private GameObject TrajectoryControlsGO;

        [SerializeField]
        private MessageConsole console;

        [SerializeField]
        private OtherSettingsPanel applicationSettings;

        [SerializeField]
        private MoleculesSettingsPanel moleculeSettingsPanel;

        [SerializeField]
        private ElementsSettingsPanel elementsSettingsPanel;

        [SerializeField]
        private ResiduesSettingsPanel residuesSettingsPanel;

        [SerializeField]
        private bool toggleWholeInterface = false;

        public bool IsActive { get { return UserInterfaceCanvas.activeSelf; } }

        int frameCount;
        int reportFrequency = 1; // in seconds
        float timer;

        private void Start() {

            HideUserInterface();
        }

        private void Update() {

            // if the window loses focus (e.g. alt-tab) then the cursor gets unlocked.
            // need to lock again when user uses the application.
            if (Cursor.lockState != CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.BackQuote)) {
                ToogleUserInterface();
            }

            if (Settings.DebugMessages) {

                timer += Time.unscaledDeltaTime;
                frameCount += 1;

                if (timer >= reportFrequency) {
                    if (console == null) {
                        Debug.Log("Console is null");
                    }
                    console.BannerFPS = (frameCount / reportFrequency).ToString();
                    timer = frameCount = 0;
                }
            }
        }

        public void SetSceneSettings(GeneralSettings settings) {
            applicationSettings.SetSceneSettings(settings);
        }

        public void LoadMolecule(string filePath, MoleculeRenderSettings settings = null) {
            moleculeSettingsPanel.LoadMolecule(filePath, settings);
        }

        public void MoleculeLoaded(int id, string name, PrimaryStructure primaryStructure) {

            moleculeSettingsPanel.MoleculeLoaded(id, name, primaryStructure.Title, primaryStructure.AtomCount(), primaryStructure.ResidueCount());
            elementsSettingsPanel.SetModelElements(id, primaryStructure.ElementNames); 
            residuesSettingsPanel.SetPrimaryStructure(id, primaryStructure); 
        }

        public void MoleculeTrajectoryLoaded(int moleculeID, string filePath, int frameCount) {
            moleculeSettingsPanel.TrajectoryLoaded(moleculeID, filePath, frameCount);
        }

        public void ToogleUserInterface() {

            if (toggleWholeInterface) {
                UserInterfaceCanvas.SetActive(!UserInterfaceCanvas.activeSelf);
            }
            else {
                SettingsGO.SetActive(!SettingsGO.activeSelf);
                ConsoleGO.SetActive(!ConsoleGO.activeSelf);
            }
        }

        public void ShowUserInterface() {

            if (toggleWholeInterface) {
                UserInterfaceCanvas.SetActive(true);
            }
            else {
                SettingsGO.SetActive(true);
                ConsoleGO.SetActive(true);
            }
        }

        public void HideUserInterface() {

            if (toggleWholeInterface) {
                UserInterfaceCanvas.SetActive(false);
            }
            else {
                SettingsGO.SetActive(false);
                ConsoleGO.SetActive(false);
            }
        }

        public bool UIActive() {

            if (toggleWholeInterface) {

                return UserInterfaceCanvas.activeSelf;
            }
            else {

                if (SettingsGO.activeSelf && ConsoleGO.activeSelf) {
                    return true;
                }

                return false;
            }
        }

        public void ShowTrajectoryControls() {
            TrajectoryControlsGO.SetActive(true);
        }

        public void HideTrajectoryControls() {
            TrajectoryControlsGO.SetActive(false);
        }

        public void ShowConsoleMessage(string message) {

            UserInterfaceCanvas.SetActive(true);
            console.ShowMessage(message);
        }

        public void ShowConsoleError(string message) {

            UserInterfaceCanvas.SetActive(true);
            console.ShowError(message);
        }

        public void ConsoleSetSilent(bool silent) {
            console.Silent = silent;
        }

        public bool HasInputFocus() {


// this needs to be fixed

            //// file inputs
            //if (FileBrowser.startFrameInput.isFocused || FileBrowser.frameCountInput.isFocused || FileBrowser.frameFrequencyInput.isFocused)
            //    return true;

            //// trajectory input
            //if (TrajectoryControls.FrameNumber.isFocused)
            //    return true;

            return false;
        }
    }
}
