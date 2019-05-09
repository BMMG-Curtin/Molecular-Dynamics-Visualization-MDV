using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Visualization;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class Scene : MonoBehaviour {

        public GameObject MainCameraTransform;
        public GameObject Ground;
        public Lighting Lighting;

        public static Scene Instance;

        private SceneSettings settings;

        private void Awake() {

            if (Instance == null) {
                Instance = this;
            }
            else if (Instance != this) {
                Destroy(gameObject);
            }
        }

        public SceneSettings Settings {

            get {
                return settings;
            }

            set {

                settings = value;
                Ground.SetActive(settings.ShowGround);
                Debug.Log("Ground showing: " + settings.ShowGround);
            }
        }
    }
}
