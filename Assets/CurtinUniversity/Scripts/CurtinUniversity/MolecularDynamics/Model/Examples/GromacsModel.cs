﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class GromacsModel {

        //static string filepath = @"D:\Molecule Examples\";
        //static string structureFile = @"protein+water.gro";
        //static string trajectoryFile = @"protein_folding.xtc";
        
        private static string filepath = @"D:\Molecule Examples\Problem Files 20170127\HIVE\";
        private static string structureFile = @"HDL.gro";
        private static string trajectoryFile = @"HDL_traj.xtc";

        public static void ParseGromacs() {

            Console.WriteLine("Parsing GRO File");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            PrimaryStructure model = GROStructureParser.GetStructure(filepath + structureFile);
            stopWatch.Stop();

            Console.WriteLine("Structure Parsing Complete [" + stopWatch.ElapsedMilliseconds + " ms]");
            Console.WriteLine("Atom Count: " + model.Atoms().Count);


            foreach (Atom atom in model.Atoms()) {
                Console.WriteLine("Atom " + atom.Index + ": " + atom.ToString());
            }

            Console.WriteLine("\n---------------------\n");

            Dictionary<Element, Dictionary<int, Atom>> atomSet = model.GetAtomsByElement();

            foreach (KeyValuePair<Element, Dictionary<int, Atom>> set in atomSet) {
                Element element = set.Key;

                Console.WriteLine("Element: " + element.ToString());
                Dictionary<int, Atom> atoms = set.Value;

                foreach (KeyValuePair<int, Atom> atom in atoms) {
                    Console.WriteLine("Atom: " + atom.Value.ToString());
                }
            }

            foreach (Chain chain in model.Chains()) {

                Console.WriteLine("\n---------------------\n");
                Console.WriteLine("Chain ID: " + chain.ID);
                Console.WriteLine("Chain Residue Count: " + chain.Residues.Count);
                foreach(Residue residue in chain.Residues) {
                    Console.WriteLine("ResidueID: " + residue.ID + ", Residue Name: " + residue.Name);
                }
            }


            stopWatch.Reset();
            stopWatch.Start();

            int NumberOfProcessorCores = 6;

            Dictionary<int, Bond> bonds = model.GenerateBonds(NumberOfProcessorCores);
            Console.WriteLine("Bond generation complete [" + stopWatch.ElapsedMilliseconds + " ms]");

            int count = 0;

            foreach (KeyValuePair<int, Bond> bond in bonds) {

                Atom atom1 = model.Atoms()[bond.Value.Atom1Index];
                Atom atom2 = model.Atoms()[bond.Value.Atom2Index];

                if ((atom1.Element == Element.O && atom2.Element == Element.H) ||
                    (atom2.Element == Element.O && atom1.Element == Element.H)) {

                    Console.WriteLine("O-H bond found. Distance: " + Vector3.Distance(atom1.Position, atom2.Position));
                    count++;
                    if (count > 5) {
                        break;
                    }
                }
            }

            Console.WriteLine("Getting frame count from trajectory file");

            stopWatch.Reset();
            stopWatch.Start();
            count = XTCTrajectoryParser.GetFrameCount(filepath + trajectoryFile);
            stopWatch.Stop();

            Console.WriteLine("Frame count complete [" + stopWatch.ElapsedMilliseconds + " ms]");
            Console.WriteLine("Frames counted: " + count);
        }
    }
}
