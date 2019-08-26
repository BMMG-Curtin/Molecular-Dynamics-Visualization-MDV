using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class OtherSettingsPanel : MonoBehaviour {

        public GameObject MouseCursor;

        public Toggle GroundToggle;
        public Toggle ShadowsToggle;
        public Toggle LightsToggle;
        public Toggle AutoMeshQualityToggle;

        public Text MeshQualityText;
        private int meshQuality;

        public Text MouseSpeedText;
        public float mouseSpeedMultiplier = 3f;
        private string playerPrefsMouseSpeedKey = @"MouseSpeed";
        private int mouseSpeed = 1;

        private GeneralSettings generalSettings;

        public void Awake() {
            
            if (PlayerPrefs.HasKey(playerPrefsMouseSpeedKey)) {

                mouseSpeed = PlayerPrefs.GetInt(playerPrefsMouseSpeedKey);

                if(mouseSpeed < 1) {
                    mouseSpeed = 1;
                }

                setMouseSpeed(mouseSpeed);
            }

            AutoMeshQualityToggle.isOn = Settings.DefaultAutoMeshQuality;
            meshQuality = Settings.DefaultMeshQuality;
            meshQuality = Mathf.Clamp(meshQuality, 0, Settings.MeshQualityValues.Length - 1);
            MeshQualityText.text = AutoMeshQualityToggle.isOn ? "Auto" : MeshQualityText.text = Settings.MeshQualityValues[meshQuality];
        }

        public void SetSceneSettings(GeneralSettings settings) {

            generalSettings = settings;
            reloadUIControls();
        }

        private void reloadUIControls() {

            GroundToggle.isOn = generalSettings.ShowGround;
            ShadowsToggle.isOn = generalSettings.ShowShadows;
            LightsToggle.isOn = generalSettings.LightsOn;
        }

        public void OnGroundToggleChanged() {

            generalSettings.ShowGround = GroundToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
        }

        public void OnShadowsToggleChanged() {

            generalSettings.ShowShadows = ShadowsToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
        }

        public void OnLightsToggleChanged() {

            generalSettings.LightsOn= LightsToggle.isOn;
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

            MouseCursor.GetComponent<MousePointer>().MouseSpeed = mouseSpeed * Settings.MouseSpeedMultiplier;
            MouseSpeedText.text = mouseSpeed.ToString();

            PlayerPrefs.SetInt(playerPrefsMouseSpeedKey, speed);
        }

        public void IncreaseMeshQuality() {

            if(AutoMeshQualityToggle.isOn) {
                return;
            }

            if (meshQuality < Settings.MeshQualityValues.Length - 1) {

                meshQuality++;
                MeshQualityText.text = Settings.MeshQualityValues[meshQuality];

                generalSettings.MeshQuality = meshQuality;
                UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            }
        }

        public void DecreaseMeshQuality() {

            if (AutoMeshQualityToggle.isOn) {
                return;
            }

            if (meshQuality > 0) {
                meshQuality--;
                MeshQualityText.text = Settings.MeshQualityValues[meshQuality];

                generalSettings.MeshQuality = meshQuality;
                UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
            }
        }

        public void OnAutoMeshQualityToggleChanged() {

            MeshQualityText.text = AutoMeshQualityToggle.isOn ? "Auto" : MeshQualityText.text = Settings.MeshQualityValues[meshQuality];
            generalSettings.AutoMeshQuality = AutoMeshQualityToggle.isOn;
            UserInterfaceEvents.RaiseGeneralSettingsUpdated(generalSettings);
        }
    }
}
