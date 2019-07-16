using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model.Analysis;
using CurtinUniversity.MolecularDynamics.Model.FileParser;
using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class MoleculeManager : MonoBehaviour {

        [SerializeField]
        private GameObject MoleculePrefab;

        private Dictionary<int, Molecule> molecules;

        private bool loadingFile;

        private void Awake() {

            molecules = new Dictionary<int, Molecule>();
            loadingFile = false;
        }

        public void LoadMolecule(int moleculeID, string filePath, MoleculeRenderSettings settings) {

            if(molecules.ContainsKey(moleculeID)) {

                MoleculeEvents.RaiseRenderMessage("Error Loading Molecule: already loaded", true);
                return;
            }

            if (!loadingFile) {
                StartCoroutine(loadMolecule(moleculeID, filePath, settings));
            }
        }

        public void LoadMoleculeTrajectory(int moleculeID, string filePath) {

            if (!molecules.ContainsKey(moleculeID)) {

                MoleculeEvents.RaiseRenderMessage("Can't load molecule trajectory. No molecule found.", true);
                return;
            }

            PrimaryStructure primaryStructure = molecules[moleculeID].PrimaryStructure;
            int atomCount = loadTrajectoryAtomCount(filePath);

            if (atomCount != primaryStructure.AtomCount()) {

                MoleculeEvents.RaiseRenderMessage("Trajectory atom count [" + atomCount + " doesn't match loaded structure atom count [" + primaryStructure.AtomCount() + "]", true);
                return;
            }

            PrimaryStructureTrajectory trajectory = null;

            try {

                int startFrame = 0;
                int frameFrequency = 1;
                int frameCount = Settings.MaxTrajectoryFrames;

                if (filePath.EndsWith(".xtc")) {
                    trajectory = XTCTrajectoryParser.GetTrajectory(filePath, startFrame, frameCount, frameFrequency);
                }
                else if (filePath.EndsWith(".dcd")) {
                    trajectory = DCDTrajectoryParser.GetTrajectory(filePath, startFrame, frameCount, frameFrequency);
                }
                else if (filePath.EndsWith(".gro")) {
                    trajectory = GROTrajectoryParser.GetTrajectory(filePath, startFrame, frameCount, frameFrequency);
                }
            }
            catch (FileParseException ex) {

                MoleculeEvents.RaiseRenderMessage("Error Loading Trajectory File: " + ex.Message, true);
                return;
            }

            if (trajectory != null) {

                molecules[moleculeID].SetTrajectory(trajectory);
                MoleculeEvents.RaiseTrajectoryLoaded(moleculeID, trajectory.FrameCount());
            }
        }

        public void UpdateMoleculeRenderSettings(int moleculeID, MoleculeRenderSettings settings, int? frameNumber = null) {

            if(molecules.ContainsKey(moleculeID)) {
                StartCoroutine(molecules[moleculeID].Render(settings, frameNumber));
            }
        }

        public void ShowMolecule(int moleculeID) {

            if (molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].gameObject.SetActive(true);
            }
        }

        public void HideMolecule(int moleculeID) {

            if (molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].gameObject.SetActive(false);
            }
        }

        public void RemoveMolecule(int moleculeID) {

            if(molecules.ContainsKey(moleculeID)) {
                GameObject.Destroy(molecules[moleculeID].gameObject);
                molecules.Remove(moleculeID);
            }
        }

        private IEnumerator loadMolecule(int moleculeID, string filePath, MoleculeRenderSettings settings) {

            loadingFile = true;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            MoleculeEvents.RaiseRenderMessage("Loading Structure File: " + filePath, false);
            yield return new WaitForSeconds(0.05f);

            PrimaryStructure primaryStructure = null;

            try {
                if (filePath.EndsWith(".gro")) {
                    primaryStructure = GROStructureParser.GetStructure(filePath);
                }
                else if (filePath.EndsWith(".xyz")) {
                    primaryStructure = XYZStructureParser.GetStructure(filePath);
                }
                else if (filePath.EndsWith(".pdb")) {
                    primaryStructure = PDBStructureParser.GetPrimaryStructure(filePath);
                }
            }
            catch (FileParseException ex) {

                Debug.Log("Error Loading Structure File: " + ex.Message);
                MoleculeEvents.RaiseRenderMessage("Error Loading Structure File: " + ex.Message, true);
                loadingFile = false;
                yield break;
            }

            watch.Stop();
            MoleculeEvents.RaiseRenderMessage("Structure File Load Complete [" + watch.ElapsedMilliseconds + "ms]", false);
            yield return new WaitForSeconds(0.05f);

            if (primaryStructure != null) {

                GameObject moleculeGO = GameObject.Instantiate(MoleculePrefab);
                moleculeGO.transform.parent = this.transform;
                moleculeGO.SetActive(true);

                Molecule molecule = moleculeGO.GetComponent<Molecule>();
                molecule.Initialise(primaryStructure, settings);
                yield return StartCoroutine(molecule.Render(settings));

                molecules.Add(moleculeID, molecule);

                MoleculeEvents.RaiseMoleculeLoaded(moleculeID, Path.GetFileName(filePath), primaryStructure.Title, primaryStructure.ElementNames, primaryStructure.ResidueNames);
            }

            loadingFile = false;
        }

        private int loadTrajectoryAtomCount(string trajectoryFile) {

            int count = 0;

            try {
                if (trajectoryFile.EndsWith(".xtc")) {
                    count = XTCTrajectoryParser.GetAtomCount(trajectoryFile);
                }
                else if (trajectoryFile.EndsWith(".dcd")) {
                    count = DCDTrajectoryParser.GetAtomCount(trajectoryFile);
                }
                else if (trajectoryFile.EndsWith(".gro")) {
                    count = GROTrajectoryParser.GetAtomCount(trajectoryFile);
                }
            }
            catch (FileParseException ex) {
                MoleculeEvents.RaiseRenderMessage("Error Loading Trajectory File: " + ex.Message, true);
            }

            return count;
        }

        private int loadAvailableTrajectoryFrames(string trajectoryFile) {

            int count = 0;

            try {
                if (trajectoryFile.EndsWith(".xtc")) {
                    count = XTCTrajectoryParser.GetFrameCount(trajectoryFile);
                }
                else if (trajectoryFile.EndsWith(".dcd")) {
                    count = DCDTrajectoryParser.GetFrameCount(trajectoryFile);
                }
                else if (trajectoryFile.EndsWith(".gro")) {
                    count = GROTrajectoryParser.GetFrameCount(trajectoryFile);
                }
            }
            catch (FileParseException ex) {
                MoleculeEvents.RaiseRenderMessage("Error Loading Trajectory File: " + ex.Message, true);
            }

            return count;
        }
    }
}
