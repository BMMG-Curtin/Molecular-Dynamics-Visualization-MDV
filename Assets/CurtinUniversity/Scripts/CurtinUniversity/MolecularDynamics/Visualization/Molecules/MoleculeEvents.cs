
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

        public delegate void TrajectoryLoaded(int moleculeID, int frameCount);
        public static event TrajectoryLoaded OnTrajectoryLoaded;
        public static void RaiseTrajectoryLoaded(int moleculeID, int frameCount) {
            if (OnTrajectoryLoaded != null) {
                OnTrajectoryLoaded(moleculeID, frameCount);
            }
        }

        public delegate void RenderMessage(string message, bool error);
        public static event RenderMessage OnRenderMessage;
        public static void RaiseRenderMessage(string message, bool error) {
            if (OnRenderMessage != null) {
                OnRenderMessage(message, error);
            }
        }
    }
}