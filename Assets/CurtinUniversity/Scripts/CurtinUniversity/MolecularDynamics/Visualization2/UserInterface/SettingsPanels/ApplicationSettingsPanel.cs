using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class ApplicationSettingsPanel : MonoBehaviour {

        public GameObject MouseCursor;

        public Toggle GroundToggle;
        public Toggle ShadowsToggle;
        public Toggle LightsToggle;

        public Text AtomMeshQualityText;
        public Text BondMeshQualityText;

        public Text MouseSpeedText;
        public float mouseSpeedMultiplier = 3f;
        private string playerPrefsMouseSpeedKey = @"MouseSpeed";
        private int mouseSpeed = 1;

        private SceneSettings sceneSettings;

        public void Awake() {
            
            if (PlayerPrefs.HasKey(playerPrefsMouseSpeedKey)) {

                mouseSpeed = PlayerPrefs.GetInt(playerPrefsMouseSpeedKey);

                if(mouseSpeed < 1) {
                    mouseSpeed = 1;
                }

                setMouseSpeed(mouseSpeed);
            }
        }

        public void SetSceneSettings(SceneSettings settings) {

            sceneSettings = settings;
            reloadUIControls();
        }

        private void reloadUIControls() {

            GroundToggle.isOn = sceneSettings.ShowGround;
            ShadowsToggle.isOn = sceneSettings.ShowShadows;
            LightsToggle.isOn = sceneSettings.LightsOn;
        }

        public void OnGroundToggleChanged() {

            Debug.Log("Ground Toggle Clicked. Value: " + GroundToggle.isOn);

            sceneSettings.ShowGround = GroundToggle.isOn;
            UserInterfaceEvents.RaiseSceneSettingsUpdated(sceneSettings);
        }

        public void OnShadowsToggleChanged() {

            sceneSettings.ShowShadows = ShadowsToggle.isOn;
            UserInterfaceEvents.RaiseSceneSettingsUpdated(sceneSettings);
        }

        public void OnLightsToggleChanged() {

            sceneSettings.LightsOn= LightsToggle.isOn;
            UserInterfaceEvents.RaiseSceneSettingsUpdated(sceneSettings);
        }


        //public void IncreaseAtomMeshQuality() {

        //    if (currentAtomMeshIndex < Settings.AtomMeshQualityValues.Length - 1) {
        //        currentAtomMeshIndex++;
        //        AtomMeshQualityText.text = Settings.AtomMeshQualityValues[currentAtomMeshIndex];
        //        Settings.AtomMeshQuality = currentAtomMeshIndex;
        //        StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
        //    }

        //    PlayerPrefs.SetInt(playerPrefsAtomMeshQualityKey, Settings.AtomMeshQuality);
        //}

        //public void DecreaseAtomMeshQuality() {

        //    if (currentAtomMeshIndex > 0) {
        //        currentAtomMeshIndex--;
        //        AtomMeshQualityText.text = Settings.AtomMeshQualityValues[currentAtomMeshIndex];
        //        Settings.AtomMeshQuality = currentAtomMeshIndex;
        //        StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
        //    }

        //    PlayerPrefs.SetInt(playerPrefsAtomMeshQualityKey, Settings.AtomMeshQuality);
        //}

        //public void IncreaseBondMeshQuality() {

        //    if (currentBondMeshIndex < Settings.BondMeshQualityValues.Length - 1) {
        //        currentBondMeshIndex++;
        //        BondMeshQualityText.text = Settings.BondMeshQualityValues[currentBondMeshIndex];
        //        Settings.BondMeshQuality = currentBondMeshIndex;
        //        StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
        //    }

        //    PlayerPrefs.SetInt(playerPrefsBondMeshQualityKey, Settings.BondMeshQuality);
        //}

        //public void DecreaseBondMeshQuality() {

        //    if (currentBondMeshIndex > 0) {
        //        currentBondMeshIndex--;
        //        BondMeshQualityText.text = Settings.BondMeshQualityValues[currentBondMeshIndex];
        //        Settings.BondMeshQuality = currentBondMeshIndex;
        //        StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
        //    }

        //    PlayerPrefs.SetInt(playerPrefsBondMeshQualityKey, Settings.BondMeshQuality);
        //}

        public void InreaseMouseSpeed() {
            setMouseSpeed(mouseSpeed + 1);
        }

        public void DecreaseMouseSpeed() {
            setMouseSpeed(mouseSpeed - 1);
        }

        private void setMouseSpeed(int speed) {

            if (speed > Settings.MaxGUIMouseCursorSpeed || speed < Settings.MinGUIMouseCursorSpeed) {
                return;
            }

            mouseSpeed = speed;
            Debug.Log("Mouse speed set to: " + speed);

            MouseCursor.GetComponent<Visualization.MousePointer>().MouseSpeed = mouseSpeed * mouseSpeedMultiplier;
            MouseSpeedText.text = mouseSpeed.ToString();

            PlayerPrefs.SetInt(playerPrefsMouseSpeedKey, speed);
        }
    }
}
