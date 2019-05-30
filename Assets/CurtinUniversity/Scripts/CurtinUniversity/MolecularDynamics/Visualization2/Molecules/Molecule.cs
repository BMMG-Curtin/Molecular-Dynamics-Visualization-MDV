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
        private PrimaryStructureTrajectory trajectory;
        private MoleculeRenderSettings renderSettings;

        private MoleculeRenderer moleculeRenderer;

        private bool rendering;
        private bool settingsChanged;

        private void Awake() {

            rendering = false;
            settingsChanged = false;

            renderSettings = MoleculeRenderSettings.Default();
            moleculeRenderer = GetComponent<MoleculeRenderer>();
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

                if (primaryStructure != null) {

                    rendering = true;
                    yield return StartCoroutine(moleculeRenderer.Render(primaryStructure, null, renderSettings));
                    rendering = false;
                }
            }
        }
    }
}