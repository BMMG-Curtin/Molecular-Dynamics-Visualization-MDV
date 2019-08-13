using System;
using System.Collections;
using System.Collections.Generic;


namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeEvents {

        public delegate void MoleculeLoaded(int moleculeID, string name, string description, HashSet<string> elements, Dictionary<string, HashSet<int>> residues, int atomCount, int residueCount);
        public static event MoleculeLoaded OnMoleculeLoaded;
        public static void RaiseMoleculeLoaded(int moleculeID, string name, string description, HashSet<string> elements, Dictionary<string, HashSet<int>> residues, int atomCount, int residueCount) {
            OnMoleculeLoaded?.Invoke(moleculeID, name, description, elements, residues, atomCount, residueCount);
        }

        public delegate void TrajectoryLoaded(int moleculeID, int frameCount);
        public static event TrajectoryLoaded OnTrajectoryLoaded;
        public static void RaiseTrajectoryLoaded(int moleculeID, int frameCount) {
            OnTrajectoryLoaded?.Invoke(moleculeID, frameCount);
        }

        public delegate void RenderMessage(string message, bool error);
        public static event RenderMessage OnRenderMessage;
        public static void RaiseRenderMessage(string message, bool error) {
            OnRenderMessage?.Invoke(message, error);
        }
    }
}
