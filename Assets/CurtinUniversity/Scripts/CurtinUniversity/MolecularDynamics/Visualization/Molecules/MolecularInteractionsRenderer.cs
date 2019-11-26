using System;
using System.Collections.Generic;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// Manages the rendering of molecule interactions calculated in the MolecularInteractions class
    /// </summary>
    public class MolecularInteractionsRenderer : MonoBehaviour {

        [SerializeField]
        private GameObject interactionLinePrefab;

        [SerializeField]
        private GameObject interactionLineParent;

        public Molecule Molecule1 { get; set; }
        public Molecule Molecule2 { get; set; }

        private List<AtomInteraction> interactions;
        private Dictionary<int, LineRenderer> interactionLines;

        private void Awake() {
            ClearInteractions();
        }
        
        // interactions are updated on an interval
        public void SetInteractions(List<AtomInteraction> interactions) {
            this.interactions = interactions;
        }

        public void ClearInteractions() {

            interactions = new List<AtomInteraction>();

            ClearInteractionLines();
            ClearAtomHighlights();
        }

        public void ClearInteractionLines() {

            interactionLines = new Dictionary<int, LineRenderer>();

            foreach (Transform transform in interactionLineParent.transform) {
                GameObject.Destroy(transform.gameObject);
            }
        }

        public void ClearAtomHighlights() {

            if (Molecule1 != null) {
                Molecule1.ClearAtomHighlights();
            }

            if (Molecule2 != null) {
                Molecule2.ClearAtomHighlights();
            }
        }

        // interaction lines are generally rendered every frame
        public void RenderInteractionLines(MolecularInteractionSettings interactionSettings) {

            if (interactions == null || Molecule1 == null || Molecule2 == null) {
                return;
            }

            HashSet<int> newInteractions = new HashSet<int>();

            foreach (AtomInteraction interaction in interactions) {

                if (interaction.InteractionColour == null) {
                    continue;
                }

                if ((interaction.InteractionType == InteractionType.Attractive && !interactionSettings.ShowAttractiveInteractions) ||
                    (interaction.InteractionType == InteractionType.Stable && !interactionSettings.ShowStableInteractions) ||
                    (interaction.InteractionType == InteractionType.Repulsive && !interactionSettings.ShowRepulsiveInteractions)) {
                    continue;
                }

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
                    Molecule1.MoleculeRender.transform.TransformPoint(atom1Position),
                    Molecule2.MoleculeRender.transform.TransformPoint(atom2Position)
                };

                lineRenderer.SetPositions(positions);
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.material.color = (Color)interaction.InteractionColour;

                lineRenderer.gameObject.SetActive(true);
                newInteractions.Add(key);
            }

            foreach (KeyValuePair<int, LineRenderer> line in interactionLines) {
                if (!newInteractions.Contains(line.Key)) {
                    line.Value.gameObject.SetActive(false);
                }
            }
        }

        // atom highlights are generally re-rendered only when interaction settings have been updated
        public void RenderAtomHighlights(MolecularInteractionSettings interactionSettings) {

            List<HighLightedAtom> molecule1Atoms = new List<HighLightedAtom>();
            List<HighLightedAtom> molecule2Atoms = new List<HighLightedAtom>();

            if (interactions != null) {

                foreach (AtomInteraction interaction in interactions) {

                    if(interaction.InteractionColour == null) {
                        continue;
                    }

                    if((interaction.InteractionType == InteractionType.Attractive && !interactionSettings.ShowAttractiveInteractions) ||
                        (interaction.InteractionType == InteractionType.Stable && !interactionSettings.ShowStableInteractions) ||
                        (interaction.InteractionType == InteractionType.Repulsive && !interactionSettings.ShowRepulsiveInteractions)) {
                        continue;
                    }

                    HighLightedAtom atom1 = new HighLightedAtom();
                    atom1.Atom = interaction.Atom1;
                    atom1.HighlightColor = (Color)interaction.InteractionColour;
                    molecule1Atoms.Add(atom1);

                    HighLightedAtom atom2 = new HighLightedAtom();
                    atom2.Atom = interaction.Atom2;
                    atom2.HighlightColor = (Color)interaction.InteractionColour;
                    molecule2Atoms.Add(atom2);
                }
            }

            Molecule1.RenderAtomHighlights(molecule1Atoms);
            Molecule2.RenderAtomHighlights(molecule2Atoms);
        }
    }
}
