using System;
using System.Collections.Generic;

using CurtinUniversity.MolecularDynamics.Model.Model;
using CurtinUniversity.MolecularDynamics.Model.Definitions;

using CurtinUniversity.MolecularDynamics.Model.DataStructures.KdTree;
using CurtinUniversity.MolecularDynamics.Model.DataStructures.KdTree.Math;

using System.Diagnostics;

namespace CurtinUniversity.MolecularDynamics.Model.Utility {

    public static class BondCalculator {

        // default value for hydrogen simple calculations, 
        // can be overriden in dictionary below
        public const float MaximumLengthHydrogen = 0.11f;

        // this will limit the tree search to improve performance. 
        // Need to experiment with higher values to see performance impact
        public const int MaxBondsPerAtom = 5;

        // Calculate bonds between the specified set of atoms based on the distance between bond atoms.
        // 
        // This uses a KDTree internally and is relatively fast.
        // Performance could be further improved by not searching for bonds between protein residues except for the main chain atoms.
        // 
        // Uses a lookup table to retrieve element to element bond length maximums.
        // Is values not found in lookup table it falls back to a default max bond distance of 1.9 Ångstroms for all atoms except hydrogen which is 1.1 Ångstroms 
        // http://proteopedia.org/wiki/index.php/Atomic_coordinate_file
        // 
        public static Dictionary<int, Bond> CalculateBonds(Dictionary<int, Atom> atoms) {
            return CalculateBonds(atoms, MaximumBondLengths.Lengths);
        }

        // same as above method but allows for custom max bond lengths
        public static Dictionary<int, Bond> CalculateBonds(Dictionary<int, Atom> atoms, Dictionary<ElementPair, float> maxBondLengths) {

            UnityEngine.Debug.Log("Generating Atom Tree");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Dictionary<int, Bond> bonds = new Dictionary<int, Bond>();
            KdTree<float, int> tree = new KdTree<float, int>(3, new FloatMath());

            foreach (KeyValuePair<int, Atom> atom in atoms) {
                tree.Add(new float[] { atom.Value.Position.x, atom.Value.Position.y, atom.Value.Position.z }, atom.Key);
            }

            watch.Stop();
            UnityEngine.Debug.Log("Generated Atom Tree: " + watch.ElapsedMilliseconds.ToString("N2"));
            watch.Reset();
            watch.Start();

            // Iterative tree search will find duplicate bonds if the atoms at the search position aren't removed. 
            // Tree removal is more expensive than keeping a seperate collection and checking if found bond already in collection
            // HashSet<long> addedBonds = new HashSet<long>();
            HashSet<string> addedBonds = new HashSet<string>();

            int bondID = 0;

            KdTreeNode<float, int>[] bondAtoms = null;

            // for each atom find all linked atoms 
            foreach (int atomKey in atoms.Keys) {

                Atom atom = atoms[atomKey];

                bondAtoms = tree.RadialSearch(new float[] { atom.Position.x, atom.Position.y, atom.Position.z }, MaximumBondLengths.MaximumLengthAllElements, MaxBondsPerAtom);

                foreach (KdTreeNode<float, int> bondAtomNode in bondAtoms) {

                    // atom can't bond to itself
                    // (tree search always finds the supplied atom as well since it's only searching from a position, not an atom)
                    if (atomKey == bondAtomNode.Value) {
                        continue;
                    }

                    string bondKey = getBondKey(atom.Index, bondAtomNode.Value);

                    // check it hasn't been added previously (i.e. bond in reverse). 
                    if (!addedBonds.Contains(bondKey)) {

                        Atom bondAtom = atoms[bondAtomNode.Value];

                        // get the maximum bond length for this bond

                        // first try a lookup
                        float maxBondLength;
                        if (!maxBondLengths.TryGetValue(new ElementPair(atom.Element, bondAtom.Element), out maxBondLength)) {

                            // if lookup fails use defaults 
                            if (atom.Element == ChemicalElement.H || bondAtom.Element == ChemicalElement.H) {
                                maxBondLength = MaximumLengthHydrogen;
                            }
                            else {
                                maxBondLength = MaximumBondLengths.MaximumLengthAllElements;
                            }
                        }

                        //if (maxBondLength != MaximumBondLengths.MaximumLengthAllElements) {
                        //    UnityEngine.Debug.Log("MaxBondLength [" + atom.Element + ", " + bondAtom.Element + "]" + maxBondLength);
                        //}

                        // if the maxBondLength is lower than the search radius in the KDTree search
                        // then calculate the bond length between the atoms and check it doesn't exceed maxBondLength
                        if (maxBondLength < MaximumBondLengths.MaximumLengthAllElements) {
                            if (atomDistance(atom, bondAtom) > maxBondLength) {
                                continue;
                            }
                        }

                        // add to return value
                        bonds.Add(bondID, new Bond(atomKey, bondAtomNode.Value));

                        // add to lookup checklist
                        addedBonds.Add(bondKey);

                        bondID++;
                    }
                }
            }

            watch.Stop();
            UnityEngine.Debug.Log("Generated Bond Results: " + watch.ElapsedMilliseconds.ToString("N2"));
            watch.Reset();

            return bonds;
        }

        private static float atomDistance(Atom atom1, Atom atom2) {

            float deltaX = atom2.Position.x - atom1.Position.x;
            float deltaY = atom2.Position.y - atom1.Position.y;
            float deltaZ = atom2.Position.z - atom1.Position.z;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        private static string getBondKey(int atomID1, int atomID2) {

            if (atomID1 < atomID2) {
                return atomID1 + "|" + atomID2;
            }
            else {
                return atomID2 + "|" + atomID1;
            }
        }
    }
}
