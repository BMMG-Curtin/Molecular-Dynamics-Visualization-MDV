using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MeshBuilder : MonoBehaviour {

        private struct MeshData {
            public Vector3[] Vertices;
            public int[] Triangles;
        }

        /// <summary>
        /// Generates a combined mesh from the mesh of a prefab and an array of transforms. Uses multiple meshes if max vertices exceeded in a single mesh. 
        /// Meshes are assigned to new gameobjects instantiated from the prefab. All gameobjects are childed to the parent.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="transforms"></param>
        /// <param name="colour"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerator CombinedMesh(GameObject prefab, Matrix4x4[] transforms, Color32 colour, GameObject parent) {

            Mesh prefabMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] inputMeshVertices = prefabMesh.vertices;
            int[] inputMeshTriangles = prefabMesh.triangles;

            List<MeshData> meshData = null;

            Thread thread = new Thread(() => {
                meshData = generateMesh(inputMeshVertices, inputMeshTriangles, transforms);
            });
            thread.Start();

            while (thread.IsAlive) {
                yield return null;
            }

            if (meshData != null) {

                foreach (MeshData mesh in meshData) {

                    Mesh combineMesh = new Mesh();
                    combineMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                    combineMesh.vertices = mesh.Vertices;
                    combineMesh.triangles = mesh.Triangles;
                    combineMesh.RecalculateNormals();

                    GameObject meshObject = (GameObject)Instantiate(prefab);
                    meshObject.GetComponent<MeshFilter>().sharedMesh = combineMesh;
                    meshObject.GetComponent<Renderer>().material.color = colour;
                    meshObject.transform.parent = parent.transform;
                }
            }
        }

        private List<MeshData> generateMesh(Vector3[] inputMeshVertices, int[] inputMeshTriangles, Matrix4x4[] transforms) {

            List<MeshData> meshData = new List<MeshData>();

            int maxVertexCount = 65534;
            int objectsToMerge = transforms.Length > (maxVertexCount / inputMeshVertices.Length) ? maxVertexCount / inputMeshVertices.Length : transforms.Length;

            MeshData mesh = new MeshData();
            mesh.Vertices = new Vector3[objectsToMerge * inputMeshVertices.Length];
            mesh.Triangles = new int[objectsToMerge * inputMeshTriangles.Length];

            int vertPos, triPos, index;
            vertPos = triPos = index = 0;

            for (int i = 0; i < transforms.Length; i++) {

                for (int j = 0; j < inputMeshVertices.Length; j++)
                    mesh.Vertices[vertPos + j] = transforms[i].MultiplyPoint3x4(inputMeshVertices[j]);

                for (int j = 0; j < inputMeshTriangles.Length; j++)
                    mesh.Triangles[triPos + j] = inputMeshTriangles[j] + vertPos;

                vertPos += inputMeshVertices.Length;
                triPos += inputMeshTriangles.Length;
                index++;

                if (index >= objectsToMerge) {

                    meshData.Add(mesh);
                    mesh = new MeshData();

                    // This should only be true on the last mesh merge. 
                    // Every other mesh merge has the same object count and we just overwrite the arrays.
                    if (transforms.Length - i - 1 < maxVertexCount / inputMeshVertices.Length) {

                        objectsToMerge = transforms.Length - i - 1;
                        mesh.Vertices = new Vector3[objectsToMerge * inputMeshVertices.Length];
                        mesh.Triangles = new int[objectsToMerge * inputMeshTriangles.Length];
                    }
                    else {
                        mesh.Vertices = new Vector3[objectsToMerge * inputMeshVertices.Length];
                        mesh.Triangles = new int[objectsToMerge * inputMeshTriangles.Length];
                    }

                    index = vertPos = triPos = 0;
                }
            }

            return meshData;
        }
    }
}
