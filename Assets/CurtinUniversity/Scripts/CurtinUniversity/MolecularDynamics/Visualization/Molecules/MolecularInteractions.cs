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
        private MolecularInteractionsRenderer interactionsRenderer;

        public Molecule Molecule1 { get; private set; }
        public Molecule Molecule2 { get; private set; }

        public bool Active { get; private set; }

        private float reportInterval = 0f;
        private float lastReportTime = 0;

        private bool processingInteractions = false;
        private bool interactionsUpdated = false;
        private float processingTime;

        private List<AtomInteraction> interactions;
        private List<AtomInteraction> closestInteractions;
        private MolecularInteractionSettings settings;

        private void LateUpdate() {

            if(Active) {

                if (!processingInteractions) {

                    if (lastReportTime + reportInterval < Time.time) {

                        StartCoroutine(processInteractions());
                        lastReportTime = Time.time;
                    }
                }

                if(interactionsUpdated) {

                    if (settings.RenderClosestInteractionsOnly) {

                        outputInteractionResults(closestInteractions);
                        interactionsRenderer.SetInteractions(closestInteractions);
                    }
                    else {

                        outputInteractionResults(interactions);
                        interactionsRenderer.SetInteractions(interactions);
                    }
                }

                interactionsRenderer.RenderInteractions(Molecule1.MoleculeRender.transform, Molecule2.MoleculeRender.transform);
            }
        }

        public void StartMonitoring(Molecule molecule1, Molecule molecule2, MolecularInteractionSettings settings) {

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
            this.settings = settings;

            Active = true;
        }

        public void StopMonitoring() {

            Molecule1 = null;
            Molecule2 = null;
            Active = false;

            interactions = null;
            closestInteractions = null;
            interactionsUpdated = false;

            interactionsRenderer.ClearInteractions();
        }

        public void UpdateMolecularInteractionSettings(MolecularInteractionSettings settings) {
            this.settings = settings;
        }

        private IEnumerator processInteractions() {

            processingInteractions = true;

            // can't access Unity transforms in the thread
            List<Vector3> molecule1AtomPositions = getWorldPositions(Molecule1.PrimaryStructure.Atoms(), Molecule1.MoleculeRender.transform);
            List<Vector3> molecule2AtomPositions = getWorldPositions(Molecule2.PrimaryStructure.Atoms(), Molecule2.MoleculeRender.transform);

            Thread thread = new Thread(() => {

                InteractionsCalculator interactionsCalculator = new InteractionsCalculator();
                List<AtomInteraction> newInteractions = interactionsCalculator.GetAllInteractions(
                    Molecule1.PrimaryStructure.Atoms(),
                    molecule1AtomPositions,
                    Molecule2.PrimaryStructure.Atoms(),
                    molecule2AtomPositions
                );

                List<AtomInteraction> newClosestInteractions = null;
                if (settings.CalculateClosestInteractionsOnly || settings.RenderClosestInteractionsOnly) {
                    newClosestInteractions = interactionsCalculator.GetClosestInteractions(newInteractions);
                }

                if(Active) {

                    interactions = newInteractions;
                    closestInteractions = newClosestInteractions;
                    interactionsUpdated = true;
                }
            });

            thread.Start();

            while(thread.IsAlive) {
                yield return null;
            }

            processingInteractions = false;
        }

        private void outputInteractionResults(List<AtomInteraction> interactions) {

            // output to UI

            if (interactions == null || interactions.Count <= 0) {
                return;
            }

            string output = "";
            output += "Interactions calculation time: " + processingTime + "\n";

            if (interactions == null || interactions.Count <= 0) {
                output += "No molecular interactions found";
            }
            else {
                output += interactions.Count + " molecular interactions found: \n\n";
            }

            MoleculeEvents.RaiseInteractionsInformation(output);
        }

        private List<Vector3> getWorldPositions(List<Atom> atoms, Transform moleculeTransform) {

            List<Vector3> positions = new List<Vector3>();
            foreach(Atom atom in atoms) {

                Vector3 atomPosition = atom.Position;
                atomPosition.z *= -1; // flip z for left handed coordinate system 
                Vector3 transformedPosition = moleculeTransform.TransformPoint(atomPosition);
                positions.Add(transformedPosition);
            }

            return positions;
        }
    }
}
