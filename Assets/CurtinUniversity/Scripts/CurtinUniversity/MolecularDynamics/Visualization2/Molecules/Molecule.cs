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
        private bool updateRender;

        private void Awake() {

            rendering = false;
            updateRender = false;

            renderSettings = MoleculeRenderSettings.Default();
            moleculeRenderer = GetComponent<MoleculeRenderer>();
        }

        private void Update() {

            if(!rendering && updateRender) {
                StartCoroutine(render());
            }
        }

        public int ID { get { return this.GetInstanceID(); } }

        public MoleculeRenderSettings RenderSettings {

            get {
                return renderSettings;
            }

            set {
                renderSettings = value;
                updateRender = true;
            }
        }

        public PrimaryStructure PrimaryStructure {

            get {
                return primaryStructure;
            }
            set {
                primaryStructure = value;
                updateRender = true;
            }
        }

        public SecondaryStructure SecondaryStructure {

            get {
                return secondaryStructure;
            }
            set {
                secondaryStructure = value;
                updateRender = true;
            }
        }

        private IEnumerator render() {

            if(!rendering) {

                updateRender = false;

                if (primaryStructure != null) {

                    rendering = true;
                    yield return StartCoroutine(moleculeRenderer.Render(primaryStructure, null, renderSettings));
                    rendering = false;
                }
            }
        }
    }
}