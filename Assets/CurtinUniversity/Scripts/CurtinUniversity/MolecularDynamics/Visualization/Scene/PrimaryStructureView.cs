using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using CurtinUniversity.MolecularDynamics.Model.Definitions;
using CurtinUniversity.MolecularDynamics.Model.Model;
using CurtinUniversity.MolecularDynamics.Visualization.Utility;

using System.Diagnostics;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class PrimaryStructureView : MonoBehaviour {

        public GameObject AtomParent;
        public GameObject BondParent;
        public GameObject ChainParent;

        public Color32[] ChainColors;

        // unity prefabs
        public GameObject[] AtomPrefabs = new GameObject[3];
        public GameObject[] BondPrefabs = new GameObject[3];
        public GameObject ChainPrefab;

        public MeshBuilder meshBuilder;

        private SceneManager sceneManager;

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

        private float atomScale = 1f;
        private float bondScale = 1f;
        private float scaleIncrementAmount = 0.1f;

        private bool initialising = false;
        private bool buildingModel = false;

        void Start() {

            sceneManager = SceneManager.instance;
            AtomScale = Settings.DefaultAtomScale;
            BondScale = Settings.DefaultBondScale;
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

        public IEnumerator BuildModel(PrimaryStructureFrame frame) {

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
            if (Settings.EnablePrimaryStructure) {


                if (Settings.ShowAtoms) {
                    yield return StartCoroutine(createModelAtoms(frame));
                }

                if (Settings.Representation != MolecularRepresentation.VDW && Settings.ShowBonds) {

                    if (Settings.GenerateBonds && Bonds == null)
                        yield return StartCoroutine(calculateBonds());

                    if (Bonds != null && Bonds.Count > 0)
                        yield return StartCoroutine(createModelBonds(frame));
                }

                if (Settings.ShowMainChains) {
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
            if (Settings.DebugMessages)
                sceneManager.GUIManager.Console.BannerBuildTime = watch.ElapsedMilliseconds.ToString();

            yield break;
        }

        public void ResetModelView() {
            AtomParent.SetActive(Settings.ShowAtoms);
            BondParent.SetActive(Settings.ShowBonds);
        }

        public void ShowModelView(bool show) {
            AtomParent.SetActive(show);
            BondParent.SetActive(show);
        }

        public float AtomScale {

            get {
                return atomScale;
            }

            set {

                atomScale = value;

                if (atomScale > Settings.MaxAtomScale) {
                    atomScale = Settings.MaxAtomScale;
                }

                if (atomScale < Settings.MinAtomScale) {
                    atomScale = Settings.MinAtomScale;
                }
            }
        }

        public void IncreaseAtomScale() {
            AtomScale += scaleIncrementAmount;
        }

        public void DecreaseAtomScale() {
            AtomScale -= scaleIncrementAmount;
        }

        public float BondScale {

            get {
                return bondScale;
            }

            set {

                bondScale = value;

                if (bondScale > Settings.MaxBondScale) {
                    bondScale = Settings.MaxBondScale;
                }

                if (bondScale < Settings.MinBondScale) {
                    bondScale = Settings.MinBondScale;
                }
            }
        }

        public void IncreaseBondScale() {
            BondScale += scaleIncrementAmount;
        }

        public void DecreaseBondScale() {
            BondScale -= scaleIncrementAmount;
        }

        private IEnumerator calculateBonds() {

            sceneManager.GUIManager.ShowConsoleMessage("Calculating Bonds");
            yield return null;
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (primaryStructure == null) {
                UnityEngine.Debug.Log("Model is null");
            }

            Bonds = primaryStructure.GenerateBonds();
            watch.Stop();
            sceneManager.GUIManager.ShowConsoleMessage("Bonds Calculated [" + watch.ElapsedMilliseconds + "ms]");
        }

        private IEnumerator createModelAtoms(PrimaryStructureFrame frame) {

            if (frame != null && frame.Colours != null) {
                yield return StartCoroutine(createModelAtomsByColour(frame));
            }
            else {
                yield return StartCoroutine(createModelAtomsByElement(frame));
            }
        }

        private IEnumerator createModelAtomsByElement(PrimaryStructureFrame frame) {

            Quaternion atomOrientation = Quaternion.Euler(45, 45, 45);

            // generate combined meshes (i.e single GameObject) for atoms with same element/colour

            Dictionary<Color, List<Matrix4x4>> mergeTransforms = new Dictionary<Color, List<Matrix4x4>>();

            HashSet<string> enabledElementNames = null;
            HashSet<string> enabledResiduesNames = null;
            HashSet<string> customDisplayResidueNames = null;

            if (sceneManager.GUIManager.ElementsPanel.HasHiddenElements) {
                enabledElementNames = sceneManager.GUIManager.ElementsPanel.EnabledElements;
            }
            if (sceneManager.GUIManager.ResiduesPanel.HasHiddenResidues) {
                enabledResiduesNames = sceneManager.GUIManager.ResiduesPanel.EnabledResidueNames;
            }
            if (sceneManager.GUIManager.ResiduesPanel.HasCustomDisplayResidues) {
                customDisplayResidueNames = sceneManager.GUIManager.ResiduesPanel.CustomDisplayResidues;
            }

            bool filterByNumber = sceneManager.GUIManager.ResiduesPanel.FilterByNumber;
            HashSet<int> enabledResidueNumbers = sceneManager.GUIManager.ResiduesPanel.EnabledResideNumbers;

            Dictionary <int, Atom> atoms = primaryStructure.GetAtoms(Settings.ShowStandardResidues, Settings.ShowNonStandardResidues, enabledElementNames, enabledResiduesNames);

            foreach (KeyValuePair<int, Atom> item in atoms) {

                Atom atom = item.Value;

                if(filterByNumber && enabledResidueNumbers != null && !enabledResidueNumbers.Contains(atom.ResidueID)) {
                    continue;
                }

                float atomSize = getAtomScale(atom.Name);
                Vector3 scale = new Vector3(atomSize, atomSize, atomSize);

                Vector3 position;

                // if no frame use the base structure coordinates.
                if (frame == null) {
                    position = new Vector3(atom.Position.x, atom.Position.y, atom.Position.z);
                }
                else {
                    if (atom.Index >= frame.AtomCount) {
                        sceneManager.GUIManager.Console.ShowError("Atoms not found in frame record. Aborting frame render.");
                        yield break;
                    }
                    position = new Vector3(frame.Coords[atom.Index * 3], frame.Coords[(atom.Index * 3) + 1], frame.Coords[(atom.Index * 3) + 2]);
                }

                if (Settings.FlipZCoordinates) {
                    position.z = position.z * -1;
                }

                //// set colour for atoms 
                Color32 atomColour;

                if (customDisplayResidueNames != null && customDisplayResidueNames.Count > 0 && customDisplayResidueNames.Contains(atom.ResidueName)) {

                    ResidueDisplayOptions displayOptions = sceneManager.GUIManager.ResiduesPanel.ResidueOptions[atom.ResidueName];

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

            AddMeshToModel(parent, AtomParent);

            yield break;
        }

        private IEnumerator createModelAtomsByColour(PrimaryStructureFrame frame) {

            Quaternion atomOrientation = Quaternion.Euler(45, 45, 45);

            Dictionary<Color32, Dictionary<int, Atom>> atomsByColour = GetAtomsByColour(primaryStructure, frame);

            // generate combined meshes (i.e single GameObject) for atoms with same element
            foreach (KeyValuePair<Color32, Dictionary<int, Atom>> colourAtoms in atomsByColour) {

                Matrix4x4[] mergeTransforms = new Matrix4x4[colourAtoms.Value.Count];
                int index = 0;

                foreach (KeyValuePair<int, Atom> atom in colourAtoms.Value) {

                    Vector3 position;

                    // if no frame number use the base structure coordinates.
                    if (frame == null) {
                        position = new Vector3(atom.Value.Position.x, atom.Value.Position.y, atom.Value.Position.z);
                    }
                    else {
                        if (atom.Key >= frame.AtomCount) {
                            sceneManager.GUIManager.Console.ShowError("Incomplete atom information in frame. Aborting frame render.");
                            yield break;
                        }
                        position = new Vector3(frame.Coords[atom.Key * 3], frame.Coords[(atom.Key * 3) + 1], frame.Coords[(atom.Key * 3) + 2]);
                    }

                    if (Settings.FlipZCoordinates) {
                        position.z = position.z * -1;
                    }

                    float atomSize = getAtomScale(atom.Value.Element.ToString());
                    Vector3 scale = new Vector3(atomSize, atomSize, atomSize);

                    mergeTransforms[index] = Matrix4x4.TRS(position, atomOrientation, scale);
                    index++;
                }

                GameObject prefab = AtomPrefabs[Settings.AtomMeshQuality];

                // yield return StartCoroutine(prefabMerger.Merge(prefab, colourAtoms.Key, mergeTransforms, SceneManager.Model.ModelCentre, AtomParent));

                GameObject parent = new GameObject("CombinedMeshParent");
                parent.SetActive(false);
                yield return StartCoroutine(meshBuilder.CombinedMesh(prefab, mergeTransforms, colourAtoms.Key, parent));
                AddMeshToModel(parent, AtomParent);
            }

            yield break;
        }

        private Dictionary<Color32, Dictionary<int, Atom>> GetAtomsByColour(PrimaryStructure model, PrimaryStructureFrame frame) {

            Dictionary<Color32, Dictionary<int, Atom>> output = new Dictionary<Color32, Dictionary<int, Atom>>();

            Dictionary<int, Atom> atoms = model.GetAtoms(Settings.ShowStandardResidues, Settings.ShowNonStandardResidues);

            int atomCount = 0;

            foreach (KeyValuePair<int, Atom> atom in atoms) {

                float colour;
                try {
                    colour = frame.Colours[atom.Key];
                }
                catch (IndexOutOfRangeException) {
                    colour = 0f;
                }

                // discard atoms with no colour value
                if (colour == 0) {
                    continue;
                }

                atomCount++;

                Color32 colourRGB = mapValueToColour32(colour);

                if (!output.ContainsKey(colourRGB)) {
                    Dictionary<int, Atom> colourAtoms = new Dictionary<int, Atom>();
                    colourAtoms.Add(atom.Key, atom.Value);
                    output.Add(colourRGB, colourAtoms);
                }
                else {
                    Dictionary<int, Atom> colourAtoms;
                    if (output.TryGetValue(colourRGB, out colourAtoms)) {
                        colourAtoms.Add(atom.Key, atom.Value);
                    }
                }
            }

            return output;
        }

        private Color32 mapValueToColour32(float value) {

            if (value >= Settings.MidColourValue && value <= 1) {

                // blue gradient from white at midColourValue to blue at maxColourValue. 
                value = Mathf.Clamp(value, Settings.MidColourValue, Settings.MaxColourValue);
                float normalised = (value - Settings.MidColourValue) / (Settings.MaxColourValue - Settings.MidColourValue);
                int step = (int)(normalised * Settings.ColourBands);
                float saturation = (float)step / (float)Settings.ColourBands;

                Color32 colour = Colour.HSVToRGB(Settings.MaxColourHue, saturation, 1f);

                return colour;
            }
            else if (value < Settings.MidColourValue && value >= 0) {

                // red gradient from red at minColourValue to white at midColourValue
                value = Mathf.Clamp(value, Settings.MinColourValue, Settings.MidColourValue);
                float normalised = 1f - ((value - Settings.MinColourValue) / (Settings.MidColourValue - Settings.MinColourValue));
                int step = (int)(normalised * Settings.ColourBands);
                float saturation = (float)step / (float)Settings.ColourBands;

                Color32 colour = Colour.HSVToRGB(Settings.MinColourHue, saturation, 1f);

                return colour;
            }
            else {
                return new Color32(224, 0, 194, 1); // purple
            }
        }

        private IEnumerator createModelBonds(PrimaryStructureFrame frame) {

            // set colour for bonds
            Color32 bondColour;
            if (!MolecularConstants.CPKColors.TryGetValue("Bond", out bondColour)) {
                MolecularConstants.CPKColors.TryGetValue("Other", out bondColour);
            }

            //Color32 highlightedBondColour = Colour.HSVToRGB(Settings.HighlightedBondColourHue, 1f, 1f);

            List<Matrix4x4> standardTransforms = new List<Matrix4x4>();
            Dictionary<Color, List<Matrix4x4>> highlightedTransforms = new Dictionary<Color, List<Matrix4x4>>();

            float standardCylinderWidth = 0.015f * bondScale;
            float enlargedCylinderWidth = 0.040f * bondScale;

            // get atoms for bonds
            Dictionary<int, Atom> atoms;
            Dictionary<int, Atom> highLightedAtoms = new Dictionary<int, Atom>();
            HashSet<string> enabledElements = null;
            HashSet<string> enabledResidues = null;
            HashSet<string> customDisplayResidues = null;

            if (sceneManager.GUIManager.ElementsPanel.HasHiddenElements) {
                enabledElements = sceneManager.GUIManager.ElementsPanel.EnabledElements;
            }
            if (sceneManager.GUIManager.ResiduesPanel.HasHiddenResidues) {
                enabledResidues = sceneManager.GUIManager.ResiduesPanel.EnabledResidueNames;
            }
            if (sceneManager.GUIManager.ResiduesPanel.HasCustomDisplayResidues) {
                customDisplayResidues = sceneManager.GUIManager.ResiduesPanel.CustomDisplayResidues;
            }

            atoms = primaryStructure.GetAtoms(Settings.ShowStandardResidues, Settings.ShowNonStandardResidues, enabledElements, enabledResidues);

            Dictionary<string, ResidueDisplayOptions> residueOptions = null;
            if (customDisplayResidues != null) {
                highLightedAtoms = primaryStructure.GetAtoms(Settings.ShowStandardResidues, Settings.ShowNonStandardResidues, enabledElements, customDisplayResidues);
                residueOptions = sceneManager.GUIManager.ResiduesPanel.ResidueOptions;
            }

            bool filterByNumber = sceneManager.GUIManager.ResiduesPanel.FilterByNumber;
            HashSet<int> enabledResidueNumbers = sceneManager.GUIManager.ResiduesPanel.EnabledResideNumbers;

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

                        ResidueDisplayOptions options = residueOptions[atom1residue];

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
            AddMeshToModel(parent, BondParent);


            parent = new GameObject("HighligtedCombinedMeshParent");
            parent.SetActive(false);

            foreach (KeyValuePair<Color, List<Matrix4x4>> item in highlightedTransforms) {
                yield return StartCoroutine(meshBuilder.CombinedMesh(prefab, item.Value.ToArray(), item.Key, parent));
            }

            AddMeshToModel(parent, BondParent);
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
                AddMeshToModel(chainStructure, ChainParent);

                currentIndex++;
                yield return null;
            }
        }

        private void AddMeshToModel(GameObject meshObject, GameObject modelObject) {

            sceneManager.Model.SaveTransform();
            sceneManager.Model.ResetTransform();
            float modelHover = sceneManager.Model.HoverHeight();
            //UnityEngine.Debug.Log("Add mesh to model. Hover height: " + modelHover);
            meshObject.transform.position = new Vector3(-1 * sceneManager.Model.ModelCentre.x, modelHover, -1 * sceneManager.Model.ModelCentre.z);
            meshObject.transform.parent = modelObject.transform;
            sceneManager.Model.RestoreTransform();
        }

        private float getAtomScale(string elementName) {

            // set atom size
            float atomSize = 1;
            float representationScale = 1;

            if (Settings.Representation == MolecularRepresentation.VDW) {
                if (!MolecularConstants.VDWRadius.TryGetValue(elementName, out representationScale)) {
                    MolecularConstants.VDWRadius.TryGetValue("Other", out representationScale);
                }
            }
            else if (Settings.Representation == MolecularRepresentation.CPK) {
                if (!MolecularConstants.AtomicRadius.TryGetValue(elementName, out representationScale)) {
                    MolecularConstants.AtomicRadius.TryGetValue("Other", out representationScale);
                }
                representationScale *= Settings.CPKScaleFactor;
            }
            else {  // default all atoms to single standard atomic size
                MolecularConstants.AtomicRadius.TryGetValue("Other", out representationScale);
            }

            atomSize = AtomScale * representationScale * 2; // need to double radius to set scale (diameter) of prefab

            // fudge here needs to be fixed with new prefab models
            if (Settings.AtomMeshQuality == 0) {
                atomSize = atomSize * 0.65f;
            }
            // fudge here needs to be fixed with new prefab models

            return atomSize;
        }
    }
}
