
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    // Component to help MoleculeInputController find the main camera
    public class SceneCamera : MonoBehaviour {

        private static SceneCamera _instance;
        public static SceneCamera Instance { get { return _instance; } }

        private Camera sceneCamera;

        private void Awake() {

            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            }
            else {
                _instance = this;
            }

            sceneCamera = GetComponent<Camera>();
        }

        public Camera GetCamera() {
            return sceneCamera;
        }
    }
}
