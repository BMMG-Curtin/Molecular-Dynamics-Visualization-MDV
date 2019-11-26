using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public struct HighLightedAtom {
        public Atom Atom;
        public Color HighlightColor;
    }

    // Renders atom highlight discs using point mesh and PCX point cloud shader
    // Uses the render settings to determine how large the discs need to be so 
    // render settings should be updated whenever the atom render sizes change.
    public class AtomHighlightsRenderer : MonoBehaviour {

        [SerializeField]
        private GameObject atomHighlightsMeshPrefab;

        [SerializeField]
        private GameObject atomHighlightsParent;

        [SerializeField]
        private Dictionary<float, GameObject> atomHighlightMeshes;

        private MoleculeRenderSettings renderSettings;

        private void Awake() {
            atomHighlightMeshes = new Dictionary<float, GameObject>();
            renderSettings = MoleculeRenderSettings.Default();
        }

        public void SetRenderSettings(MoleculeRenderSettings settings) {
            renderSettings = settings;
        }

        public void RenderAtomHighlights(List<HighLightedAtom> atoms) {

            ClearHighlights();

            // separate List into atoms by size
            Dictionary<float, List<HighLightedAtom>> atomsBySize = new Dictionary<float, List<HighLightedAtom>>();

            foreach(HighLightedAtom highlighedAtom in atoms) {

                if ((renderSettings.EnabledResidueNames != null && !renderSettings.EnabledResidueNames.Contains(highlighedAtom.Atom.ResidueName)) ||
                    (renderSettings.EnabledResidueIDs != null && !renderSettings.EnabledResidueIDs.Contains(highlighedAtom.Atom.ResidueID))) {
                    continue;
                }

                // get atom size 
                float atomDiameter = getAtomRadius(highlighedAtom.Atom) * 2;

                if (!atomsBySize.ContainsKey(atomDiameter)) {
                    atomsBySize.Add(atomDiameter, new List<HighLightedAtom>());
                }

                atomsBySize[atomDiameter].Add(highlighedAtom);
            }

            // render separate mesh for atom size
            foreach(KeyValuePair<float, List<HighLightedAtom>> item in atomsBySize) {
                renderAtomHighlights(item.Key, item.Value);
            }
        }

        private void renderAtomHighlights(float size, List<HighLightedAtom> atoms) {

            GameObject atomHighlightsMesh;

            // we maintain a list of atom meshes so we don't need to recreate the mesh object each time we render
            if (atomHighlightMeshes.ContainsKey(size)) {
                atomHighlightsMesh = atomHighlightMeshes[size];
            }
            else {

                atomHighlightsMesh = GameObject.Instantiate(atomHighlightsMeshPrefab);
                atomHighlightMeshes.Add(size, atomHighlightsMesh);
                atomHighlightsMesh.transform.SetParent(atomHighlightsParent.transform, false);

                MeshRenderer mr = atomHighlightsMesh.GetComponent<MeshRenderer>();
                mr.material.SetFloat("_PointSize", size * 0.7f);
            }

            Vector3[] vertices = new Vector3[atoms.Count];
            int[] indices = new int[vertices.Length];
            Color[] colors = new Color[vertices.Length];

            for (int i = 0; i < atoms.Count; i++) {

                vertices[i] = atoms[i].Atom.Position;
                vertices[i].z *= -1;
                indices[i] = i;
                colors[i] = atoms[i].HighlightColor;
            }

            Mesh mesh = atomHighlightsMesh.GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Points, 0);
            mesh.colors = colors;
            mesh.bounds = new Bounds(transform.position, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

            atomHighlightsMesh.SetActive(true);
        }

        private float getAtomRadius(Atom atom) {

            MolecularRepresentation? customRepresentation = null;

            if (renderSettings.CustomResidueRenderSettings != null && renderSettings.CustomResidueRenderSettings.ContainsKey(atom.ResidueID)) {

                ResidueRenderSettings residueSettings = renderSettings.CustomResidueRenderSettings[atom.ResidueID];

                if (residueSettings != null) { 

                    // use the atom specific settings if available. 
                    if (residueSettings.AtomSettings.ContainsKey(atom.Name)) {

                        AtomRenderSettings atomSettings = residueSettings.AtomSettings[atom.Name];
                        if (atomSettings.Representation != MolecularRepresentation.None) {
                            customRepresentation = atomSettings.Representation;
                        }
                    }

                    // if we didn't get from atom specific settings then try residue settings
                    if (customRepresentation == null) {
                        if (residueSettings.AtomRepresentation != MolecularRepresentation.None) {
                            customRepresentation = residueSettings.AtomRepresentation;
                        }
                    }
                }
            }

            float atomRadius;

            if(!renderSettings.ShowAtoms) {
                atomRadius = 0.001f;
            }
            else if (customRepresentation != null && customRepresentation != MolecularRepresentation.None) {
                atomRadius = (MolecularRepresentation)customRepresentation == MolecularRepresentation.VDW ? atom.VDWRadius : atom.AtomicRadius / 2f;
                atomRadius *= renderSettings.AtomScale;
            }
            else {
                atomRadius = renderSettings.Representation == MolecularRepresentation.VDW ? atom.VDWRadius : atom.AtomicRadius / 2f;
                atomRadius *= renderSettings.AtomScale;
            }

            return atomRadius;
        }

        public void ClearHighlights() {

            foreach(GameObject mesh in atomHighlightMeshes.Values) {
                mesh.SetActive(false);
            }
        }
    }
}
