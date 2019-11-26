
namespace CurtinUniversity.MolecularDynamics.Visualization {

    // Collection of molecule settings that are serialised to a settings file
    public class SettingsFile {

        public string StructureFilePath { get; set; }
        public string TrajectoryFilePath { get; set; }
        public MoleculeRenderSettings RenderSettings { get; set; }
        public SerializableTransform MoleculeTransform { get; set; }
        public SerializableTransform CameraTransform { get; set; }

        public override string ToString() {

            return
                "StructureFilePath: " + StructureFilePath + "\n" +
                "TrajectoryFilePath: " + TrajectoryFilePath + "\n" +
                "RenderSettings:\n" + RenderSettings + "\n" +
                "MoleculeTransform:\n" + MoleculeTransform + "\n" +
                "CameraTransform:\n" + CameraTransform + "\n";
        }
    }
}
