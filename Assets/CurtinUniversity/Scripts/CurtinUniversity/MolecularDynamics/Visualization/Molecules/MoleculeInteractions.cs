using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeInteractions : MonoBehaviour {

        private Molecule molecule1;
        private Molecule molecule2;

        private bool monitoring;

        private float reportInterval = 1f;
        private float lastReportTime = 0;

        private void Update() {

            if(monitoring) {

                if(lastReportTime + reportInterval < Time.time) {

                    MoleculeEvents.RaiseInteractionsInformation("Time: " + Time.time);
                    lastReportTime = Time.time;
                }
            }
        }

        public void StopMonitoring() {

            molecule1 = null;
            molecule2 = null;
            monitoring = false;
        }

        public void StartMonitoring(Molecule molecule1, Molecule molecule2) {

            if (molecule1 != null && molecule2 != null) {

                this.molecule1 = molecule1;
                this.molecule2 = molecule2;
                monitoring = true;
            }
            else {

                if (molecule1 == null) {
                    MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. First molecule is null", true);
                }
                else {
                    MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. Second molecule is null", true);
                }
            }
        }
    }
}
