using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeInteractions : MonoBehaviour {

        public Molecule Molecule1 { get; private set; }
        public Molecule Molecule2 { get; private set; }

        public bool Active { get; private set; }

        private float reportInterval = 0.05f;
        private float lastReportTime = 0;

        private void Update() {

            if(Active) {

                if(lastReportTime + reportInterval < Time.time) {
                    MoleculeEvents.RaiseInteractionsInformation(interactions());
                    lastReportTime = Time.time;
                }
            }
        }

        private string interactions() {

            string output = "";

            output += "Molecule1 position: " + gsetPosition(Molecule1.transform) + "\n";
            output += "Molecule2 position: " + gsetPosition(Molecule2.transform) + "\n";

            return output;
        }

        private string gsetPosition(Transform transform) {


            return "x: " + transform.position.x.ToString("n3").PadRight(8) +
                " y: " + transform.position.y.ToString("n3").PadRight(8) +
                " z: " + transform.position.z.ToString("n3").PadRight(8);
        }

        public void StopMonitoring() {

            Molecule1 = null;
            Molecule2 = null;
            Active = false;
        }

        public void StartMonitoring(Molecule molecule1, Molecule molecule2) {

            if(molecule1 == null) {
                MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. First molecule is null.", true);
                return;
            }

            if (molecule2 == null) {
                MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. Second molecule is null.", true);
                return;
            }

            if (molecule1.PrimaryStructureTrajectory != null || molecule2.PrimaryStructureTrajectory != null) {
                MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. Monitored molecules cannot have trajectories loaded.", true);
                return;
            }

            this.Molecule1 = molecule1;
            this.Molecule2 = molecule2;
            Active = true;
            //MoleculeEvents.RaiseInteractionsMessage("Started monitoring molecular interactions", false);
        }
    }
}
