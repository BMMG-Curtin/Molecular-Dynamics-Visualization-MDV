using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class Molecule : MonoBehaviour {

        public GameObject MoleculeRender;
        public MoleculeBox MoleculeBox;

        private PrimaryStructure primaryStructure;
        private SecondaryStructure secondaryStructure;
        private PrimaryStructureTrajectory trajectory;
        private BoundingBox boundingBox;
        private Vector3 boundingBoxCentre; // cannot use original bounding box centre as z coords need to be flipped
        private MoleculeRenderSettings renderSettings;

        private MoleculeRenderer moleculeRenderer;

        private bool centredAtOrigin = false;

        private Quaternion saveRotation;
        private Vector3 savePosition;
        private Vector3 saveScale;

        private float scale = 1;
        private float scaleIncrementAmount = 0.1f;

        private bool rendering;
        private bool updateRender;

        private void Awake() {

            rendering = false;
            updateRender = false;

            renderSettings = MoleculeRenderSettings.Default();
            moleculeRenderer = MoleculeRender.GetComponent<MoleculeRenderer>();
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
                boundingBox = new BoundingBox(primaryStructure); //.OriginalBoundingBox;
                MoleculeRender.transform.position = new Vector3(-1 * boundingBox.Centre.x, -1 * boundingBox.Centre.y, boundingBox.Centre.z);
                transform.position = new Vector3(transform.position.x, (boundingBox.Height / 2f) + 0.5f, transform.position.z);

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

                    MoleculeBox.Build(boundingBox);
                    yield return StartCoroutine(moleculeRenderer.Render(primaryStructure, null, renderSettings));

                    rendering = false;
                }
            }
        }
    }
}