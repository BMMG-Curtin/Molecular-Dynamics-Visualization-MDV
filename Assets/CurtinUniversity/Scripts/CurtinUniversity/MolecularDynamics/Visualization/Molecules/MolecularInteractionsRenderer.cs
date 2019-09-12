using System;
using System.Collections.Generic;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractionsRenderer : MonoBehaviour {

        [SerializeField]
        private GameObject interactionLinePrefab;

        [SerializeField]
        private GameObject interactionLineParent;


        private Transform molecule1;
        private Transform molecule2;

        private List<AtomInteraction> interactions;

        private Dictionary<int, LineRenderer> interactionLines;

        private void Awake() {
            interactionLines = new Dictionary<int, LineRenderer>();
        }

        public void SetInteractions(List<AtomInteraction> interactions) {
            this.interactions = interactions;
        }

        public void ClearInteractions() {

            this.interactions = null;
            interactionLines = new Dictionary<int, LineRenderer>();

            foreach (Transform transform in interactionLineParent.transform) {
                GameObject.Destroy(transform.gameObject);
            }

        }

        public void RenderInteractions(Transform molecule1, Transform molecule2) {

            if (interactions == null || molecule1 == null || molecule2 == null) {
                return;
            }

            HashSet<int> newInteractions = new HashSet<int>();

            foreach (AtomInteraction interaction in interactions) {

                int key = interaction.GetHashCode();

                LineRenderer lineRenderer = null;

                if (interactionLines.ContainsKey(key)) {
                    lineRenderer = interactionLines[key];
                }
                else {

                    GameObject lineGO = GameObject.Instantiate(interactionLinePrefab);
                    lineGO.transform.SetParent(interactionLineParent.transform);
                    lineRenderer = lineGO.GetComponent<LineRenderer>();
                    interactionLines.Add(key, lineRenderer);
                }

                Vector3 atom1Position = interaction.Atom1.Position;
                atom1Position.z *= -1;
                Vector3 atom2Position = interaction.Atom2.Position;
                atom2Position.z *= -1;

                Vector3[] positions = new Vector3[] {
                    molecule1.transform.TransformPoint(atom1Position),
                    molecule2.transform.TransformPoint(atom2Position)
                };

                lineRenderer.SetPositions(positions);
                lineRenderer.startWidth = 0.005f;
                lineRenderer.endWidth = 0.005f;
                lineRenderer.gameObject.SetActive(true);
                newInteractions.Add(key);
            }

            HashSet<int> toRemove = new HashSet<int>();

            foreach (KeyValuePair<int, LineRenderer> line in interactionLines) {

                if (!newInteractions.Contains(line.Key)) {

                    GameObject.Destroy(line.Value.gameObject);
                    toRemove.Add(line.Key);
                }
            }

            foreach (int key in toRemove) {
                interactionLines.Remove(key);
            }
        }
    }
}
