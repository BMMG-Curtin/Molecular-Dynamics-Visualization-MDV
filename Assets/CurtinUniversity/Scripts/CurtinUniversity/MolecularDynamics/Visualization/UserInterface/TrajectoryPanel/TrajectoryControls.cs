using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// Component to allow management of trajectory animation.
    /// Trajectory controls only show when a trajectory file is loaded
    /// </summary>
    public class TrajectoryControls : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private TMP_InputField frameNumber;

        [SerializeField]
        private TextMeshProUGUI totalFrames;

        [SerializeField]
        private TextMeshProUGUI animationSpeedText;

        private int animationSpeed;

        private bool animating = false;
        private float lastAnimationUpdate = 0;
        private float secondsBetweenFrames;

        private int lastFrameNumber;

        private void Start() {
            animationSpeed = Settings.DefaultFrameAnimationSpeed;
            setAnimationSpeed(animationSpeed);
        }

        private void Update() {

            if (animating && validMoleculeSelected()) {

                if (Time.time - lastAnimationUpdate > secondsBetweenFrames) {

                    stepForwardAnimation();
                    lastAnimationUpdate = Time.time;
                }
            }
        }

        public void StopAnimation() {
            animating = false;
        }

        public void UpdateFrameNumberInfo() {

            MoleculeSettings selectedMolecule = molecules.GetSelected();

            if (selectedMolecule.HasTrajectory) {

                int? frameNumber = selectedMolecule.CurrentTrajectoryFrameNumber;

                if (frameNumber == null) {
                    this.frameNumber.text = "-";
                }
                else {
                    this.frameNumber.text = (frameNumber + 1).ToString(); // trajectory indexes start at zero but display starts at 1
                }

                totalFrames.text = selectedMolecule.TrajectoryFrameCount.ToString();
            }
            else {

                frameNumber.text = "-";
                totalFrames.text = "-";
            }
        }

        public void OnPlayButton() {

            if (validMoleculeSelected()) {
                animating = true;
            }
        }

        public void OnPauseButton() {

            if (validMoleculeSelected()) {
                animating = false;
            }
        }

        public void OnStopButton() {

            if (validMoleculeSelected()) {

                animating = false;
                MoleculeSettings molecule = molecules.GetSelected();
                molecule.CurrentTrajectoryFrameNumber = null;
                UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
                UpdateFrameNumberInfo();
            }
        }

        public void OnForwardButton() {

            if (validMoleculeSelected()) {

                animating = false;
                stepForwardAnimation();
            }
        }

        public void OnBackButton() {

            if (validMoleculeSelected()) {

                animating = false;
                stepBackwardAnimation();
            }
        }

        public void OnEnterFrameInput() {

            Debug.Log("On frame input");

            if (validMoleculeSelected()) {

                Debug.Log("Storing last frame number");
                try {
                    lastFrameNumber = int.Parse(frameNumber.text);
                }
                catch (Exception) {
                    lastFrameNumber = 1;
                }
                animating = false;
            }
        }

        public void OnEndEditFrameInput() {

            Debug.Log("End frame input");

            try {

                int frameNumber = int.Parse(this.frameNumber.text);

                MoleculeSettings molecule = molecules.GetSelected();
                frameNumber = Mathf.Clamp(frameNumber, 1, molecule.TrajectoryFrameCount);
                this.frameNumber.text = frameNumber.ToString();

                if (frameNumber != lastFrameNumber) {

                    Debug.Log("New frameNumber: " + frameNumber);
                    molecule.CurrentTrajectoryFrameNumber = frameNumber - 1;
                    UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
                }
            }
            catch (Exception) {
                frameNumber.text = lastFrameNumber.ToString();
            }
        }

        public void OnIncreaseFrameSpeedButton() {

            if (validMoleculeSelected()) {
                setAnimationSpeed(animationSpeed + 1);
            }
        }

        public void OnDecreaseFrameSpeedButton() {

            if (validMoleculeSelected()) {
                setAnimationSpeed(animationSpeed - 1);
            }
        }

        private void setAnimationSpeed(int newSpeed) {

            animationSpeed = Mathf.Clamp(newSpeed, Settings.MinFrameAnimationSpeed, Settings.MaxFrameAnimationSpeed);

            float normalisedAnimationSpeed = (float)(animationSpeed - Settings.MinFrameAnimationSpeed) / (float)(Settings.MaxFrameAnimationSpeed - Settings.MinFrameAnimationSpeed);
            secondsBetweenFrames = Settings.MinSecondsBetweenFrames + ((1f - normalisedAnimationSpeed) * (Settings.MaxSecondsBetweenFrames - Settings.MinSecondsBetweenFrames));

            animationSpeedText.text = "x" + animationSpeed;
        }

        private void stepForwardAnimation() {

            MoleculeSettings molecule = molecules.GetSelected();
            molecule.CurrentTrajectoryFrameNumber++;

            if (molecule.CurrentTrajectoryFrameNumber == null || molecule.CurrentTrajectoryFrameNumber > molecule.TrajectoryFrameCount - 1) {
                molecule.CurrentTrajectoryFrameNumber = 0;
            }

            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
            UpdateFrameNumberInfo();
        }

        private void stepBackwardAnimation() {

            MoleculeSettings molecule = molecules.GetSelected();
            molecule.CurrentTrajectoryFrameNumber--;

            if (molecule.CurrentTrajectoryFrameNumber == null || molecule.CurrentTrajectoryFrameNumber < 0) {
                molecule.CurrentTrajectoryFrameNumber = molecule.TrajectoryFrameCount - 1;
            }

            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(molecule.ID, molecule.RenderSettings, molecule.CurrentTrajectoryFrameNumber);
            UpdateFrameNumberInfo();
        }

        private bool validMoleculeSelected() {

            MoleculeSettings molecule = molecules.GetSelected();
            if (molecule == null || molecule.Hidden || !molecule.HasTrajectory) {
                return false;
            }

            return true;
        }
    }
}
