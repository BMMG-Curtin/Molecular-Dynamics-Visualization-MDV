
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class SceneCamera : MonoBehaviour {

        private static SceneCamera _instance;
        public static SceneCamera Instance { get { return _instance; } }

        private Camera camera;

        private void Awake() {

            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            }
            else {
                _instance = this;
            }

            camera = GetComponent<Camera>();
        }

        public Camera GetCamera() {
            return camera;
        }
    }
}
