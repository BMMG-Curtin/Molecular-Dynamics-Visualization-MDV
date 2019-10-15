using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

using System.Diagnostics;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    [RequireComponent(typeof(MeshBuilder))]
    public class PrimaryStructureRenderer : MonoBehaviour {

        [SerializeField]
        private GameObject AtomParent;

        [SerializeField]
        private GameObject BondParent;

        [SerializeField]
        private GameObject ChainParent;

        [SerializeField]
        private Color32[] ChainColors;

        [SerializeField]
        private GameObject[] AtomPrefabs = new GameObject[3];

        [SerializeField]
        private GameObject[] BondPrefabs = new GameObject[3];

        [SerializeField]
        private GameObject ChainPrefab;

        private MeshBuilder meshBuilder;

        private PrimaryStructure primaryStructure;
        private PrimaryStructureTrajectory modelTrajectory;
        private Dictionary<int, Bond> bonds;

        private void Awake() {
            meshBuilder = GetComponent<MeshBuilder>();
        }

        public void Initialise(PrimaryStructure structure) {
            primaryStructure = structure;
        }

        private IEnumerator calculateBonds() {

            MoleculeEvents.RaiseShowMessage("Calculating bonds. Please wait", false);
            yield return null;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Thread thread = new Thread(() => {
                bonds = primaryStructure.GenerateBonds(Settings.NumberOfProcessorCores);
            });

            thread.Start();

            while (thread.IsAlive) {
                yield return null;
            }

            watch.Stop();
            MoleculeEvents.RaiseShowMessage("Bonds calculated [" + watch.ElapsedMilliseconds + "ms]", false);
        }

        public IEnumerator Render(MoleculeRenderSettings settings, PrimaryStructureFrame frame, int meshQuality) {

            Stopwatch watch = new Stopwatch();
            watch.Start();

            // store all the old model objects
            List<GameObject> oldObjects = new List<GameObject>();

            foreach (Transform child in AtomParent.transform) {
                oldObjects.Add(child.gameObject);
            }

            foreach (Transform child in BondParent.transform) {
                oldObjects.Add(child.gameObject);
            }

            foreach (Transform child in ChainParent.transform) {
                oldObjects.Add(child.gameObject);
            }


            // create the new model objects
            if (settings.ShowPrimaryStructure) {


                if (settings.ShowAtoms) {
                    yield return StartCoroutine(createModelAtomsByElement(settings, frame, meshQuality));
                }

                if (settings.Representation != MolecularRepresentation.VDW && settings.ShowBonds) {

                    if (bonds == null)
                        yield return StartCoroutine(calculateBonds());

                    if (bonds != null && bonds.Count > 0)
                        yield return StartCoroutine(createModelBonds(settings, frame, meshQuality));
                }

                if (settings.ShowMainChains) {
                    yield return StartCoroutine(createMainChains(frame));
                }

                // show the new model objects
                foreach (Transform child in AtomParent.transform) {
                    child.gameObject.SetActive(true);
                }

                foreach (Transform child in BondParent.transform) {
                    child.gameObject.SetActive(true);
                }

                foreach (Transform child in ChainParent.transform) {
                    child.gameObject.SetActive(true);
                }
            }

            // delete old model objects
            foreach (GameObject oldObject in oldObjects) {
                if (oldObject != null) {
                    oldObject.SetActive(false);
                }
            }

            foreach (GameObject oldObject in oldObjects) {
                if (oldObject != null) {
                    Destroy(oldObject);
                }
            }

            watch.Stop();
            //if (Settings.DebugMessages)
            //    console.BannerBuildTime = watch.ElapsedMilliseconds.ToString();

            yield break;
        }

        private IEnumerator createModelAtomsByElement(MoleculeRenderSettings renderSettings, PrimaryStructureFrame frame, int meshQuality) {

            Quaternion atomOrientation = Quaternion.Euler(45, 45, 45);

            // generate combined meshes (i.e single GameObject) for atoms with same element/colour
            Dictionary<Color, List<Matrix4x4>> mergeTransforms = new Dictionary<Color, List<Matrix4x4>>();

            Dictionary<int, Atom> atoms = primaryStructure.GetAtoms(renderSettings.ShowStandardResidues, renderSettings.ShowNonStandardResidues, renderSettings.EnabledElements, renderSettings.EnabledResidueNames, renderSettings.EnabledResidueIDs);

            foreach (KeyValuePair<int, Atom> item in atoms) {

                Atom atom = item.Value;

                Vector3 position;

                // if no frame use the base structure coordinates.
                if (frame == null) {
                    position = new Vector3(atom.Position.x, atom.Position.y, atom.Position.z);
                }
                else {
                    if (atom.Index >= frame.AtomCount) {
                        MoleculeEvents.RaiseShowMessage("Atoms not found in frame record. Aborting frame render.", true);
                        yield break;
                    }
                    position = new Vector3(frame.Coords[atom.Index * 3], frame.Coords[(atom.Index * 3) + 1], frame.Coords[(atom.Index * 3) + 2]);
                }

                // flip coord system for Unity
                position.z *= -1;

                Color32? customColour = null;
                MolecularRepresentation? customRepresentation = null;

                if (renderSettings.CustomResidueRenderSettings != null && renderSettings.CustomResidueRenderSettings.ContainsKey(atom.ResidueID)) {

                    ResidueRenderSettings residueSettings = renderSettings.CustomResidueRenderSettings[atom.ResidueID];

                    if (residueSettings != null) {

                        // use the atom specific settings if available. 
                        if (residueSettings.AtomSettings.ContainsKey(atom.Name)) {

                            AtomRenderSettings atomSettings = residueSettings.AtomSettings[atom.Name];

                            if (atomSettings.CustomColour) {
                                customColour = atomSettings.AtomColour;
                            }

                            if (atomSettings.Representation != MolecularRepresentation.None) {
                                customRepresentation = atomSettings.Representation;
                            }
                        }

                        // if we didn't get from atom specific settings then get from residue settings
                        if (customColour == null && residueSettings.ColourAtoms) {
                            customColour = residueSettings.ResidueColour;
                        }

                        if (customRepresentation == null) {
                            if (residueSettings.AtomRepresentation != MolecularRepresentation.None) {
                                customRepresentation = residueSettings.AtomRepresentation;
                            }
                        }
                    }
                }

                Color32 atomColour = Color.white;

                if (customColour != null) {
                    atomColour = (Color)customColour;
                }
                else {
                    if (!MolecularConstants.CPKColors.TryGetValue(atom.Element.ToString(), out atomColour)) {
                        MolecularConstants.CPKColors.TryGetValue("Other", out atomColour);
                    }
                }

                float atomDiameter = getAtomRadius(atom, renderSettings, customRepresentation) * 2;
                Vector3 scale = new Vector3(atomDiameter, atomDiameter, atomDiameter);

                Matrix4x4 atomTransform = Matrix4x4.TRS(position, atomOrientation, scale);

                if (!mergeTransforms.ContainsKey(atomColour)) {
                    mergeTransforms.Add(atomColour, new List<Matrix4x4>());
                }

                mergeTransforms[atomColour].Add(atomTransform);
            }


            // create the meshes by colour
            GameObject prefab = AtomPrefabs[meshQuality];
            GameObject parent = new GameObject("CombinedMeshParent");
            parent.SetActive(false);

            foreach (KeyValuePair<Color, List<Matrix4x4>> item in mergeTransforms) {
                yield return StartCoroutine(meshBuilder.CombinedMesh(prefab, item.Value.ToArray(), item.Key, parent));
            }

            parent.transform.SetParent(AtomParent.transform, false);

            yield break;
        }

        private IEnumerator createModelBonds(MoleculeRenderSettings renderSettings, PrimaryStructureFrame frame, int meshQuality) {

            // set colour for bonds
            Color32 bondColour;
            if (!MolecularConstants.CPKColors.TryGetValue("Bond", out bondColour)) {
                MolecularConstants.CPKColors.TryGetValue("Other", out bondColour);
            }

            List<Matrix4x4> standardTransforms = new List<Matrix4x4>();
            Dictionary<Color, List<Matrix4x4>> highlightedTransforms = new Dictionary<Color, List<Matrix4x4>>();

            float standardCylinderWidth = 0.015f * renderSettings.BondScale;
            float enlargedCylinderWidth = 0.040f * renderSettings.BondScale;

            // get atoms for bonds
            Dictionary<int, Atom> atoms;
            Dictionary<int, Atom> highLightedAtoms = new Dictionary<int, Atom>();

            atoms = primaryStructure.GetAtoms(renderSettings.ShowStandardResidues, renderSettings.ShowNonStandardResidues, renderSettings.EnabledElements, renderSettings.EnabledResidueNames, renderSettings.EnabledResidueIDs);

            if (renderSettings.CustomResidueRenderSettings != null) {

                HashSet<int> customResidueIDs = new HashSet<int>(renderSettings.CustomResidueRenderSettings.Keys.ToList());
                highLightedAtoms = primaryStructure.GetAtoms(renderSettings.ShowStandardResidues, renderSettings.ShowNonStandardResidues, renderSettings.EnabledElements, renderSettings.EnabledResidueNames, customResidueIDs);
            }

            foreach (KeyValuePair<int, Bond> bond in bonds) {

                Vector3 atom1pos, atom2pos;
                Atom atom1 = null;
                Atom atom2 = null;

                if (!atoms.TryGetValue(bond.Value.Atom1Index, out atom1) || !atoms.TryGetValue(bond.Value.Atom2Index, out atom2)) {
                    continue;
                }

                if (frame == null) {

                    atom1pos = new Vector3(atom1.Position.x, atom1.Position.y, atom1.Position.z);
                    atom2pos = new Vector3(atom2.Position.x, atom2.Position.y, atom2.Position.z);
                }
                else {

                    if (bond.Value.Atom1Index >= frame.AtomCount || bond.Value.Atom2Index >= frame.AtomCount) {
                        // no need to send error message as this will have already been done in the atom render
                        yield break;
                    }

                    atom1pos = new Vector3(frame.Coords[bond.Value.Atom1Index * 3], frame.Coords[(bond.Value.Atom1Index * 3) + 1], frame.Coords[(bond.Value.Atom1Index * 3) + 2]);
                    atom2pos = new Vector3(frame.Coords[bond.Value.Atom2Index * 3], frame.Coords[(bond.Value.Atom2Index * 3) + 1], frame.Coords[(bond.Value.Atom2Index * 3) + 2]);
                }

                // flip coord system for Unity
                atom1pos.z *= -1;
                atom2pos.z *= -1;

                // bonds aren't recalculated on each frame. In some frames atoms jump from one side of the simulation box to another. When this happens need to disable bond view
                float bondLength = (atom2pos - atom1pos).magnitude / 2;
                if (bondLength > BondLengths.MaximumLengthAllElements) {
                    continue;
                }

                Vector3 position = ((atom1pos - atom2pos) / 2.0f) + atom2pos;
                float length = (atom2pos - atom1pos).magnitude;

                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, atom1pos - atom2pos);

                if (highLightedAtoms != null && highLightedAtoms.Count > 0 && highLightedAtoms.ContainsKey(atom1.Index) && highLightedAtoms.ContainsKey(atom2.Index)) {

                    int atom1residue = atom1.ResidueID;
                    int atom2residue = atom2.ResidueID;

                    // only colour or highlight bonds between atoms of the same residue
                    if (atom1residue == atom2residue && renderSettings.CustomResidueRenderSettings.ContainsKey(atom1residue) && renderSettings.CustomResidueRenderSettings[atom1residue].ColourBonds) {

                        ResidueRenderSettings options = renderSettings.CustomResidueRenderSettings[atom1residue];

                        float cylinderWidth = standardCylinderWidth;
                        if (options.LargeBonds) {
                            cylinderWidth = enlargedCylinderWidth;
                        }

                        Vector3 localScale = new Vector3(cylinderWidth, length, cylinderWidth);

                        Matrix4x4 bondTransform = Matrix4x4.TRS(position, rotation, localScale);

                        if (!highlightedTransforms.ContainsKey(options.ResidueColour)) {
                            highlightedTransforms.Add(options.ResidueColour, new List<Matrix4x4>());
                        }

                        highlightedTransforms[options.ResidueColour].Add(bondTransform);
                    }
                    else {

                        float cylinderWidth = standardCylinderWidth;
                        if (atom1residue == atom2residue && renderSettings.CustomResidueRenderSettings.ContainsKey(atom1residue) && renderSettings.CustomResidueRenderSettings[atom1residue].LargeBonds) {
                            cylinderWidth = enlargedCylinderWidth;
                        }

                        Vector3 localScale = new Vector3(cylinderWidth, length, cylinderWidth);
                        standardTransforms.Add(Matrix4x4.TRS(position, rotation, localScale));
                    }
                }
                else {

                    Vector3 localScale = new Vector3(standardCylinderWidth, length, standardCylinderWidth);
                    standardTransforms.Add(Matrix4x4.TRS(position, rotation, localScale));
                }
            }

            GameObject prefab = BondPrefabs[meshQuality];

            GameObject parent = new GameObject("StandardCombinedMeshParent");
            parent.SetActive(false);
            yield return StartCoroutine(meshBuilder.CombinedMesh(prefab, standardTransforms.ToArray(), bondColour, parent));
            parent.transform.SetParent(BondParent.transform, false);

            parent = new GameObject("HighligtedCombinedMeshParent");
            parent.SetActive(false);

            foreach (KeyValuePair<Color, List<Matrix4x4>> item in highlightedTransforms) {
                yield return StartCoroutine(meshBuilder.CombinedMesh(prefab, item.Value.ToArray(), item.Key, parent));
            }

            parent.transform.SetParent(BondParent.transform, false);
        }

        private IEnumerator createMainChains(PrimaryStructureFrame frame) {

            int interpolation = 5;
            int resolution = 5; // should be in config
            float radius = 0.015f;
            int currentIndex = 0;

            foreach (Chain chain in primaryStructure.Chains()) {

                if (chain.ResidueType != StandardResidue.AminoAcid) {
                    // UnityEngine.Debug.Log("Skipping main chain build. Non protein main chain not currently supported.");
                    continue;
                }

                List<Vector3> nodePositions = new List<Vector3>();

                foreach (Atom atom in chain.MainChainAtoms) {

                    // if no frame number use the base structure coordinates.
                    Vector3 position;
                    if (frame == null) {
                        if (atom == null) {
                            UnityEngine.Debug.Log("Main chain atom is null");
                        }

                        position = new Vector3(atom.Position.x, atom.Position.y, atom.Position.z);
                    }
                    else {
                        position = new Vector3(frame.Coords[atom.Index * 3], frame.Coords[(atom.Index * 3) + 1], frame.Coords[(atom.Index * 3) + 2]);
                    }

                    // flip coord system for Unity
                    position.z *= -1;

                    nodePositions.Add(position);
                }

                List<DynamicMeshNode> nodes = new List<DynamicMeshNode>();
                IEnumerable splinePoints = Interpolate.NewCatmullRom(nodePositions.ToArray(), interpolation, false);
                Color32 chainColour = ChainColors[currentIndex % ChainColors.Length];

                foreach (Vector3 position in splinePoints) {

                    DynamicMeshNode node = new DynamicMeshNode();
                    node.Position = position;
                    node.VertexColor = chainColour;
                    nodes.Add(node);
                }

                DynamicMesh mesh = new DynamicMesh(nodes, radius, resolution, interpolation + 1);
                Mesh chainMesh = mesh.Build(Settings.DebugFlag);

                GameObject chainStructure = (GameObject)Instantiate(ChainPrefab);
                chainStructure.GetComponent<MeshFilter>().sharedMesh = chainMesh;

                chainStructure.SetActive(false);
                chainStructure.transform.SetParent(ChainParent.transform, false);

                currentIndex++;
                yield return null;
            }
        }

        private float getAtomRadius(Atom atom, MoleculeRenderSettings settings, MolecularRepresentation? customRepresentation) {

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
                atomRadius = atom.AtomicRadius / 2f;
            }

            atomRadius *= settings.AtomScale;

            return atomRadius; // * settings.AtomScale; 
        }
    }
}
