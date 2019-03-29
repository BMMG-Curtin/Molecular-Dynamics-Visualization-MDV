using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class PrefabMerger : MonoBehaviour {

        private int maxObjectVertexCount = 65534;

        /// <summary>
        /// Note. This class is slower than the MeshBuilder class. Uses Unity inbuilt mesh merge routines.
        /// Will return a GameObject with a merged mesh of 'transforms.count' meshes with transforms taken from the list of transforms. Material will be taken from the prefab.
        /// If total vertex count is more than maxObjectVertexCount vertices the resulting gameobject will be split into child GameObjects with sub meshes below the maxObjectVertexCount
        /// </summary>
        public IEnumerator Merge(GameObject prefab, Color32 colour, List<Matrix4x4> transforms, Vector3 modelCentre, GameObject parent) {

            GameObject output;
            GameObject prefabGO = Instantiate(prefab);
            prefabGO.SetActive(false);
            prefabGO.name = "temp prefab";
            prefabGO.GetComponent<Renderer>().material.color = colour;

            // calculate how many objects can be merged for each call to Unity based on vertex counts
            int prefabVertexCount = prefab.GetComponent<MeshFilter>().sharedMesh.vertexCount;
            int objectsPerMerge = maxObjectVertexCount / prefabVertexCount;

            // check if can do in a single merge call
            if (transforms.Count <= objectsPerMerge) {
                output = MergeSingle(prefabGO, transforms);
                output.name = "Merged Mesh Single";
                output.SetActive(false);
            }
            else {

                output = new GameObject("Merged Mesh Combined");
                output.SetActive(false);

                output.transform.localPosition = Vector3.zero;
                output.transform.localRotation = Quaternion.identity;
                output.transform.localScale = Vector3.one;

                // split the transforms into batches. Unity merge will only take a maximum amount of vertices
                List<List<Matrix4x4>> batches = new List<List<Matrix4x4>>();

                for (int i = 0; i < transforms.Count; i += objectsPerMerge) {
                    batches.Add(transforms.GetRange(i, Math.Min(objectsPerMerge, transforms.Count - i)));
                }

                // merge batch and attach each resulting gameobject to a single parent gameObject
                int childID = 1;
                foreach (List<Matrix4x4> batch in batches) {
                    GameObject subMerge = MergeSingle(prefabGO, batch);
                    subMerge.name = "Merged Mesh Child " + childID;
                    subMerge.transform.parent = output.transform;
                    childID++;
                    yield return null;
                }
            }

            Destroy(prefabGO);

            SceneManager sceneManager = SceneManager.instance;

            sceneManager.Model.SaveTransform();
            sceneManager.Model.ResetTransform();

            float modelHover = sceneManager.Model.HoverHeight();

            output.transform.position = new Vector3(-1 * modelCentre.x, modelHover, -1 * modelCentre.z);
            output.transform.parent = parent.transform;

            sceneManager.Model.RestoreTransform();

            yield break;
        }

        /// <summary>
        /// Will return a GameObject with a merged mesh of 'transforms.count' meshes with transforms taken from the list of transforms. Material will be taken from the prefab.
        /// Unity will not merge more than maxObjectVertexCount vertices in total into a single Mesh.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="transforms"></param>
        /// <returns></returns>
        private GameObject MergeSingle(GameObject prefab, List<Matrix4x4> transforms) {

            List<CombineInstance> combines = new List<CombineInstance>();

            Mesh mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            Material material = prefab.GetComponent<MeshRenderer>().sharedMaterial;

            foreach (Matrix4x4 matrix in transforms) {

                CombineInstance instance = new CombineInstance();
                instance.mesh = mesh;
                instance.transform = matrix;
                combines.Add(instance);
            }

            var prefabMerge = new GameObject("Prefab Merge");

            var filter = prefabMerge.AddComponent<MeshFilter>();
            filter.mesh.CombineMeshes(combines.ToArray()); //, true, true);

            var arenderer = prefabMerge.AddComponent<MeshRenderer>();
            arenderer.material = material;

            return prefabMerge;
        }
    }
}