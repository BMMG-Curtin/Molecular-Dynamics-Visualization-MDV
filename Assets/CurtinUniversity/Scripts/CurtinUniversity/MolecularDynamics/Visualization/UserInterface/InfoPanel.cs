using UnityEngine;
using UnityEngine.UI;

using CurtinUniversity.MolecularDynamics.Model.Model;
using CurtinUniversity.MolecularDynamics.Visualization.Utility;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class InfoPanel : MonoBehaviour {

        public Text InfoText;

        public int AvailableTrajectoryFrames { set { availableFrames = value; } }

        private int availableFrames = 0;

        private string modelInformation;

        void OnEnable() {
            SetModelInformation();
        }

        public void SetModelInformation() {

            SceneManager sceneManager = SceneManager.instance;

            string info = "\n\nModel Title: " + sceneManager.StructureView.Title
                + "\nAtom Count: " + sceneManager.StructureView.AtomCount
                + "\nBond Count: " + sceneManager.StructureView.BondCount
                + "\nResidue Count: " + sceneManager.StructureView.ResidueCount
                + "\nModel Box: " + (sceneManager.ModelBox.Width).ToString("n3") + ", " + (sceneManager.ModelBox.Height).ToString("n3") + ", " + (sceneManager.ModelBox.Depth).ToString("n3");

            if (sceneManager.StructureView.FrameCount != 0) {
                info += "\n\nLoaded Trajectory Frames: " + sceneManager.StructureView.FrameCount;
                info += "\nAvailable Trajectory Frames In File: " + availableFrames;
            }

            InfoText.text = "\n";
            InfoText.text += info;
        }
    }
}
