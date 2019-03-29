using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class Lighting : MonoBehaviour {

        public Light MainLight;
        public Light AreaLight;
        public Light[] FillLights = new Light[4];
        public Light[] SpotLights = new Light[4];
        public GameObject[] LightGlobes = new GameObject[4];

        private SceneManager sceneManager;

        private LightShadows mainLightShadow;
        private Dictionary<Light, LightShadows> lights = new Dictionary<Light, LightShadows>();

        private float minHeight;
        private float height;

        // Use this for initialization
        void Start() {

            sceneManager = SceneManager.instance;

            mainLightShadow = MainLight.shadows;
            foreach (Light light in SpotLights) {
                lights[light] = light.shadows;
            }

            EnableShadows(Settings.ShowShadows);
        }

        // Update is called once per frame
        void Update() {

        }

        public void EnableShadows(bool enable) {

            if (enable) {
                MainLight.shadows = mainLightShadow;
            }
            else {
                MainLight.shadows = LightShadows.None;
            }

            foreach (KeyValuePair<Light, LightShadows> light in lights) {
                if (enable) {
                    light.Key.shadows = light.Value;
                }
                else {
                    light.Key.shadows = LightShadows.None;
                }
            }
        }

        public void SetLighting(PrimaryStructure model, BoundingBox boundingBox) {

            Vector3 modelCentre = new Vector3(0, boundingBox.Centre.y, 0);

            float boxWidth = sceneManager.ModelBox.Width;
            float boxDepth = sceneManager.ModelBox.Depth;
            float distance = boxWidth > boxDepth ? boxWidth : boxDepth;

            distance *= 1.5f;
            distance = Mathf.Clamp(distance, 5, 20);

            float lightHeight = sceneManager.ModelBox.Height * 2;
            // float lightHeight = boxMax * 2;

            if (sceneManager.ModelBox.Height < Settings.ModelHoverHeight) {
                lightHeight = Settings.ModelHoverHeight * 2;
                //boxMax *= 1.5f;
            }

            MainLight.transform.position = new Vector3(modelCentre.x, lightHeight * 3f, modelCentre.z);

            SpotLights[0].transform.position = new Vector3(modelCentre.x - distance, lightHeight, modelCentre.z - distance);
            SpotLights[1].transform.position = new Vector3(modelCentre.x + distance, lightHeight, modelCentre.z - distance);
            SpotLights[2].transform.position = new Vector3(modelCentre.x - distance, lightHeight, modelCentre.z + distance);
            SpotLights[3].transform.position = new Vector3(modelCentre.x + distance, lightHeight, modelCentre.z + distance);

            foreach (Light spotlight in SpotLights) {
                spotlight.transform.LookAt(modelCentre);
                if (!spotlight.gameObject.activeSelf && Settings.ShowLights) {
                    spotlight.gameObject.SetActive(true);
                }
            }
        }

        public void EnableLighting(bool enable) {

            EnableLightGlobes(enable);

            foreach (Light light in SpotLights) {
                light.gameObject.SetActive(enable);
            }

            foreach (Light light in FillLights) {
                light.gameObject.SetActive(enable);
            }

            MainLight.gameObject.SetActive(enable);
            AreaLight.gameObject.SetActive(enable);

            Settings.ShowGround = enable;
            sceneManager.Ground.SetActive(enable);
            sceneManager.GUIManager.ReloadOptions();

            if (enable) {
                RenderSettings.ambientIntensity = 0f;
                RenderSettings.ambientLight = Color.black;
            }
            else {
                RenderSettings.ambientIntensity = 1f;
                RenderSettings.ambientLight = Color.white;
            }
        }

        public void EnableLightGlobes(bool show) {

            foreach (Light light in SpotLights) {
                Behaviour halo = (Behaviour)light.GetComponent("Halo");
                halo.enabled = show;
            }

            foreach (GameObject globe in LightGlobes) {
                globe.SetActive(show);
            }
        }

        public IEnumerator DimToBlack(float time) {

            float mainLightIntensity = MainLight.intensity;
            float areaLightIntensity = AreaLight.intensity;
            float spotLightIntensity = SpotLights[0].intensity;
            float fillLightIntensity = FillLights[0].intensity;
            float startGlobeEmission = 1f;

            foreach (Light light in SpotLights) {
                ((Behaviour)light.GetComponent("Halo")).enabled = false;
            }

            float i = 0;
            float rate = 1 / time;

            while (i < 1.0) {

                i += Time.deltaTime * rate;

                MainLight.intensity = Mathf.Lerp(mainLightIntensity, 0, Mathf.SmoothStep(0, 1, i));
                AreaLight.intensity = Mathf.Lerp(areaLightIntensity, 0, Mathf.SmoothStep(0, 1, i));

                float fillIntensity = Mathf.Lerp(fillLightIntensity, 0, Mathf.SmoothStep(0, 1, i));
                foreach (Light light in FillLights) {
                    light.intensity = fillIntensity;
                }

                float spotIntensity = Mathf.Lerp(spotLightIntensity, 0, Mathf.SmoothStep(0, 1, i));
                foreach (Light light in SpotLights) {
                    light.intensity = spotIntensity;
                }

                foreach (GameObject globe in LightGlobes) {
                    float emission = Mathf.Lerp(startGlobeEmission, 0, Mathf.SmoothStep(0, 1, i));
                    globe.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * emission);
                }

                yield return null;
            }
        }

        public IEnumerator LightToDefaults(float time) {

            float i = 0;
            float rate = 1 / time;

            float mainLight = Settings.DefaultMainLightBrightness * Settings.GeneralLightBrightness;
            float areaLight = Settings.DefaultAreaLightBrightness * Settings.GeneralLightBrightness;
            float spotLight = Settings.DefaultSpotLightBrightness * Settings.GeneralLightBrightness;
            float fillLight = Settings.DefaultFillLightBrightness * Settings.GeneralLightBrightness;

            while (i < 1.0) {

                i += Time.deltaTime * rate;

                MainLight.intensity = Mathf.Lerp(0, mainLight, Mathf.SmoothStep(0, 1, i));
                AreaLight.intensity = Mathf.Lerp(0, areaLight, Mathf.SmoothStep(0, 1, i));

                float spotIntensity = Mathf.Lerp(0, spotLight, Mathf.SmoothStep(0, 1, i));
                foreach (Light light in SpotLights) {
                    light.intensity = spotIntensity;
                }

                float fillIntensity = Mathf.Lerp(0, fillLight, Mathf.SmoothStep(0, 1, i));
                foreach (Light light in FillLights) {
                    light.intensity = fillIntensity;
                }

                foreach (GameObject globe in LightGlobes) {
                    float emission = Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, i));
                    globe.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * emission);
                }

                yield return null;
            }

            foreach (Light light in SpotLights) {
                ((Behaviour)light.GetComponent("Halo")).enabled = true;
            }

            yield break;
        }
    }
}
