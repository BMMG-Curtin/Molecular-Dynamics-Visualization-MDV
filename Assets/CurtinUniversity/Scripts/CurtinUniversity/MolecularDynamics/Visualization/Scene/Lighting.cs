using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    // Component to control lighting in scene
    public class Lighting : MonoBehaviour {

        [SerializeField]
        private Light MainLight1;

        [SerializeField]
        private Light MainLight2;

        [SerializeField]
        private Light MainLight3;

        [SerializeField]
        private Light FillLight1;

        private LightShadows mainLight1Shadows;
        private LightShadows mainLight2Shadows;
        private LightShadows mainLight3Shadows;

        private float minMainLightIntensity = 0.1f;
        private float maxMainLightIntensity = 0.6f;
        private float minFillLightIntensity = 0.4f;
        private float maxFillLightIntensity = 1.0f;

        private bool ambientLightEnabled = true;
        private Color ambientLightColor;

        void Awake() {

            mainLight1Shadows = MainLight1.shadows;
            mainLight2Shadows = MainLight2.shadows;
            mainLight3Shadows = MainLight3.shadows;

            FillLight1.shadows = LightShadows.None;

            EnableShadows(true);
        }

        public void EnableShadows(bool enable) {

            if (enable) {
                MainLight1.shadows = mainLight1Shadows;
                MainLight2.shadows = mainLight2Shadows;
                MainLight3.shadows = mainLight3Shadows;
            }
            else {
                MainLight1.shadows = LightShadows.None;
                MainLight2.shadows = LightShadows.None;
                MainLight3.shadows = LightShadows.None;
            }
        }

        public void SetLightIntensity(float intensity) {

            float mainLightIntensity = (intensity * (maxMainLightIntensity - minMainLightIntensity)) + minMainLightIntensity;
            float fillLightIntensity = (intensity * (maxFillLightIntensity - minFillLightIntensity)) + minFillLightIntensity;

            MainLight1.intensity = mainLightIntensity;
            MainLight2.intensity = mainLightIntensity;
            MainLight3.intensity = mainLightIntensity;

            FillLight1.intensity = fillLightIntensity;

            ambientLightColor = new Color(intensity, intensity, intensity);

            if (ambientLightEnabled) {
                RenderSettings.ambientLight = ambientLightColor;
            }
            else {
                RenderSettings.ambientLight = Color.black;
            }
        }

        public void EnableLighting(bool mainEnable, bool fillEnable, bool ambientEnable) {

            MainLight1.gameObject.SetActive(mainEnable);
            MainLight2.gameObject.SetActive(mainEnable);
            MainLight3.gameObject.SetActive(mainEnable);

            FillLight1.gameObject.SetActive(fillEnable);

            ambientLightEnabled = ambientEnable;

            if (ambientLightEnabled) {
                RenderSettings.ambientLight = ambientLightColor;
            }
            else {
                RenderSettings.ambientLight = Color.black;
            }
        }
    }
}
