using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MousePointer : MonoBehaviour {

        [SerializeField]
        private GameObject canvas;

        [SerializeField]
        public float MouseSpeed = 8;

        private RectTransform rectTransform;

        void Start() {

            rectTransform = GetComponent<RectTransform>();
        }

        void Update() {

            // find mouse input in the last frame
            Vector2 mouseMove = new Vector2();

            mouseMove.x = Input.GetAxis("Mouse X");
            mouseMove.y = Input.GetAxis("Mouse Y");

            // calculate the new mouse position
            mouseMove *= MouseSpeed;
            Vector2 newPosition = new Vector2(rectTransform.localPosition.x + mouseMove.x, rectTransform.localPosition.y + mouseMove.y);

            // dont allow position outside of canvas bounds
            Rect canvasRect = canvas.GetComponent<RectTransform>().rect;
            newPosition.x = Mathf.Clamp(newPosition.x, (canvasRect.width / 2) * -1, canvasRect.width / 2);
            newPosition.y = Mathf.Clamp(newPosition.y, (canvasRect.height / 2) * -1, canvasRect.height / 2);

            // assign new position to mouse pointer
            rectTransform.localPosition = newPosition;
        }
    }
}
