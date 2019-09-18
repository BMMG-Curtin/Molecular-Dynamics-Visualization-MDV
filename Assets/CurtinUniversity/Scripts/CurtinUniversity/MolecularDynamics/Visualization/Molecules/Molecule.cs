using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class Molecule : MonoBehaviour {

        public GameObject MoleculeRender;
        public MoleculeBox MoleculeBox;

        [SerializeField]
        private MoleculeInputController moleculeInput;

        public PrimaryStructureRenderer PrimaryStructureRenderer;
        public SecondaryStructureRenderer SecondaryStructureRenderer;

        public int ID { get; private set; }
        public PrimaryStructure PrimaryStructure { get; private set; }
        public PrimaryStructureTrajectory PrimaryStructureTrajectory { get; private set; }

        public float AutoRotateSpeed {
            set {
                autoRotateSpeed = (Mathf.Clamp(value, 0, 1) * (maxAutoRotateSpeed - minAutoRotateSpeed)) + minAutoRotateSpeed;
            }
        }

        private SecondaryStructure secondaryStructure;
        private SecondaryStructureTrajectory secondaryStructureTrajectory;
        private BoundingBox boundingBox;

        private bool buildSecondaryStructureTrajectory = true;

        private bool rendering = false;
        private bool awaitingRender = false;

        private MoleculeRenderSettings awaitingRenderSettings;
        private int awaitingMeshQuality;
        private int? awaitingFrameNumber;

        private bool autoRotateEnabled = false;
        private float autoRotateSpeed = 10f;
        private float maxAutoRotateSpeed = 50f;
        private float minAutoRotateSpeed = 2f;

        public void Initialise(int id, PrimaryStructure primaryStructure, MoleculeRenderSettings renderSettings) {

            if(primaryStructure == null) {
                return;
            }

            this.ID = id;
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

            moleculeInput.enabled = false;
            autoRotateEnabled = false;
            AutoRotateSpeed = 0f;
        }

        private void Update() {

            if (moleculeInput.enabled) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    autoRotateEnabled = !autoRotateEnabled;
                }
            }

            if (autoRotateEnabled) {
                transform.RotateAround(transform.position, Vector3.up, autoRotateSpeed * Time.deltaTime);
            }

            if (!rendering && awaitingRender) {

                awaitingRender = false;
                StartCoroutine(Render(awaitingRenderSettings, awaitingMeshQuality, awaitingFrameNumber));
            }
        }

        public void SetTrajectory(PrimaryStructureTrajectory trajectory) {

            this.PrimaryStructureTrajectory = trajectory;
            this.secondaryStructureTrajectory = new SecondaryStructureTrajectory(PrimaryStructure, trajectory, Settings.StrideExecutablePath, Settings.TmpFilePath);
        }

        public void EnableInput(bool inputEnabled) {
            moleculeInput.enabled = inputEnabled;
        }

        public void SetSpaceNavigatorControlEnabled(bool enabled) {
            moleculeInput.GetComponent<MoleculeInputController>().EnableSpaceNavigatorInput(enabled);
        }

        public BoundingBox GetBoundingBox() {
            return boundingBox.Clone();
        }

        public void Show() {
            MoleculeRender.SetActive(true);
        }

        public void Hide() {
            MoleculeRender.SetActive(false);
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
            if (PrimaryStructureTrajectory != null && frameNumber != null) {
                frame = PrimaryStructureTrajectory.GetFrame((int)frameNumber);
            }

            yield return StartCoroutine(PrimaryStructureRenderer.Render(renderSettings, frame, meshQuality));

            // secondary structure render

            SecondaryStructure secondaryStructureToBuild = null;

            if (secondaryStructureTrajectory != null && frameNumber != null && buildSecondaryStructureTrajectory) {

                string loadException = null;

                Thread thread = new Thread(() => {

                    try {
                        secondaryStructureToBuild = secondaryStructureTrajectory.GetStructure((int)frameNumber);
                    }
                    catch (FileParseException ex) {
                        loadException = ex.Message;
                    }
                });

                thread.Start();

                while (thread.IsAlive) {
                    yield return null;
                }

                if (loadException != null) {

                    MoleculeEvents.RaiseRenderMessage(loadException + " - Aborting trajectory secondary structure builds.", true);
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