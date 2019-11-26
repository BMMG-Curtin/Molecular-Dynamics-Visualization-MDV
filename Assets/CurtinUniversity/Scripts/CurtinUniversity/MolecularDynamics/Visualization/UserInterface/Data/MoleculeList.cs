using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// The list of molecules loaded in the application.
    /// Manages settings data for molecule across all settings panels.
    /// </summary>
    public class MoleculeList : MonoBehaviour {

        private int? selectedMoleculeID;
        public int? SelectedMoleculeID {

            get {
                return selectedMoleculeID;
            }

            set {
                selectedMoleculeID = null;
                if (value.HasValue) {
                    if (molecules.ContainsKey((int)value)) {
                        selectedMoleculeID = value;
                    }
                }
            }
        }

        private SortedDictionary<int, MoleculeSettings> molecules;

        private int lastMoleculeID;

        private void Awake() {
            molecules = new SortedDictionary<int, MoleculeSettings>();
            lastMoleculeID = 0;
            SelectedMoleculeID = null;
        }

        public MoleculeSettings Add(string filePath) {

            lastMoleculeID++;
            MoleculeSettings molecule = new MoleculeSettings(lastMoleculeID, filePath);
            molecules[lastMoleculeID] = molecule;
            return molecule;
        }

        public MoleculeSettings Get(int moleculeID) {

            if(molecules.ContainsKey(moleculeID)) {
                return molecules[moleculeID];
            }
            else {
                return null;
            }
        }

        public MoleculeSettings GetSelected() {

            if (selectedMoleculeID != null && molecules.ContainsKey((int)selectedMoleculeID)) {
                return molecules[(int)selectedMoleculeID];
            }

            return null;
        }

        public void Remove(int moleculeID, bool selectNext = false) {

            if (molecules.ContainsKey(moleculeID)) {

                if (selectedMoleculeID != null && moleculeID == (int)selectedMoleculeID) {

                    if (selectNext) {
                        selectedMoleculeID = NextMoleculeID();
                    }
                    else {
                        selectedMoleculeID = null;
                    }
                }

                molecules.Remove(moleculeID);
            }
        }

        public bool Contains(int moleculeID) {
            return molecules.ContainsKey(moleculeID);
        }

        public List<int> GetIDs() {

            if (molecules != null) {

                List<int> keys = molecules.Keys.ToList();
                keys.Sort();
                return keys;
            }

            return null;
        }

        public int? SelectFirst() {

            if(molecules == null || molecules.Count == 0) {
                selectedMoleculeID = null;
                return null;
            }

            int id = molecules.Keys.ToArray()[0];
            selectedMoleculeID = id;
            return id;
        }

        public int? SelectNext() {

            selectedMoleculeID = NextMoleculeID();
            return selectedMoleculeID;
        }

        public int? NextMoleculeID() {

            if (molecules == null || molecules.Count == 0) {
                return null;
            }

            if(selectedMoleculeID == null || molecules.Count == 1) {
                return molecules.First().Key;
            }

            bool returnNext = false;

            foreach (int moleculeID in molecules.Keys) {

                if (returnNext) {
                    return moleculeID;
                }

                if (moleculeID == selectedMoleculeID) {
                    returnNext = true;
                }
            }

            return molecules.First().Key;
        }

        public override string ToString() {

            string output = "Total molecules: " + molecules.Count + "\n";
            output += "Molecule List: \n";

            foreach (int id in molecules.Keys) {
                output += "molecule ID: " + id + "\n";
            }

            return output;
        }
    }
}
