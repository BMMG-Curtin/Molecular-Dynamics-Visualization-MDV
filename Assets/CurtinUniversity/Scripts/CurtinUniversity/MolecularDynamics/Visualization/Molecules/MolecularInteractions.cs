using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using UnityEngine;


using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractions : MonoBehaviour {

        [SerializeField]
        private MolecularInteractionsRenderer renderer;

        public Molecule Molecule1 { get; private set; }
        public Molecule Molecule2 { get; private set; }

        public bool Active { get; private set; }

        private float reportInterval = 0.1f;
        private float lastReportTime = 0;

        private bool processingInteractions = false;

        Vector3 molecule1Position;
        Vector3 molecule2Position;
        List<Vector3> molecule1AtomPositions;
        List<Vector3> molecule2AtomPositions;

        List<AtomInteraction> interactions;
        private float processingTime;

        private void Update() {

            if(Active && !processingInteractions) {

                if(lastReportTime + reportInterval < Time.time) {

                    StartCoroutine(processInteractions());
                    lastReportTime = Time.time;
                }
            }
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
        }

        public void StopMonitoring() {

            Molecule1 = null;
            Molecule2 = null;
            Active = false;
        }

        private IEnumerator processInteractions() {

            processingInteractions = true;

            // can't process Unity transforms in the thread
            molecule1Position = Molecule1.MoleculeRender.transform.position;
            molecule2Position = Molecule2.MoleculeRender.transform.position;
            molecule1AtomPositions = getWorldPositions(Molecule1.PrimaryStructure.Atoms(), Molecule1.MoleculeRender.transform);
            molecule2AtomPositions = getWorldPositions(Molecule2.PrimaryStructure.Atoms(), Molecule2.MoleculeRender.transform);

            Thread newThread = new Thread(processInteractionsThread);
            newThread.Start();

            while(newThread.IsAlive) {
                yield return null;
            }

            outputInteractionResults();

            processingInteractions = false;
        }

        private void processInteractionsThread() {

            Stopwatch watch = new Stopwatch();
            watch.Start();

            InteractionsCalculator interactionsCalculator = new InteractionsCalculator();
            interactions = interactionsCalculator.GetInteractions(
                Molecule1.PrimaryStructure.Atoms(), 
                molecule1AtomPositions, 
                Molecule2.PrimaryStructure.Atoms(), 
                molecule2AtomPositions
            );

            watch.Stop();
            processingTime = watch.ElapsedMilliseconds;
        }

        private void outputInteractionResults() {

            // renderer output
            renderer.ClearAtoms();
            renderer.ShowInteractingMolecule(molecule1Position, molecule1AtomPositions);

            // debug output to UI

            string output = "";

            output += "Molecule1 position: " + printPosition(molecule1Position) + "\n";
            output += "Molecule2 position: " + printPosition(molecule2Position) + "\n";
            output += "Processing time: " + processingTime + "\n";

            if (interactions == null || interactions.Count <= 0) {
                output += "No molecular interactions found";
            }
            else {

                output += interactions.Count + " molecular interactions found: \n\n";

                //foreach (AtomInteraction interaction in interactions) {
                //    output += interaction.ToString() + "\n";
                //}
            }

            MoleculeEvents.RaiseInteractionsInformation(output);
        }

        private List<Vector3> getWorldPositions(List<Atom> atoms, Transform moleculeTransform) {

            List<Vector3> positions = new List<Vector3>();
            foreach(Atom atom in atoms) {

                Vector3 atomPosition = atom.Position;
                atomPosition.z *= -1; // flip z for Unity 
                Vector3 transformedPosition = moleculeTransform.TransformPoint(atomPosition);
                positions.Add(transformedPosition);

                //UnityEngine.Debug.Log("molecule: " + moleculeTransform.position + ", atom: " + atom.Position  + ", transformed: " + transformedPosition);
            }

            return positions;
        }

        private string printPosition(Vector3 position) {

            return "x: " + position.x.ToString("n3").PadRight(8) +
                " y: " + position.y.ToString("n3").PadRight(8) +
                " z: " + position.z.ToString("n3").PadRight(8);
        }
    }
}
