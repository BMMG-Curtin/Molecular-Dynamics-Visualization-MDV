using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class InteractionsSettingsPanel : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI selectedMoleculeText;

        [SerializeField]
        private Toggle renderClosestInteractionsToggle;

        [SerializeField]
        private Toggle calculateClosestInteractionsToggle;

        [SerializeField]
        private TextMeshProUGUI StartStopButtonText;

        [SerializeField]
        private Button ResetPositionsButton;

        [SerializeField]
        private TextMeshProUGUI Informationtext;

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private MessageConsole console;

        private MolecularInteractionSettings interactionSettings;

        private MoleculeSettings selectedMolecule;

        public bool MonitoringEnabled { get; private set; }

        public void Awake() {

            Informationtext.text = "";
            MonitoringEnabled = false;
            ResetPositionsButton.interactable = false;

            interactionSettings = MolecularInteractionSettings.Default();

            renderClosestInteractionsToggle.isOn = interactionSettings.RenderClosestInteractionsOnly;
            calculateClosestInteractionsToggle.isOn = interactionSettings.CalculateClosestInteractionsOnly;
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

        public void OnRenderClosestInteractionsToggle() {

            interactionSettings.RenderClosestInteractionsOnly = renderClosestInteractionsToggle.isOn;
            UserInterfaceEvents.RaiseMolecularInteractionSettingsUpdated(interactionSettings);
        }

        public void OnCalculateClosestInteractionsToggle() {

            interactionSettings.CalculateClosestInteractionsOnly = renderClosestInteractionsToggle.isOn;
            UserInterfaceEvents.RaiseMolecularInteractionSettingsUpdated(interactionSettings);
        }

        public void ShowInformation(string text) {
            Informationtext.text = text;
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
            MoleculeRenderSettings molecule2Settings = molecules.Get(moleculeIDs[0]).RenderSettings;

            if (molecules.Get(moleculeIDs[0]).HasTrajectory || molecules.Get(moleculeIDs[1]).HasTrajectory) {

                MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. Monitored molecules cannot have trajectories loaded.", true);
                return;
            }

            MonitoringEnabled = true;
            StartStopButtonText.text = "Stop Monitoring Molecules";
            UserInterfaceEvents.RaiseStartMonitoringMoleculeInteractions(moleculeIDs[0], moleculeIDs[1], interactionSettings, molecule1Settings, molecule2Settings);

            ResetPositionsButton.interactable = true;
        }

        private void stopInteracations() {

            MonitoringEnabled = false;
            StartStopButtonText.text = "Start Monitoring Molecules";
            UserInterfaceEvents.RaiseStopMonitoringMoleculeInteractions();

            ResetPositionsButton.interactable = false;
        }
    }
}
