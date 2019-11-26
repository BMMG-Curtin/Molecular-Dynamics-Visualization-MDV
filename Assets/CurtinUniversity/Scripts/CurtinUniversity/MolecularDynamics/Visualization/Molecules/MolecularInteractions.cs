using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// Manages the calulation of molecule interactions and the rendering of the calculation results
    /// </summary>
    public class MolecularInteractions : MonoBehaviour {

        [SerializeField]
        private MolecularInteractionsRenderer interactionsRenderer;

        [SerializeField]
        private Gradient repulsiveGradient;

        [SerializeField]
        private Gradient strongAttractiveGradient;

        [SerializeField]
        private Gradient weakAttractiveGradient;

        public Molecule Molecule1 { get; private set; }
        public Molecule Molecule2 { get; private set; }

        public bool Active { get; private set; }

        private float reportInterval = 0.1f;
        private float lastReportTime = 0;

        private bool processingInteractions = false;
        private bool interactionsUpdated = false;
        private float processingTime;

        private List<AtomInteraction> interactions;
        private MolecularInteractionSettings interactionSettings;

        private List<Atom> molecule1RenderedAtoms;
        private List<Atom> molecule2RenderedAtoms;

        private Thread processingThread;

        private void Awake() {
            interactionSettings = MolecularInteractionSettings.Default();
        }

        // While calculations are set to 'Active' interactions are calculated constantly
        // Rendering of atom hhighlights is done every time interaction calculations are completed
        // Rendering of interaction lines is done every frame
        private void LateUpdate() {

            if(Active) {

                if (!processingInteractions) {

                    if (lastReportTime + reportInterval < Time.time) {

                        StartCoroutine(processInteractions());
                        lastReportTime = Time.time;
                    }
                }

                if(interactionsUpdated) {

                    outputInteractionResults(interactions);
                    interactionsRenderer.SetInteractions(interactions);

                    if (interactionSettings.HighlightInteracingAtoms) {
                        interactionsRenderer.RenderAtomHighlights(interactionSettings);
                    }
                }

                if (interactionSettings.RenderInteractionLines) {
                    interactionsRenderer.RenderInteractionLines(interactionSettings);
                }
            }
        }

        public void StartMonitoring(Molecule molecule1, Molecule molecule2, MolecularInteractionSettings interactionSettings, MoleculeRenderSettings molecule1Settings, MoleculeRenderSettings molecule2Settings) {

            UnityEngine.Debug.Log("Start monitoring " + Time.time);

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

            reportSigmaEpsilonValueDefaults(molecule1, molecule2);

            Active = true;
        }

        public void StopMonitoring() {

            UnityEngine.Debug.Log("Stop monitoring " + Time.time);

            if (processingInteractions) {

                processingThread.Abort();
                processingThread = null;
                processingInteractions = false;
            }

            Molecule1 = null;
            Molecule2 = null;
            Active = false;

            interactions = null;
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

            UnityEngine.Debug.Log("Molecule1Interaction Settings " + molecule1Settings);

            if (!molecule1Settings.ShowPrimaryStructure) {
                this.molecule1RenderedAtoms = new List<Atom>();
                UnityEngine.Debug.Log("Interactions calc. Molecule1 Atom empty list");
            }
            else {
                this.molecule1RenderedAtoms = Molecule1.PrimaryStructure.GetAtoms(molecule1Settings.ShowStandardResidues, molecule1Settings.ShowNonStandardResidues, molecule1Settings.EnabledElements, molecule1Settings.EnabledResidueNames, molecule1Settings.EnabledResidueIDs).Values.ToList();
                UnityEngine.Debug.Log(Time.time + " Interactions calc. Molecule1 Atom count: " + this.molecule1RenderedAtoms.Count);
            }
        }

        public void SetMolecule2RenderSettings(MoleculeRenderSettings molecule2Settings) {

            UnityEngine.Debug.Log("Molecule2Interaction Settings " + molecule2Settings);

            if (!molecule2Settings.ShowPrimaryStructure) {
                this.molecule2RenderedAtoms = new List<Atom>();
                UnityEngine.Debug.Log("Interactions calc. Molecule2 Atom empty list");
            }
            else {
                this.molecule2RenderedAtoms = Molecule2.PrimaryStructure.GetAtoms(molecule2Settings.ShowStandardResidues, molecule2Settings.ShowNonStandardResidues, molecule2Settings.EnabledElements, molecule2Settings.EnabledResidueNames, molecule2Settings.EnabledResidueIDs).Values.ToList();
                UnityEngine.Debug.Log(Time.time + " Interactions calc. Molecule2 Atom count: " + this.molecule2RenderedAtoms.Count);
            }
        }

        // Process the interactions in a thread. Thread can be stopped by the StopMonitoring method
        private IEnumerator processInteractions() {

            processingInteractions = true;

            // can't access Unity transforms in the thread
            List<Vector3> molecule1AtomPositions = getWorldPositions(molecule1RenderedAtoms, Molecule1.MoleculeRender.transform);
            List<Vector3> molecule2AtomPositions = getWorldPositions(molecule2RenderedAtoms, Molecule2.MoleculeRender.transform);

            processingThread = new Thread(() => {

                InteractionsCalculator interactionsCalculator = new InteractionsCalculator();

                List<AtomInteraction> newInteractions = interactionsCalculator.GetAllInteractions(
                    molecule1RenderedAtoms,
                    molecule1AtomPositions,
                    molecule2RenderedAtoms,
                    molecule2AtomPositions,
                    repulsiveGradient, 
                    strongAttractiveGradient, 
                    weakAttractiveGradient
                );

                if (Active) {

                    interactions = newInteractions;
                    interactionsUpdated = true;
                }
            });

            processingThread.Start();

            // StopMonitoring method can kill thread and make reference null
            while(processingThread != null && processingThread.IsAlive) {
                yield return null;
            }

            processingThread = null;
            processingInteractions = false;
        }

        // Exports inetaction results as an info class. Used to report interactions data in the UI
        private void outputInteractionResults(List<AtomInteraction> interactions) {

            MolecularInteractionsInformation info = new MolecularInteractionsInformation();

            foreach(AtomInteraction interaction in interactions) {

                if (interaction.InteractionType != null) {

                    if (interaction.LennardJonesPotential != null && interaction.LennardJonesPotential != 0) {

                        info.SummedInteractionEnergy += (double)interaction.LennardJonesPotential;
                        info.SummedLennardJonesEnergy += (double)interaction.LennardJonesPotential;

                        if ((double)interaction.LennardJonesPotential >= 0) {
                            info.SummedRepulsionForce += (double)interaction.LennardJonesPotential;
                            info.SummedLennardJonesRepulsionEnergy += (double)interaction.LennardJonesPotential;
                        }
                        else {
                            info.SummedAttractionForce += (double)interaction.LennardJonesPotential;
                            info.SummedLennardJonesAttractionEnergy += (double)interaction.LennardJonesPotential;
                        }
                    }

                    if (interaction.ElectrostaticEnergy != null && interaction.ElectrostaticEnergy != 0) {

                        info.SummedInteractionEnergy += (double)interaction.ElectrostaticEnergy;
                        info.SummedElectrostaticForce += (double)interaction.ElectrostaticEnergy;

                        if ((double)interaction.ElectrostaticEnergy >= 0) {
                            //info.SummedRepulsionForce += (double)interaction.ElectrostaticForce;
                            info.SummedElectrostaticRepulsionForce += (double)interaction.ElectrostaticEnergy;
                        }
                        else {
                            //info.SummedAttractionForce += (double)interaction.ElectrostaticForce;
                            info.SummedElectrostaticAttractionForce += (double)interaction.ElectrostaticEnergy;
                        }
                    }

                    info.TotalInteractions++;
                    if(interaction.InteractionType == InteractionType.Attractive) {
                        info.TotalAttractiveInteractions++;
                    }
                    else if(interaction.InteractionType == InteractionType.Stable) {
                        info.TotalStableInteractions++;
                    }
                    else {
                        info.TotalRepulsiveInteractions++;
                    }
                }
            }

            MoleculeEvents.RaiseInteractionsInformation(info);
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

        // This method return whether default sigma and epsilson values are used in the interaction energy caluclations
        private void reportSigmaEpsilonValueDefaults(Molecule molecule1, Molecule molecule2) {

            foreach(Atom atom in molecule1.PrimaryStructure.Atoms()) {

                if(InteractionForces.GetAtomSigmaEpsilonNanometres(atom).IsDefault) {

                    MoleculeEvents.RaiseInteractionsMessage("Sigma epsilon values for " + atom.Element + " atom not found. Using default values", true);
                    return;
                }
            }

            foreach (Atom atom in molecule2.PrimaryStructure.Atoms()) {

                if (InteractionForces.GetAtomSigmaEpsilonNanometres(atom).IsDefault) {

                    MoleculeEvents.RaiseInteractionsMessage("Sigma epsilon values for " + atom.Element + " atom not found. Using default values", true);
                    return;
                }
            }
        }
    }
}
