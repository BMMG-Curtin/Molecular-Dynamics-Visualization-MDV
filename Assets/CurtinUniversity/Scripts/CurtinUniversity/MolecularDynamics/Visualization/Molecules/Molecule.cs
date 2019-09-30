using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class Molecule : MonoBehaviour {

        [SerializeField]
        private GameObject moleculeRender;

        [SerializeField]
        private MoleculeBox moleculeBox;

        [SerializeField]
        private MoleculeInputController moleculeInput;

        [SerializeField]
        public PrimaryStructureRenderer primaryStructureRenderer;

        [SerializeField]
        public SecondaryStructureRenderer secondaryStructureRenderer;

        [SerializeField]
        public AtomHighlightsRenderer atomHighlightsRenderer;

        public int ID { get; private set; }
        public PrimaryStructure PrimaryStructure { get; private set; }
        public PrimaryStructureTrajectory PrimaryStructureTrajectory { get; private set; }
        public GameObject MoleculeRender { get; private set; }

        public float AutoRotateSpeed {
            set {
                autoRotateSpeed = (Mathf.Clamp(value, 0, 1) * (maxAutoRotateSpeed - minAutoRotateSpeed)) + minAutoRotateSpeed;
            }
        }

        [SerializeField]
        public GameObject atomHighlightsParent;
        public GameObject AtomHighlightsParent { get { return atomHighlightsParent; } }

        private SecondaryStructure secondaryStructure;
        private SecondaryStructureTrajectory secondaryStructureTrajectory;
        private BoundingBox boundingBox;

        private bool buildSecondaryStructureTrajectory = true;

        private MoleculeRenderSettings renderSettings;

        private bool rendering = false;
        private bool awaitingRender = false;

        private int awaitingMeshQuality;
        private int? awaitingFrameNumber;

        private bool autoRotateEnabled = false;
        private float autoRotateSpeed = 10f;
        private float maxAutoRotateSpeed = 50f;
        private float minAutoRotateSpeed = 2f;

        private void Awake() {
            MoleculeRender = moleculeRender;
        }

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

            moleculeBox.gameObject.SetActive(renderSettings.ShowSimulationBox);
            boundingBox = new BoundingBox(primaryStructure); //.OriginalBoundingBox;
            moleculeRender.transform.position = new Vector3(-1 * boundingBox.Centre.x, -1 * boundingBox.Centre.y, boundingBox.Centre.z);
            transform.position = new Vector3(transform.position.x, (boundingBox.Height / 2f) + 0.5f, transform.position.z);

            if (renderSettings.ShowSimulationBox) {
                moleculeBox.Build(boundingBox);
            }

            primaryStructureRenderer.Initialise(primaryStructure);
            secondaryStructureRenderer.Initialise(primaryStructure);

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
                StartCoroutine(Render(awaitingMeshQuality, awaitingFrameNumber));
            }
        }

        public void SetTrajectory(PrimaryStructureTrajectory trajectory) {

            this.PrimaryStructureTrajectory = trajectory;
            this.secondaryStructureTrajectory = new SecondaryStructureTrajectory(PrimaryStructure, trajectory, Settings.StrideExecutablePath, Settings.TmpFilePath);
        }

        public void SetRenderSettings(MoleculeRenderSettings settings) {

            renderSettings = settings;
            atomHighlightsRenderer.SetRenderSettings(settings);
        }

        public void EnableInput(bool inputEnabled) {
            moleculeInput.enabled = inputEnabled;
        }

        public void SetSpaceNavigatorControlEnabled(bool enabled) {
            moleculeInput.GetComponent<MoleculeInputController>().EnableSpaceNavigatorInput(enabled);
        }

        public void SetInputSensitivity(float sensitivity) {
            moleculeInput.InputSensitivity = sensitivity;
        }

        public BoundingBox GetBoundingBox() {
            return boundingBox.Clone();
        }

        public void Show() {
            moleculeRender.SetActive(true);
        }

        public void Hide() {
            moleculeRender.SetActive(false);
        }

        public IEnumerator Render(int meshQuality, int? frameNumber = null) {

            if(rendering) {

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

            // we clone the render settings so any updates dont interfere with the builds
            MoleculeRenderSettings renderSettingsClone = renderSettings.Clone();


            yield return StartCoroutine(primaryStructureRenderer.Render(renderSettingsClone, frame, meshQuality));

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

            yield return secondaryStructureRenderer.Render(renderSettingsClone, frame, secondaryStructureToBuild);

            // simulation box render

            if (renderSettingsClone.ShowSimulationBox) {

                moleculeBox.gameObject.SetActive(true);
                BoundingBox box = boundingBox;

                if (renderSettingsClone.CalculateBoxEveryFrame && frame != null) {
                    box = new BoundingBox(frame);
                }

                moleculeBox.Build(box);
            }
            else {
                moleculeBox.gameObject.SetActive(false);
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

        public void RenderAtomHighlights(List<HighLightedAtom> atoms) {
            atomHighlightsRenderer.RenderAtomHighlights(atoms);
        }

        public void ClearAtomHighlights() {
            atomHighlightsRenderer.ClearHighlights();
        }
    }
}