using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class CameraController : MonoBehaviour {

        public List<CanvasGroup> CanvasGroups;

        public float CameraSensitivityX = 90;
        public float CameraSensitivityY = 90;
        public float ClimbSpeed = 4;
        public float NormalMoveSpeed = 10;
        public float FastMoveFactor = 3;

        public float MinimumXPosition = -100;
        public float MaximumXPosition = 100;
        public float MinimumYPosition = 1.5f;
        public float MaximumYPosition = 10;
        public float MinimumZPosition = -100;
        public float MaximumZPosition = 100;

        private float rotationX = 120.0f;
        private float rotationY = 0.0f;
        private bool movementEnabled;

        void Start() {

            rotationX = 0 - transform.localRotation.eulerAngles.x;
            rotationY = transform.localRotation.eulerAngles.y;

            movementEnabled = false;
        }

        void Update() {

            if (Input.GetMouseButtonDown(1)) {
                movementEnabled = true;
                rotationX = transform.localRotation.eulerAngles.x;
                if (rotationX > 270) {
                    rotationX -= 360;
                }
                rotationY = transform.localRotation.eulerAngles.y;
            }

            if (Input.GetMouseButtonUp(1)) {
                movementEnabled = false;
            }

            if (movementEnabled) {

                rotationY += Input.GetAxis("Mouse X") * CameraSensitivityX * Time.deltaTime;
                rotationX -= Input.GetAxis("Mouse Y") * CameraSensitivityY * Time.deltaTime;
                rotationX = Mathf.Clamp(rotationX, -90, 90);

                transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

                float moveSpeed = NormalMoveSpeed;

                // this is only good for absolute values, not incremental values
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
                    transform.position = transformPosition(transform.position, 0, ClimbSpeed * Time.deltaTime, 0);
                }
                if (Input.GetKey(KeyCode.Q)) {
                    transform.position = transformPosition(transform.position, 0, -1 * ClimbSpeed * Time.deltaTime, 0);
                }
            }
        }

        private Vector3 transformPosition(Vector3 pos, float x, float y, float z) {

            pos.x += x;
            pos.y += y;
            pos.z += z;

            pos.x = Mathf.Clamp(pos.x, MinimumXPosition, MaximumXPosition);
            pos.y = Mathf.Clamp(pos.y, MinimumYPosition, MaximumYPosition);
            pos.z = Mathf.Clamp(pos.z, MinimumZPosition, MaximumZPosition);

            return pos;
        }
    }
}