using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Visualization;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class Scene : MonoBehaviour {

        public static Scene Instance;

        private void Awake() {

            if (Instance == null) {
                Instance = this;
            }
            else if (Instance != this) {
                Destroy(gameObject);
            }
        }
    }
}
