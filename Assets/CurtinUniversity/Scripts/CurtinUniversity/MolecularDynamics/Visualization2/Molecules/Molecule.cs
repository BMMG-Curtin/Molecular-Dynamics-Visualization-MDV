using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model.Analysis;
using CurtinUniversity.MolecularDynamics.Model.Model;
using CurtinUniversity.MolecularDynamics.Model.FileParser;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class Molecule : MonoBehaviour {

        public GameObject MoleculeRender;
        public MoleculeBox MoleculeBox;

        public PrimaryStructureRenderer PrimaryStructureRenderer;
        public SecondaryStructureRenderer SecondaryStructureRenderer;

        private PrimaryStructure primaryStructure;
        private SecondaryStructure secondaryStructure;
        private PrimaryStructureTrajectory trajectory;
        private BoundingBox boundingBox;
        private Vector3 boundingBoxCentre; // cannot use original bounding box centre as z coords need to be flipped

        private bool centredAtOrigin = false;

        private Quaternion saveRotation;
        private Vector3 savePosition;
        private Vector3 saveScale;

        private float scale = 1;
        private float scaleIncrementAmount = 0.1f;

        private bool displayTrajectory = false;

        private int currentFrameIndex = 0;
        private bool animating = false;
        private float lastAnimationUpdate = 0;

        private float secondsBetweenFrames;

        private bool initialised = false;
        private bool rendering = false;
        private bool awaitingRender = false;
        private MoleculeRenderSettings awaitingRenderSettings;

        private bool hasSecondaryStructure = false;




        public int ID { get { return this.GetInstanceID(); } }

        public void Initialise(PrimaryStructure primaryStructure, MoleculeRenderSettings renderSettings) {

            this.primaryStructure = primaryStructure;

            MoleculeBox.gameObject.SetActive(renderSettings.ShowSimulationBox);

            boundingBox = new BoundingBox(primaryStructure); //.OriginalBoundingBox;
            MoleculeRender.transform.position = new Vector3(-1 * boundingBox.Centre.x, -1 * boundingBox.Centre.y, boundingBox.Centre.z);
            transform.position = new Vector3(transform.position.x, (boundingBox.Height / 2f) + 0.5f, transform.position.z);

            MoleculeBox.Build(boundingBox);
            //StartCoroutine(moleculeRenderer.Initialise(primaryStructure, null, renderSettings));

            StartCoroutine(PrimaryStructureRenderer.Initialise(primaryStructure));
            SecondaryStructureRenderer.Initialise(primaryStructure);

            initialised = true;
        }

        private void Update() {
            if(!rendering && awaitingRender) {
                StartCoroutine(Render(awaitingRenderSettings));
            }
        }

        public IEnumerator Render(MoleculeRenderSettings renderSettings) {

            if(rendering) {
                awaitingRenderSettings = renderSettings;
                awaitingRender = true;
                yield break;
            }

            rendering = true;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            PrimaryStructureFrame frame = null;
            //if (frameNumber != null) {
            //    frame = primaryStructureTrajectory.GetFrame((int)frameNumber);
            //}

            SecondaryStructure secondaryStructureToBuild = null;

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
                    secondaryStructureToBuild = secondaryStructure;
            //    }
            //}

            yield return StartCoroutine(PrimaryStructureRenderer.Render(renderSettings, frame));
            yield return SecondaryStructureRenderer.Render(renderSettings, frame);

            //if (Settings.CalculateBoxEveryFrame) {
            //    sceneManager.UpdateModelBox(frame);
            //}

            //Cleanup.ForeceGC();

            rendering = false;
            watch.Stop();
            if (Settings.DebugMessages) {
                //console.BannerBuildTime = watch.ElapsedMilliseconds.ToString();
            }

            //UnityEngine.Debug.Log("Ending model build. Elapsed time [" + watch.ElapsedMilliseconds.ToString() + "]");
            yield break;
        }

        private IEnumerator loadSecondaryStructure() {



            yield return new WaitForSeconds(0.05f);
        }
    }
}