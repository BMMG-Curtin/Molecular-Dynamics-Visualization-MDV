using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class GROTrajectoryParser {

        private const int DEFAULT_FRAME_READ = 1000; // maximum amount of frames that will be read into a trajectory file by this class

        // assumes first frame is structure and doesn't count to total
        public static int GetFrameCount(string filename) {

            StreamReader sr = null;
            int frameCount = 0;

            try {

                sr = new StreamReader(filename);

                while (true) {

                    string titleLine = sr.ReadLine();

                    // remove any blank lines before title line
                    while (titleLine != null && titleLine.Trim().Equals("")) {
                        titleLine = sr.ReadLine();
                    }
                    
                    if (titleLine == null) {
                        break;
                    }

                    int atomCount = Int32.Parse(sr.ReadLine().Trim());

                    for (int i = 0; i < atomCount + 1; i++) { // + 1 = inlude the box line in the check
                        string line = sr.ReadLine();
                        if (line == null) { // if something is wrong with the file, i.e. ends before total atom count plus box bounds
                            break;
                        }
                    }

                    frameCount++;
                }
            }
            catch (Exception e) {

                throw new FileParseException(e.Message);
            }
            finally {

                if (sr != null) {
                    sr.Close();
                }
            }

            return frameCount - 1;
        }

        /// <summary>
        /// Returns the number of atoms in the first frame of the trajectory.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static int GetAtomCount(string filename) {

            StreamReader sr = null;
            int atomCount = 0;

            try {

                sr = new StreamReader(filename);

                // discard first frame, which should be the structure
                getFrame(sr); // discard frame

                PrimaryStructureFrame frame = getFrame(sr); // get first trajectory frame
                if (frame != null) {
                    atomCount = frame.AtomCount;
                }
            }
            catch (Exception e) {
                throw new FileParseException(e.Message);
            }
            finally {

                if (sr != null) {
                    sr.Close();
                }
            }

            return atomCount;
        }

        /// <summary>
        /// This method parses a GRO formatted file for a trajectory.
        /// It will discard the first frame in the file, assuming it's the structure. 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static PrimaryStructureTrajectory GetTrajectory(string filename) {
            return GetTrajectory(filename, 0, DEFAULT_FRAME_READ, 1);
        }

        public static PrimaryStructureTrajectory GetTrajectory(string filename, int startFrame, int numFrames, int frameFrequency) {

            StreamReader sr = null;
            PrimaryStructureTrajectory trajectory = new PrimaryStructureTrajectory();

            try {

                sr = new StreamReader(filename);

                // discard first frame, which should be the structure
                getFrame(sr); // discard frame

                for (int i = 0; i < startFrame; i++) {
                    getFrame(sr); // discard frame
                }

                int framesAdded = 0;
                int currentFrame = 0;

                while (framesAdded < numFrames) {

                    currentFrame++;

                    if (currentFrame % frameFrequency == 0) {

                        PrimaryStructureFrame frame = getFrame(sr);

                        if(frame == null) { // end of file, no more frames in file
                            break;
                        }

                        trajectory.AddFrame(frame);
                        framesAdded++;
                    }
                    else {

                        PrimaryStructureFrame frame = getFrame(sr); // discard frame
                        if(frame == null) { // end of file, no more frames in file
                            break;
                        }
                    }
                }
            }
            catch (Exception e) {
                throw new FileParseException(e.Message);
            }
            finally {

                if (sr != null) {
                    sr.Close();
                }
            }

            return trajectory;
        }

        private static PrimaryStructureFrame getFrame(StreamReader sr) {

            PrimaryStructureFrame frame;

            try {

                frame = new PrimaryStructureFrame();

                string titleLine = sr.ReadLine();

                // remove any blank lines before title line
                while (titleLine != null && titleLine.Trim().Equals("")) {
                    titleLine = sr.ReadLine();
                }

                if (titleLine == null) { // if we have reached the end of the file
                    return null;
                }

                frame.Time = 0;

                Regex g = new Regex(@"\st=\s*(\d+\.?\d*)");
                Match m = g.Match(titleLine);
                if (m.Success) {
                    string frameTimeString = m.Groups[1].Value;
                    frame.Time = float.Parse(frameTimeString);
                    // Console.WriteLine("Success parsing [" + titleLine + "] for frame time");
                }

                frame.AtomCount = Int32.Parse(sr.ReadLine().Trim());
                float[] coords = new float[3 * frame.AtomCount];

                for (int i = 0; i < frame.AtomCount; i++) {

                    string atomLine = sr.ReadLine();
                    if (atomLine == null) { // if something is wrong with the file, i.e. ends before total atom count
                        throw new FileParseException("Error reading atoms. Frame atom count doesn't equal frame atom total");
                    }

                    coords[3 * i] = float.Parse(atomLine.Substring(20, 8));
                    coords[3 * i] = float.Parse(atomLine.Substring(28, 8));
                    coords[3 * i] = float.Parse(atomLine.Substring(36, 8));
                    frame.Coords = coords;
                }

                // read box vectors. 
                string boxLine = sr.ReadLine();
            }
            catch (Exception e) {
                throw new FileParseException(e.Message);
            }

            return frame;
        }
    }
}
