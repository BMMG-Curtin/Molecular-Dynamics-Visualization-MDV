
using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

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

        public delegate void RenderMessage(string message, bool error);
        public static event RenderMessage OnRenderMessage;
        public static void RaiseRenderMessage(string message, bool error) {
            if (OnRenderMessage != null) {
                OnRenderMessage(message, error);
            }
        }

        public delegate void InteractionsInformation(string information);
        public static event InteractionsInformation OnInteractionsInformation;
        public static void RaiseInteractionsInformation(string information) {
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