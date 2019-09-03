using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class OtherSettingsPanel : MonoBehaviour {

        [SerializeField]
        private GameObject mouseCursor;

        [SerializeField]
        private Toggle groundToggle;

        [SerializeField]
        private Toggle shadowsToggle;

        [SerializeField]
        private Toggle mainLightsToggle;

        [SerializeField]
        private Toggle fillLightsToggle;

        [SerializeField]
        private Text lightIntensityText;

        [SerializeField]
        private Text autoRotateSpeedText;

        [SerializeField]
        private Toggle autoMeshQualityToggle;

        [SerializeField]
        private Text meshQualityText;

        [SerializeField]
        private Text mouseSpeedText;

        [SerializeField]
        private float mouseSpeedMultiplier = 3f;

        [SerializeField]
        private ConfirmDialog confirmDialog;

        private string playerPrefsMouseSpeedKey = @"MouseSpeed";
        private string playerPrefsGroundKey = @"SceneGround";
        private string playerPrefsShadowsKey = @"SceneShadows";
        private string playerPrefsMainLightKey = @"SceneMainLights";
        private string playerPrefsFillLightKey = @"SceneFillLights";
        private string playerPrefsLightIntensityKey = @"SceneLightIntensity";
        private string playerPrefsAutoRotateKey = @"AnimationAutoRotateSpeed";
        private string playerPrefsAutoMeshQualityKey = @"RenderAutoMeshQuality";
        private string playerPrefsMeshQualityKey = @"RenderMeshQuality";

        private int lightIntensity = 1;
        private int mouseSpeed = 1;
        private int autoRotateSpeed = 1;

        private GeneralSettings generalSettings;

        public void Awake() {

            // Set UI specific settings here
            
            if (PlayerPrefs.HasKey(playerPrefsMouseSpeedKey)) {

                mouseSpeed = PlayerPrefs.GetInt(playerPrefsMouseSpeedKey);

                if(mouseSpeed < 1) {
                    mouseSpeed = 1;
                }

                SetMouseSpeed(mouseSpeed);
            }
        }

        public void SetSceneSettings(GeneralSettings settings) {

            generalSettings = settings;

            if (PlayerPrefs.HasKey(playerPrefsMainLightKey)) {
                generalSettings.MainLightsOn = PlayerPrefs.GetInt(playerPrefsMainLightKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsFillLightKey)) {
                generalSettings.FillLightsOn = PlayerPrefs.GetInt(playerPrefsFillLightKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsShadowsKey)) {
                generalSettings.ShowShadows = PlayerPrefs.GetInt(playerPrefsShadowsKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsGroundKey)) {
                generalSettings.ShowGround = PlayerPrefs.GetInt(playerPrefsGroundKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsLightIntensityKey)) {
                lightIntensity = PlayerPrefs.GetInt(playerPrefsLightIntensityKey);
            }

            if (PlayerPrefs.HasKey(playerPrefsAutoMeshQualityKey)) {
                generalSettings.AutoMeshQuality = PlayerPrefs.GetInt(playerPrefsAutoMeshQualityKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsMeshQualityKey)) {
                generalSettings.MeshQuality = PlayerPrefs.GetInt(playerPrefsMeshQualityKey);
            }

            if (PlayerPrefs.HasKey(playerPrefsAutoRotateKey)) {
                autoRotateSpeed = PlayerPrefs.GetInt(playerPrefsAutoRotateKey);
            }

            groundToggle.isOn = generalSettings.ShowGround;
            shadowsToggle.isOn = generalSettings.ShowShadows;
            mainLightsToggle.isOn = generalSettings.MainLightsOn;
            fillLightsToggle.isOn = generalSettings.FillLightsOn;

            lightIntensityText.text = lightIntensity.ToString();
            generalSettings.LightIntensity = (float)(lightIntensity - Settings.MinLightIntensity) / (float)(Settings.MaxLightIntensity - Settings.MinLightIntensity);

            autoMeshQualityToggle.isOn = generalSettings.AutoMeshQuality;
            generalSettings.MeshQuality = Mathf.Clamp(generalSettings.MeshQuality, 0, Settings.MeshQualityValues.Length - 1);
            meshQualityText.text = generalSettings.AutoMeshQuality ? "Auto" : Settings.MeshQualityValues[generalSettings.MeshQuality];

            autoRotateSpeedText.text = autoRotateSpeed.ToString();
            generalSettings.AutoRotateSpeed = (float)(autoRotateSpeed - Settings.MinAutoRotateSpeed) / (float)(Settings.MaxAutoRotateSpeed- Settings.MinAutoRotateSpeed);

            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
        }

        public void OnGroundToggleChanged() {

            generalSettings.ShowGround = groundToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsGroundKey, groundToggle.isOn ? 1 : 0);
        }

        public void OnShadowsToggleChanged() {

            generalSettings.ShowShadows = shadowsToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsShadowsKey, shadowsToggle.isOn ? 1 : 0);
        }

        public void OnMainLightsToggleChanged() {

            generalSettings.MainLightsOn= mainLightsToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsMainLightKey, mainLightsToggle.isOn ? 1 : 0);
        }

        public void OnFillLightsToggleChanged() {

            generalSettings.FillLightsOn = fillLightsToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsFillLightKey, fillLightsToggle.isOn ? 1 : 0);
        }


        public void IncreaseLightIntensity() {
            SetLightIntensity(lightIntensity + 1);
        }

        public void DecreaseLightIntensity() {
            SetLightIntensity(lightIntensity - 1);
        }

        public void SetLightIntensity(int intensity) {

            if (lightIntensity < Settings.MinLightIntensity || lightIntensity > Settings.MaxLightIntensity) {
                return;
            }

            lightIntensity = intensity;
            lightIntensityText.text = lightIntensity.ToString();

            generalSettings.LightIntensity = (float)(lightIntensity - Settings.MinLightIntensity) / (float)(Settings.MaxLightIntensity - Settings.MinLightIntensity);
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsLightIntensityKey, lightIntensity);
        }

        public void InreaseAutoRotateSpeed() {
            SetAutoRotateSpeed(autoRotateSpeed + 1);
        }

        public void DecreaseAutoRotateSpeed() {
            SetAutoRotateSpeed(autoRotateSpeed - 1);
        }

        public void SetAutoRotateSpeed(int speed) {

            if (speed > Settings.MaxAutoRotateSpeed || speed < Settings.MinAutoRotateSpeed) {
                return;
            }

            autoRotateSpeed = speed;
            autoRotateSpeedText.text = autoRotateSpeed.ToString();

            generalSettings.AutoRotateSpeed = (float)(autoRotateSpeed - Settings.MinAutoRotateSpeed) / (float)(Settings.MaxAutoRotateSpeed - Settings.MinAutoRotateSpeed);
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsAutoRotateKey, autoRotateSpeed);
        }

        public void InreaseMouseSpeed() {
            SetMouseSpeed(mouseSpeed + 1);
        }

        public void DecreaseMouseSpeed() {
            SetMouseSpeed(mouseSpeed - 1);
        }

        public void SetMouseSpeed(int speed) {

            if (speed > Settings.MaxMouseSpeed || speed < Settings.MinMouseSpeed) {
                return;
            }

            mouseSpeed = speed;

            mouseCursor.GetComponent<MousePointer>().MouseSpeed = mouseSpeed * Settings.MouseSpeedMultiplier;
            mouseSpeedText.text = mouseSpeed.ToString();

            PlayerPrefs.SetInt(playerPrefsMouseSpeedKey, speed);
        }

        public void IncreaseMeshQuality() {
            SetMeshQuality(generalSettings.MeshQuality + 1);
        }

        public void DecreaseMeshQuality() {
            SetMeshQuality(generalSettings.MeshQuality -1);
        }

        public void SetMeshQuality(int meshQuality) { 

            if(autoMeshQualityToggle.isOn ||
                generalSettings.MeshQuality > Settings.MeshQualityValues.Length - 1 ||
                generalSettings.MeshQuality < 0) {
                return;
            }

            generalSettings.MeshQuality = meshQuality;
            meshQualityText.text = Settings.MeshQualityValues[generalSettings.MeshQuality];
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsMeshQualityKey, generalSettings.MeshQuality);
        }

        public void OnAutoMeshQualityToggleChanged() {

            meshQualityText.text = autoMeshQualityToggle.isOn ? "Auto" : Settings.MeshQualityValues[generalSettings.MeshQuality];
            generalSettings.AutoMeshQuality = autoMeshQualityToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsAutoMeshQualityKey, autoMeshQualityToggle.isOn ? 1 : 0);
        }

        public void OnQuitApplicationButton() {

            confirmDialog.gameObject.SetActive(true);
            confirmDialog.Initialise("Quit application?", quitApplication);
        }

        private void quitApplication(bool confirmed, object data = null) {
            if(confirmed) {
                Application.Quit();
            }
        }
    }
}
