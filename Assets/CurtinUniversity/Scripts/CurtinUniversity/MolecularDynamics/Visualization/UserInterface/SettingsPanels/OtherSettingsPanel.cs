using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class OtherSettingsPanel : MonoBehaviour {

        public GameObject MouseCursor;

        public Toggle GroundToggle;
        public Toggle ShadowsToggle;
        public Toggle LightsToggle;

        public Text PrimaryStructureMeshQualityText;
        private int primaryStructureMeshQuality;
        private string playerPrefsMeshQualityKey = @"PrimaryStructureMeshQuality";

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

            if (PlayerPrefs.HasKey(playerPrefsMeshQualityKey)) {
                primaryStructureMeshQuality = PlayerPrefs.GetInt(playerPrefsMeshQualityKey);
            }
            else {
                primaryStructureMeshQuality = Settings.DefaultPrimaryStructureMeshQuality;
            }

            primaryStructureMeshQuality = Mathf.Clamp(primaryStructureMeshQuality, 0, Settings.PrimaryStructureMeshQualityValues.Length - 1);
            PlayerPrefs.SetInt(playerPrefsMeshQualityKey, primaryStructureMeshQuality);
            PrimaryStructureMeshQualityText.text = Settings.PrimaryStructureMeshQualityValues[primaryStructureMeshQuality];
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

        public void IncreaseAtomMeshQuality() {

            if (primaryStructureMeshQuality < Settings.PrimaryStructureMeshQualityValues.Length - 1) {
                primaryStructureMeshQuality++;
                PrimaryStructureMeshQualityText.text = Settings.PrimaryStructureMeshQualityValues[primaryStructureMeshQuality];

                // request rerender
            }

            PlayerPrefs.SetInt(playerPrefsMeshQualityKey, primaryStructureMeshQuality);
        }

        public void DecreaseAtomMeshQuality() {

            if (primaryStructureMeshQuality > 0) {
                primaryStructureMeshQuality--;
                PrimaryStructureMeshQualityText.text = Settings.PrimaryStructureMeshQualityValues[primaryStructureMeshQuality];

                // request rerender
            }

            PlayerPrefs.SetInt(playerPrefsMeshQualityKey, primaryStructureMeshQuality);
        }
    }
}
