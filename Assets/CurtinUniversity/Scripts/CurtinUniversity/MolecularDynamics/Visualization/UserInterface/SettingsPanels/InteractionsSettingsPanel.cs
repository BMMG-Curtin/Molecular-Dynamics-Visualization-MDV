using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class InteractionsSettingsPanel : MonoBehaviour {

        [SerializeField]
        private Toggle renderClosestInteractionsToggle;

        [SerializeField]
        private Toggle calculateClosestInteractionsToggle;

        [SerializeField]
        private TextMeshProUGUI StartStopButtonText;

        [SerializeField]
        private TextMeshProUGUI Informationtext;

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private MessageConsole console;

        private MolecularInteractionSettings interactionSetttings;

        public bool MonitoringEnabled { get; private set; }

        public void Awake() {

            Informationtext.text = "";
            MonitoringEnabled = false;

            interactionSetttings = MolecularInteractionSettings.Default();
        }

        private void initialiseSettingsPanel() {

            renderClosestInteractionsToggle.isOn = interactionSetttings.RenderClosestInteractionsOnly;
            calculateClosestInteractionsToggle.isOn = interactionSetttings.CalculateClosestInteractionsOnly;
        }

        public void OnRenderClosestInteractionsToggle() {

            interactionSetttings.RenderClosestInteractionsOnly = renderClosestInteractionsToggle.isOn;
            UserInterfaceEvents.RaiseMolecularInteractionSettingsUpdated(interactionSetttings);
        }

        public void OnCalculateClosestInteractionsToggle() {

            interactionSetttings.CalculateClosestInteractionsOnly = renderClosestInteractionsToggle.isOn;
            UserInterfaceEvents.RaiseMolecularInteractionSettingsUpdated(interactionSetttings);
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

            if (molecules.Get(moleculeIDs[0]).HasTrajectory || molecules.Get(moleculeIDs[1]).HasTrajectory) {

                MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. Monitored molecules cannot have trajectories loaded.", true);
                return;
            }

            MonitoringEnabled = true;
            StartStopButtonText.text = "Stop Monitoring Molecules";
            UserInterfaceEvents.RaiseStartMonitoringMoleculeInteractions(moleculeIDs[0], moleculeIDs[1], interactionSetttings);
        }

        private void stopInteracations() {

            MonitoringEnabled = false;
            StartStopButtonText.text = "Start Monitoring Molecules";
            UserInterfaceEvents.RaiseStopMonitoringMoleculeInteractions();
        }
    }
}
