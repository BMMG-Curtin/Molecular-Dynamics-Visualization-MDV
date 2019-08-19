
namespace CurtinUniversity.MolecularDynamics.Model {

    public class PrimaryStructureFrame {

        public int Index { get; set; }
        public int AtomCount { get; set; }
        public int Step { get; set; }
        public float Time { get; set; }

        // Single float array for performance. Array length is 3 * AtomCount. Coords are xyzxyz...
        public float[] Coords { get; set; }

        // Values for colours for trajectory frame. Currently single floating point value per atom 
        public float[] Colours { get; set; }

        public float GetAtomColour(int atomIndex) {
            if(Colours != null && atomIndex > 0 && atomIndex < Colours.Length -1) {
                return Colours[atomIndex - 1];
            }
            return -1;
        }

        public float[] GetAtomCoords(int atomIndex) {

            float[] atomCoords = null;

            if (Coords != null && Coords.Length > (atomIndex * 3) + 2) {

                int startIndex = atomIndex * 3;
                atomCoords = new float[3];
                atomCoords[0] = Coords[startIndex];
                atomCoords[1] = Coords[startIndex + 1];
                atomCoords[2] = Coords[startIndex + 2];
            }

            return atomCoords;
        }
    }
}
