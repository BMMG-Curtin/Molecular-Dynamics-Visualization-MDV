using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

using CurtinUniversity.MolecularDynamics.Visualization.Camera;

using System.Runtime.InteropServices;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class InputManager : MonoBehaviour {

        public GameObject MouseCursor;
        private SceneManager sceneManager;
        public CameraController CameraController;

        public bool ShiftPressed { get; private set; }
        public bool ControlPressed { get; private set; }
        public bool AltPressed { get; private set; }
        public bool NoModifiersPressed { get; private set; }

        public bool KeyboardUIControlEnabled { get; set; }
        public bool KeyboardSceneControlEnabled {
            get { return keyboardSceneControlEnabled; }
            set {
                keyboardSceneControlEnabled = value;
                // CameraController.MovementEnabled = value;
            }
        }
        private bool keyboardSceneControlEnabled;

        private float cameraUpDownSpeed = 2f;
        private float cameraUpDownSpeedFast = 5f;

        private string playerPrefsMouseSpeedKey = @"MouseSpeed";
        private string playerPrefsGUICursorSpeedKey = @"CursorSpeed";

        private bool modelRotateActivate = false;

        // Use this for initialization
        void Start() {

            sceneManager = SceneManager.instance;

            if (PlayerPrefs.HasKey(playerPrefsMouseSpeedKey) || PlayerPrefs.HasKey(playerPrefsGUICursorSpeedKey)) {

                if (PlayerPrefs.HasKey(playerPrefsMouseSpeedKey)) {
                    SetSceneMouseSpeed(PlayerPrefs.GetInt(playerPrefsMouseSpeedKey));
                }

                if (PlayerPrefs.HasKey(playerPrefsGUICursorSpeedKey)) {
                    SetGUICursorSpeed(PlayerPrefs.GetInt(playerPrefsGUICursorSpeedKey));
                }

                sceneManager.GUIManager.ReloadOptions();
            }

            KeyboardUIControlEnabled = true;
            KeyboardSceneControlEnabled = true;
        }

        // Update is called once per frame
        void Update() {

            handleMouseInput();

            getKeyboardModifiers();
            if (KeyboardUIControlEnabled) {
                handleKeyboardInput();
            }
        }

        public void SetSceneMouseSpeed(int speed) {

            if (speed > Settings.MaxSceneMouseCursorSpeed || speed < Settings.MinSceneMouseCursorSpeed) {
                return;
            }

            if (CameraController != null) {

                Settings.SceneMouseCursorSpeed = speed;
                CameraController.CameraSensitivityX = Settings.SceneMouseCursorSpeed * Settings.SceneMouseCursorSpeedMultiplier;
                CameraController.CameraSensitivityY = Settings.SceneMouseCursorSpeed * Settings.SceneMouseCursorSpeedMultiplier;
            }

            PlayerPrefs.SetInt(playerPrefsMouseSpeedKey, speed);
        }

        public void SetGUICursorSpeed(int speed) {

            if (speed > Settings.MaxGUIMouseCursorSpeed || speed < Settings.MinGUIMouseCursorSpeed) {
                return;
            }

            Settings.GUIMouseCursorSpeed = speed;
            MouseCursor.GetComponent<MousePointer>().MouseSpeed = Settings.GUIMouseCursorSpeed * Settings.GUIMouseCursorSpeedMultiplier;

            PlayerPrefs.SetInt(playerPrefsGUICursorSpeedKey, speed);
        }

        private void handleMouseInput() {

            // handle mouse look
            if (CameraController != null && Input.GetMouseButtonDown(1)) {
                MouseCursor.SetActive(false);
            }

            if (CameraController != null && Input.GetMouseButtonUp(1)) {
                MouseCursor.SetActive(true);
            }

            float delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta != 0 && !sceneManager.GUIManager.IsActive) {
                sceneManager.Model.Scale += delta;
            }

            float xAngle = Input.GetAxis("Mouse Y") * Time.deltaTime * 200f;
            float yAngle = Input.GetAxis("Mouse X") * Time.deltaTime * 200f;

            if (AltPressed) {
                sceneManager.Model.Rotate(xAngle, yAngle, 0f);
            }

            if (!sceneManager.GUIManager.UIActive() && Input.GetMouseButtonDown(0)) {
                modelRotateActivate = true;
            }

            if (modelRotateActivate && Input.GetMouseButtonUp(0)) {
                modelRotateActivate = false;
            }

            if (modelRotateActivate) {
                float rotation = Input.GetAxis("Mouse X") * Settings.SceneMouseCursorSpeed * -1;
                if (Mathf.Abs(rotation) > 0.05f) {
                    sceneManager.Model.Rotate(0, rotation, 0);
                }
            }
        }

        private void getKeyboardModifiers() {

            ShiftPressed = false;
            ControlPressed = false;
            AltPressed = false;
            NoModifiersPressed = true;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                ShiftPressed = true;
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                ControlPressed = true;
            }


            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
                AltPressed = true;
            }

            if (ShiftPressed || ControlPressed || AltPressed) {
                NoModifiersPressed = false;
            }
        }


        private void handleKeyboardInput() {

            // handle quit
            if ((Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftControl)) || (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.Q))) {
                sceneManager.Quit();
            }

            // toggle UI
            if (Input.GetKeyDown(KeyCode.Escape)) {
                sceneManager.GUIManager.ToogleUserInterface();
            }

            bool reloadUIOptions = false;
            bool primaryStructureReloadRequired = false;
            bool secondaryStructureReloadRequired = false;

            if (!sceneManager.GUIManager.UIActive() || !sceneManager.GUIManager.HasInputFocus()) {

                // toggle model rotate
                if (NoModifiersPressed && Input.GetKeyDown(KeyCode.Space)) {

                    Settings.ModelRotate = !Settings.ModelRotate;
                    reloadUIOptions = true;
                }

                // toggle primary structure display
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))) {

                    Settings.EnablePrimaryStructure = !Settings.EnablePrimaryStructure;
                    primaryStructureReloadRequired = true;

                    if (Settings.ToggleStructures) {
                        if (Settings.EnablePrimaryStructure && Settings.EnableSecondaryStructure) {
                            Settings.EnableSecondaryStructure = false;
                            secondaryStructureReloadRequired = true;
                        }
                        else if (!Settings.EnablePrimaryStructure && !Settings.EnableSecondaryStructure) {
                            Settings.EnableSecondaryStructure = true;
                            primaryStructureReloadRequired = true;
                            secondaryStructureReloadRequired = true;
                        }
                    }

                    reloadUIOptions = true;
                }

                // toggle atom display
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))) {

                    Settings.ShowAtoms = !Settings.ShowAtoms;
                    primaryStructureReloadRequired = true;
                    reloadUIOptions = true;
                }

                // toggle bond display
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))) {

                    Settings.ShowBonds = !Settings.ShowBonds;
                    primaryStructureReloadRequired = true;
                    reloadUIOptions = true;
                }

                // toggle standard residue display
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))) {

                    Settings.ShowStandardResidues = !Settings.ShowStandardResidues;
                    primaryStructureReloadRequired = true;
                    reloadUIOptions = true;
                }

                // toggle non standard residue display
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))) {

                    Settings.ShowNonStandardResidues = !Settings.ShowNonStandardResidues;
                    primaryStructureReloadRequired = true;
                    reloadUIOptions = true;
                }

                // toggle main chain display
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))) {

                    Settings.ShowMainChains = !Settings.ShowMainChains;
                    primaryStructureReloadRequired = true;
                    reloadUIOptions = true;
                }

                // toggle box display
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7))) {

                    Settings.ShowSimulationBox = !Settings.ShowSimulationBox;
                    sceneManager.ModelBox.Show(Settings.ShowSimulationBox);
                    reloadUIOptions = true;
                }

                // toggle ground
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8))) {

                    Settings.ShowGround = !Settings.ShowGround;
                    sceneManager.Ground.SetActive(Settings.ShowGround);
                    reloadUIOptions = true;
                }

                // toggle shadows
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9))) {

                    Settings.ShowShadows = !Settings.ShowShadows;
                    sceneManager.Lighting.EnableShadows(Settings.ShowShadows);
                    reloadUIOptions = true;
                }

                // toggle main vs ambient lights
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) {

                    Settings.ShowLights = !Settings.ShowLights;
                    sceneManager.Lighting.EnableLighting(Settings.ShowLights);
                    reloadUIOptions = true;
                }


                // adjust model scale
                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.KeypadPlus))) {

                    sceneManager.Model.IncreaseScale();
                    reloadUIOptions = true;
                }

                if (NoModifiersPressed && (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.KeypadMinus))) {

                    sceneManager.Model.DecreaseScale();
                    reloadUIOptions = true;
                }

                // used for debugging model mesh changes
                if (NoModifiersPressed && Input.GetKeyDown(KeyCode.BackQuote)) {

                    Settings.DebugFlag = !Settings.DebugFlag;

                    primaryStructureReloadRequired = true;
                    secondaryStructureReloadRequired = true;
                    reloadUIOptions = true;
                }


                // options with the shift key pressed

                // toggle secondary structure display
                if (ShiftPressed && (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))) {

                    Settings.EnableSecondaryStructure = !Settings.EnableSecondaryStructure;
                    secondaryStructureReloadRequired = true;

                    if (Settings.ToggleStructures) {
                        if (Settings.EnablePrimaryStructure && Settings.EnableSecondaryStructure) {
                            Settings.EnablePrimaryStructure = false;
                            primaryStructureReloadRequired = true;
                        }
                        else if (!Settings.EnablePrimaryStructure && !Settings.EnableSecondaryStructure) {
                            Settings.EnablePrimaryStructure = true;
                            primaryStructureReloadRequired = true;
                        }
                    }

                    reloadUIOptions = true;
                }

                // toggle helices display
                if (ShiftPressed && (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))) {

                    Settings.ShowHelices = !Settings.ShowHelices;
                    secondaryStructureReloadRequired = true;
                    reloadUIOptions = true;
                }

                // toggle sheets display
                if (ShiftPressed && (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))) {

                    Settings.ShowSheets = !Settings.ShowSheets;
                    secondaryStructureReloadRequired = true;
                    reloadUIOptions = true;
                }

                // toggle turns display
                if (ShiftPressed && (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))) {

                    Settings.ShowTurns = !Settings.ShowTurns;
                    secondaryStructureReloadRequired = true;
                    reloadUIOptions = true;
                }

                // adjust atom scale
                if (ShiftPressed && ((Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.KeypadPlus)))) {

                    sceneManager.StructureView.PrimaryStructureView.IncreaseAtomScale();
                    reloadUIOptions = true;
                    primaryStructureReloadRequired = true;
                }

                if (ShiftPressed && ((Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.KeypadMinus)))) {
                    sceneManager.StructureView.PrimaryStructureView.DecreaseAtomScale();
                    reloadUIOptions = true;
                    primaryStructureReloadRequired = true;
                }


                // options with the control key pressed

                // adjust bond scale
                if (ControlPressed && (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.KeypadPlus))) {

                    sceneManager.StructureView.PrimaryStructureView.IncreaseBondScale();
                    reloadUIOptions = true;
                    primaryStructureReloadRequired = true;
                }

                if (ControlPressed && (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.KeypadMinus))) {

                    sceneManager.StructureView.PrimaryStructureView.DecreaseBondScale();
                    reloadUIOptions = true;
                    primaryStructureReloadRequired = true;
                }



                if (reloadUIOptions) {
                    sceneManager.GUIManager.ReloadOptions();
                }

                if (primaryStructureReloadRequired || secondaryStructureReloadRequired) {
                    StartCoroutine(sceneManager.ReloadModelView(primaryStructureReloadRequired, secondaryStructureReloadRequired));
                }
            }
        }
    }
}
