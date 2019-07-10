
namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class MoleculeSettings {

        public int ID;
        public string FilePath;
        public bool Loaded;
        public bool Hidden;
        public bool PendingRerender;
        public string Name;
        public string Description;
        public bool HasTrajectory;
        public MoleculeRenderSettings RenderSettings;

        public MoleculeSettings(int moleculeID, string filePath) {

            ID = moleculeID;
            FilePath = filePath;
            Loaded = false;
            Hidden = false;
            PendingRerender = false;
            Name = "";
            Description = "";
            HasTrajectory = false;
            RenderSettings = MoleculeRenderSettings.Default();
        }
    }
}