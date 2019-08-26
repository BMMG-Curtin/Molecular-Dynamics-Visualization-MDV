
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
                Lighting.EnableLighting(settings.LightsOn);
                Lighting.EnableShadows(settings.ShowShadows);
            }
        }
    }
}
