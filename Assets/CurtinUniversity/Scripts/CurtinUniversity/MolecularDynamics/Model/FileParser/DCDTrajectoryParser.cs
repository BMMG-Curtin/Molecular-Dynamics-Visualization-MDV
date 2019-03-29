using System;
using System.IO;

using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.Model.FileParser {

    // This parser assumes input file is in 32 bit little endian. Also assumes non-CHARMm format. Will error otherwise.
    public class DCDTrajectoryParser {

        private const int DEFAULT_FRAME_READ = 1000; // maximum amount of frames that will be read into a trajectory file by this class
                                                     // roughly, 12 bytes per atom per frame. Assume 500k max atoms = 6mb per frame. 1000 frames = 6gb data in memory max 

        /// <summary>
        /// Returns number of frames in trajectory file. 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static int GetFrameCount(string filename) {

            BinaryReader reader = null;
            int frameCount = 0;
            int atomCount;
            bool cellInfo;
            string title;

            try {
                reader = new BinaryReader(new FileStream(filename, FileMode.Open));
                parseDCDHeader(reader, out frameCount, out atomCount, out cellInfo, out title);
            }
            catch (Exception e) {
                throw new FileParseException(e.Message);
            }
            finally {
                if (reader != null) {
                    reader.Close();
                }
            }

            return frameCount;
        }

        /// <summary>
        /// Returns the number of atoms in the first frame of the trajectory file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static int GetAtomCount(string filename) {

            BinaryReader reader = null;
            int frameCount = 0;
            int atomCount;
            bool cellInfo;
            string title;

            try {
                reader = new BinaryReader(new FileStream(filename, FileMode.Open));
                parseDCDHeader(reader, out frameCount, out atomCount, out cellInfo, out title);
            }
            catch (Exception e) {
                throw new FileParseException(e.Message);
            }
            finally {
                if (reader != null) {
                    reader.Close();
                }
            }

            return atomCount;
        }

        public static PrimaryStructureTrajectory GetTrajectory(string filename) {
            return GetTrajectory(filename, 0, DEFAULT_FRAME_READ, 1);
        }

        public static PrimaryStructureTrajectory GetTrajectory(string filename, int startFrame, int numFrames, int frameFrequency) {

            PrimaryStructureTrajectory trajectory = new PrimaryStructureTrajectory();
            BinaryReader reader = null;

            try {
                int frameCount;
                int atomCount;
                bool cellInfo;
                string title;

                reader = new BinaryReader(new FileStream(filename, FileMode.Open));
                parseDCDHeader(reader, out frameCount, out atomCount, out cellInfo, out title);

                for (int i = 0; i < startFrame; i++) {
                    discardFrame(reader, cellInfo, atomCount);
                }

                int frameIndex = 0;
                int currentFrame = 0;

                while (reader.BaseStream.Position != reader.BaseStream.Length && frameIndex < numFrames) {

                    currentFrame++;

                    if (currentFrame % frameFrequency == 0) {

                        PrimaryStructureFrame frame = new PrimaryStructureFrame();
                        frame.AtomCount = atomCount;
                        //frame.Step = ;
                        //frame.Time = ;

                        parseFrame(reader, cellInfo, frame);
                        trajectory.AddFrame(frame);
                        frameIndex++;
                    }
                    else {
                        discardFrame(reader, cellInfo, atomCount);
                    }
                }
            }
            catch (Exception e) {

                throw new FileParseException(e.Message);
            }
            finally {

                if (reader != null) {
                    reader.Close();
                }
            }

            return trajectory;
        }

        private static void parseDCDHeader(BinaryReader reader, out int frameCount, out int atomCount, out bool cellInfo, out string title) {

            frameCount = 0;
            atomCount = 0;
            cellInfo = false;
            title = "";

            // check header format
            int blockSize = reader.ReadInt32();
            String magic2 = new string(reader.ReadChars(4));

            if (blockSize != 84 || magic2 != "CORD") {
                throw new FileParseException("Bad file header.");
            }

            // get header information

            frameCount = reader.ReadInt32(); //1
            int startTimestep = reader.ReadInt32(); //2
            int timestepFrequency = reader.ReadInt32(); //3
            int timestepLastDCD = reader.ReadInt32(); //4

            for (int i = 0; i < 4; i++) {
                reader.ReadInt32(); // discard 5 - 8
            }

            int fixedAtoms = reader.ReadInt32(); //9
            float delta = reader.ReadSingle(); //10
            int cellsExist = reader.ReadInt32(); //11
            if(cellsExist != 0) {
                cellInfo = true;
            }

            for (int i = 0; i < 9; i++) {
                reader.ReadInt32(); // discard 12 - 20
            }

            reader.ReadInt32(); // discard block size
            reader.ReadInt32(); // discard block size

            int nTitles = reader.ReadInt32();

            for (int i = 0; i < nTitles; i++) {
                title += new String(reader.ReadChars(80));
            }
            title = title.Trim();

            reader.ReadInt32(); // discard block size
            reader.ReadInt32(); // discard '4'
            atomCount = reader.ReadInt32();
            reader.ReadInt32(); // discard '4'
        }

        private static void parseFrame(BinaryReader reader, bool cellInfo, PrimaryStructureFrame frame) {

            // discard cell information if present

            if (cellInfo) {
                int cellInfoLength = reader.ReadInt32();
                reader.ReadBytes(cellInfoLength);
                reader.ReadInt32(); // discard end of index array length
            }

            float[] coords = new float[3 * frame.AtomCount];

            reader.ReadInt32(); // discard block size

            for (int i = 0; i < frame.AtomCount; i++) {
                coords[3 * i] = (float)Math.Round((decimal)(reader.ReadSingle() / 10f), 3);
            }

            reader.ReadInt32(); // discard block size
            reader.ReadInt32(); // discard block size

            for (int i = 0; i < frame.AtomCount; i++) {
                coords[3 * i + 1] = (float)Math.Round((decimal)(reader.ReadSingle() / 10f), 3);
            }

            reader.ReadInt32(); // discard block size
            reader.ReadInt32(); // discard block size

            for (int i = 0; i < frame.AtomCount; i++) {
                byte[] input = reader.ReadBytes(4);
                float num = System.BitConverter.ToSingle(input, 0);
                if(num < 0) {
                    // Console.WriteLine("Negative number: " + num);
                }
                coords[3 * i + 2] = (float)Math.Round((decimal)(num / 10f), 3);
            }

            reader.ReadInt32(); // discard block size

            frame.Coords = coords;
        }

        private static void discardFrame(BinaryReader reader, bool cellInfo, int atomCount) {

            // needs to be optimised to seek the whole amount.

            // discard cell information if present
            if (cellInfo) {
                int cellInfoLength = reader.ReadInt32();
                reader.ReadBytes(cellInfoLength);
                reader.ReadInt32(); // discard end of index array length
            }

            int totalBytes = ((atomCount * 3) + 6) * 4;
            reader.ReadBytes(totalBytes);
        }

    }
}
