using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

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

        private Dictionary<int, MoleculeSettings> molecules;

        private int lastMoleculeID;

        private void Awake() {
            molecules = new Dictionary<int, MoleculeSettings>();
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

        public void Remove(int moleculeID) {

            if(molecules.ContainsKey(moleculeID)) {
                molecules.Remove(moleculeID);
            }

            if(selectedMoleculeID != null && moleculeID == (int)selectedMoleculeID) {
                selectedMoleculeID = null;
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
    }
}
