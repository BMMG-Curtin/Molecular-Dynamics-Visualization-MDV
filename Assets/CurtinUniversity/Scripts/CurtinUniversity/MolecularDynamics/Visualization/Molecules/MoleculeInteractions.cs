using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeInteractions : MonoBehaviour {

        private Molecule molecule1;
        private Molecule molecule2;

        private bool monitoring;

        private float reportInterval = 0.05f;
        private float lastReportTime = 0;

        private void Update() {

            if(monitoring) {

                if(lastReportTime + reportInterval < Time.time) {
                    MoleculeEvents.RaiseInteractionsInformation(interactions());
                    lastReportTime = Time.time;
                }
            }
        }

        private string interactions() {

            string output = "";

            output += "Molecule1 position: " + gsetPosition(molecule1.transform) + "\n";
            output += "Molecule2 position: " + gsetPosition(molecule2.transform) + "\n";

            return output;
        }

        private string gsetPosition(Transform transform) {


            return "x: " + transform.position.x.ToString("n3").PadRight(8) +
                " y: " + transform.position.y.ToString("n3").PadRight(8) +
                " z: " + transform.position.z.ToString("n3").PadRight(8);
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
