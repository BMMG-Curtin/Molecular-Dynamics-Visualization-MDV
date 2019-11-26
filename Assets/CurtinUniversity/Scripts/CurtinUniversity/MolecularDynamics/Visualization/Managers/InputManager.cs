
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    // Helper class to privide information on modifiers to other input managers
    // See the '\Input' folder for user interface and molecule input managers
    public class InputManager : MonoBehaviour {

        public bool ShiftPressed { get; private set; }
        public bool ControlPressed { get; private set; }
        public bool AltPressed { get; private set; }
        public bool NoModifiersPressed { get; private set; }

        private static InputManager _instance;
        public static InputManager Instance { get { return _instance; } }

        private void Awake() {

            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            }
            else {
                _instance = this;
            }
        }

        private void Start() {
            getKeyboardModifiers();
        }

        void Update() {
            getKeyboardModifiers();
        }

        private void getKeyboardModifiers() {

            ShiftPressed = false;
            ControlPressed = false;
            AltPressed = false;
            NoModifiersPressed = true;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                ShiftPressed = true;
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                ControlPressed = true;
            }

            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
                AltPressed = true;
            }

            if (ShiftPressed || ControlPressed || AltPressed) {
                NoModifiersPressed = false;
            }
        }
    }
}
