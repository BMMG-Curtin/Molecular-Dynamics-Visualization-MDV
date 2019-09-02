using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class Lighting : MonoBehaviour {

        public Light MainLight1;
        public Light MainLight2;
        public Light MainLight3;
        public Light FillLight1;

        private SceneManager sceneManager;

        private LightShadows mainLight1Shadows;
        private LightShadows mainLight2Shadows;
        private LightShadows mainLight3Shadows;

        private float minHeight;
        private float height;

        private float generalLightBrightness = 1f;

        private float startAmbientIntensity;
        private Color startAmbientLight;

        private float minMainLightIntensity = 0.1f;
        private float maxMainLightIntensity = 0.6f;
        private float minFillLightIntensity = 0.4f;
        private float maxFillLightIntensity = 1.0f;

        void Awake() {

            startAmbientLight = RenderSettings.ambientLight;
            startAmbientIntensity = RenderSettings.ambientIntensity;

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
        }

        public void EnableLighting(bool mainEnable, bool fillEnable) {

            MainLight1.gameObject.SetActive(mainEnable);
            MainLight2.gameObject.SetActive(mainEnable);
            MainLight3.gameObject.SetActive(mainEnable);

            FillLight1.gameObject.SetActive(fillEnable);


            if (mainEnable || fillEnable) {

                RenderSettings.ambientLight = startAmbientLight;
                RenderSettings.ambientIntensity = startAmbientIntensity;
            }
            else {

                RenderSettings.ambientLight = Color.white;
                RenderSettings.ambientIntensity = 1f;
            }
        }
    }
}
