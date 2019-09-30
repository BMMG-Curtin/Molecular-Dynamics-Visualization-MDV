using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public struct HighLightedAtom {
        public Atom Atom;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public Color HighlightColor;
    }

    public class AtomHighlightsRenderer : MonoBehaviour {

        [SerializeField]
        private GameObject atomHighlightsMeshPrefab;

        [SerializeField]
        private GameObject atomHighlightsParent;

        [SerializeField]
        private Dictionary<Element, GameObject> atomHighlightMeshes;

        private MoleculeRenderSettings renderSettings;

        private void Awake() {
            atomHighlightMeshes = new Dictionary<Element, GameObject>();
            renderSettings = MoleculeRenderSettings.Default();
        }

        public void SetRenderSettings(MoleculeRenderSettings settings) {
            renderSettings = settings;
        }

        public void RenderAtomHighlights(List<HighLightedAtom> atoms) {

            ClearHighlights();

            Dictionary<Element, List<HighLightedAtom>> atomsByElement = new Dictionary<Element, List<HighLightedAtom>>();

            // separate List into atoms by element
            foreach(HighLightedAtom atom in atoms) {

                Element atomElement = atom.Atom.Element;

                if (!atomsByElement.ContainsKey(atomElement)) {
                    atomsByElement.Add(atomElement, new List<HighLightedAtom>());
                }

                atomsByElement[atomElement].Add(atom);
            }

            // render mesh for each element
            foreach(KeyValuePair<Element, List<HighLightedAtom>> item in atomsByElement) {
                renderAtomHighlights(item.Key, item.Value);
            }
        }

        private void renderAtomHighlights(Element element, List<HighLightedAtom> atoms) {

            GameObject atomHighlightsMesh;

            if (atomHighlightMeshes.ContainsKey(element)) {
                atomHighlightsMesh = atomHighlightMeshes[element];
            }
            else {

                atomHighlightsMesh = GameObject.Instantiate(atomHighlightsMeshPrefab);
                atomHighlightMeshes.Add(element, atomHighlightsMesh);
                atomHighlightsMesh.transform.SetParent(atomHighlightsParent.transform, false);
            }

            Vector3[] vertices = new Vector3[atoms.Count];
            int[] indices = new int[vertices.Length];
            Color[] colors = new Color[vertices.Length];

            for (int i = 0; i < atoms.Count; i++) {

                vertices[i] = atoms[i].Position;
                indices[i] = i;
                colors[i] = atoms[i].HighlightColor;
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Points, 0);
            mesh.colors = colors;

            MeshFilter mf = atomHighlightsMesh.GetComponent<MeshFilter>();
            mf.mesh = mesh;

            atomHighlightsMesh.SetActive(true);
        }

        private float getAtomScale(Atom atom, MoleculeRenderSettings settings, MolecularRepresentation? customRepresentation) {

            MolecularRepresentation atomRepresentation;

            if (customRepresentation != null && customRepresentation != MolecularRepresentation.None) {
                atomRepresentation = (MolecularRepresentation)customRepresentation;
            }
            else {
                atomRepresentation = settings.Representation;
            }

            float atomRadius;
            if (atomRepresentation == MolecularRepresentation.VDW) {
                atomRadius = atom.VDWRadius;
            }
            else { // default to CPK
                atomRadius = atom.AtomicRadius;
            }

            return settings.AtomScale * atomRadius;
        }

        public void ClearHighlights() {

            foreach(GameObject mesh in atomHighlightMeshes.Values) {
                mesh.SetActive(false);
            }
        }
    }
}
