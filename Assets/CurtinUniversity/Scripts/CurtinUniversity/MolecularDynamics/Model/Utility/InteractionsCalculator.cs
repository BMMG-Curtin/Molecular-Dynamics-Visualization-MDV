using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    public struct AtomInteraction {

        public Atom Atom1;
        public Atom Atom2;
        public float Distance;
        public float SimpleVDWInteractionForce;

        public override int GetHashCode() {

            int hashcode = 23;

            if (Atom1.Index < Atom2.Index) {
                hashcode = (hashcode * 37) + Atom1.Index;
                hashcode = (hashcode * 37) + Atom2.Index;
            }
            else {
                hashcode = (hashcode * 37) + Atom2.Index;
                hashcode = (hashcode * 37) + Atom1.Index;
            }

            return hashcode;
        }

        public override string ToString() {

            return
                "Atom1: " + Atom1 +
                "Atom2: " + Atom2 +
                "InteractionForce: " + SimpleVDWInteractionForce;
        }
    }

    public class InteractionsCalculator {

        private const int maxInteractionsPerAtom = 5;

        public List<AtomInteraction> GetAllInteractions(List<Atom> molecule1Atoms, List<Vector3> molecule1AtomPositions, List<Atom> molecule2Atoms, List<Vector3> molecule2AtomPositions, int processorCores = 1) {

            if (molecule1Atoms == null || molecule1AtomPositions == null || molecule1Atoms.Count != molecule1AtomPositions.Count) {
                Debug.Log("Interactions calculator, Molecule 1 atoms count and molecule 1 atom positions count don't match");
                return new List<AtomInteraction>();
            }

            if (molecule2Atoms == null || molecule2AtomPositions == null || molecule2Atoms.Count != molecule2AtomPositions.Count) {
                Debug.Log("Interactions calculator, Molecule 2 atoms count and molecule 2 atom positions count don't match");
                return new List<AtomInteraction>();
            }

            KdTree<float, int> molecule2AtomTree = new KdTree<float, int>(3, new FloatMath());

            for (int i = 0; i < molecule2Atoms.Count; i++) {

                Vector3 molecule2AtomPosition = molecule2AtomPositions[i];
                molecule2AtomTree.Add(new float[] { molecule2AtomPosition.x, molecule2AtomPosition.y, molecule2AtomPosition.z }, i);
            }

            List<AtomInteraction> interactions = new List<AtomInteraction>();

            for (int i = 0; i < molecule1Atoms.Count; i++) {

                Atom atom = molecule1Atoms[i];
                Vector3 atomPosition = molecule1AtomPositions[i];

                KdTreeNode<float, int>[] interactingAtoms = molecule2AtomTree.RadialSearch(new float[] { atomPosition.x, atomPosition.y, atomPosition.z }, BondSettings.MaximumLengthAllElements, maxInteractionsPerAtom);

                foreach(KdTreeNode<float, int> node in interactingAtoms) {

                    Atom interactingAtom = molecule2Atoms[node.Value];

                    Vector3 interactingAtomPosition = molecule2AtomPositions[node.Value];

                    // get distance between atoms
                    float deltaX = interactingAtomPosition.x - atomPosition.x;
                    float deltaY = interactingAtomPosition.y - atomPosition.y;
                    float deltaZ = interactingAtomPosition.z - atomPosition.z;
                    float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                    // interaction force is -1 to +1 
                    float? interactionForce = GetSimpleBondingForce(atom, interactingAtom, distance);

                    if (interactionForce != null) {

                        AtomInteraction interaction = new AtomInteraction() {
                            Atom1 = atom,
                            Atom2 = interactingAtom,
                            Distance = distance,
                            SimpleVDWInteractionForce = (float)interactionForce
                        };

                        interactions.Add(interaction);
                    }
                }
            }

            return interactions;
        }

        public List<AtomInteraction> GetClosestInteractions(List<AtomInteraction> interactions) {

            List<AtomInteraction> sortedInteractions = interactions.OrderBy(o => o.Distance).ToList();
            List<AtomInteraction> closestInteractions = new List<AtomInteraction>();

            HashSet<int> usedAtoms = new HashSet<int>();

            foreach(AtomInteraction interaction in sortedInteractions) {

                if(!usedAtoms.Contains(interaction.Atom1.Index) && !usedAtoms.Contains(interaction.Atom2.Index)) {

                    closestInteractions.Add(interaction);
                    usedAtoms.Add(interaction.Atom1.Index);
                    usedAtoms.Add(interaction.Atom2.Index);
                }

            }

            return closestInteractions;
        }

        private float getVDWAttractionScore(Atom atom1, Atom atom2, float distanceBetweenAtoms) {

            distanceBetweenAtoms *= 10f; // Nanomaters to Angstroms

            // need to get these from CHARMM19 lookup - see paper
            float sumOfAtomicRadii = (atom1.AtomicRadius * 10) + (atom2.AtomicRadius * 10);
            float energyWellDepth = 0f;

            float attractionScore;
            if (distanceBetweenAtoms > 0.89f * sumOfAtomicRadii && distanceBetweenAtoms < 8f) {


                attractionScore = (Mathf.Pow(sumOfAtomicRadii, 12) / Mathf.Pow(distanceBetweenAtoms, 12)) - (2 * (Mathf.Pow(sumOfAtomicRadii, 6) / Mathf.Pow(distanceBetweenAtoms, 6)));
            }

            return 0;
        }

        public float? GetSimpleBondingForce(Atom atom1, Atom atom2, float distance) {

            // null if distance > combined Atomic radii
            // 0 at distance at combined Atomic radii 
            // -1 at distance 2/3 of combined Atomic radii 
            // 0 at distance 1/3 of combined Atomic radii 
            // +1 at distance 0 of combined Atomic radii 

            float combinedAtomicRadius = atom1.AtomicRadius + atom2.AtomicRadius;

            if (distance > combinedAtomicRadius) {
                return null;
            }

            float distanceRatio = (combinedAtomicRadius - distance) / combinedAtomicRadius;
            float force = Mathf.Cos((Mathf.PI / 2) + (Mathf.PI * 1.5f * distanceRatio));

            return force;
        }
    }
}
