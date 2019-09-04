using UnityEngine;

using SpaceNavigatorDriver;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public enum CameraInputSource {
        Mouse,
        SpaceNavigator
    }

    public class CameraController : MonoBehaviour {

        private CameraInputSource cameraInputSource;

        private float CameraSensitivityX = 90;
        private float CameraSensitivityY = 90;
        private float ClimbSpeed = 4;
        private float NormalMoveSpeed = 10;
        private float FastMoveFactor = 3;
        private float rotationX = 120.0f;
        private float rotationY = 0.0f;

        private void Awake() {

            cameraInputSource = CameraInputSource.Mouse;
            rotationX = 0 - transform.localRotation.eulerAngles.x;
            rotationY = transform.localRotation.eulerAngles.y;
        }

        private void Update() {

            if (cameraInputSource == CameraInputSource.Mouse) {
                handleMouseInput();
            }

            if (cameraInputSource == CameraInputSource.SpaceNavigator) {

                // Don't move camera when moving molecules
                // See moleculeInputController for input mappings
                if(!InputManager.Instance.ShiftPressed && !InputManager.Instance.ControlPressed && !InputManager.Instance.AltPressed) {
                    handleSpaceNavigator();
                }
            }
        }

        public void SetInputSource(CameraInputSource source) {
            cameraInputSource = source;
        }

        private void handleMouseInput() { 

            if (Input.GetMouseButtonDown(1)) {
                rotationX = transform.localRotation.eulerAngles.x;
                if (rotationX > 270) {
                    rotationX -= 360;
                }
                rotationY = transform.localRotation.eulerAngles.y;
            }

            if (Input.GetMouseButton(1)) {

                // handle camera rotation

                rotationY += Input.GetAxis("Mouse X") * CameraSensitivityX * Time.deltaTime;
                rotationX -= Input.GetAxis("Mouse Y") * CameraSensitivityY * Time.deltaTime;
                rotationX = Mathf.Clamp(rotationX, -90, 90);

                transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

                // handle movement
                float moveSpeed = NormalMoveSpeed;

                // slow down diagonal movement
                if (Input.GetAxis("Vertical") != 0 && Input.GetAxis("Horizontal") != 0) {
                    moveSpeed *= 0.70710678118f;
                }

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                    moveSpeed *= FastMoveFactor;
                }

                Vector3 newPosition = transform.position;
                newPosition += transform.forward * moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
                newPosition += transform.right * moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
                newPosition.y = transform.position.y;
                transform.position = newPosition;

                if (Input.GetKey(KeyCode.E)) {
                    transform.Translate(new Vector3(0, ClimbSpeed * Time.deltaTime, 0));
                }
                if (Input.GetKey(KeyCode.Q)) {
                    transform.Translate(new Vector3(0, -1 * ClimbSpeed * Time.deltaTime, 0));
                }
            }
        }

        private void handleSpaceNavigator() {

            Vector3 translation = SpaceNavigator.Translation * 0.5f;
            Vector3 rotation = SpaceNavigator.Rotation.eulerAngles;

            transform.Translate(translation, Space.Self);
            transform.Rotate(Vector3.up, rotation.y, Space.Self);
        }
    }
}