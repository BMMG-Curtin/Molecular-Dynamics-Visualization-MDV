using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class TrajectoryControls : MonoBehaviour {

        public InputField FrameNumber;
        public Text TotalFrames;

        private SceneManager sceneManager;

        public void Start() {
            sceneManager = SceneManager.instance;
        }

        public void StartAnimation() {

            sceneManager.StructureView.StartAnimation();
        }

        public void StopAnimation() {

            sceneManager.StructureView.StopAnimation();
            StartCoroutine(sceneManager.StructureView.ResetFrame());
            FrameNumber.text = "-";
        }

        public void PauseAnimation() {

            sceneManager.StructureView.StopAnimation();
        }

        public void StepForwardAnimation() {

            sceneManager.StructureView.StopAnimation();
            sceneManager.StructureView.DisplayNextFrame();
        }

        public void StepBackwardAnimation() {

            sceneManager.StructureView.StopAnimation();
            sceneManager.StructureView.DisplayPreviousFrame();
        }

        public void OnEnterFrameInput() {

            sceneManager.InputManager.KeyboardUIControlEnabled = false;
            sceneManager.InputManager.KeyboardSceneControlEnabled = false;
        }

        public void OnEndEditFrameInput() {

            sceneManager.InputManager.KeyboardUIControlEnabled = true;
            sceneManager.InputManager.KeyboardSceneControlEnabled = true;
            sceneManager.StructureView.DisplayFrame(GetFrameNumber());
        }

        public void SetFrameNumber(string number) {
            FrameNumber.text = number;
        }

        public int GetFrameNumber() {

            try {
                return int.Parse(FrameNumber.text);
            }
            catch (Exception) {
                return -1;
            }
        }

        public void SetTotalFrames(string total) {
            TotalFrames.text = total;
        }
    }
}
