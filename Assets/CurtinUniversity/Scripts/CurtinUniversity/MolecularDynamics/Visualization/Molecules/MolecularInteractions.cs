using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private MolecularInteractionSettings interactionSettings;

        private List<Atom> molecule1RenderedAtoms;
        private List<Atom> molecule2RenderedAtoms;

        private void LateUpdate() {

            if(Active) {

                if (!processingInteractions) {

                    if (lastReportTime + reportInterval < Time.time) {

                        StartCoroutine(processInteractions());
                        lastReportTime = Time.time;
                    }
                }

                if(interactionsUpdated) {

                    if (interactionSettings.RenderClosestInteractionsOnly) {

                        outputInteractionResults(closestInteractions);
                        interactionsRenderer.SetInteractions(closestInteractions);
                    }
                    else {

                        outputInteractionResults(interactions);
                        interactionsRenderer.SetInteractions(interactions);
                    }

                    if (interactionSettings.HighlightInteracingAtoms) {
                        interactionsRenderer.RenderAtomHighlights();
                    }
                }

                if (interactionSettings.RenderInteractionLines) {
                    interactionsRenderer.RenderInteractionLines();
                }
            }
        }

        public void StartMonitoring(Molecule molecule1, Molecule molecule2, MolecularInteractionSettings interactionSettings, MoleculeRenderSettings molecule1Settings, MoleculeRenderSettings molecule2Settings) {

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
            this.interactionSettings = interactionSettings;

            SetMolecule1RenderSettings(molecule1Settings);
            SetMolecule2RenderSettings(molecule2Settings);

            interactionsRenderer.Molecule1 = molecule1;
            interactionsRenderer.Molecule2 = molecule2;

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

            interactionsRenderer.Molecule1 = null;
            interactionsRenderer.Molecule2 = null;
        }

        public void SetMolecularInteractionSettings(MolecularInteractionSettings settings) {

            this.interactionSettings = settings;

            if(!interactionSettings.HighlightInteracingAtoms) {
                interactionsRenderer.ClearAtomHighlights();
            }

            if (!interactionSettings.RenderInteractionLines) {
                interactionsRenderer.ClearInteractionLines();
            }
        }

        public void SetMolecule1RenderSettings(MoleculeRenderSettings molecule1Settings) {

            if (!molecule1Settings.ShowPrimaryStructure) {
                this.molecule1RenderedAtoms = new List<Atom>();
            }
            else {
                this.molecule1RenderedAtoms = Molecule1.PrimaryStructure.GetAtoms(molecule1Settings.ShowStandardResidues, molecule1Settings.ShowNonStandardResidues, molecule1Settings.EnabledElements, molecule1Settings.EnabledResidueNames, molecule1Settings.EnabledResidueIDs).Values.ToList();
            }
        }

        public void SetMolecule2RenderSettings(MoleculeRenderSettings molecule2Settings) {
            if (!molecule2Settings.ShowPrimaryStructure) {
                this.molecule2RenderedAtoms = new List<Atom>();
            }
            else {
                this.molecule2RenderedAtoms = Molecule2.PrimaryStructure.GetAtoms(molecule2Settings.ShowStandardResidues, molecule2Settings.ShowNonStandardResidues, molecule2Settings.EnabledElements, molecule2Settings.EnabledResidueNames, molecule2Settings.EnabledResidueIDs).Values.ToList();
            }
        }

        private IEnumerator processInteractions() {

            processingInteractions = true;

            // can't access Unity transforms in the thread
            List<Vector3> molecule1AtomPositions = getWorldPositions(molecule1RenderedAtoms, Molecule1.MoleculeRender.transform);
            List<Vector3> molecule2AtomPositions = getWorldPositions(molecule2RenderedAtoms, Molecule2.MoleculeRender.transform);

            Thread thread = new Thread(() => {

                InteractionsCalculator interactionsCalculator = new InteractionsCalculator();
                List<AtomInteraction> newInteractions = interactionsCalculator.GetAllInteractions(
                    molecule1RenderedAtoms,
                    molecule1AtomPositions,
                    molecule2RenderedAtoms,
                    molecule2AtomPositions
                );

                List<AtomInteraction> newClosestInteractions = null;
                if (interactionSettings.CalculateClosestInteractionsOnly || interactionSettings.RenderClosestInteractionsOnly) {
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

            if (interactions == null || interactions.Count <= 0) {
                output += "No molecular interactions found";
            }
            else {
                output += interactions.Count + " molecular interactions found";
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
