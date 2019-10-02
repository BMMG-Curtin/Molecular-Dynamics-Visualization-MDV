using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class BondCalculator {

        private Dictionary<int, Atom> atoms;
        private Dictionary<ElementPair, float> maxBondLengths;

        private Dictionary<int, Bond> bonds;
        private KdTree<float, int> tree;

        // Iterative tree search will find duplicate bonds if the atoms at the search position aren't removed. 
        // Tree removal is more expensive than keeping a seperate collection and checking if found bond already in collection
        private HashSet<string> addedBonds;
        private int bondID = 0;

        private readonly object bondAddLock = new object();

        // Calculate bonds between the specified set of atoms based on the distance between bond atoms.
        // 
        // This uses a KDTree internally and is relatively fast.
        // Performance could be further improved by not searching for bonds between protein residues except for the main chain atoms.
        // 
        // Uses a lookup table to retrieve element to element bond length maximums.
        // If values not found in lookup table it falls back to a default max bond distance of 1.9 Ångstroms for all atoms except hydrogen which is 1.1 Ångstroms 
        // http://proteopedia.org/wiki/index.php/Atomic_coordinate_file
        // 
        public Dictionary<int, Bond> CalculateBonds(Dictionary<int, Atom> atoms, int processorCores) {
            return CalculateBonds(atoms, BondLengths.Lengths, processorCores);
        }

        // same as above method but allows for custom max bond lengths
        public Dictionary<int, Bond> CalculateBonds(Dictionary<int, Atom> atoms, Dictionary<ElementPair, float> maxBondLengths, int processorCores) {

            this.atoms = atoms;
            this.maxBondLengths = maxBondLengths;

            bonds = new Dictionary<int, Bond>();
            tree = new KdTree<float, int>(3, new FloatMath());
            addedBonds = new HashSet<string>();
            bondID = 0;

            UnityEngine.Debug.Log("Generating Atom Tree");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            foreach (KeyValuePair<int, Atom> atom in atoms) {
                tree.Add(new float[] { atom.Value.Position.x, atom.Value.Position.y, atom.Value.Position.z }, atom.Key);
            }

            watch.Stop();
            UnityEngine.Debug.Log("Generated Atom Tree: " + watch.ElapsedMilliseconds.ToString("N2"));
            watch.Reset();
            watch.Start();

            //for each atom find all linked atoms

            List<Thread> threadList = new List<Thread>();
            List<int> atomIndexes = atoms.Keys.ToList();
            int threadCount = processorCores - 1;
            if (threadCount <= 0) {
                threadCount = 1;
            }

            int maxAtomsPerThread = Mathf.Max(1, atomIndexes.Count / threadCount);

            for (int i = 0; i < atomIndexes.Count; i += maxAtomsPerThread) {

                int threadAtomCount = maxAtomsPerThread;
                if (i > atomIndexes.Count - threadAtomCount - 1) {
                    threadAtomCount = atomIndexes.Count - i - 1;
                }

                List<int> threadAtoms = atomIndexes.GetRange(i, threadAtomCount);

                Thread newThread = new Thread(findBonds);
                newThread.Start(threadAtoms);
                threadList.Add(newThread);
            }

            foreach (var thread in threadList) {
                thread.Join();
            }

            watch.Stop();
            UnityEngine.Debug.Log("Generated Bond Results: " + watch.ElapsedMilliseconds.ToString("N2"));
            watch.Reset();

            return bonds.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        private void findBonds(object argument) {

            List<int> threadAtoms = (List<int>)argument;

            foreach (int atomIndex in threadAtoms) {

                Atom atom = atoms[atomIndex];

                KdTreeNode<float, int>[] bondAtoms = tree.RadialSearch(new float[] { atom.Position.x, atom.Position.y, atom.Position.z }, BondLengths.MaximumLengthAllElements, BondLengths.MaxBondsPerAtom);

                foreach (KdTreeNode<float, int> bondAtomNode in bondAtoms) {

                    // atom can't bond to itself
                    // (tree search always finds the supplied atom as well since it's only searching from a position, not an atom)
                    if (atomIndex == bondAtomNode.Value) {
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
                            if (atom.Element == Element.H || bondAtom.Element == Element.H) {
                                maxBondLength = BondLengths.MaximumLengthHydrogen;
                            }
                            else {
                                maxBondLength = BondLengths.MaximumLengthAllElements;
                            }
                        }

                        // if the maxBondLength is lower than the search radius in the KDTree search
                        // then calculate the bond length between the atoms and check it doesn't exceed maxBondLength
                        if (maxBondLength < BondLengths.MaximumLengthAllElements) {
                            if (atomDistance(atom, bondAtom) > maxBondLength) {
                                continue;
                            }
                        }

                        lock (bondAddLock) {
                            bonds.Add(++bondID, new Bond(atomIndex, bondAtomNode.Value));
                            addedBonds.Add(bondKey);
                            //addBond(atom.Index, bondAtomNode.Value);
                        }
                    }
                }
            }
        }

        private float atomDistance(Atom atom1, Atom atom2) {

            float deltaX = atom2.Position.x - atom1.Position.x;
            float deltaY = atom2.Position.y - atom1.Position.y;
            float deltaZ = atom2.Position.z - atom1.Position.z;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        private string getBondKey(int atom1, int atom2) {

            string hash;

            if (atom1 < atom2) {
                hash = atom1 + "|" + atom2;
            }
            else {
                hash = atom2 + "|" + atom1;
            }

            return hash;
        }
    }
}

