using System;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Model.Model {

    public class PrimaryStructureTrajectory {

        private List<PrimaryStructureFrame> frames;

        public PrimaryStructureTrajectory() {
            frames = new List<PrimaryStructureFrame>();
        }

        public void AddFrame(PrimaryStructureFrame frame) {
            frames.Add(frame);
        }

        public PrimaryStructureFrame GetFrame(int number) {
            return frames[number];
        }

        public List<PrimaryStructureFrame> GetFrames() {
            return frames;
        }

        public int FrameCount() {
            if(frames != null) {
                return frames.Count;
            }
            return 0;
        }

        public int AtomCount() {

            if (frames != null && frames.Count > 0) {
                return frames[0].AtomCount;
            }
            else {
                return 0;
            }
        }

        public override string ToString() {

            string output = "";

            if(frames != null) {

                for (int i = 0; i < frames.Count; i++) {

                    if (i > 10) break;
                    output += "Frame[" + (i + 1) + "]\n";

                    PrimaryStructureFrame frame = frames[i];
                    int totalCoords = 0;

                    for(int j = 0; j < frame.AtomCount; j++) {

                        if (j > 10) break;

                        float x = frame.Coords[j * 3];
                        float y = frame.Coords[j * 3 + 1];
                        float z = frame.Coords[j * 3 + 2];

                        output += "Time [" + frame.Time + "] Coords[" + j + "]: [" + x + ", " + y + ", " + z + "]\n";
                        totalCoords++;
                    }

                    output += "\n";
                }
            }
            else {
                output = "No frames in trajectory";
            }

            return output;
        }
    }
}
