
namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class MoleculeEvents {

        public delegate void LoadedMolecule();
        public static event LoadedMolecule OnMoleculeLoaded;
        public static void RaiseOnLoadedMolecule() {
            if (OnMoleculeLoaded != null) {
                OnMoleculeLoaded();
            }
        }

        public delegate void RenderMessage(string message, bool error);
        public static event RenderMessage OnRenderMessage;
        public static void RaiseOnRenderMessage(string message, bool error) {
            if (OnRenderMessage != null) {
                OnRenderMessage(message, error);
            }
        }
    }
}
