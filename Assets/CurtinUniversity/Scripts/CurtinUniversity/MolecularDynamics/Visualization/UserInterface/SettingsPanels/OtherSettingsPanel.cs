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
        private Toggle ambientLightsToggle;

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
        private Text moleculeMovementSpeedText;

        [SerializeField]
        private Toggle spaceNavigatorCameraControlToggle;

        [SerializeField]
        private Toggle spaceNavigatorMoleculeControlToggle;

        [SerializeField]
        private ConfirmDialog confirmDialog;

        private string playerPrefsMouseSpeedKey = @"MouseSpeed";
        private string playerPrefsSpaceNavigatorCameraControlKey = @"SpaceNavigatorCameraControl";
        private string playerPrefsSpaceNavigatorMoleculeControlKey = @"SpaceNavigatorMoleculeControl";
        private string playerPrefsMoleculeMovementSpeedKey = @"InputMoleculeMovementSpeed";
        private string playerPrefsGroundKey = @"SceneGround";
        private string playerPrefsShadowsKey = @"SceneShadows";
        private string playerPrefsMainLightKey = @"SceneMainLights";
        private string playerPrefsFillLightKey = @"SceneFillLights";
        private string playerPrefsAmbientLightKey = @"SceneAmbientLight";
        private string playerPrefsLightIntensityKey = @"SceneLightIntensity";
        private string playerPrefsAutoRotateKey = @"AnimationAutoRotateSpeed";
        private string playerPrefsAutoMeshQualityKey = @"RenderAutoMeshQuality";
        private string playerPrefsMeshQualityKey = @"RenderMeshQuality";

        private int lightIntensity = 0;
        private int mouseSpeed = 0;
        private int moleculeMovementSpeed = 0;
        private int autoRotateSpeed = 0;

        private GeneralSettings generalSettings;

        public void Awake() {

            if (PlayerPrefs.HasKey(playerPrefsMouseSpeedKey)) {
                SetMouseSpeed(PlayerPrefs.GetInt(playerPrefsMouseSpeedKey));
            }
            else {
                SetMouseSpeed(Settings.DefaultMouseSpeed);
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

            if (PlayerPrefs.HasKey(playerPrefsAmbientLightKey)) {
                generalSettings.AmbientLightsOn = PlayerPrefs.GetInt(playerPrefsAmbientLightKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsShadowsKey)) {
                generalSettings.ShowShadows = PlayerPrefs.GetInt(playerPrefsShadowsKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsGroundKey)) {
                generalSettings.ShowGround = PlayerPrefs.GetInt(playerPrefsGroundKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsLightIntensityKey)) {

                lightIntensity = PlayerPrefs.GetInt(playerPrefsLightIntensityKey);
                generalSettings.LightIntensity = (float)(lightIntensity - Settings.MinLightIntensity) / (float)(Settings.MaxLightIntensity - Settings.MinLightIntensity);
            }
            else {
                lightIntensity = (int)((generalSettings.LightIntensity * (float)(Settings.MaxLightIntensity - Settings.MinLightIntensity)) + Settings.MinLightIntensity);
            }

            if (PlayerPrefs.HasKey(playerPrefsAutoMeshQualityKey)) {
                generalSettings.AutoMeshQuality = PlayerPrefs.GetInt(playerPrefsAutoMeshQualityKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsMeshQualityKey)) {
                generalSettings.MeshQuality = PlayerPrefs.GetInt(playerPrefsMeshQualityKey);
            }

            if (PlayerPrefs.HasKey(playerPrefsAutoRotateKey)) {

                autoRotateSpeed = PlayerPrefs.GetInt(playerPrefsAutoRotateKey);
                generalSettings.AutoRotateSpeed = (float)(autoRotateSpeed - Settings.MinAutoRotateSpeed) / (float)(Settings.MaxAutoRotateSpeed - Settings.MinAutoRotateSpeed);
            }
            else {
                autoRotateSpeed = (int)((generalSettings.AutoRotateSpeed * (float)(Settings.MaxAutoRotateSpeed - Settings.MinAutoRotateSpeed)) + Settings.MinAutoRotateSpeed);
            }

            if (PlayerPrefs.HasKey(playerPrefsSpaceNavigatorCameraControlKey)) {
                generalSettings.SpaceNavigatorCameraControlEnabled = PlayerPrefs.GetInt(playerPrefsSpaceNavigatorCameraControlKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsSpaceNavigatorMoleculeControlKey)) {
                generalSettings.SpaceNavigatorMoleculeControlEnabled = PlayerPrefs.GetInt(playerPrefsSpaceNavigatorMoleculeControlKey) > 0 ? true : false;
            }

            if (PlayerPrefs.HasKey(playerPrefsMoleculeMovementSpeedKey)) {

                moleculeMovementSpeed = PlayerPrefs.GetInt(playerPrefsMoleculeMovementSpeedKey);
                generalSettings.MoleculeInputSensitivity = (float)(moleculeMovementSpeed - Settings.MinMoleculeMovementSpeed) / (float)(Settings.MaxMoleculeMovementSpeed - Settings.MinMoleculeMovementSpeed);
            }
            else {
                moleculeMovementSpeed = (int)((generalSettings.MoleculeInputSensitivity * (float)(Settings.MaxMoleculeMovementSpeed - Settings.MinMoleculeMovementSpeed)) + Settings.MinMoleculeMovementSpeed);
            }

            groundToggle.isOn = generalSettings.ShowGround;
            shadowsToggle.isOn = generalSettings.ShowShadows;
            mainLightsToggle.isOn = generalSettings.MainLightsOn;
            fillLightsToggle.isOn = generalSettings.FillLightsOn;
            ambientLightsToggle.isOn = generalSettings.AmbientLightsOn;
            lightIntensityText.text = lightIntensity.ToString();

            autoRotateSpeedText.text = autoRotateSpeed.ToString();
            moleculeMovementSpeedText.text = moleculeMovementSpeed.ToString();
            spaceNavigatorCameraControlToggle.isOn = generalSettings.SpaceNavigatorCameraControlEnabled;
            spaceNavigatorMoleculeControlToggle.isOn = generalSettings.SpaceNavigatorMoleculeControlEnabled;

            autoMeshQualityToggle.isOn = generalSettings.AutoMeshQuality;
            generalSettings.MeshQuality = Mathf.Clamp(generalSettings.MeshQuality, 0, Settings.MeshQualityValues.Length - 1);
            meshQualityText.text = generalSettings.AutoMeshQuality ? "Auto" : Settings.MeshQualityValues[generalSettings.MeshQuality];

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

        public void OnAmbientLightsToggleChanged() {

            generalSettings.AmbientLightsOn = ambientLightsToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsAmbientLightKey, ambientLightsToggle.isOn ? 1 : 0);
        }

        public void IncreaseLightIntensity() {
            SetLightIntensity(lightIntensity + 1);
        }

        public void DecreaseLightIntensity() {
            SetLightIntensity(lightIntensity - 1);
        }

        public void SetLightIntensity(int intensity) {

            int startLighIntensity = lightIntensity;
            lightIntensity = Mathf.Clamp(intensity, Settings.MinLightIntensity, Settings.MaxLightIntensity);

            if (lightIntensity != startLighIntensity) {

                lightIntensityText.text = lightIntensity.ToString();
                generalSettings.LightIntensity = (float)(lightIntensity - Settings.MinLightIntensity) / (float)(Settings.MaxLightIntensity - Settings.MinLightIntensity);
                UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
                PlayerPrefs.SetInt(playerPrefsLightIntensityKey, lightIntensity);
            }
        }

        public void OnSpaceNavigatorCameraControlChanged() {

            generalSettings.SpaceNavigatorCameraControlEnabled = spaceNavigatorCameraControlToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsSpaceNavigatorCameraControlKey, spaceNavigatorCameraControlToggle.isOn ? 1 : 0);
        }

        public void OnSpaceNavigatorMoleculeControlChanged() {

            generalSettings.SpaceNavigatorMoleculeControlEnabled = spaceNavigatorMoleculeControlToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            PlayerPrefs.SetInt(playerPrefsSpaceNavigatorMoleculeControlKey, spaceNavigatorMoleculeControlToggle.isOn ? 1 : 0);
        }

        public void InreaseAutoRotateSpeed() {
            SetAutoRotateSpeed(autoRotateSpeed + 1);
        }

        public void DecreaseAutoRotateSpeed() {
            SetAutoRotateSpeed(autoRotateSpeed - 1);
        }

        public void SetAutoRotateSpeed(int speed) {

            int startSpeed = autoRotateSpeed;
            autoRotateSpeed = Mathf.Clamp(speed, Settings.MinAutoRotateSpeed, Settings.MaxAutoRotateSpeed);

            if (autoRotateSpeed != startSpeed) {

                autoRotateSpeedText.text = autoRotateSpeed.ToString();

                generalSettings.AutoRotateSpeed = (float)(autoRotateSpeed - Settings.MinAutoRotateSpeed) / (float)(Settings.MaxAutoRotateSpeed - Settings.MinAutoRotateSpeed);
                UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
                PlayerPrefs.SetInt(playerPrefsAutoRotateKey, autoRotateSpeed);
            }
        }

        public void InreaseMouseSpeed() {
            SetMouseSpeed(mouseSpeed + 1);
        }

        public void DecreaseMouseSpeed() {
            SetMouseSpeed(mouseSpeed - 1);
        }

        public void SetMouseSpeed(int speed) {

            int startSpeed = mouseSpeed;
            mouseSpeed = Mathf.Clamp(speed, Settings.MinMouseSpeed, Settings.MaxMouseSpeed);

            if (mouseSpeed != startSpeed) {

                mouseSpeed = speed;

                mouseCursor.GetComponent<MousePointer>().MouseSpeed = mouseSpeed * Settings.MouseSpeedMultiplier;
                mouseSpeedText.text = mouseSpeed.ToString();

                PlayerPrefs.SetInt(playerPrefsMouseSpeedKey, mouseSpeed);
            }
        }

        public void InreaseMoleculeMovementSpeed() {
            SetMoleculeMovementSpeed(moleculeMovementSpeed + 1);
        }

        public void DecreaseMoleculeMovementSpeed() {
            SetMoleculeMovementSpeed(moleculeMovementSpeed - 1);
        }

        public void SetMoleculeMovementSpeed(int speed) {

            int startSpeed = moleculeMovementSpeed;
            moleculeMovementSpeed = Mathf.Clamp(speed, Settings.MinMoleculeMovementSpeed, Settings.MaxMoleculeMovementSpeed);

            if (moleculeMovementSpeed != startSpeed) {

                moleculeMovementSpeedText.text = moleculeMovementSpeed.ToString();

                generalSettings.MoleculeInputSensitivity = (float)(moleculeMovementSpeed- Settings.MinMoleculeMovementSpeed) / (float)(Settings.MaxMoleculeMovementSpeed - Settings.MinMoleculeMovementSpeed);
                UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
                PlayerPrefs.SetInt(playerPrefsMoleculeMovementSpeedKey, moleculeMovementSpeed);
            }
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
