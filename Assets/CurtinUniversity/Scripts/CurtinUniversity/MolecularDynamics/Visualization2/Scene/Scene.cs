
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class Scene : MonoBehaviour {

        public GameObject Ground;
        public Lighting Lighting;

        private SceneSettings settings;

        public SceneSettings Settings {

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
