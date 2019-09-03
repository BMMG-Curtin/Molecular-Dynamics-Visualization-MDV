
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class Scene : MonoBehaviour {

        public GameObject Ground;
        public Lighting Lighting;

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
