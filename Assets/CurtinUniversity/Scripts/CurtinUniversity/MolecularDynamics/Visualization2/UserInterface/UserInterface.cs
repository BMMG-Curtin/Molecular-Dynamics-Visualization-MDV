using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

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
        private ApplicationSettingsPanel applicationSettings;

        [SerializeField]
        private MoleculesSettingsPanel moleculeSettings;

        //public MainMenu Menu;
        //public TrajectoryControls TrajectoryControls;
        //public ApplicationPanel ApplicationPanel;
        //public VisualisationPanel VisualisationPanel;
        public ElementsSettingsPanel ElementsPanel;
        public Visualization.ResiduesSettingsPanel ResiduesPanel;
        //public FileBrowser FileBrowser;
        //public InfoPanel InfoPanel;

        public bool ToggleWholeInterface = false;

        public bool IsActive { get { return UserInterfaceCanvas.activeSelf; } }

        // private variables
        //private float userInterfacePosY = 0f;
        //private float userInterfacePosZ = 1f;
        //private string playerPrefsUIDistance = @"UIDistance";

        //private GameObject cameraPosition;

        int frameCount;
        int reportFrequency = 1; // in seconds
        float timer;

        private void Start() {

            //if (Settings.HideHardwareMouseCursor) {
            //    Cursor.lockState = CursorLockMode.Locked;
            //    Cursor.visible = false;
            //}

            HideUserInterface();
        }

        private void Update() {

            // if the window loses focus (e.g. alt-tab) then the cursor gets unlocked.
            // need to lock again when user uses the application.
            if (Settings.HideHardwareMouseCursor && Cursor.lockState != CursorLockMode.Locked) {

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

        public void MoleculeLoaded(int id, string name, string description, HashSet<string> elements, HashSet<string> residues) {

            moleculeSettings.MoleculeLoaded(id, name, description);
            ElementsPanel.SetModelElements(id, elements);
            ResiduesPanel.SetModelResidues(id, residues);
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
