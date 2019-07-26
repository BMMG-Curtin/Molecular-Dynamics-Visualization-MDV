using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

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
