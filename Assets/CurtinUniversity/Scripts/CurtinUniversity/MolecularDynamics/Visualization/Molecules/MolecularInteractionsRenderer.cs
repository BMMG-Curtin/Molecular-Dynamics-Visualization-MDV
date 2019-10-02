﻿using System;
using System.Collections.Generic;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractionsRenderer : MonoBehaviour {

        [SerializeField]
        private GameObject interactionLinePrefab;

        [SerializeField]
        private GameObject interactionLineParent;

        [SerializeField]
        private Gradient positiveGradient;

        [SerializeField]
        private Gradient negativeGradient;

        public Molecule Molecule1 { get; set; }
        public Molecule Molecule2 { get; set; }

        private List<AtomInteraction> interactions;
        private Dictionary<int, LineRenderer> interactionLines;

        private void Awake() {
            ClearInteractions();
        }

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

        public void RenderInteractionLines() {

            if (interactions == null || Molecule1 == null || Molecule2 == null) {
                return;
            }

            HashSet<int> newInteractions = new HashSet<int>();

            foreach (AtomInteraction interaction in interactions) {

                if (interaction.SimpleBondingForce == null && interaction.VDWForce == null) {
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
                lineRenderer.startWidth = 0.005f;
                lineRenderer.endWidth = 0.005f;

                //Color color = interaction.SimpleBondingForce > 0 ? positiveGradient.Evaluate((float)interaction.SimpleBondingForce) : negativeGradient.Evaluate((float)interaction.SimpleBondingForce * -1);
                Color color = interaction.VDWForce > 0 ? positiveGradient.Evaluate((float)interaction.VDWForce) : negativeGradient.Evaluate((float)interaction.VDWForce * -1);
                lineRenderer.material.color = color;

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

        public void RenderAtomHighlights() {

            List<HighLightedAtom> molecule1Atoms = new List<HighLightedAtom>();
            List<HighLightedAtom> molecule2Atoms = new List<HighLightedAtom>();

            if (interactions != null) {

                foreach (AtomInteraction interaction in interactions) {

                    if(interaction.SimpleBondingForce == null && interaction.VDWForce == null) {
                        continue;
                    }

                    //Color color = interaction.SimpleBondingForce > 0 ? positiveGradient.Evaluate((float)interaction.SimpleBondingForce) : negativeGradient.Evaluate((float)interaction.SimpleBondingForce * -1);
                    Color color = interaction.VDWForce > 0 ? positiveGradient.Evaluate((float)interaction.VDWForce) : negativeGradient.Evaluate((float)interaction.VDWForce * -1);

                    HighLightedAtom atom1 = new HighLightedAtom();
                    atom1.Atom = interaction.Atom1;
                    atom1.HighlightColor = color;
                    molecule1Atoms.Add(atom1);

                    HighLightedAtom atom2 = new HighLightedAtom();
                    atom2.Atom = interaction.Atom2;
                    atom2.HighlightColor = color;
                    molecule2Atoms.Add(atom2);
                }
            }

            Molecule1.RenderAtomHighlights(molecule1Atoms);
            Molecule2.RenderAtomHighlights(molecule2Atoms);
        }
    }
}
