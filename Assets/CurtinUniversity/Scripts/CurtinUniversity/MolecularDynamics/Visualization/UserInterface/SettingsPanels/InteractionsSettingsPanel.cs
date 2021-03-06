﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class InteractionsSettingsPanel : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI selectedMoleculeText;

        [SerializeField]
        private Toggle highlightInteractingAtomsToggle;

        [SerializeField]
        private Toggle renderInteractionLinesToggle;

        [SerializeField]
        private Toggle renderAttractiveInteractionsToggle;

        [SerializeField]
        private Toggle renderStableInteractionsToggle;

        [SerializeField]
        private Toggle renderRepulsiveInteractionsToggle;

        [SerializeField]
        private TextMeshProUGUI StartStopButtonText;

        [SerializeField]
        private Button ResetPositionsButton;

        [SerializeField]
        private TextMeshProUGUI InteractionScoreText;

        [SerializeField]
        private TextMeshProUGUI InformationText;

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private MessageConsole console;

        private MolecularInteractionSettings interactionSettings;

        private MoleculeSettings selectedMolecule;

        public bool MonitoringEnabled { get; private set; }

        public void Awake() {

            InteractionScoreText.text = "";
            InformationText.text = "";

            MonitoringEnabled = false;
            ResetPositionsButton.interactable = false;

            interactionSettings = MolecularInteractionSettings.Default();

            highlightInteractingAtomsToggle.isOn = interactionSettings.HighlightInteracingAtoms;
            renderInteractionLinesToggle.isOn = interactionSettings.RenderInteractionLines;
            renderAttractiveInteractionsToggle.isOn = interactionSettings.ShowAttractiveInteractions;
            renderStableInteractionsToggle.isOn = interactionSettings.ShowStableInteractions;
            renderRepulsiveInteractionsToggle.isOn = interactionSettings.ShowRepulsiveInteractions;
        }

        private void OnEnable() {
            UpdateSelectedMolecule();
        }

        public void UpdateSelectedMolecule() {

            if (isActiveAndEnabled) {

                selectedMolecule = molecules.GetSelected();

                if (selectedMolecule != null) {
                    selectedMoleculeText.text = "Selected molecule - " + selectedMolecule.FileName;
                }
                else {
                    selectedMoleculeText.text = "< no molecule selected >";
                }
            }
        }

        public void OnHighlightInteractingAtomsToggle() {

            interactionSettings.HighlightInteracingAtoms = highlightInteractingAtomsToggle.isOn;
            UserInterfaceEvents.RaiseMolecularInteractionSettingsUpdated(interactionSettings);
        }

        public void OnRenderInteractionLinesToggle() {

            interactionSettings.RenderInteractionLines = renderInteractionLinesToggle.isOn;
            UserInterfaceEvents.RaiseMolecularInteractionSettingsUpdated(interactionSettings);
        }

        public void OnRenderAttractiveInteractionsToggle() {

            interactionSettings.ShowAttractiveInteractions = renderAttractiveInteractionsToggle.isOn;
            UserInterfaceEvents.RaiseMolecularInteractionSettingsUpdated(interactionSettings);
        }

        public void OnRenderStableInteractionsToggle() {

            interactionSettings.ShowStableInteractions = renderStableInteractionsToggle.isOn;
            UserInterfaceEvents.RaiseMolecularInteractionSettingsUpdated(interactionSettings);
        }

        public void OnRenderRepulsiveInteractionsToggle() {

            interactionSettings.ShowRepulsiveInteractions = renderRepulsiveInteractionsToggle.isOn;
            UserInterfaceEvents.RaiseMolecularInteractionSettingsUpdated(interactionSettings);
        }

        public void ShowInformation(MolecularInteractionsInformation information) {

            InteractionScoreText.text = getForceString(information.SummedInteractionEnergy) + " kJ/mol";

            InformationText.text = "Attraction Energy: " + getForceString(information.SummedAttractionForce) + "\n" +
                "Repulsion Energy: " + getForceString(information.SummedRepulsionForce) + "\n\n" +

                "Stable Interactions: " + information.TotalStableInteractions + "\n" +
                "Attractive Interactions: " + information.TotalAttractiveInteractions + "\n" +
                "Repulsive Interactions: " + information.TotalRepulsiveInteractions + "\n" +
                "Total Interactions: " + information.TotalInteractions + "\n\n" +

                "VDW Energy: " + getForceString(information.SummedLennardJonesEnergy) + "\n" +
                "VDW Attraction: " + getForceString(information.SummedLennardJonesAttractionEnergy) + "\n" +
                "VDW Repulsion: " + getForceString(information.SummedLennardJonesRepulsionEnergy) + "\n\n" +

                "Electrostatic Energy: " + getForceString(information.SummedElectrostaticForce) + "\n" +
                "Electrostatic Attraction: " + getForceString(information.SummedElectrostaticAttractionForce) + "\n" +
                "Electrostatic Repulsion: " + getForceString(information.SummedElectrostaticRepulsionForce); // + "\n" + 

                //"\nDebug Info: " + information.DebugString + "\n";
        }

        private string getForceString(double force) {

            if (force > 100000) {
                return force.ToString("0.00000e0");
            }
            else {
                return force.ToString("N2");
            }
        }

        public void StopInteractions() {
            stopInteracations();
        }

        public void OnStartStopButton() {

            if (MonitoringEnabled) {
                stopInteracations();
            }
            else {
                startInteractions();
            }
        }

        public void OnResetMoleculePositions() {

            foreach(int moleculeID in molecules.GetIDs()) {
                UserInterfaceEvents.RaiseResetMoleculeTransform(moleculeID);
            }
        }

        private void startInteractions() {

            List<int> moleculeIDs = molecules.GetIDs();

            if (moleculeIDs.Count != 2) {

                if (moleculeIDs.Count > 2) {
                    console.ShowError("Can't monitor molecule interactions. Only two molecules can to be loaded");
                }
                else {
                    console.ShowError("Can't monitor molecule interactions. At least two molecules need to be loaded");
                }

                return;
            }

            MoleculeRenderSettings molecule1Settings = molecules.Get(moleculeIDs[0]).RenderSettings;
            MoleculeRenderSettings molecule2Settings = molecules.Get(moleculeIDs[1]).RenderSettings;

            if (molecules.Get(moleculeIDs[0]).HasTrajectory || molecules.Get(moleculeIDs[1]).HasTrajectory) {

                MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. Monitored molecules cannot have trajectories loaded.", true);
                return;
            }

            MonitoringEnabled = true;
            StartStopButtonText.text = "Stop Monitoring Interactions";
            UserInterfaceEvents.RaiseStartMonitoringMoleculeInteractions(moleculeIDs[0], moleculeIDs[1], interactionSettings, molecule1Settings, molecule2Settings);

            ResetPositionsButton.interactable = true;
        }

        private void stopInteracations() {

            MonitoringEnabled = false;
            StartStopButtonText.text = "Start Monitoring Interactions";

            InteractionScoreText.text = "";
            InformationText.text = ""; 

            UserInterfaceEvents.RaiseStopMonitoringMoleculeInteractions();

            ResetPositionsButton.interactable = false;
        }
    }
}
