using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using CurtinUniversity.MolecularDynamics.Model.Analysis;
using CurtinUniversity.MolecularDynamics.Model.FileParser;
using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class SceneManager : MonoBehaviour {

        // inspector properties
        public GameObject MolecularModelBox;
        public GameObject MainCameraTransform;
        public GameObject Ground;

        public GUIManager GUIManager;
        public InputManager InputManager;
        public Lighting Lighting;
        public StructureView StructureView;
        public ModelBox ModelBox;
        public Model Model;
        public EventSystem EventSystem;

        public static SceneManager instance = null;

        // private variables
        private PrimaryStructure primaryStructure;
        private SecondaryStructure secondaryStructure;
        private BoundingBox primaryStructureBoundingBox;
        private BoundingBox calculatedBoundingBox;
        private PrimaryStructureTrajectory trajectory;

        // private GameObject viewerPosition;
        private float startingHeight;

        private bool loading = false;
        private bool startupLoad = true;
        private bool firstUserLoad = true;

        // private bool awaitingRebuild = false;
        private bool awaitingPrimaryStructureRebuild = false;
        private bool awaitingSecondaryStructureRebuild = false;

        void Awake() {

            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            Config.LoadConfig();
            Settings.Load();
        }

        void Start() {

            GUIManager.HideTrajectoryControls();
            Ground.SetActive(Settings.ShowGround);

            loadStartupModel();
        }

        void Update() {

            if (!loading && !StructureView.BuildingModel
                && (awaitingPrimaryStructureRebuild || awaitingSecondaryStructureRebuild)) {

                StartCoroutine(ReloadModelView(awaitingPrimaryStructureRebuild, awaitingSecondaryStructureRebuild));
                awaitingPrimaryStructureRebuild = false;
                awaitingSecondaryStructureRebuild = false;
            }
        }

        public void Quit() {
            Application.Quit();
        }

        /// <summary>
        /// Recreate views with current settings. Do not reload the model data.
        /// </summary>
        public IEnumerator ReloadModelView(bool primaryStructure, bool secondaryStructure) {

            if (!loading) { // don't reload while loading model from file

                if (StructureView.BuildingModel) {

                    if (primaryStructure) {
                        awaitingPrimaryStructureRebuild = true;
                    }

                    if (secondaryStructure) {
                        awaitingSecondaryStructureRebuild = true;
                    }
                }
                else {
                    yield return StructureView.Rebuild(primaryStructure, secondaryStructure);
                    StructureView.ResetModelView();
                }
            }
            else {
                awaitingPrimaryStructureRebuild = false;
                awaitingSecondaryStructureRebuild = false;
            }

            yield break;
        }

        // Gromacs files can include a structure in their first frame and trajectory in subsequent frames
        // Loads both structure and trajectory from the same file. 
        public IEnumerator LoadGromacsFile(string filePath, int startFrame, int frameCount, int frameFrequency) {

            if (!filePath.EndsWith(Settings.GromacsFileExtension)) {
                yield break;
            }

            int fileFrameCount = GROTrajectoryParser.GetFrameCount(filePath);

            yield return LoadStructure(filePath);
            if (fileFrameCount > 0) {
                yield return LoadTrajectory(filePath, startFrame, frameCount, frameFrequency);
            }
        }

        public IEnumerator LoadStructure(string structureFile, bool resetCameraPosition = false) {

            if (!loading) {

                //Debug.Log("*** Structure load started");

                awaitingPrimaryStructureRebuild = false;
                awaitingSecondaryStructureRebuild = false;
                yield return StartCoroutine(StructureView.FinishBuilds());

                loading = true;

                GUIManager.ShowConsoleMessage("Loading Structure");
                InputManager.KeyboardUIControlEnabled = false;
                InputManager.KeyboardSceneControlEnabled = false;

                yield return StartCoroutine(loadPrimaryStructure(structureFile));
                yield return new WaitForSeconds(0.05f); // allow screen update

                secondaryStructure = null;
                yield return StartCoroutine(loadSecondaryStructure(structureFile));
                yield return new WaitForSeconds(0.05f); // allow screen update

                if (primaryStructure != null) {

                    yield return StartCoroutine(Lighting.DimToBlack(0.5f));

                    ResetScene();

                    calculatedBoundingBox = new BoundingBox(primaryStructure);

                    if (Settings.UseFileSimulationBox && primaryStructure.OriginalBoundingBox != null) {
                        primaryStructureBoundingBox = primaryStructure.OriginalBoundingBox;
                    }
                    else {
                        primaryStructureBoundingBox = calculatedBoundingBox;
                    }

                    Model.Initialise(primaryStructure, primaryStructureBoundingBox);
                    ModelBox.Initialise(primaryStructureBoundingBox);
                    Lighting.SetLighting(primaryStructure, primaryStructureBoundingBox);
                    GUIManager.ElementsPanel.SetModelElements(primaryStructure.ElementNames);
                    //GUIManager.ResiduesPanel.SetModelResidues(primaryStructure.ResidueNames);

                    if (primaryStructure.AtomCount() > Settings.LowMeshQualityThreshold) {
                        Settings.AtomMeshQuality = Settings.LowMeshQualityValue;
                        Settings.BondMeshQuality = Settings.LowMeshQualityValue;
                    }

                    GUIManager.ReloadOptions();

                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    GUIManager.ShowConsoleMessage("Setting Up Structure View");

                    yield return StartCoroutine(StructureView.Initialise(primaryStructure, secondaryStructure));

                    watch.Stop();
                    GUIManager.ShowConsoleMessage("Structure View Setup Complete [" + watch.ElapsedMilliseconds + "ms]");
                    GUIManager.Console.BannerBuildTime = watch.ElapsedMilliseconds.ToString();

                    if (!startupLoad) {
                        ModelBox.Show(Settings.ShowSimulationBox);
                    }
                    StructureView.ResetModelView();

                    if (resetCameraPosition) {
                        positionCamera();
                    }

                    //Debug.Log("Scene Manager, start suspect issue");
                    //yield return StartCoroutine(Lighting.LightToDefaults(1f));
                    StartCoroutine(Lighting.LightToDefaults(1f));
                    //Debug.Log("Scene Manager, end suspect issue");

                    //Debug.Log("Scene manager, end primary structure condition");
                }

                InputManager.KeyboardUIControlEnabled = true;
                InputManager.KeyboardSceneControlEnabled = true;
                GUIManager.ShowConsoleMessage("Structure Load Complete");

                loading = false;
                //Debug.Log("** Structure load complete");
            }

            yield break;
        }

        public IEnumerator LoadTrajectory(string trajectoryFile, int startFrame, int frameCount, int frameFrequency) {

            if (!loading) {

                awaitingPrimaryStructureRebuild = false;
                awaitingSecondaryStructureRebuild = false;
                yield return StartCoroutine(StructureView.FinishBuilds());

                loading = true;
                GUIManager.ShowConsoleMessage("Loading Trajectory");

                trajectory = null;

                if (primaryStructure == null) {
                    GUIManager.ShowConsoleMessage("Can't load trajectory without structure loaded");
                }
                else {
                    int atomCount = LoadTrajectoryAtomCount(trajectoryFile);
                    if (atomCount == primaryStructure.AtomCount()) {

                        GUIManager.InfoPanel.AvailableTrajectoryFrames = LoadAvailableTrajectoryFrames(trajectoryFile);
                        yield return StartCoroutine(loadModelTrajectory(trajectoryFile, primaryStructure, startFrame, frameCount, frameFrequency));
                        yield return new WaitForSeconds(0.05f); // allow screen update

                        if (trajectory != null) {
                            StructureView.AddTrajectory(trajectory);
                            GUIManager.ShowConsoleMessage("Trajectory Initialised");
                            GUIManager.ShowTrajectoryControls();
                        }
                        else {
                            GUIManager.HideTrajectoryControls(); // may have loaded the trajectory twice and errored on the second load
                        }
                    }
                    else {
                        GUIManager.ShowConsoleMessage("Trajectory atom count [" + atomCount + " doesn't match loaded structure atom count [" + primaryStructure.AtomCount() + "]");
                    }
                }

                GUIManager.ShowConsoleMessage("Trajectory Load Complete");
                loading = false;
            }

            yield break;
        }

        public IEnumerator LoadTrajectoryColour(string colourFile, int startFrame, int frameCount, int frameFrequency) {

            if (!loading) {

                awaitingPrimaryStructureRebuild = false;
                awaitingSecondaryStructureRebuild = false;
                yield return StartCoroutine(StructureView.FinishBuilds());

                loading = true;
                GUIManager.ShowConsoleMessage("Loading Colours");

                if (primaryStructure == null) {
                    GUIManager.ShowConsoleMessage("Can't load trajectory colour file without a structure loaded");
                }
                else if (trajectory == null) {
                    GUIManager.ShowConsoleMessage("Can't load trajectory colour file without a trajectory loaded");
                }
                else {

                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    GUIManager.ShowConsoleMessage("Loading Trajectory Colour File: " + colourFile);
                    yield return new WaitForSeconds(0.05f);

                    try {
                        if (colourFile.EndsWith(".col")) {
                            TrajectoryColourFileParser.GetTrajectoryColours(colourFile, primaryStructure, trajectory, startFrame, frameCount, frameFrequency);
                        }
                    }
                    catch (FileParseException ex) {
                        GUIManager.ShowConsoleError("Error Loading Trajectory Colour File: " + ex.Message);
                    }

                    watch.Stop();
                    GUIManager.ShowConsoleMessage("Trajectory Colour File Load Complete [" + watch.ElapsedMilliseconds + "ms]");
                }

                GUIManager.ShowConsoleMessage("Colour Load Complete");
                loading = false;
            }

            yield break;
        }

        public void UpdateModelBox(PrimaryStructureFrame frame = null) {

            Debug.Log("Recreating bounding box");

            Model.SaveTransform();
            Model.ResetTransform();

            if (Settings.UseFileSimulationBox && primaryStructure.OriginalBoundingBox != null) {

                primaryStructureBoundingBox = primaryStructure.OriginalBoundingBox;
                Debug.Log("Setting box to file based bounding box.");
            }
            else {

                if (frame != null) {
                    Debug.Log("Calculating box from frame.");
                    primaryStructureBoundingBox = new BoundingBox(frame);
                }
                else {
                    Debug.Log("Setting box from original saved calculate box.");
                    primaryStructureBoundingBox = calculatedBoundingBox;
                }
            }

            ModelBox.Initialise(primaryStructureBoundingBox);

            Model.RestoreTransform();
        }

        private void ResetScene() {

            GUIManager.HideTrajectoryControls();
            trajectory = null;
            StructureView.ShowModelView(false);
            ModelBox.Show(false);
            Settings.ModelRotate = false;
            Model.ResetRotation();
            Model.Scale = Settings.DefaultModelScale;
            StructureView.PrimaryStructureView.AtomScale = Settings.DefaultAtomScale;
            StructureView.PrimaryStructureView.BondScale = Settings.DefaultBondScale;

            // set full model show on first user load unless settings have been updated already
            if (!startupLoad && firstUserLoad) {
                firstUserLoad = false;
                if (!GUIManager.VisualisationPanel.SettingsChanged) {
                    Settings.ShowAtoms = true;
                }
            }
        }

        private IEnumerator loadPrimaryStructure(string structureFile) {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            GUIManager.ShowConsoleMessage("Loading Structure File: " + structureFile);
            yield return new WaitForSeconds(0.05f);

            try {
                if (structureFile.EndsWith(".gro")) {
                    primaryStructure = GROStructureParser.GetStructure(structureFile);
                }
                else if (structureFile.EndsWith(".xyz")) {
                    primaryStructure = XYZStructureParser.GetStructure(structureFile);
                }
                else if (structureFile.EndsWith(".pdb")) {
                    primaryStructure = PDBStructureParser.GetPrimaryStructure(structureFile);
                }
            }
            catch (FileParseException ex) {
                Debug.Log("Error Loading Structure File: " + ex.Message);
                GUIManager.ShowConsoleError("Error Loading Structure File: " + ex.Message);
            }

            watch.Stop();
            GUIManager.ShowConsoleMessage("Structure File Load Complete [" + watch.ElapsedMilliseconds + "ms]");
            yield return new WaitForSeconds(0.05f);
        }

        private IEnumerator loadSecondaryStructure(string structureFile) {

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            GUIManager.ShowConsoleMessage("Attempting to generate secondary structure from primary structure file: " + structureFile);
            yield return new WaitForSeconds(0.05f);

            try {
                secondaryStructure = SecondaryStructure.CreateFromPrimaryStructure(primaryStructure, Settings.StrideExecutablePath, Settings.TmpFilePath);
            }
            catch (FileParseException ex) {
                Debug.Log("Error Loading Structure File: " + ex.Message);
                GUIManager.ShowConsoleError("Error Loading Secondary Structure File: " + ex.Message);
            }
            catch (StrideException ex) {
                StructureView.BypassSecondaryStructureBuild = true;
                Debug.Log("Error Loading Structure File: " + ex.Message);
                GUIManager.ShowConsoleError(ex.Message + "- Cannot build secondary structure.");
            }

            watch.Stop();

            if (secondaryStructure != null) {
                GUIManager.ShowConsoleMessage("Secondary Structure generation complete [" + watch.ElapsedMilliseconds + "ms]");
            }
            else {
                GUIManager.ShowConsoleMessage("Couldn't generate secondary structure from the primary structure file.");
            }
            yield return new WaitForSeconds(0.05f);
        }

        private IEnumerator loadModelTrajectory(string trajectoryFile, PrimaryStructure model, int startFrame, int frameCount, int frameFrequency) {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            GUIManager.ShowConsoleMessage("Loading Trajectory File: " + trajectoryFile);
            yield return new WaitForSeconds(0.05f);

            try {
                if (trajectoryFile.EndsWith(".xtc")) {
                    trajectory = XTCTrajectoryParser.GetTrajectory(trajectoryFile, startFrame, frameCount, frameFrequency);
                }
                else if (trajectoryFile.EndsWith(".dcd")) {
                    trajectory = DCDTrajectoryParser.GetTrajectory(trajectoryFile, startFrame, frameCount, frameFrequency);
                }
                else if (trajectoryFile.EndsWith(".gro")) {
                    trajectory = GROTrajectoryParser.GetTrajectory(trajectoryFile, startFrame, frameCount, frameFrequency);
                }
            }
            catch (FileParseException ex) {
                GUIManager.ShowConsoleError("Error Loading Trajectory File: " + ex.Message);
            }

            watch.Stop();
            GUIManager.ShowConsoleMessage("Trajectory File Load Complete [" + watch.ElapsedMilliseconds + "ms]");
            yield break;
        }

        private int LoadAvailableTrajectoryFrames(string trajectoryFile) {

            int count = 0;
            try {
                if (trajectoryFile.EndsWith(".xtc")) {
                    count = XTCTrajectoryParser.GetFrameCount(trajectoryFile);
                }
                else if (trajectoryFile.EndsWith(".dcd")) {
                    count = DCDTrajectoryParser.GetFrameCount(trajectoryFile);
                }
                else if (trajectoryFile.EndsWith(".gro")) {
                    count = GROTrajectoryParser.GetFrameCount(trajectoryFile);
                }
            }
            catch (FileParseException ex) {
                GUIManager.ShowConsoleError("Error Loading Trajectory File: " + ex.Message);
            }

            return count;
        }

        private int LoadTrajectoryAtomCount(string trajectoryFile) {

            int count = 0;
            try {
                if (trajectoryFile.EndsWith(".xtc")) {
                    count = XTCTrajectoryParser.GetAtomCount(trajectoryFile);
                }
                else if (trajectoryFile.EndsWith(".dcd")) {
                    count = DCDTrajectoryParser.GetAtomCount(trajectoryFile);
                }
                else if (trajectoryFile.EndsWith(".gro")) {
                    count = GROTrajectoryParser.GetAtomCount(trajectoryFile);
                }
            }
            catch (FileParseException ex) {
                GUIManager.ShowConsoleError("Error Loading Trajectory File: " + ex.Message);
            }

            return count;
        }

        private void loadStartupModel() {

            if (Config.GetString("LoadMoleculeOnStart") == "True") {
                string filename = Config.GetString("LoadMoleculeFileName");
                if (filename == null || filename.Trim() == "") {
                    return;
                }

                StartCoroutine(LoadStructureSilent(Application.streamingAssetsPath + Path.DirectorySeparatorChar + filename));
            }
        }

        private IEnumerator LoadStructureSilent(string filepath) {

            GUIManager.ConsoleSetSilent(true);
            Settings.ShowAtoms = false;
            Settings.ShowSimulationBox = false;

            yield return StartCoroutine(LoadStructure(filepath, true));

            GUIManager.ConsoleSetSilent(false);

            Settings.ModelRotate = true;
            startupLoad = false;
        }

        private void positionCamera() {

            float posZ = (ModelBox.Width > ModelBox.Depth ? ModelBox.Width : ModelBox.Depth);
            posZ = (posZ > ModelBox.Height ? posZ : ModelBox.Height);
            posZ *= 1.5f;

            float viewerHeight = 1.8f;

            GameObject temp = new GameObject();
            temp.transform.position = new Vector3(-2, viewerHeight, posZ);
            temp.transform.LookAt(new Vector3(0, primaryStructureBoundingBox.Centre.y + Model.HoverHeight(), 0));
            Vector3 rotation = temp.transform.rotation.eulerAngles;

            MainCameraTransform.transform.position = temp.transform.position;
            MainCameraTransform.transform.localRotation = Quaternion.Euler(new Vector3(0f, rotation.y, 0f));

            Destroy(temp);
        }
    }
}
