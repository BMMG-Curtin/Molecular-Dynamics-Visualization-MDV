using System;
using System.IO;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// Central settings class for a specific molecule
    /// Used as a store by UserInterface for molecule specific settings in settings panels.
    /// Is serialised into a settings file for storage between application usages.
    /// </summary>
    public class MoleculeSettings {

        public int ID;

        private string filePath;
        public string FilePath {
            get {
                return filePath;
            }
            set {

                filePath = value;

                try {
                    FileName = Path.GetFileName(value);
                }
                catch(Exception) {}
            }
        }
        public string FileName { get; private set; }

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