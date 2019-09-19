using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeBox : MonoBehaviour {

        [SerializeField]
        private GameObject boxEdgePrefab;

        private BoundingBox box;

        public void Build(BoundingBox box) {

            this.box = box;

            // remove any existing box
            foreach (Transform child in transform) {
                GameObject.Destroy(child.gameObject);
            }

            // build new box
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

            RenderBoxEdge(vertex1, vertex2, box);
            RenderBoxEdge(vertex2, vertex3, box);
            RenderBoxEdge(vertex3, vertex4, box);
            RenderBoxEdge(vertex4, vertex1, box);
            RenderBoxEdge(vertex1, vertex5, box);
            RenderBoxEdge(vertex2, vertex6, box);
            RenderBoxEdge(vertex3, vertex7, box);
            RenderBoxEdge(vertex4, vertex8, box);
            RenderBoxEdge(vertex5, vertex6, box);
            RenderBoxEdge(vertex6, vertex7, box);
            RenderBoxEdge(vertex7, vertex8, box);
            RenderBoxEdge(vertex8, vertex5, box);
        }

        private void RenderBoxEdge(Vector3 startPoint, Vector3 endPoint, BoundingBox box) {

            float edgeWidth = this.boxEdgeWidth(box);

            Vector3 position = ((startPoint - endPoint) / 2.0f) + endPoint;

            GameObject boxEdge = (GameObject)Instantiate(boxEdgePrefab, position, Quaternion.identity);
            boxEdge.transform.localScale = new Vector3(edgeWidth, (endPoint - startPoint).magnitude + edgeWidth, edgeWidth);
            boxEdge.transform.rotation = Quaternion.FromToRotation(Vector3.up, startPoint - endPoint);

            boxEdge.transform.SetParent(transform, false);
            boxEdge.SetActive(true);
        }

        public float boxEdgeWidth(BoundingBox box) {

            float edgeWidth = Settings.ModelBoxEdgeWidthDefault;
            edgeWidth *= box.Height / Settings.ModelHoverHeight;

            if (edgeWidth > Settings.ModelBoxEdgeWidthDefault) {
                edgeWidth = Settings.ModelBoxEdgeWidthDefault;
            }

            return edgeWidth;
        }

        public Vector3 Origin() {
            return box.Origin;
        }
    }
}

