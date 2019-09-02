using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

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

        private Quaternion saveRotation;
        private Vector3 savePosition;
        private Vector3 saveScale;

        private bool rendering = false;
        private bool awaitingRender = false;

        private MoleculeRenderSettings awaitingRenderSettings;
        private int awaitingMeshQuality;
        private int? awaitingFrameNumber;

        public void Initialise(PrimaryStructure primaryStructure, MoleculeRenderSettings renderSettings) {

            if(primaryStructure == null) {
                return;
            }

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

            PrimaryStructureRenderer.Initialise(primaryStructure);
            SecondaryStructureRenderer.Initialise(primaryStructure);
        }

        public void SetTrajectory(PrimaryStructureTrajectory trajectory) {

            this.primaryStructureTrajectory = trajectory;
            this.secondaryStructureTrajectory = new SecondaryStructureTrajectory(PrimaryStructure, trajectory, Settings.StrideExecutablePath, Settings.TmpFilePath);
        }

        public void Show() {
            MoleculeRender.SetActive(true);
        }

        public void Hide() {
            MoleculeRender.SetActive(false);
        }

        private void Update() {

            if (!rendering && awaitingRender) {

                awaitingRender = false;
                StartCoroutine(Render(awaitingRenderSettings, awaitingMeshQuality, awaitingFrameNumber));
            }
        }

        public IEnumerator Render(MoleculeRenderSettings renderSettings, int meshQuality, int? frameNumber = null) {

            if(rendering) {
                awaitingRenderSettings = renderSettings;
                awaitingMeshQuality = meshQuality;
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

            yield return StartCoroutine(PrimaryStructureRenderer.Render(renderSettings, frame, meshQuality));

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

                MoleculeBox.Build(box);
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