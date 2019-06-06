
namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class MoleculeEvents {

        public delegate void MoleculeLoaded(int moleculeID, string name, string description);
        public static event MoleculeLoaded OnMoleculeLoaded;
        public static void RaiseMoleculeLoaded(int moleculeID, string name, string description) {
            if (OnMoleculeLoaded != null) {
                OnMoleculeLoaded(moleculeID, name, description);
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
