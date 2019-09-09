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

        private bool monitoringEnabled = false;

        public void Awake() {
            Informationtext.text = "";
        }

        public void OnDisable() {

            if (monitoringEnabled) {

                monitoringEnabled = false;
                StartStopButtonText.text = "Start Monitoring Molecules";
                UserInterfaceEvents.RaiseStopMonitoringMoleculeInteractions();
            }
        }

        public void ShowInformation(string text) {
            Informationtext.text = text;
        }

        public void OnStartStopButton() {

            if(monitoringEnabled) {

                monitoringEnabled = false;
                StartStopButtonText.text = "Start Monitoring Molecules";
                UserInterfaceEvents.RaiseStopMonitoringMoleculeInteractions();
                return;
            }

            List<int> moleculeIDs = molecules.GetIDs();
            if(moleculeIDs == null || moleculeIDs.Count != 2) {

                if (moleculeIDs.Count > 2) {
                    console.ShowError("Can't monitor molecule interactions. Only two molecules can to be loaded");
                }
                else {
                    console.ShowError("Can't monitor molecule interactions. At least two molecules need to be loaded");
                }

                return;
            }

            monitoringEnabled = true;
            StartStopButtonText.text = "Stop Monitoring Molecules";
            UserInterfaceEvents.RaiseStartMonitoringMoleculeInteractions(moleculeIDs[0], moleculeIDs[1]);
        }
    }
}
