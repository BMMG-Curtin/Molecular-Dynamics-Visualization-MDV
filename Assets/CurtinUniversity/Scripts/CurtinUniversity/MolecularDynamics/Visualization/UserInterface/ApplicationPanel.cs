using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ApplicationPanel : MonoBehaviour {

        public Text CurrentResolutionText;

        public Text AtomMeshQualityText;
        public Text BondMeshQualityText;

        public Text MouseSpeedText;
        public Text GUICursorSpeedText;
        public Text UIDistanceText;

        public Toggle FullscreenToggle;
        public Toggle HoverUIToggle;

        private Resolution[] resolutions;
        private int currentResolutionIndex;
        private int currentAtomMeshIndex;
        private int currentBondMeshIndex;
        private bool fullScreen = false;
        private bool initialised;

        private string playerPrefsHoverUIKey = @"HoverUI";
        private string playerPrefsAtomMeshQualityKey = @"AtomMeshQuality";
        private string playerPrefsBondMeshQualityKey = @"BondMeshQuality";

        void Start() {

            if (!initialised) {
                Initialise();
            }
        }

        private void Initialise() {

            initialised = true;

            //if (Settings.DisplayPlatform == Platform.Desktop) {
                resolutions = Screen.resolutions;
                currentResolutionIndex = 0;
                for (int i = 0; i < resolutions.Length; i++) {
                    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) {
                        currentResolutionIndex = i;
                    }
                }
            //}

            fullScreen = Screen.fullScreen;

            if (PlayerPrefs.HasKey(playerPrefsAtomMeshQualityKey)) {
                Settings.AtomMeshQuality = PlayerPrefs.GetInt(playerPrefsAtomMeshQualityKey);
            }

            if (PlayerPrefs.HasKey(playerPrefsBondMeshQualityKey)) {
                Settings.BondMeshQuality = PlayerPrefs.GetInt(playerPrefsBondMeshQualityKey);
            }
        }

        void OnEnable() {

            if (Settings.Loaded) { // first enable can fire before scenemanager load

                if (!initialised) { // first enable can fire before start has initialised
                    Initialise();
                }

                LoadSettings();
            }
        }

        public void LoadSettings() {

            // external setting changes can call this while panel is not visible. Not need to update panel if this is the case
            if (!enabled) {
                return;
            }

            if (resolutions != null) {
                CurrentResolutionText.text = resolutions[currentResolutionIndex].width + " X " + resolutions[currentResolutionIndex].height;
            }
            else {
                CurrentResolutionText.text = "-";
            }

            AtomMeshQualityText.text = Settings.AtomMeshQualityValues[Settings.AtomMeshQuality];
            BondMeshQualityText.text = Settings.BondMeshQualityValues[Settings.BondMeshQuality];
            currentAtomMeshIndex = Settings.AtomMeshQuality;
            currentBondMeshIndex = Settings.BondMeshQuality;

            MouseSpeedText.text = Settings.SceneMouseCursorSpeed.ToString();
            GUICursorSpeedText.text = Settings.GUIMouseCursorSpeed.ToString();
            UIDistanceText.text = Settings.UIDistance.ToString();

            FullscreenToggle.isOn = fullScreen;
        }

        public void SetResolution(int width, int height, bool fullScreen) {

            Screen.SetResolution(width, height, fullScreen);

            if (Settings.HideHardwareMouseCursor) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            //Camera.main.ResetAspect();
        }

        public void IncreaseScreenResolution() {

            if (currentResolutionIndex + 1 < resolutions.Length) {
                currentResolutionIndex++;
                CurrentResolutionText.text = resolutions[currentResolutionIndex].width + " X " + resolutions[currentResolutionIndex].height;
                SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, FullscreenToggle.isOn);
            }
        }

        public void DecreaseScreenResolution() {

            if (currentResolutionIndex > 0) {
                currentResolutionIndex--;
                CurrentResolutionText.text = resolutions[currentResolutionIndex].width + " X " + resolutions[currentResolutionIndex].height;
                SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, FullscreenToggle.isOn);
            }
        }

        public void FullScreen() {
            Screen.fullScreen = FullscreenToggle.isOn;

        }

        public void IncreaseAtomMeshQuality() {

            if (currentAtomMeshIndex < Settings.AtomMeshQualityValues.Length - 1) {
                currentAtomMeshIndex++;
                AtomMeshQualityText.text = Settings.AtomMeshQualityValues[currentAtomMeshIndex];
                Settings.AtomMeshQuality = currentAtomMeshIndex;
                StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
            }

            PlayerPrefs.SetInt(playerPrefsAtomMeshQualityKey, Settings.AtomMeshQuality);
        }

        public void DecreaseAtomMeshQuality() {

            if (currentAtomMeshIndex > 0) {
                currentAtomMeshIndex--;
                AtomMeshQualityText.text = Settings.AtomMeshQualityValues[currentAtomMeshIndex];
                Settings.AtomMeshQuality = currentAtomMeshIndex;
                StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
            }

            PlayerPrefs.SetInt(playerPrefsAtomMeshQualityKey, Settings.AtomMeshQuality);
        }

        public void IncreaseBondMeshQuality() {

            if (currentBondMeshIndex < Settings.BondMeshQualityValues.Length - 1) {
                currentBondMeshIndex++;
                BondMeshQualityText.text = Settings.BondMeshQualityValues[currentBondMeshIndex];
                Settings.BondMeshQuality = currentBondMeshIndex;
                StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
            }

            PlayerPrefs.SetInt(playerPrefsBondMeshQualityKey, Settings.BondMeshQuality);
        }

        public void DecreaseBondMeshQuality() {

            if (currentBondMeshIndex > 0) {
                currentBondMeshIndex--;
                BondMeshQualityText.text = Settings.BondMeshQualityValues[currentBondMeshIndex];
                Settings.BondMeshQuality = currentBondMeshIndex;
                StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
            }

            PlayerPrefs.SetInt(playerPrefsBondMeshQualityKey, Settings.BondMeshQuality);
        }

        public void InreaseMouseSpeed() {
            SceneManager.instance.InputManager.SetSceneMouseSpeed(Settings.SceneMouseCursorSpeed + 1);
            MouseSpeedText.text = Settings.SceneMouseCursorSpeed.ToString();
        }

        public void DecreaseMouseSpeed() {
            SceneManager.instance.InputManager.SetSceneMouseSpeed(Settings.SceneMouseCursorSpeed - 1);
            MouseSpeedText.text = Settings.SceneMouseCursorSpeed.ToString();
        }

        public void InreaseGUICursorSpeed() {
            SceneManager.instance.InputManager.SetGUICursorSpeed(Settings.GUIMouseCursorSpeed + 1);
            GUICursorSpeedText.text = Settings.GUIMouseCursorSpeed.ToString();
        }

        public void DecreaseGUICursorSpeed() {
            SceneManager.instance.InputManager.SetGUICursorSpeed(Settings.GUIMouseCursorSpeed - 1);
            GUICursorSpeedText.text = Settings.GUIMouseCursorSpeed.ToString();
        }

        public void IncreaseUIDistance() {
            SceneManager.instance.GUIManager.SetUIDistance(Settings.UIDistance + Settings.UIDistanceStep);
            UIDistanceText.text = Settings.UIDistance.ToString();
        }

        public void DecreaseUIDistance() {
            SceneManager.instance.GUIManager.SetUIDistance(Settings.UIDistance - Settings.UIDistanceStep);
            UIDistanceText.text = Settings.UIDistance.ToString();
        }
    }
}
