using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class InteractionsSettingsPanel : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private MessageConsole console;

        [SerializeField]
        private TextMeshProUGUI StartStopButtonText;

        [SerializeField]
        private TextMeshProUGUI Informationtext;

        public bool MonitoringEnabled { get; private set; }

        public void Awake() {
            Informationtext.text = "";
            MonitoringEnabled = false;
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
            UserInterfaceEvents.RaiseStartMonitoringMoleculeInteractions(moleculeIDs[0], moleculeIDs[1]);
        }

        private void stopInteracations() {

            MonitoringEnabled = false;
            StartStopButtonText.text = "Start Monitoring Molecules";
            UserInterfaceEvents.RaiseStopMonitoringMoleculeInteractions();
        }
    }
}
