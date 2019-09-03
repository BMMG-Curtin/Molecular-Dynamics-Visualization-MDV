
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class SettingsFile {

        public int MoleculeID { get; set; }
        public string StructureFilePath { get; set; }
        public string TrajectoryFilePath { get; set; }
        public MoleculeRenderSettings RenderSettings { get; set; }
        public SerializableTransform MoleculeTransform { get; set; }
        public SerializableTransform CameraTransform { get; set; }

        public override string ToString() {

            return
                "Molecule ID: " + MoleculeID + "\n" +
                "StructureFilePath: " + StructureFilePath + "\n" +
                "TrajectoryFilePath: " + TrajectoryFilePath + "\n" +
                "RenderSettings:\n" + RenderSettings + "\n" +
                "MoleculeTransform:\n" + MoleculeTransform + "\n" +
                "CameraTransform:\n" + CameraTransform + "\n";
        }
    }
}
