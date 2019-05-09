using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    [RequireComponent(typeof(MoleculeRenderer))]
    public class Molecule : MonoBehaviour {

        private PrimaryStructure primaryStructure;
        private SecondaryStructure secondaryStructure;
        private PrimaryStructureTrajectory Trajectory;
        private MoleculeRenderSettings renderSettings;

        private MoleculeRenderer renderer;

        private bool rendering;
        private bool settingsChanged;

        private void Awake() {

            rendering = false;
            settingsChanged = false;

            renderer = GetComponent<MoleculeRenderer>();
        }

        private void Update() {

            if(!rendering && settingsChanged) {
                StartCoroutine(render());
            }
        }

        public int ID { get { return this.GetInstanceID(); } }

        public MoleculeRenderSettings MoleculeRenderSettings {
            get {
                return renderSettings;
            }

            set {
                renderSettings = value;
                settingsChanged = true;
            }
        }

        public PrimaryStructure PrimaryStructure {

            get {
                return primaryStructure;
            }
            set {
                primaryStructure = value;
                settingsChanged = true;
            }
        }

        public SecondaryStructure SecondaryStructure {

            get {
                return secondaryStructure;
            }
            set {
                secondaryStructure = value;
                settingsChanged = true;
            }
        }

        private IEnumerator render() {

            if(!rendering) {

                settingsChanged = false;

                if (renderSettings != null && primaryStructure != null) {

                    rendering = true;

                    // do rendering
                    Debug.Log("Molecule is done rendering");

                    Debug.Log(primaryStructure.Title);

                    renderer.Initialise(primaryStructure, null, )

                    rendering = false;
                }
            }

            yield break;
        }
    }
}