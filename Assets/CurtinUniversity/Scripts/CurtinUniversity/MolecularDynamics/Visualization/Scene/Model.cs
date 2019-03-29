using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class Model : MonoBehaviour {

        public Vector3 ModelCentre { get; set; }

        private SceneManager sceneManager;
        private PrimaryStructure model;
        private BoundingBox boundingBox;

        private bool centredAtOrigin = false;

        private Quaternion saveRotation;
        private Vector3 savePosition;
        private Vector3 saveScale;

        private float rotationSpeed = 15f;
        private float scale = 1;
        private float scaleIncrementAmount = 0.1f;

        public void Start() {
            sceneManager = SceneManager.instance;
        }

        public void Initialise(PrimaryStructure model, BoundingBox box) {

            this.model = model;
            this.boundingBox = box;

            float z = boundingBox.Centre.z;
            if (Settings.FlipZCoordinates) {
                z = z * -1;
            }

            ModelCentre = new Vector3(boundingBox.Centre.x, boundingBox.Centre.y, z);
        }

        void Update() {

            if (Settings.ModelRotate) {
                transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
            }
        }

        public void Rotate(float xAngle, float yAngle, float zAngle) {

            Settings.ModelRotate = false;
            sceneManager.GUIManager.ReloadOptions();

            transform.Rotate(Vector3.up, yAngle);
        }

        public void Show(bool show) {
            gameObject.SetActive(show);
        }

        public void SaveRotation() {
            saveRotation = transform.rotation;
        }

        public void RestoreRotation() {

            transform.rotation = saveRotation;
        }

        public void ResetRotation() {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        public void SaveTransform() {

            saveRotation = transform.rotation;
            savePosition = transform.position;
            saveScale = transform.localScale;
        }

        public void RestoreTransform() {

            transform.rotation = saveRotation;
            transform.position = savePosition;
            transform.localScale = saveScale;
        }

        public void ResetTransform() {

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            transform.position = new Vector3(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }

        public void SetPosition(PrimaryStructure model, BoundingBox boundingBox) {

            // centre model gameObject on model coordinates
            Vector3 modelCentre = new Vector3(boundingBox.Centre.x, 0, boundingBox.Centre.z);
            if (Settings.FlipZCoordinates) {
                modelCentre.z = modelCentre.z * -1;
            }

            transform.position = modelCentre;
        }

        public float HoverHeight() {

            float hover;
            float edgeWidth = sceneManager.ModelBox.BoxEdgeWidth();

            if (boundingBox.Height < Settings.ModelHoverHeight) {
                hover = Settings.ModelHoverHeight - (boundingBox.Height / 2);
            }
            else {
                hover = Settings.ModelCentre.y - boundingBox.Origin.y + (edgeWidth / 2);
            }

            return hover;
        }

        public float Scale {

            get {
                return scale;
            }

            set {

                scale = value;

                if (scale > Settings.MaxModelScale) {
                    scale = Settings.MaxModelScale;
                }

                if (scale < Settings.MinModelScale) {
                    scale = Settings.MinModelScale;
                }

                transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        public void IncreaseScale() {
            Scale += scaleIncrementAmount;
        }

        public void DecreaseScale() {
            Scale -= scaleIncrementAmount;
        }

        private float boundingBoxEdgeWidth() {

            float edgeWidth = Settings.ModelBoxEdgeWidthDefault;
            edgeWidth *= boundingBox.Height / Settings.ModelHoverHeight;

            if (edgeWidth > Settings.ModelBoxEdgeWidthDefault) {
                edgeWidth = Settings.ModelBoxEdgeWidthDefault;
            }

            return edgeWidth;
        }
    }
}
