
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeSettings {

        public int ID;
        public string FilePath;
        public bool Loaded;
        public bool Hidden;
        public bool PendingRerender;
        public string Name;
        public string Description;
        public int AtomCount;
        public int BondCount;
        public int ResidueCount;
        public bool HasTrajectory;
        public string TrajectoryFilePath;
        public int TrajectoryFrameCount;
        public int? CurrentTrajectoryFrameNumber;
        public MoleculeRenderSettings RenderSettings;

        public MoleculeSettings(int moleculeID, string filePath) {

            ID = moleculeID;
            FilePath = filePath;
            Loaded = false;
            Hidden = false;
            PendingRerender = false;
            Name = "";
            Description = "";
            AtomCount = 0;
            ResidueCount = 0;
            HasTrajectory = false;
            TrajectoryFilePath = "";
            TrajectoryFrameCount = 0;
            CurrentTrajectoryFrameNumber = null;
            RenderSettings = MoleculeRenderSettings.Default();
        }
    }
}