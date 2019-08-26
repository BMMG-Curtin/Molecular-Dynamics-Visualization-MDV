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
        private Toggle lightsToggle;

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
        private int mouseSpeed = 1;
        private int meshQuality;

        private GeneralSettings generalSettings;

        public void Awake() {
            
            if (PlayerPrefs.HasKey(playerPrefsMouseSpeedKey)) {

                mouseSpeed = PlayerPrefs.GetInt(playerPrefsMouseSpeedKey);

                if(mouseSpeed < 1) {
                    mouseSpeed = 1;
                }

                setMouseSpeed(mouseSpeed);
            }

            autoMeshQualityToggle.isOn = Settings.DefaultAutoMeshQuality;
            meshQuality = Settings.DefaultMeshQuality;
            meshQuality = Mathf.Clamp(meshQuality, 0, Settings.MeshQualityValues.Length - 1);
            meshQualityText.text = autoMeshQualityToggle.isOn ? "Auto" : meshQualityText.text = Settings.MeshQualityValues[meshQuality];
        }

        public void SetSceneSettings(GeneralSettings settings) {

            generalSettings = settings;
            reloadUIControls();
        }

        private void reloadUIControls() {

            groundToggle.isOn = generalSettings.ShowGround;
            shadowsToggle.isOn = generalSettings.ShowShadows;
            lightsToggle.isOn = generalSettings.LightsOn;
        }

        public void OnGroundToggleChanged() {

            generalSettings.ShowGround = groundToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
        }

        public void OnShadowsToggleChanged() {

            generalSettings.ShowShadows = shadowsToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
        }

        public void OnLightsToggleChanged() {

            generalSettings.LightsOn= lightsToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
        }

        public void InreaseMouseSpeed() {
            setMouseSpeed(mouseSpeed + 1);
        }

        public void DecreaseMouseSpeed() {
            setMouseSpeed(mouseSpeed - 1);
        }

        private void setMouseSpeed(int speed) {

            if (speed > Settings.MaxMouseSpeed || speed < Settings.MinMouseSpeed) {
                return;
            }

            mouseSpeed = speed;

            mouseCursor.GetComponent<MousePointer>().MouseSpeed = mouseSpeed * Settings.MouseSpeedMultiplier;
            mouseSpeedText.text = mouseSpeed.ToString();

            PlayerPrefs.SetInt(playerPrefsMouseSpeedKey, speed);
        }

        public void IncreaseMeshQuality() {

            if(autoMeshQualityToggle.isOn) {
                return;
            }

            if (meshQuality < Settings.MeshQualityValues.Length - 1) {

                meshQuality++;
                meshQualityText.text = Settings.MeshQualityValues[meshQuality];

                generalSettings.MeshQuality = meshQuality;
                UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            }
        }

        public void DecreaseMeshQuality() {

            if (autoMeshQualityToggle.isOn) {
                return;
            }

            if (meshQuality > 0) {
                meshQuality--;
                meshQualityText.text = Settings.MeshQualityValues[meshQuality];

                generalSettings.MeshQuality = meshQuality;
                UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            }
        }

        public void OnAutoMeshQualityToggleChanged() {

            meshQualityText.text = autoMeshQualityToggle.isOn ? "Auto" : meshQualityText.text = Settings.MeshQualityValues[meshQuality];
            generalSettings.AutoMeshQuality = autoMeshQualityToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
        }

        public void OnQuitApplicationButton() {

            confirmDialog.gameObject.SetActive(true);
            confirmDialog.Initialise("Quit application?", quitApplication);
        }

        private void quitApplication(bool confirmed) {
            if(confirmed) {
                Application.Quit();
            }
        }
    }
}
