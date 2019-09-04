using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeInputController : MonoBehaviour {

        [SerializeField]
        private float moveSensitivity = 4;

        [SerializeField]
        private float rotateSensitivity = 100;

        public float MinimumXPosition = -100;
        public float MaximumXPosition = 100;
        public float MinimumYPosition = 1.5f;
        public float MaximumYPosition = 10;
        public float MinimumZPosition = -100;
        public float MaximumZPosition = 100;

        private Camera sceneCamera;
        private bool initialised = false;

        public void Initialise(Camera sceneCamera, Vector3 moleculeCentre) {

            this.sceneCamera = sceneCamera;
            initialised = true;
        }

        private void Update() {

            if (initialised) {

                if (InputManager.Instance.ShiftPressed) {
                    DoMove();
                }

                if (InputManager.Instance.ControlPressed) {
                    DoRotateForwardAxis();
                    DoRotateHorizontalAxis();
                }

                if (InputManager.Instance.AltPressed) {
                    DoRotateYAxis();
                }
            }
        }

        private void DoMove() {

            float horizontalAxis = Input.GetAxis("Mouse X");
            float verticalAxis = Input.GetAxis("Mouse Y");

            Vector3 forward = sceneCamera.transform.forward;
            Vector3 right = sceneCamera.transform.right;
            Vector3 up = sceneCamera.transform.up;
            //Vector3 up = Vector3.up;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            //Vector3 desiredMoveDirection = forward * verticalAxis + right * horizontalAxis;
            Vector3 desiredMoveDirection = up * verticalAxis + right * horizontalAxis;
            transform.Translate(desiredMoveDirection * moveSensitivity * Time.deltaTime, Space.World);
        }

        private void DoRotateForwardAxis() {

            float horizontalAxis = Input.GetAxis("Mouse X");
            transform.RotateAround(transform.position, sceneCamera.transform.forward, horizontalAxis * Time.deltaTime * rotateSensitivity);
        }

        private void DoRotateHorizontalAxis() {

            float horizontalAxis = Input.GetAxis("Mouse Y");
            transform.RotateAround(transform.position, sceneCamera.transform.right, horizontalAxis * Time.deltaTime * rotateSensitivity);
        }

        private void DoRotateYAxis() {

            float horizontalAxis = Input.GetAxis("Mouse X") * -1;
            transform.RotateAround(transform.position, Vector3.up, horizontalAxis * Time.deltaTime * rotateSensitivity);
        }
    }
}
