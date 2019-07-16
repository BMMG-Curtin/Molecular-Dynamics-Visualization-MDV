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

        private int frameAnimationSpeed;

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

                if (frameNumber == null || frameNumber == 0) {
                    FrameNumber.text = "-";
                }
                else {
                    FrameNumber.text = "0";
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
            molecules.GetSelected().CurrentTrajectoryFrameNumber = 0;
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
                return frameAnimationSpeed;
            }

            set {

                int animationSpeed = value;

                if (animationSpeed < 1)
                    animationSpeed = 1;

                if (animationSpeed > Settings.MaxFrameAnimationSpeed)
                    animationSpeed = Settings.MaxFrameAnimationSpeed;

                frameAnimationSpeed = animationSpeed;
                setFrameDelay(animationSpeed);
            }
        }


        // this needs to be reworked to be more clear
        private void setFrameDelay(int animationSpeed) {

            // Decrement animation speed to allow for scaling from 0. 
            animationSpeed = animationSpeed <= 0 ? 0 : animationSpeed - 1;

            // Decrement max speed to match animation speed decrement and still allow scaling up to 1. Don't allow maxSpeed of 0 
            int maxSpeed = Settings.MaxFrameAnimationSpeed;
            maxSpeed = maxSpeed > 1 ? maxSpeed - 1 : 1;

            float speedScale = ((float)animationSpeed / (float)maxSpeed); // should range from 0 to 1 unless Settings.MaxFrameAnimationSpeed was set to less than 2
            secondsBetweenFrames = (1 - speedScale) * Settings.MaxSecondsBetweenFrames;

            Debug.Log("Setting frame speed: " + secondsBetweenFrames);
        }

        private void stepForwardAnimation() {

            MoleculeSettings molecule = molecules.GetSelected();

            molecule.CurrentTrajectoryFrameNumber++;

            if (molecule.CurrentTrajectoryFrameNumber > molecule.TrajectoryFrameCount) {
                molecule.CurrentTrajectoryFrameNumber = 1;
            }

            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
            UpdateControls();

            Debug.Log("Stepping forward trajectory to frame: " + molecule.CurrentTrajectoryFrameNumber);
        }

        private void stepBackwardAnimation() {

            MoleculeSettings molecule = molecules.GetSelected();

            molecule.CurrentTrajectoryFrameNumber--;

            if (molecule.CurrentTrajectoryFrameNumber <= 0) {
                molecule.CurrentTrajectoryFrameNumber = molecule.TrajectoryFrameCount;
            }

            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
            UpdateControls();
        }
    }
}
