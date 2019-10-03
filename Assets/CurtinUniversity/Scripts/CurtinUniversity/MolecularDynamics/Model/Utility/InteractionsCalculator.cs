using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    public struct AtomInteraction {

        public Atom Atom1;
        public Atom Atom2;
        public float Distance;
        public double? VDWForce;
        public double? ElectrostaticForce;

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
                "Atom1: " + Atom1 + "\n" +
                "Atom2: " + Atom2 + "\n" +
                "Distance: " + Distance + "\n" +
                "VDWForce: " + VDWForce + "\n" + 
                "SimpleForce: " + ElectrostaticForce + "\n";
        }
    }

    public class InteractionsCalculator {

        private const float maxInteractionDistance = 0.8f;
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

                KdTreeNode<float, int>[] interactingAtoms = molecule2AtomTree.RadialSearch(new float[] { atomPosition.x, atomPosition.y, atomPosition.z }, maxInteractionDistance, maxInteractionsPerAtom);

                foreach(KdTreeNode<float, int> node in interactingAtoms) {

                    Atom interactingAtom = molecule2Atoms[node.Value];

                    Vector3 interactingAtomPosition = molecule2AtomPositions[node.Value];

                    // get distance between atoms
                    float deltaX = interactingAtomPosition.x - atomPosition.x;
                    float deltaY = interactingAtomPosition.y - atomPosition.y;
                    float deltaZ = interactingAtomPosition.z - atomPosition.z;
                    float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                    double? electrostaticForce = getElecrostaticForce(atom, interactingAtom, distance);
                    double? ljPotential = getLJPotential(atom, interactingAtom, distance);

                    AtomInteraction interaction = new AtomInteraction() {

                        Atom1 = atom,
                        Atom2 = interactingAtom,
                        Distance = distance,
                        VDWForce = ljPotential,
                        ElectrostaticForce = electrostaticForce,
                    };

                    interactions.Add(interaction);
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

        // Calculates the Lennard-Jones Potential for two atoms
        private double? getLJPotential(Atom atom1, Atom atom2, double distanceBetweenAtoms) {

            AtomSigmaEpsilon atom1SigmaEpsilon = InteractionForces.GetAtomSigmaEpsilon(atom1);
            AtomSigmaEpsilon atom2SigmaEpsilon = InteractionForces.GetAtomSigmaEpsilon(atom1);

            double sumOfAtomicRadii = ((double)atom1SigmaEpsilon.Sigma + (double)atom2SigmaEpsilon.Sigma) / 2d;
            double energyWellDepth = Math.Sqrt((double)atom1SigmaEpsilon.Epsilon * (double)atom2SigmaEpsilon.Epsilon);
            double distanceBetweenAtoms6 = Math.Pow(distanceBetweenAtoms, 6);
            double distanceBetweenAtoms12 = Math.Pow(distanceBetweenAtoms, 12);
            double sumOfAtomicRadii6 = Math.Pow(sumOfAtomicRadii, 6);
            double sumOfAtomicRadii12 = Math.Pow(sumOfAtomicRadii, 12);

            if(distanceBetweenAtoms >= 8d) {
                return null;
            }

            double score = energyWellDepth * ((sumOfAtomicRadii12 / distanceBetweenAtoms12) - (2d * sumOfAtomicRadii6 / distanceBetweenAtoms6));
            return Double.IsPositiveInfinity(score) ? Double.MaxValue : score;
        }

        private float? getElecrostaticForce(Atom atom1, Atom atom2, float distance) {
            return null;
        }
    }
}
