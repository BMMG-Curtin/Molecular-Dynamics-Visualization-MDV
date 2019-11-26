
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    // Component to update lighting and ground settings from settings data
    public class Scene : MonoBehaviour {

        [SerializeField]
        private GameObject Ground;

        [SerializeField]
        private Lighting Lighting;

        private GeneralSettings settings;

        public GeneralSettings Settings {

            get {
                return settings;
            }

            set {

                settings = value;

                Ground.SetActive(settings.ShowGround);
                Lighting.EnableLighting(settings.MainLightsOn, settings.FillLightsOn, settings.AmbientLightsOn);
                Lighting.EnableShadows(settings.ShowShadows);
                Lighting.SetLightIntensity(settings.LightIntensity);
            }
        }
    }
}
