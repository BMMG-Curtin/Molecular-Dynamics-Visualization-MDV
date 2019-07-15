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

        public PrimaryStructure PrimaryStructure { get; private set; }

        private SecondaryStructure secondaryStructure;
        private PrimaryStructureTrajectory primaryStructureTrajectory;
        private SecondaryStructureTrajectory secondaryStructureTrajectory;
        private BoundingBox boundingBox;
        private Vector3 boundingBoxCentre; // cannot use original bounding box centre as z coords need to be flipped

        private bool buildSecondaryStructureTrajectory = true;
        private int? frameNumber = null;

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
        private int? awaitingFrameNumber;

        public int ID { get { return this.GetInstanceID(); } }

        public void Initialise(PrimaryStructure primaryStructure, MoleculeRenderSettings renderSettings) {

            this.PrimaryStructure = primaryStructure;

            try {
                secondaryStructure = SecondaryStructure.CreateFromPrimaryStructure(primaryStructure, Settings.StrideExecutablePath, Settings.TmpFilePath);
            }
            catch (Exception ex) {
                Debug.Log("Error Parsing Secondary Structure from Structure File: " + ex.Message);
                buildSecondaryStructureTrajectory = false;
            }

            MoleculeBox.gameObject.SetActive(renderSettings.ShowSimulationBox);
            boundingBox = new BoundingBox(primaryStructure); //.OriginalBoundingBox;
            MoleculeRender.transform.position = new Vector3(-1 * boundingBox.Centre.x, -1 * boundingBox.Centre.y, boundingBox.Centre.z);
            transform.position = new Vector3(transform.position.x, (boundingBox.Height / 2f) + 0.5f, transform.position.z);

            if (renderSettings.ShowSimulationBox) {
                MoleculeBox.Build(boundingBox);
            }

            StartCoroutine(PrimaryStructureRenderer.Initialise(primaryStructure));
            StartCoroutine(SecondaryStructureRenderer.Initialise(primaryStructure));

            initialised = true;
        }

        public void SetTrajectory(PrimaryStructureTrajectory trajectory) {

            this.primaryStructureTrajectory = trajectory;
            this.secondaryStructureTrajectory = new SecondaryStructureTrajectory(PrimaryStructure, trajectory, Settings.StrideExecutablePath, Settings.TmpFilePath);
        }

        private void Update() {

            if (!rendering && awaitingRender) {

                awaitingRender = false;
                StartCoroutine(Render(awaitingRenderSettings, awaitingFrameNumber));
            }
        }

        public IEnumerator Render(MoleculeRenderSettings renderSettings, int? frameNumber = null) {

            if(rendering) {
                awaitingRenderSettings = renderSettings;
                awaitingFrameNumber = frameNumber;
                awaitingRender = true;
                yield break;
            }

            rendering = true;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            // primary structure render

            PrimaryStructureFrame frame = null;
            if (primaryStructureTrajectory != null && frameNumber != null) {
                frame = primaryStructureTrajectory.GetFrame((int)frameNumber);
            }

            yield return StartCoroutine(PrimaryStructureRenderer.Render(renderSettings, frame));

            // secondary structure render

            SecondaryStructure secondaryStructureToBuild = null;

            if (secondaryStructureTrajectory != null && frameNumber != null && buildSecondaryStructureTrajectory) {
                try {
                    secondaryStructureToBuild = secondaryStructureTrajectory.GetStructure((int)frameNumber);
                }
                catch (Exception ex) {
                    MoleculeEvents.RaiseRenderMessage(ex.Message + " - Aborting trajectory secondary structure builds.", true);
                    buildSecondaryStructureTrajectory = false;
                }
            }
            else {
                secondaryStructureToBuild = secondaryStructure;
            }

            yield return SecondaryStructureRenderer.Render(renderSettings, frame, secondaryStructureToBuild);

            // simulation box render

            if (renderSettings.ShowSimulationBox) {

                MoleculeBox.gameObject.SetActive(true);
                BoundingBox box = boundingBox;

                if (renderSettings.CalculateBoxEveryFrame && frame != null) {
                    box = new BoundingBox(frame);
                }

                MoleculeBox.Build(boundingBox);
            }
            else {
                MoleculeBox.gameObject.SetActive(false);
            }

            //Cleanup.ForeceGC();

            rendering = false;
            watch.Stop();
            if (Settings.DebugMessages) {
                //console.BannerBuildTime = watch.ElapsedMilliseconds.ToString();
            }

            //UnityEngine.Debug.Log("Ending model build. Elapsed time [" + watch.ElapsedMilliseconds.ToString() + "]");
            yield break;
        }
    }
}