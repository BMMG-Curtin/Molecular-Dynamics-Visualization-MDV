using UnityEngine;
using System.Collections;

using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ModelBox : MonoBehaviour {

        public GameObject BoxEdgePrefab;
        public GameObject MolecularModel;

        public float Width { get { return boundingBox.Width; } }
        public float Height { get { return boundingBox.Height; } }
        public float Depth { get { return boundingBox.Depth; } }

        private SceneManager sceneManager;
        private BoundingBox boundingBox = null;

        public void Start() {
            sceneManager = SceneManager.instance;
        }

        public void Initialise(BoundingBox box) {

            this.boundingBox = box;
            Utility.Cleanup.DestroyGameObjects(sceneManager.MolecularModelBox);
            BuildBox(box);
        }

        public void Show(bool show) {
            this.gameObject.SetActive(show);
        }

        public float BoxEdgeWidth() {

            float edgeWidth = Settings.ModelBoxEdgeWidthDefault;
            edgeWidth *= boundingBox.Height / Settings.ModelHoverHeight;

            if (edgeWidth > Settings.ModelBoxEdgeWidthDefault) {
                edgeWidth = Settings.ModelBoxEdgeWidthDefault;
            }

            return edgeWidth;
        }

        /// <summary>
        /// Only rectangular boxes are currently supported.
        /// </summary>
        public void BuildBox(BoundingBox box) {

            float originz = box.Origin.z;
            float vectorz = box.Vector3.z;

            if (Settings.FlipZCoordinates) {
                originz = originz * -1;
                vectorz = vectorz * -1;
            }

            Vector3 vertex1 = new Vector3(box.Origin.x, box.Origin.y, originz);
            Vector3 vertex2 = new Vector3(box.Origin.x, box.Vector2.y, originz);
            Vector3 vertex3 = new Vector3(box.Vector1.x, box.Vector2.y, originz);
            Vector3 vertex4 = new Vector3(box.Vector1.x, box.Origin.y, originz);
            Vector3 vertex5 = new Vector3(box.Origin.x, box.Origin.y, vectorz);
            Vector3 vertex6 = new Vector3(box.Origin.x, box.Vector2.y, vectorz);
            Vector3 vertex7 = new Vector3(box.Vector1.x, box.Vector2.y, vectorz);
            Vector3 vertex8 = new Vector3(box.Vector1.x, box.Origin.y, vectorz);

            RenderBoxEdge(vertex1, vertex2);
            RenderBoxEdge(vertex2, vertex3);
            RenderBoxEdge(vertex3, vertex4);
            RenderBoxEdge(vertex4, vertex1);
            RenderBoxEdge(vertex1, vertex5);
            RenderBoxEdge(vertex2, vertex6);
            RenderBoxEdge(vertex3, vertex7);
            RenderBoxEdge(vertex4, vertex8);
            RenderBoxEdge(vertex5, vertex6);
            RenderBoxEdge(vertex6, vertex7);
            RenderBoxEdge(vertex7, vertex8);
            RenderBoxEdge(vertex8, vertex5);
        }

        private void RenderBoxEdge(Vector3 startPoint, Vector3 endPoint) {

            float edgeWidth = sceneManager.ModelBox.BoxEdgeWidth();

            Vector3 position = ((startPoint - endPoint) / 2.0f) + endPoint;
            position.x = position.x - sceneManager.Model.ModelCentre.x;
            position.y = position.y + sceneManager.Model.HoverHeight();
            position.z = position.z - sceneManager.Model.ModelCentre.z;

            GameObject boxEdge = (GameObject)Instantiate(BoxEdgePrefab, position, Quaternion.identity);
            boxEdge.transform.localScale = new Vector3(edgeWidth, (endPoint - startPoint).magnitude + edgeWidth, edgeWidth);
            boxEdge.transform.rotation = Quaternion.FromToRotation(Vector3.up, startPoint - endPoint);

            boxEdge.transform.SetParent(sceneManager.MolecularModelBox.transform);
            boxEdge.SetActive(true);
        }
    }
}
