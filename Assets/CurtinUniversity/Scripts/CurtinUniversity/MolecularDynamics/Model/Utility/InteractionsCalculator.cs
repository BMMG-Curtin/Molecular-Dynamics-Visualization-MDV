using System;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    public struct AtomInteraction {

        public Atom Atom1;
        public Atom Atom2;
        public float InteractionForce;

        public override int GetHashCode() {

            int hashcode = 23;
            hashcode = (hashcode * 37) + Atom1.Index;
            hashcode = (hashcode * 37) + Atom2.Index;
            return hashcode;
        }

        public override string ToString() {

            return
                "Atom1: " + Atom1 +
                "Atom2: " + Atom2 +
                "InteractionForce: " + InteractionForce;
        }
    }

    public class InteractionsCalculator {

        private const int MaxBondsPerAtom = 5;

        public List<AtomInteraction> GetInteractions(List<Atom> molecule1Atoms, List<Vector3> molecule1AtomPositions, List<Atom> molecule2Atoms, List<Vector3> molecule2AtomPositions, int processorCores = 1) {

            List<AtomInteraction> interactions = new List<AtomInteraction>();

            if (molecule1Atoms == null || molecule1AtomPositions == null || molecule1Atoms.Count != molecule1AtomPositions.Count) {
                Debug.Log("Interactions calculator, Molecule 1 atoms count and molecule 1 atom positions count don't match");
                return interactions;
            }

            if (molecule2Atoms == null || molecule2AtomPositions == null || molecule2Atoms.Count != molecule2AtomPositions.Count) {
                Debug.Log("Interactions calculator, Molecule 2 atoms count and molecule 2 atom positions count don't match");
                return interactions;
            }


            KdTree<float, int> molecule2AtomTree = new KdTree<float, int>(3, new FloatMath());

            for (int i = 0; i < molecule2Atoms.Count; i++) {

                Vector3 molecule2AtomPosition = molecule2AtomPositions[i];
                molecule2AtomTree.Add(new float[] { molecule2AtomPosition.x, molecule2AtomPosition.y, molecule2AtomPosition.z }, i);
            }

            for (int i = 0; i < molecule1Atoms.Count; i++) {

                Atom atom = molecule1Atoms[i];
                Vector3 atomPosition = molecule1AtomPositions[i];

                KdTreeNode<float, int>[] interactingAtoms = molecule2AtomTree.RadialSearch(new float[] { atomPosition.x, atomPosition.y, atomPosition.z }, MaximumBondLengths.MaximumLengthAllElements, MaxBondsPerAtom);

                foreach(KdTreeNode<float, int> node in interactingAtoms) {

                    Atom interactingAtom = molecule2Atoms[node.Value];
                    Vector3 interactingAtomPosition = molecule2AtomPositions[node.Value];

                    // get distance between atoms
                    float deltaX = interactingAtomPosition.x - atomPosition.x;
                    float deltaY = interactingAtomPosition.y - atomPosition.y;
                    float deltaZ = interactingAtomPosition.z - atomPosition.z;
                    float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                    // interaction force is -1 to +1 
                    float interactionForce = ((0.5f * MaximumBondLengths.MaximumLengthAllElements) - distance) / (0.5f * MaximumBondLengths.MaximumLengthAllElements);

                    AtomInteraction interaction = new AtomInteraction() {
                        Atom1 = atom,
                        Atom2 = interactingAtom,
                        InteractionForce = interactionForce
                    };

                    interactions.Add(interaction);
                }
            }

            return interactions;
        }
    }
}
