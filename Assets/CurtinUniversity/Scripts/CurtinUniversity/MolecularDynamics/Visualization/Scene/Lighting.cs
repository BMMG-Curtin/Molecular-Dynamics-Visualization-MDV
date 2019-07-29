using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class Lighting : MonoBehaviour {

        public Light Light1;
        public Light Light2;
        public Light Light3;

        private SceneManager sceneManager;

        private LightShadows light1Shadows;
        private LightShadows light2Shadows;
        private LightShadows light3Shadows;

        private float minHeight;
        private float height;

        private float generalLightBrightness = 1f;

        private float startAmbientIntensity;
        private Color startAmbientLight;

        void Awake() {

            startAmbientLight = RenderSettings.ambientLight;
            startAmbientIntensity = RenderSettings.ambientIntensity;

            light1Shadows = Light1.shadows;
            light2Shadows = Light2.shadows;
            light3Shadows = Light3.shadows;

            EnableShadows(true);
        }

        public void EnableShadows(bool enable) {

            if (enable) {
                Light1.shadows = light1Shadows;
                Light2.shadows = light2Shadows;
                Light3.shadows = light3Shadows;
            }
            else {
                Light1.shadows = LightShadows.None;
                Light2.shadows = LightShadows.None;
                Light3.shadows = LightShadows.None;
            }
        }

        public void SetLighting(Vector3 lightFocus, float height, float radius) {

            //MainLight.transform.position = new Vector3(lightFocus.x, height * 3f, lightFocus.z);

            //SpotLights[0].transform.position = new Vector3(lightFocus.x - radius, height, lightFocus.z - radius);
            //SpotLights[1].transform.position = new Vector3(lightFocus.x + radius, height, lightFocus.z - radius);
            //SpotLights[2].transform.position = new Vector3(lightFocus.x - radius, height, lightFocus.z + radius);
            //SpotLights[3].transform.position = new Vector3(lightFocus.x + radius, height, lightFocus.z + radius);

            //foreach (Light spotlight in SpotLights) {
            //    spotlight.transform.LookAt(lightFocus);
            //    if (!spotlight.gameObject.activeSelf) { 
            //        spotlight.gameObject.SetActive(true);
            //    }
            //}
        }

        public float Brightness {

            get {
                return generalLightBrightness;
            }

            set {
                generalLightBrightness = value;
            }
        }

        public void EnableLighting(bool enable) {

            //foreach (Light light in SpotLights) {
            //    light.gameObject.SetActive(enable);
            //}

            Light1.gameObject.SetActive(enable);
            Light2.gameObject.SetActive(enable);
            Light3.gameObject.SetActive(enable);

            if (enable) {

                RenderSettings.ambientLight = startAmbientLight;
                RenderSettings.ambientIntensity = startAmbientIntensity;
            }
            else {

                RenderSettings.ambientLight = Color.white;
                RenderSettings.ambientIntensity = 1f;
            }
        }

        public IEnumerator DimToBlack(float time) {

            yield return null;

            //float mainLightIntensity = MainLight.intensity;
            //float areaLightIntensity = AreaLight.intensity;
            //float spotLightIntensity = SpotLights[0].intensity;
            //float fillLightIntensity = FillLights[0].intensity;
            //float startGlobeEmission = 1f;

            //foreach (Light light in SpotLights) {
            //    ((Behaviour)light.GetComponent("Halo")).enabled = false;
            //}

            //float i = 0;
            //float rate = 1 / time;

            //while (i < 1.0) {

            //    i += Time.deltaTime * rate;

            //    MainLight.intensity = Mathf.Lerp(mainLightIntensity, 0, Mathf.SmoothStep(0, 1, i));
            //    AreaLight.intensity = Mathf.Lerp(areaLightIntensity, 0, Mathf.SmoothStep(0, 1, i));

            //    float fillIntensity = Mathf.Lerp(fillLightIntensity, 0, Mathf.SmoothStep(0, 1, i));
            //    foreach (Light light in FillLights) {
            //        light.intensity = fillIntensity;
            //    }

            //    float spotIntensity = Mathf.Lerp(spotLightIntensity, 0, Mathf.SmoothStep(0, 1, i));
            //    foreach (Light light in SpotLights) {
            //        light.intensity = spotIntensity;
            //    }

            //    foreach (GameObject globe in LightGlobes) {
            //        float emission = Mathf.Lerp(startGlobeEmission, 0, Mathf.SmoothStep(0, 1, i));
            //        globe.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * emission);
            //    }

            //    yield return null;
            //}
        }

        public IEnumerator LightToDefaults(float time) {


            yield return null;


            //float i = 0;
            //float rate = 1 / time;

            //float mainLight = Settings.DefaultMainLightBrightness * Brightness;
            //float areaLight = Settings.DefaultAreaLightBrightness * Brightness;
            //float spotLight = Settings.DefaultSpotLightBrightness * Brightness;
            //float fillLight = Settings.DefaultFillLightBrightness * Brightness;

            //while (i < 1.0) {

            //    i += Time.deltaTime * rate;

            //    MainLight.intensity = Mathf.Lerp(0, mainLight, Mathf.SmoothStep(0, 1, i));
            //    AreaLight.intensity = Mathf.Lerp(0, areaLight, Mathf.SmoothStep(0, 1, i));

            //    float spotIntensity = Mathf.Lerp(0, spotLight, Mathf.SmoothStep(0, 1, i));
            //    foreach (Light light in SpotLights) {
            //        light.intensity = spotIntensity;
            //    }

            //    float fillIntensity = Mathf.Lerp(0, fillLight, Mathf.SmoothStep(0, 1, i));
            //    foreach (Light light in FillLights) {
            //        light.intensity = fillIntensity;
            //    }

            //    foreach (GameObject globe in LightGlobes) {
            //        float emission = Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, i));
            //        globe.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * emission);
            //    }

            //    yield return null;
            //}

            //foreach (Light light in SpotLights) {
            //    ((Behaviour)light.GetComponent("Halo")).enabled = true;
            //}

            //yield break;
        }
    }
}
