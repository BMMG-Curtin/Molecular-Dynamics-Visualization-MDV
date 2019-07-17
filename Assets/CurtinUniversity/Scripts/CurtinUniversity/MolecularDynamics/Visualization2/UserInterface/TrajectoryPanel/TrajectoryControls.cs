using System;

using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class TrajectoryControls : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private InputField FrameNumber;

        [SerializeField]
        private Text TotalFrames;

        private int animationSpeed;

        private bool animating = false;
        private float lastAnimationUpdate = 0;
        private float secondsBetweenFrames;

        private void Start() {
            AnimationSpeed = Settings.MaxFrameAnimationSpeed;
        }

        private void Update() {

            if (animating) {

                if (Time.time - lastAnimationUpdate > secondsBetweenFrames) {

                    stepForwardAnimation();
                    lastAnimationUpdate = Time.time;
                }
            }
        }

        public void UpdateControls() {

            MoleculeSettings selectedMolecule = molecules.GetSelected();

            if (selectedMolecule.HasTrajectory) {

                int? frameNumber = selectedMolecule.CurrentTrajectoryFrameNumber;

                if (frameNumber == null) {
                    FrameNumber.text = "-";
                }
                else {
                    FrameNumber.text = (frameNumber + 1).ToString(); // trajectory indexes start at zero but display starts at 1
                }

                TotalFrames.text = selectedMolecule.TrajectoryFrameCount.ToString();
            }
            else {

                FrameNumber.text = "-";
                TotalFrames.text = "-";
            }
        }

        public void StopAnimation() {
            animating = false;
        }

        public void OnPlayButton() {
            animating = true;
        }

        public void OnPauseButton() {
            animating = false;
        }

        public void OnStopButton() {

            animating = false;
            MoleculeSettings molecule = molecules.GetSelected();
            molecule.CurrentTrajectoryFrameNumber = null;
            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
            UpdateControls();
        }

        public void OnForwardButton() {

            stepForwardAnimation();
            animating = false;
        }

        public void OnBackButton() {

            stepBackwardAnimation();
            animating = false;
        }

        public void OnEnterFrameInput() {

            //sceneManager.InputManager.KeyboardUIControlEnabled = false;
            //sceneManager.InputManager.KeyboardSceneControlEnabled = false;
        }

        public void OnEndEditFrameInput() {

            //sceneManager.InputManager.KeyboardUIControlEnabled = true;
            //sceneManager.InputManager.KeyboardSceneControlEnabled = true;
            //sceneManager.StructureView.DisplayFrame(GetFrameNumber());
        }

        public void OnIncreaseFrameSpeedButton() {
            AnimationSpeed++;
        }

        public void OnDecreaseFrameSpeedButton() {
            AnimationSpeed--;
        }

        public int AnimationSpeed {

            get {
                return animationSpeed;
            }

            set {

                animationSpeed = Mathf.Clamp(value, Settings.MinFrameAnimationSpeed, Settings.MaxFrameAnimationSpeed);
                float normalisedAnimationSpeed = (float)(animationSpeed - Settings.MinFrameAnimationSpeed) / (float)(Settings.MaxFrameAnimationSpeed - Settings.MinFrameAnimationSpeed);
                Debug.Log("Animation speed: " + animationSpeed.ToString() + "[" + normalisedAnimationSpeed.ToString("N2") + "]");
                secondsBetweenFrames = Settings.MinSecondsBetweenFrames + ((1f - normalisedAnimationSpeed) * (Settings.MaxSecondsBetweenFrames - Settings.MinSecondsBetweenFrames));
                Debug.Log("Seconds between frames: " + secondsBetweenFrames.ToString("N2"));
            }
        }

        private void stepForwardAnimation() {

            MoleculeSettings molecule = molecules.GetSelected();

            molecule.CurrentTrajectoryFrameNumber++;

            if (molecule.CurrentTrajectoryFrameNumber == null || molecule.CurrentTrajectoryFrameNumber > molecule.TrajectoryFrameCount - 1) {
                molecule.CurrentTrajectoryFrameNumber = 0;
            }

            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
            UpdateControls();
        }

        private void stepBackwardAnimation() {

            MoleculeSettings molecule = molecules.GetSelected();

            molecule.CurrentTrajectoryFrameNumber--;

            if (molecule.CurrentTrajectoryFrameNumber == null || molecule.CurrentTrajectoryFrameNumber < 0) {
                molecule.CurrentTrajectoryFrameNumber = molecule.TrajectoryFrameCount - 1;
            }

            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
            UpdateControls();
        }
    }
}
