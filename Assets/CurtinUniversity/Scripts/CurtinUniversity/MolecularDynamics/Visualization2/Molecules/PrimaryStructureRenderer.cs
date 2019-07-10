using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using CurtinUniversity.MolecularDynamics.Model.Definitions;
using CurtinUniversity.MolecularDynamics.Model.Model;
using CurtinUniversity.MolecularDynamics.Visualization.Utility;

using System.Diagnostics;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    [RequireComponent(typeof(MeshRenderer))]
    public class PrimaryStructureRenderer : MonoBehaviour {

        public GameObject AtomParent;
        public GameObject BondParent;
        public GameObject ChainParent;

        public Color32[] ChainColors;

        // unity prefabs
        public GameObject[] AtomPrefabs = new GameObject[3];
        public GameObject[] BondPrefabs = new GameObject[3];
        public GameObject ChainPrefab;

        private MeshBuilder meshBuilder;

        private GameObject modelAtomsObject;
        private GameObject modelBondsObject;

        public Dictionary<int, Bond> Bonds { get; private set; }

        [NonSerialized]
        public bool Initialised = false;

        public bool Initialising { get { return initialising; } }
        public bool BuildingModel { get { return buildingModel; } }

        // model data store
        private PrimaryStructure primaryStructure;
        private PrimaryStructureTrajectory modelTrajectory;

        private bool initialising = false;
        private bool buildingModel = false;

        private void Awake() {
            meshBuilder = GetComponent<MeshBuilder>();
        }

        public IEnumerator Initialise(PrimaryStructure structure) {

            initialising = true;

            primaryStructure = structure;

            if (Bonds != null) {
                Bonds.Clear();
                Bonds = null;
            }

            if (Settings.GenerateBonds && Settings.GenerateBondsOnModelLoad)
                yield return StartCoroutine(calculateBonds());

            Initialised = true;
            initialising = false;
        }

        public IEnumerator Render(MoleculeRenderSettings settings, PrimaryStructureFrame frame) {

            Stopwatch watch = new Stopwatch();
            watch.Start();

            buildingModel = true;

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
                    yield return StartCoroutine(createModelAtomsByElement(settings, frame));
                }

                if (settings.Representation != MolecularRepresentation.VDW && settings.ShowBonds) {

                    if (Settings.GenerateBonds && Bonds == null)
                        yield return StartCoroutine(calculateBonds());

                    if (Bonds != null && Bonds.Count > 0)
                        yield return StartCoroutine(createModelBonds(settings, frame));
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

            buildingModel = false;
            watch.Stop();
            //if (Settings.DebugMessages)
            //    console.BannerBuildTime = watch.ElapsedMilliseconds.ToString();

            yield break;
        }

        private IEnumerator calculateBonds() {

            MoleculeEvents.RaiseRenderMessage("Calculating Bonds", false);
            yield return null;
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (primaryStructure == null) {
                UnityEngine.Debug.Log("Model is null");
            }

            Bonds = primaryStructure.GenerateBonds();
            watch.Stop();
            MoleculeEvents.RaiseRenderMessage("Bonds Calculated [" + watch.ElapsedMilliseconds + "ms]", false);
        }

        private IEnumerator createModelAtomsByElement(MoleculeRenderSettings renderSettings, PrimaryStructureFrame frame) {

            Quaternion atomOrientation = Quaternion.Euler(45, 45, 45);

            // generate combined meshes (i.e single GameObject) for atoms with same element/colour

            Dictionary<Color, List<Matrix4x4>> mergeTransforms = new Dictionary<Color, List<Matrix4x4>>();

            HashSet<string> enabledElementNames = null;
            HashSet<string> enabledResiduesNames = null;
            HashSet<string> customDisplayResidueNames = null;

            if (renderSettings.EnabledElements != null && primaryStructure.ElementNames.Count > renderSettings.EnabledElements.Count) {
                enabledElementNames = renderSettings.EnabledElements;
            }

            if(renderSettings.EnabledResidueNames != null && primaryStructure.ResidueNames.Count > renderSettings.EnabledResidueNames.Count) {
                enabledResiduesNames = renderSettings.EnabledResidueNames;
            }

            if (renderSettings.CustomDisplayResidues != null && renderSettings.CustomDisplayResidues.Count > 0) {
                customDisplayResidueNames = renderSettings.CustomDisplayResidues;
            }

            bool filterByNumber = renderSettings.FilterResiduesByNumber;

            HashSet<int> enabledResidueNumbers = renderSettings.EnabledResidueNumbers;

            Dictionary<int, Atom> atoms = primaryStructure.GetAtoms(renderSettings.ShowStandardResidues, renderSettings.ShowNonStandardResidues, enabledElementNames, enabledResiduesNames);

            foreach (KeyValuePair<int, Atom> item in atoms) {

                Atom atom = item.Value;

                if (filterByNumber && enabledResidueNumbers != null && !enabledResidueNumbers.Contains(atom.ResidueID)) {
                    continue;
                }

                float atomSize = getAtomScale(atom.Name, renderSettings);
                Vector3 scale = new Vector3(atomSize, atomSize, atomSize);

                Vector3 position;

                // if no frame use the base structure coordinates.
                if (frame == null) {
                    position = new Vector3(atom.Position.x, atom.Position.y, atom.Position.z);
                }
                else {
                    if (atom.Index >= frame.AtomCount) {
                        MoleculeEvents.RaiseRenderMessage("Atoms not found in frame record. Aborting frame render.", true);
                        yield break;
                    }
                    position = new Vector3(frame.Coords[atom.Index * 3], frame.Coords[(atom.Index * 3) + 1], frame.Coords[(atom.Index * 3) + 2]);
                }

                if (Settings.FlipZCoordinates) {
                    position.z = position.z * -1;
                }

                //// set colour for atoms 
                Color32 atomColour;

                if (customDisplayResidueNames != null && customDisplayResidueNames.Contains(atom.ResidueName)) {

                    Visualization.ResidueDisplayOptions displayOptions = renderSettings.ResidueOptions[atom.ResidueName];

                    if (displayOptions != null && displayOptions.ColourAtoms) {
                        atomColour = displayOptions.CustomColour;
                    }
                    else {
                        if (!MolecularConstants.CPKColors.TryGetValue(atom.Element.ToString(), out atomColour)) {
                            MolecularConstants.CPKColors.TryGetValue("Other", out atomColour);
                        }
                    }
                }
                else {
                    if (!MolecularConstants.CPKColors.TryGetValue(atom.Element.ToString(), out atomColour)) {
                        MolecularConstants.CPKColors.TryGetValue("Other", out atomColour);
                    }
                }

                Matrix4x4 atomTransform = Matrix4x4.TRS(position, atomOrientation, scale);

                if (!mergeTransforms.ContainsKey(atomColour)) {
                    mergeTransforms.Add(atomColour, new List<Matrix4x4>());
                }

                mergeTransforms[atomColour].Add(atomTransform);
            }


            // create the meshes by colour
            GameObject prefab = AtomPrefabs[Settings.AtomMeshQuality];
            GameObject parent = new GameObject("CombinedMeshParent");
            parent.SetActive(false);

            foreach (KeyValuePair<Color, List<Matrix4x4>> item in mergeTransforms) {
                yield return StartCoroutine(meshBuilder.CombinedMesh(prefab, item.Value.ToArray(), item.Key, parent));
            }

            parent.transform.SetParent(AtomParent.transform, false);

            yield break;
        }

        private IEnumerator createModelBonds(MoleculeRenderSettings renderSettings, PrimaryStructureFrame frame) {

            // set colour for bonds
            Color32 bondColour;
            if (!MolecularConstants.CPKColors.TryGetValue("Bond", out bondColour)) {
                MolecularConstants.CPKColors.TryGetValue("Other", out bondColour);
            }

            //Color32 highlightedBondColour = Colour.HSVToRGB(Settings.HighlightedBondColourHue, 1f, 1f);

            List<Matrix4x4> standardTransforms = new List<Matrix4x4>();
            Dictionary<Color, List<Matrix4x4>> highlightedTransforms = new Dictionary<Color, List<Matrix4x4>>();

            float standardCylinderWidth = 0.015f * renderSettings.BondScale;
            float enlargedCylinderWidth = 0.040f * renderSettings.BondScale;

            // get atoms for bonds
            Dictionary<int, Atom> atoms;
            Dictionary<int, Atom> highLightedAtoms = new Dictionary<int, Atom>();
            HashSet<string> enabledElementNames = null;
            HashSet<string> enabledResidues = null;
            HashSet<string> customDisplayResidues = null;


            if (renderSettings.EnabledElements != null && primaryStructure.ElementNames.Count > renderSettings.EnabledElements.Count) {
                enabledElementNames = renderSettings.EnabledElements;
            }

            if (renderSettings.EnabledResidueNames != null && primaryStructure.ResidueNames.Count > renderSettings.EnabledResidueNames.Count) {
                enabledResidues = renderSettings.EnabledResidueNames;
            }

            if (renderSettings.CustomDisplayResidues != null && renderSettings.CustomDisplayResidues.Count > 0) {
                customDisplayResidues = renderSettings.CustomDisplayResidues;
            }

            atoms = primaryStructure.GetAtoms(renderSettings.ShowStandardResidues, renderSettings.ShowNonStandardResidues, enabledElementNames, enabledResidues);

            Dictionary<string, Visualization.ResidueDisplayOptions> residueOptions = null;
            if (customDisplayResidues != null) {
                highLightedAtoms = primaryStructure.GetAtoms(renderSettings.ShowStandardResidues, renderSettings.ShowNonStandardResidues, renderSettings.EnabledElements, customDisplayResidues);
                residueOptions = renderSettings.ResidueOptions;
            }

            bool filterByNumber = renderSettings.FilterResiduesByNumber;
            HashSet<int> enabledResidueNumbers = renderSettings.EnabledResidueNumbers;
            
            foreach (KeyValuePair<int, Bond> bond in Bonds) {

                Vector3 atom1pos, atom2pos;
                Atom atom1 = null;
                Atom atom2 = null;

                if (!atoms.TryGetValue(bond.Value.Atom1Index, out atom1) || !atoms.TryGetValue(bond.Value.Atom2Index, out atom2)) {
                    continue;
                }

                if (filterByNumber && enabledResidueNumbers != null &&
                    (!enabledResidueNumbers.Contains(atom1.ResidueID) || !enabledResidueNumbers.Contains(atom2.ResidueID))) {
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

                if (Settings.FlipZCoordinates) {
                    atom1pos.z = atom1pos.z * -1;
                    atom2pos.z = atom2pos.z * -1;
                }

                // bonds aren't recalculated on each frame. In some frames atoms jump from one side of the simulation box to another. When this happens need to disable bond view
                float bondLength = (atom2pos - atom1pos).magnitude / 2;
                if (bondLength > Settings.MaxBondLength) {
                    continue;
                }

                Vector3 position = ((atom1pos - atom2pos) / 2.0f) + atom2pos;
                float length = (atom2pos - atom1pos).magnitude;

                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, atom1pos - atom2pos);


                if (highLightedAtoms != null && highLightedAtoms.Count > 0 && highLightedAtoms.ContainsKey(atom1.ID) && highLightedAtoms.ContainsKey(atom2.ID)) {

                    string atom1residue = primaryStructure.Atoms()[atom1.ID].ResidueName;
                    string atom2residue = primaryStructure.Atoms()[atom2.ID].ResidueName;

                    // only colour or highlight bonds between atoms of the same residue
                    if (atom1residue == atom2residue && residueOptions.ContainsKey(atom1residue) && residueOptions[atom1residue].ColourBonds) {

                        Visualization.ResidueDisplayOptions options = residueOptions[atom1residue];

                        float cylinderWidth = standardCylinderWidth;
                        if (options.LargeBonds) {
                            cylinderWidth = enlargedCylinderWidth;
                        }

                        Vector3 localScale = new Vector3(cylinderWidth, length, cylinderWidth);

                        Matrix4x4 bondTransform = Matrix4x4.TRS(position, rotation, localScale);

                        if (!highlightedTransforms.ContainsKey(options.CustomColour)) {
                            highlightedTransforms.Add(options.CustomColour, new List<Matrix4x4>());
                        }

                        highlightedTransforms[options.CustomColour].Add(bondTransform);
                    }
                    else {

                        float cylinderWidth = standardCylinderWidth;
                        if (atom1residue == atom2residue && residueOptions.ContainsKey(atom1residue) && residueOptions[atom1residue].LargeBonds) {
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

            GameObject prefab = BondPrefabs[Settings.BondMeshQuality];

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
                        else if (atom.Position == null) {
                            UnityEngine.Debug.Log("Main chain atom position is null");
                        }

                        position = new Vector3(atom.Position.x, atom.Position.y, atom.Position.z);
                    }
                    else {
                        position = new Vector3(frame.Coords[atom.Index * 3], frame.Coords[(atom.Index * 3) + 1], frame.Coords[(atom.Index * 3) + 2]);
                    }

                    if (Settings.FlipZCoordinates) {
                        position.z = position.z * -1;
                    }

                    nodePositions.Add(position);
                }

                List<DynamicMeshNode> nodes = new List<DynamicMeshNode>();
                IEnumerable splinePoints = Interpolate.NewCatmullRom(nodePositions.ToArray(), interpolation, false);

                foreach (Vector3 position in splinePoints) {

                    DynamicMeshNode node = new DynamicMeshNode();
                    node.Position = position;
                    node.VertexColor = ChainColors[currentIndex % ChainColors.Length];

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

        private float getAtomScale(string elementName, MoleculeRenderSettings settings) {

            // set atom size
            float atomSize = 1;
            float representationScale = 1;

            if (settings.Representation == MolecularRepresentation.VDW) {
                if (!MolecularConstants.VDWRadius.TryGetValue(elementName, out representationScale)) {
                    MolecularConstants.VDWRadius.TryGetValue("Other", out representationScale);
                }
            }
            else if (settings.Representation == MolecularRepresentation.CPK) {
                if (!MolecularConstants.AtomicRadius.TryGetValue(elementName, out representationScale)) {
                    MolecularConstants.AtomicRadius.TryGetValue("Other", out representationScale);
                }
                representationScale *= Settings.CPKScaleFactor;
            }
            else {  // default all atoms to single standard atomic size
                MolecularConstants.AtomicRadius.TryGetValue("Other", out representationScale);
            }

            atomSize = settings.AtomScale * representationScale * 2; // need to double radius to set scale (diameter) of prefab

            // fudge here needs to be fixed with new prefab models
            if (Settings.AtomMeshQuality == 0) {
                atomSize = atomSize * 0.65f;
            }
            // fudge here needs to be fixed with new prefab models

            return atomSize;
        }
    }
}
