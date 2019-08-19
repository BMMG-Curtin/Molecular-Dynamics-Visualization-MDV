using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class TrajectoryColourFileParser {

        private const int DEFAULT_FRAME_READ = 1000; // maximum amount of frames that will be read by this class
        private const float DEFAULT_COLOUR_VALUE = 0f;

        public static void GetTrajectoryColours(string filename, PrimaryStructure model, PrimaryStructureTrajectory trajectory) {
            GetTrajectoryColours(filename, model, trajectory, 0, DEFAULT_FRAME_READ, 1);
        }

        public static void GetTrajectoryColours(string filename, PrimaryStructure model, PrimaryStructureTrajectory trajectory, int startFrame, int numFrames, int frameFrequency) {

            Stopwatch watch = new Stopwatch();
            watch.Start();

            StreamReader reader = null;
            Regex g = new Regex(@"\s*(\d+)\s*(\d+\.?\d*)");

            try {

                reader = new StreamReader(filename);

                discardFrames(reader, startFrame);

                List<float[]> colourFrames = new List<float[]>();
                int colourFrameNumber = -1;

                int atomIndex = 0;
                float colour = 0;
                float[] colourFrame = null;
                string line;

                while ((line = readLine(reader)) != null && colourFrames.Count < numFrames) {

                    if (line.Trim().Length == 0) { // is empty string?
                        continue;
                    }

                    // discard everything but matches
                    Match m = g.Match(line);
                    if (m.Success) {

                        atomIndex = int.Parse(m.Groups[1].Value);
                        colour = float.Parse(m.Groups[2].Value);

                        if (atomIndex == 1) {

                            colourFrameNumber++;

                            // save colours
                            if (colourFrame != null) {
                                colourFrames.Add(colourFrame);
                                colourFrame = null;
                            }

                            if (colourFrameNumber % frameFrequency == 0) {

                                // initialise new colour arrary
                                colourFrame = new float[model.AtomCount()];
                                for (int i = 0; i < colourFrame.Length; i++) {
                                    colourFrame[i] = DEFAULT_COLOUR_VALUE;
                                }
                            }
                        }

                        if (colourFrame != null) {
                            if (atomIndex > 0 && atomIndex <= colourFrame.Length) {
                                colourFrame[atomIndex - 1] = colour;
                            }
                        }
                    }
                }

                PrimaryStructureFrame currentFrame = null;
                int frameNumber = 0;

                // copy all colour frames to trajectory frames
                foreach(float[] frame in colourFrames) {
                    currentFrame = trajectory.GetFrame(frameNumber);
                    currentFrame.Colours = frame;
                    frameNumber++;

                    // if we've run out of trajectory frames then stop copying
                    if(frameNumber >= trajectory.FrameCount()) {
                        break;
                    }
                }

                // if more trajectory frames than colour frames fill in rest of trajectory frames with default colours
                float[] colours = new float[model.AtomCount()];
                for (int i = 0; i < colours.Length; i++) {
                    colours[i] = 0f;
                }

                foreach (PrimaryStructureFrame frame in trajectory.GetFrames()) {
                    if(frame.Colours == null) {
                        frame.Colours = colours;
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
        }

        private static void discardFrames(StreamReader reader, int count) {

            if(count <= 0) {
                return;
            }

            Regex g = new Regex(@"\s*(\d+)\s*(\d+\.?\d*)");

            int discardCount = 0;

            try {
                String line = null;
                while ((line = reader.ReadLine()) != null) {

                    Match m = g.Match(line);

                    // discard everything but matches
                    if (m.Success) {

                        int atomIndex = int.Parse(m.Groups[1].Value);

                        if (atomIndex == 1) {

                            while ((line = reader.ReadLine()) != null) {

                                if (line.Trim().Length == 0) { // is empty string?

                                    discardCount++;

                                    if (discardCount >= count) {
                                        return;
                                    }

                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                throw new FileParseException(ex.Message);
            }
        }

        // custom readline routine because Unity mono ReadLine() implementation is faulty
        private static String readLine(StreamReader reader) {

            String output = null;

            List<char> chars = new List<char>();
            while (reader.Peek() >= 0) {

                char c = (char)reader.Read();

                if (c == '\r' || c == '\n') {
                    output = new String(chars.ToArray());
                    if (reader.Peek() >= 0) {
                        c = (char)reader.Peek();
                        if (c == '\r' || c == '\n') {
                            reader.Read(); // discard additonal end of line characters
                        }
                        break;
                    }
                }

                chars.Add(c);
            }

            return output;
        }
    }
}
