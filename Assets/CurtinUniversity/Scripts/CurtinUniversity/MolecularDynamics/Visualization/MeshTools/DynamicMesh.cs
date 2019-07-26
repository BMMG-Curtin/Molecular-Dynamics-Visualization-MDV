using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class DynamicMesh {

        private List<DynamicMeshNode> nodes;
        private float radius;
        private int resolution;
        private int meshTypeGroupCount;

        private static int maxVertexCount = 65534;

        public DynamicMesh(List<DynamicMeshNode> nodes, float radius, int resolution, int meshTypeGroupCount) {

            this.nodes = nodes;
            this.radius = radius;
            this.resolution = resolution;
            this.meshTypeGroupCount = meshTypeGroupCount;

            int expectedVertices = nodes.Count * (resolution + 1);
            if (expectedVertices > maxVertexCount) {
                this.resolution = (maxVertexCount / nodes.Count) - 1;
            }
        }

        public Mesh Build(bool debug) {

            if (nodes.Count < 2) {
                Debug.Log("Warning: trying to create a Dyanamic Mesh with less than two nodes");
            }

            Quaternion[] rotations = PointRotations(nodes, debug);
            Vector3[] vertices = Vertices(nodes, resolution, radius, rotations, debug);
            int[] triangles = Triangles(nodes, resolution);
            Color32[] vertexColors = VertexColors(nodes, resolution);

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.colors32 = vertexColors;

            return mesh;
        }

        private Quaternion[] PointRotations(List<DynamicMeshNode> nodes, bool debug) {

            // calculate point directions - direction of each point is vector from existing point to the next point
            Vector3[] directions = new Vector3[nodes.Count];

            for (int p = 0; p < nodes.Count - 1; p++) {
                directions[p].Set(
                    nodes[p + 1].Position.x - nodes[p].Position.x,
                    nodes[p + 1].Position.y - nodes[p].Position.y,
                    nodes[p + 1].Position.z - nodes[p].Position.z
                );
            }

            directions[nodes.Count - 1] = directions[nodes.Count - 2]; // set last point equal to the second last 

            // calculate point Rotations
            Quaternion[] rotations = new Quaternion[nodes.Count];
            Vector3 forward = Vector3.zero;
            Vector3 up = Vector3.up;

            for (int p = 0; p < nodes.Count; p++) {

                forward.Set(directions[p].x, directions[p].y, directions[p].z);
                forward.Normalize();

                Vector3 right;

                if (nodes[p].Rotation != null && 
                    (nodes[p].Type == DynamicMeshNodeType.Ribbon ||
                    nodes[p].Type == DynamicMeshNodeType.RibbonHead ||
                    nodes[p].Type == DynamicMeshNodeType.SpiralRibbon)) {

                    right = (Vector3)nodes[p].Rotation;
                }
                else {
                     right = Vector3.Cross(up, forward);
                }

                up = Vector3.Cross(forward, right);
                rotations[p].SetLookRotation(forward, up);
            }

            return rotations;
        }

        private Vector3[] Vertices(List<DynamicMeshNode> nodes, int resolution, float radius, Quaternion[] Rotations, bool debug) {

            Vector3[] vertices = new Vector3[nodes.Count * (resolution + 1)];
            Matrix4x4 matrix = new Matrix4x4();
            int v = 0;

            int ribbonHeadNodeCount = 0;
            bool inRibbonHead = false;

            for (int p = 0; p < nodes.Count; p++) {

                matrix.SetTRS(nodes[p].Position, Rotations[p], Vector3.one * radius);

                if (p != 0 && nodes[p - 1].Type != DynamicMeshNodeType.RibbonHead && nodes[p].Type == DynamicMeshNodeType.RibbonHead) {
                    inRibbonHead = true;
                    ribbonHeadNodeCount = -1;
                }

                if (inRibbonHead && nodes[p].Type != DynamicMeshNodeType.RibbonHead) {
                    inRibbonHead = false;
                }

                if (inRibbonHead) {
                    ribbonHeadNodeCount++;
                }

                for (int e = 0; e < resolution; e++) {

                    if (nodes[p].Type == DynamicMeshNodeType.Tube) {
                        vertices[v++] = matrix.MultiplyPoint3x4(pointInTube(e, resolution, 1));
                    }
                    if (nodes[p].Type == DynamicMeshNodeType.LargeTube) {
                        vertices[v++] = matrix.MultiplyPoint3x4(pointInTube(e, resolution, 2));
                    }
                    else if (nodes[p].Type == DynamicMeshNodeType.Ribbon) {
                        vertices[v++] = matrix.MultiplyPoint3x4(pointInRibbon(e, resolution));
                    }
                    else if (nodes[p].Type == DynamicMeshNodeType.SpiralRibbon) {
                        vertices[v++] = matrix.MultiplyPoint3x4(pointInRibbon(e, resolution));
                    }
                    else if (nodes[p].Type == DynamicMeshNodeType.RibbonHead) {
                        float size = 1f - ((float)ribbonHeadNodeCount / (float)meshTypeGroupCount);
                        vertices[v++] = matrix.MultiplyPoint3x4(pointInRibbonHead(e, resolution, size));
                    }
                }
                vertices[v] = vertices[v - resolution];
                v++;
            }

            return vertices;
        }

        private Vector3 pointInTube(int edge, int resolution, int widthModifier) {

            float pointAngle = edge * (1 / (float)resolution) * Mathf.PI * 2;
            return new Vector3(
                widthModifier * Mathf.Cos(pointAngle),
                widthModifier * Mathf.Sin(pointAngle),
                0);
        }

        private Vector3 pointInRibbon(int edge, int resolution) {

            float ellipseWidth = 1f;
            float ellipseHeight = 6f;

            float pointAngle = edge * (1 / (float)resolution) * Mathf.PI * 2;
            return new Vector3(
                ellipseWidth * Mathf.Cos(pointAngle),
                ellipseHeight * Mathf.Sin(pointAngle),
                0
            );
        }

        private Vector3 pointInRibbonHead(int edge, int resolution, float size) {

            float ellipseWidth = 1f;
            float ellipseHeight = 12f;

            ellipseHeight *= size;

            float pointAngle = edge * (1 / (float)resolution) * Mathf.PI * 2;
            return new Vector3(
                ellipseWidth * Mathf.Cos(pointAngle),
                ellipseHeight * Mathf.Sin(pointAngle),
                0
            );
        }

        private int[] Triangles(List<DynamicMeshNode> nodes, int resolution) {

            int[] triangles = new int[(nodes.Count - 1) * resolution * 3 * 2];

            int v = 0;
            int t = 0;

            int[] quad = new int[] { 0, 1, resolution + 2, 0, resolution + 2, resolution + 1 };

            for (int p = 0; p < nodes.Count - 1; p++) {

                for (int e = 0; e < resolution; e++) {

                    for (int q = 0; q < quad.Length; q++) {

                        triangles[t++] = v + quad[q];
                    }

                    v++;
                }

                v++;
            }

            return triangles;
        }

        private Color32[] VertexColors(List<DynamicMeshNode> nodes, int resolution) {

            Color32[] vertexColors = new Color32[nodes.Count * (resolution + 1)];
            int vertex = 0;
            for (int i = 0; i < nodes.Count; i++) {
                for (int j = 0; j < resolution + 1; j++) {
                    vertexColors[vertex] = nodes[i].VertexColor;
                    vertex++;
                }
            }

            return vertexColors;
        }
    }
}
