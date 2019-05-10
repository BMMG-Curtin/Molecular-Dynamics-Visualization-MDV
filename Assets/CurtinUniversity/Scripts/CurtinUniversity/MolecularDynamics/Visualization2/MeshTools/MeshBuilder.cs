using UnityEngine;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class MeshBuilder : MonoBehaviour {

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

            // sharedMesh accesses are slow so assign locally to improve performance
            Vector3[] meshVertices = prefabMesh.vertices;
            int[] meshTriangles = prefabMesh.triangles;

            int maxVertexCount = 65534;
            int objectsToMerge = transforms.Length > (maxVertexCount / meshVertices.Length) ? maxVertexCount / meshVertices.Length : transforms.Length;
            Vector3[] vertices = new Vector3[objectsToMerge * meshVertices.Length];
            int[] triangles = new int[objectsToMerge * meshTriangles.Length];

            int vertPos, triPos, index;
            vertPos = triPos = index = 0;

            for (int i = 0; i < transforms.Length; i++) {

                for (int j = 0; j < meshVertices.Length; j++)
                    vertices[vertPos + j] = transforms[i].MultiplyPoint3x4(meshVertices[j]);

                for (int j = 0; j < meshTriangles.Length; j++)
                    triangles[triPos + j] = meshTriangles[j] + vertPos;

                vertPos += meshVertices.Length;
                triPos += meshTriangles.Length;
                index++;

                if (index >= objectsToMerge) {

                    Mesh mesh = new Mesh();
                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.RecalculateNormals();

                    GameObject meshObject = (GameObject)Instantiate(prefab);
                    meshObject.GetComponent<MeshFilter>().sharedMesh = mesh;
                    meshObject.GetComponent<Renderer>().material.color = colour;
                    meshObject.transform.parent = parent.transform;

                    // This should only be true on the last mesh merge. 
                    // Every other mesh merge has the same object count and we just overwrite the arrays.
                    if (transforms.Length - i - 1 < maxVertexCount / meshVertices.Length) {
                        objectsToMerge = transforms.Length - i - 1;
                        vertices = new Vector3[objectsToMerge * meshVertices.Length];
                        triangles = new int[objectsToMerge * meshTriangles.Length];
                    }

                    index = vertPos = triPos = 0;

                    yield return null;
                }
            }

            yield break;
        }
    }
}
