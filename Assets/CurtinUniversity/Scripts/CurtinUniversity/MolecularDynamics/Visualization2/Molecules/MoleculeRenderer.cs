using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using CurtinUniversity.MolecularDynamics.Model.Model;
using CurtinUniversity.MolecularDynamics.Model.Analysis;

using System.Diagnostics;

using CurtinUniversity.MolecularDynamics.Visualization.Utility;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class MoleculeRenderer : MonoBehaviour {

        public PrimaryStructureRenderer PrimaryStructureRenderer;
        public SecondaryStructureRenderer SecondaryStructureRenderer;

        //public int AtomCount { get { return primaryStructure == null ? 0 : primaryStructure.AtomCount(); } }
        //public int ResidueCount { get { return primaryStructure == null ? 0 : primaryStructure.ResidueCount(); } }
        //public string Title { get { return primaryStructure == null || primaryStructure.Title == null ? "-- none --" : primaryStructure.Title.Trim(); } }
        //public int BondCount { get { return PrimaryStructureRenderer.Bonds == null ? 0 : PrimaryStructureRenderer.Bonds.Count; } }
        //public int FrameCount { get { return primaryStructureTrajectory == null ? 0 : primaryStructureTrajectory.FrameCount(); } }

        public bool BuildingModel { get { return buildingModel; } }

        // this should really be a check to see if model is a protein
        public bool BypassSecondaryStructureBuild { get; set; } // used to bypass further SSbuilds if fails on first build.
        public bool BypassSecondaryStructureTrajectoryBuild { get; set; } // used to bypass further SSbuilds if fails on first build.

        private SceneManager sceneManager;

        // model data store
        private PrimaryStructure primaryStructure;
        private SecondaryStructure secondaryStructure;
        private PrimaryStructureTrajectory primaryStructureTrajectory;
        private SecondaryStructureTrajectory secondaryStructureTrajectory;
        private MoleculeRenderSettings renderSettings;

        private bool displayTrajectory = false;

        private int currentFrameIndex = 0;
        private bool animating = false;
        private float lastAnimationUpdate = 0;

        private float secondsBetweenFrames;

        private bool buildingModel = false;

        void Start() {
            //AnimationSpeed = Settings.FrameAnimationSpeed;
        }

        void Update() {

            //if (primaryStructureTrajectory != null && animating && !buildingModel) {
            //    if (Time.time - lastAnimationUpdate > secondsBetweenFrames) {
            //        DisplayNextFrame();
            //        lastAnimationUpdate = Time.time;
            //    }
            //}
        }

        public IEnumerator FinishBuilds() {

            //StopAnimation();
            while (buildingModel)
                yield return null;

            yield break;
        }

        public IEnumerator Initialise(PrimaryStructure primaryStructure, SecondaryStructure secondaryStructure, MoleculeRenderSettings settings) {

            this.primaryStructure = primaryStructure;
            this.secondaryStructure = secondaryStructure;
            this.renderSettings = settings;

            BypassSecondaryStructureBuild = false;
            BypassSecondaryStructureTrajectoryBuild = false;

            yield return PrimaryStructureRenderer.Initialise(primaryStructure);
            yield return SecondaryStructureRenderer.Initialise(primaryStructure);

            primaryStructureTrajectory = null;
            secondaryStructureTrajectory = null;
            displayTrajectory = false;

            UnityEngine.Debug.Log("Initialised model");
        }

        //public IEnumerator Render(MoleculeRenderSettings settings) {

        //    if (!buildingModel) {
        //        yield return StartCoroutine(buildModel(primaryStructure, secondaryStructure));
        //    }

        //    yield break;
        //}

        //private IEnumerator buildModel() {
        //    buildingModel = true;
        //    yield return StartCoroutine(buildModel(true, true, null));
        //}

        //private IEnumerator buildModel(int? frameNumber) {
        //    buildingModel = true;
        //    yield return StartCoroutine(buildModel(true, true, frameNumber));
        //}

        private IEnumerator Render(MoleculeRenderSettings settings) {

            //UnityEngine.Debug.Log("Starting model build");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //buildingModel = true;

            ////UnityEngine.Debug.Log("Building frame number: " + frameNumber);

            //PrimaryStructureFrame frame = null;
            //if (frameNumber != null) {
            //    frame = primaryStructureTrajectory.GetFrame((int)frameNumber);
            //}

            //SecondaryStructure secondaryStructureToBuild = null;

            //if (renderSettings.EnableSecondaryStructure) {

            //    if (frameNumber != null && secondaryStructureTrajectory != null && !BypassSecondaryStructureTrajectoryBuild) {
            //        try {
            //            secondaryStructureToBuild = secondaryStructureTrajectory.GetStructure((int)frameNumber);
            //        }
            //        catch (Exception ex) {
            //            MoleculeEvents.RaiseRenderMessage(ex.Message + " - Aborting trajectory secondary structure builds.", true);
            //            BypassSecondaryStructureTrajectoryBuild = true;
            //        }
            //    }
            //    else {
            //        secondaryStructureToBuild = secondaryStructure;
            //    }
            //}

            //if (buildPrimaryStructure) {
            //    yield return StartCoroutine(PrimaryStructureRenderer.Render(renderSettings, frame));
            //    //UnityEngine.Debug.Log("Completed primary structure build");
            //}

            //if (buildSecondaryStructure) {
            //    yield return SecondaryStructureView.BuildModel(frame, secondaryStructureToBuild);
            //    //UnityEngine.Debug.Log("Completed secondary structure build");
            //}

            //if (Settings.CalculateBoxEveryFrame) {
            //    sceneManager.UpdateModelBox(frame);
            //}

            Cleanup.ForeceGC();

            buildingModel = false;
            watch.Stop();
            if (Settings.DebugMessages) {
                //console.BannerBuildTime = watch.ElapsedMilliseconds.ToString();
            }

            //UnityEngine.Debug.Log("Ending model build. Elapsed time [" + watch.ElapsedMilliseconds.ToString() + "]");
            yield break;
        }

        //public void AddTrajectory(PrimaryStructureTrajectory trajectory) {

        //    primaryStructureTrajectory = trajectory;
        //    secondaryStructureTrajectory = new SecondaryStructureTrajectory(primaryStructure, primaryStructureTrajectory, Settings.StrideExecutablePath, Settings.TmpFilePath);
        //    BypassSecondaryStructureTrajectoryBuild = false;

        //    currentFrameIndex = 0;
        //    sceneManager.GUIManager.TrajectoryControls.SetFrameNumber("-");
        //    sceneManager.GUIManager.TrajectoryControls.SetTotalFrames(trajectory.FrameCount().ToString());
        //}

        //public void DisplayNextFrame() {

        //    if (!buildingModel) {
        //        if (!displayTrajectory) {
        //            displayTrajectory = true;
        //            ResetModelView();
        //        }
        //        else {
        //            currentFrameIndex++;
        //        }

        //        if (currentFrameIndex >= primaryStructureTrajectory.FrameCount()) {
        //            currentFrameIndex = 0;
        //        }

        //        StartCoroutine(buildModel(currentFrameIndex));

        //        sceneManager.GUIManager.TrajectoryControls.SetFrameNumber((currentFrameIndex + 1).ToString());
        //    }
        //}

        //public void DisplayPreviousFrame() {

        //    if (!buildingModel) {


        //        if (!displayTrajectory) {
        //            displayTrajectory = true;
        //            ResetModelView();
        //        }
        //        else {
        //            currentFrameIndex--;
        //        }

        //        if (currentFrameIndex < 0) {
        //            currentFrameIndex = primaryStructureTrajectory.FrameCount() - 1;
        //        }

        //        StartCoroutine(buildModel(currentFrameIndex));

        //        sceneManager.GUIManager.TrajectoryControls.SetFrameNumber((currentFrameIndex + 1).ToString());

        //    }
        //}

        //public IEnumerator ResetFrame() {

        //    yield return StartCoroutine(FinishBuilds());

        //    if (!buildingModel) {

        //        currentFrameIndex = 0;
        //        StartCoroutine(buildModel());

        //        sceneManager.GUIManager.TrajectoryControls.SetFrameNumber("-");
        //        displayTrajectory = false;
        //        ResetModelView();
        //    }
        //}

        //public void DisplayStartFrame() {

        //    if (!buildingModel) {

        //        displayTrajectory = true;
        //        ResetModelView();

        //        currentFrameIndex = 0;
        //        StartCoroutine(buildModel(currentFrameIndex));

        //        sceneManager.GUIManager.TrajectoryControls.SetFrameNumber((currentFrameIndex + 1).ToString());
        //    }
        //}

        //public void DisplayEndFrame() {

        //    if (!buildingModel) {

        //        displayTrajectory = true;
        //        ResetModelView();

        //        currentFrameIndex = primaryStructureTrajectory.FrameCount() - 1;
        //        StartCoroutine(buildModel(currentFrameIndex));

        //        sceneManager.GUIManager.TrajectoryControls.SetFrameNumber((currentFrameIndex + 1).ToString());
        //    }
        //}

        //public void DisplayFrame(int frameNumber) {

        //    if (!buildingModel) {

        //        frameNumber--; // displayed frame is indexed from 1, model trajectory frames are indexed from 0

        //        // only display chosen frame if within tranjectory frame bounds
        //        if (frameNumber >= 0 && frameNumber < primaryStructureTrajectory.FrameCount()) {
        //            currentFrameIndex = frameNumber;
        //            StartCoroutine(buildModel(currentFrameIndex));
        //        }

        //        sceneManager.GUIManager.TrajectoryControls.SetFrameNumber((currentFrameIndex + 1).ToString());
        //    }
        //    else {
        //        currentFrameIndex = frameNumber - 1;
        //    }
        //}

        //public void StartAnimation() {
        //    animating = true;
        //}

        //public void StopAnimation() {
        //    animating = false;
        //}

        //public void ResetAnimation() {
        //    animating = true;
        //    StartCoroutine(ResetFrame());
        //}

        //public int AnimationSpeed {

        //    get {
        //        return Settings.FrameAnimationSpeed;
        //    }

        //    set {
        //        int animationSpeed = value;

        //        if (animationSpeed < 1)
        //            animationSpeed = 1;

        //        if (animationSpeed > Settings.MaxFrameAnimationSpeed)
        //            animationSpeed = Settings.MaxFrameAnimationSpeed;

        //        Settings.FrameAnimationSpeed = animationSpeed;
        //        setFrameDelay(animationSpeed);
        //    }
        //}

        //private void setFrameDelay(int animationSpeed) {

        //    // Decrement animation speed to allow for scaling from 0. 
        //    animationSpeed = animationSpeed <= 0 ? 0 : animationSpeed - 1;

        //    // Decrement max speed to match animation speed decrement and still allow scaling up to 1. Don't allow maxSpeed of 0 
        //    int maxSpeed = Settings.MaxFrameAnimationSpeed;
        //    maxSpeed = maxSpeed > 1 ? maxSpeed - 1 : 1;

        //    float speedScale = ((float)animationSpeed / (float)maxSpeed); // should range from 0 to 1 unless Settings.MaxFrameAnimationSpeed was set to less than 2
        //    secondsBetweenFrames = (1 - speedScale) * Settings.MaxSecondsBetweenFrames;

        //    //UnityEngine.Debug.Log("Setting frame speed: " + secondsBetweenFrames);
        //}
    }
}
