using UnityEngine;
using UnityEngine.UI;
using System;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class GUIManager : MonoBehaviour {

        // inspector properties
        public GameObject UserInterface;
        public GameObject MainMenu;
        public GameObject ConsoleGO;
        public GameObject TrajectoryControlsGO;

        public MainMenu Menu;
        public Console Console;
        public ApplicationPanel ApplicationPanel;
        public VisualisationPanel VisualisationPanel;
        public ElementsPanel ElementsPanel;
        public ResiduesPanel ResiduesPanel;
        public FileBrowser FileBrowser;
        public InfoPanel InfoPanel;
        public TrajectoryControls TrajectoryControls;

        public bool ToggleWholeInterface = false;

        public bool IsActive { get { return UserInterface.activeSelf; } }

        // private variables
        private float userInterfacePosY = 0f;
        private float userInterfacePosZ = 1f;
        private string playerPrefsUIDistance = @"UIDistance";

        private GameObject cameraPosition;

        int frameCount;
        int reportFrequency = 1; // in seconds
        float timer;

        void Start() {

            if (Settings.HideHardwareMouseCursor) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            cameraPosition = new GameObject();
            cameraPosition.name = "CameraPosition";

            if (PlayerPrefs.HasKey(playerPrefsUIDistance)) {
                SetUIDistance(PlayerPrefs.GetFloat(playerPrefsUIDistance));
            }
            else {
                SetUIDistance(Settings.UIDistance);
            }

            HideUserInterface();
        }

        void Update() {

            // if the window loses focus (e.g. alt-tab) then the cursor gets unlocked.
            // need to lock again when user uses the application.
            if (Settings.HideHardwareMouseCursor && Cursor.lockState != CursorLockMode.Locked) {

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            //cameraPosition.transform.position = SceneManager.MainCameraTransform.transform.position;
            //Vector3 rot1 = SceneManager.MainCameraTransform.transform.rotation.eulerAngles;
            //// Vector3 rot2 = SceneManager.MainCameraTransform2.transform.rotation.eulerAngles;
            //cameraPosition.transform.rotation = Quaternion.Euler(rot1.x, rot1.y + UserInterfaceOffsetAngle, rot1.z); // * Quaternion.Euler(rot2.x, 0, 0);
            //UserInterface.transform.position = cameraPosition.transform.position + (cameraPosition.transform.forward * userInterfacePosZ);
            //UserInterface.transform.rotation = cameraPosition.transform.rotation;

            if (Settings.DebugMessages) {

                timer += Time.unscaledDeltaTime;
                frameCount += 1;

                if (timer >= reportFrequency) {
                    if (Console == null) {
                        Debug.Log("Console is null");
                    }
                    Console.BannerFPS = (frameCount / reportFrequency).ToString();
                    timer = frameCount = 0;
                }
            }
        }

        public void ToogleUserInterface() {

            if (ToggleWholeInterface) {
                UserInterface.SetActive(!UserInterface.activeSelf);
            }
            else {
                MainMenu.SetActive(!MainMenu.activeSelf);
                ConsoleGO.SetActive(!ConsoleGO.activeSelf);
            }
        }

        public void ShowUserInterface() {

            if (ToggleWholeInterface) {
                UserInterface.SetActive(true);
            }
            else {
                MainMenu.SetActive(true);
                ConsoleGO.SetActive(true);
            }
        }

        public void HideUserInterface() {

            if (ToggleWholeInterface) {
                UserInterface.SetActive(false);
            }
            else {
                MainMenu.SetActive(false);
                ConsoleGO.SetActive(false);
            }
        }

        public bool UIActive() {

            if (ToggleWholeInterface) {

                return UserInterface.activeSelf;
            }
            else {

                if (MainMenu.activeSelf && ConsoleGO.activeSelf) {
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

            UserInterface.SetActive(true);
            Console.ShowMessage(message);
        }

        public void ShowConsoleError(string message) {

            UserInterface.SetActive(true);
            Console.ShowError(message);
        }

        public void ConsoleSetSilent(bool silent) {
            Console.Silent = silent;
        }

        public void SetUIDistance(float distance) {

            distance = (float)Math.Round((double)distance, 1);

            if (distance < Settings.MinUIDistance) {
                distance = Settings.MinUIDistance;
            }

            else if (distance > Settings.MaxUIDistance) {
                distance = Settings.MaxUIDistance;
            }

            userInterfacePosZ = distance;

            Settings.UIDistance = distance;
            PlayerPrefs.SetFloat(playerPrefsUIDistance, distance);
        }

        public void ReloadOptions() {
            if (VisualisationPanel.gameObject.activeSelf) {
                VisualisationPanel.LoadSettings();
                ApplicationPanel.LoadSettings();
            }
        }

        public bool HasInputFocus() {

            // file inputs
            if (FileBrowser.startFrameInput.isFocused || FileBrowser.frameCountInput.isFocused || FileBrowser.frameFrequencyInput.isFocused)
                return true;

            // trajectory input
            if (TrajectoryControls.FrameNumber.isFocused)
                return true;

            return false;
        }
    }
}
