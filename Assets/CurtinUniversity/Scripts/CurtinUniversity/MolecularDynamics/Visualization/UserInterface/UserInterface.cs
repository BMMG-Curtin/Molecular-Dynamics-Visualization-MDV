using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

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
        private Visualization.ResiduesSettingsPanel residuesSettingsPanel;

        public bool ToggleWholeInterface = false;

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

        public void SetSceneSettings(SceneSettings settings) {
            applicationSettings.SetSceneSettings(settings);
        }

        public void LoadMolecule(string filePath, MoleculeRenderSettings? settings = null) {
            moleculeSettingsPanel.LoadMolecule(filePath, settings);
        }

        public void MoleculeLoaded(int id, string name, string description, HashSet<string> elements, Dictionary<string, HashSet<int>> residues, int atomCount, int residueCount) {

            moleculeSettingsPanel.MoleculeLoaded(id, name, description, atomCount, residueCount);
            elementsSettingsPanel.SetModelElements(id, elements);
            residuesSettingsPanel.SetModelResidues(id, residues);
        }

        public void MoleculeTrajectoryLoaded(int moleculeID, int frameCount) {
            moleculeSettingsPanel.TrajectoryLoaded(moleculeID, frameCount);
        }

        public void ToogleUserInterface() {

            if (ToggleWholeInterface) {
                UserInterfaceCanvas.SetActive(!UserInterfaceCanvas.activeSelf);
            }
            else {
                SettingsGO.SetActive(!SettingsGO.activeSelf);
                ConsoleGO.SetActive(!ConsoleGO.activeSelf);
            }
        }

        public void ShowUserInterface() {

            if (ToggleWholeInterface) {
                UserInterfaceCanvas.SetActive(true);
            }
            else {
                SettingsGO.SetActive(true);
                ConsoleGO.SetActive(true);
            }
        }

        public void HideUserInterface() {

            if (ToggleWholeInterface) {
                UserInterfaceCanvas.SetActive(false);
            }
            else {
                SettingsGO.SetActive(false);
                ConsoleGO.SetActive(false);
            }
        }

        public bool UIActive() {

            if (ToggleWholeInterface) {

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
