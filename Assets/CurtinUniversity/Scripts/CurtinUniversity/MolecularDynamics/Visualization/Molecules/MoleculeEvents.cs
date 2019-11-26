
using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// Events raised by the molecule loading and rendering classes
    /// Most of these events are consumed by the UI classes
    /// </summary>
    public class MoleculeEvents {

        public delegate void MoleculeLoaded(int moleculeID, string name, PrimaryStructure primaryStructure);
        public static event MoleculeLoaded OnMoleculeLoaded;
        public static void RaiseMoleculeLoaded(int moleculeID, string name, PrimaryStructure primaryStructure) {
            if (OnMoleculeLoaded != null) {
                OnMoleculeLoaded(moleculeID, name, primaryStructure);
            }
        }

        public delegate void MoleculeLoadFailed(int moleculeID);
        public static event MoleculeLoadFailed OnMoleculeLoadFailed;
        public static void RaiseOnMoleculeLoadFailed(int moleculeID) {
            if (OnMoleculeLoadFailed != null) {
                OnMoleculeLoadFailed(moleculeID);
            }
        }

        public delegate void TrajectoryLoaded(int moleculeID, string filePath, int frameCount);
        public static event TrajectoryLoaded OnTrajectoryLoaded;
        public static void RaiseTrajectoryLoaded(int moleculeID, string filePath, int frameCount) {
            if (OnTrajectoryLoaded != null) {
                OnTrajectoryLoaded(moleculeID, filePath, frameCount);
            }
        }

        public delegate void ShowMessage(string message, bool error);
        public static event ShowMessage OnShowMessage;
        public static void RaiseShowMessage(string message, bool error) {
            if (OnShowMessage != null) {
                OnShowMessage(message, error);
            }
        }

        public delegate void InteractionsInformation(MolecularInteractionsInformation information);
        public static event InteractionsInformation OnInteractionsInformation;
        public static void RaiseInteractionsInformation(MolecularInteractionsInformation information) {
            if (OnInteractionsInformation != null) {
                OnInteractionsInformation(information);
            }
        }

        public delegate void InteractionsMessage(string message, bool error);
        public static event InteractionsMessage OnInteractionsMessage;
        public static void RaiseInteractionsMessage(string message, bool error) {
            if (OnInteractionsMessage != null) {
                OnInteractionsMessage(message, error);
            }
        }
    }
}