using System;
using System.Collections;
using System.Collections.Generic;


namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeEvents {

        public delegate void MoleculeLoaded(int moleculeID, string name, string description, HashSet<string> elements, HashSet<string> residues, int atomCount, int residueCount);
        public static event MoleculeLoaded OnMoleculeLoaded;
        public static void RaiseMoleculeLoaded(int moleculeID, string name, string description, HashSet<string> elements, HashSet<string> residues, int atomCount, int residueCount) {
            if (OnMoleculeLoaded != null) {
                OnMoleculeLoaded(moleculeID, name, description, elements, residues, atomCount, residueCount);
            }
        }

        public delegate void TrajectoryLoaded(int moleculeID, int frameCount);
        public static event TrajectoryLoaded OnTrajectoryLoaded;
        public static void RaiseTrajectoryLoaded(int moleculeID, int frameCount) {
            if (OnTrajectoryLoaded != null) {
                OnTrajectoryLoaded(moleculeID, frameCount);
            }
        }

        public delegate void RenderMessage(string message, bool error);
        public static event RenderMessage OnRenderMessage;
        public static void RaiseRenderMessage(string message, bool error) {
            if (OnRenderMessage != null) {
                OnRenderMessage(message, error);
            }
        }
    }
}
