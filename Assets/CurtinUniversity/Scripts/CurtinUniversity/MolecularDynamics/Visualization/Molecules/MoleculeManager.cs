using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeManager : MonoBehaviour {

        [SerializeField]
        private GameObject MoleculePrefab;

        private Dictionary<int, Molecule> molecules;
        private HashSet<int> selectedMolecules;
        private HashSet<int> rotatingMolecules;

        private Dictionary<int, MoleculeRenderSettings> cachedRenderSettings;
        private Dictionary<int, int?> cachedFrameNumbers;

        private bool loadingFile;

        private int meshQuality;
        private bool autoMeshQuality;

        private float moleculeAutoRotateSpeed = 0;

        private void Awake() {

            molecules = new Dictionary<int, Molecule>();
            selectedMolecules = new HashSet<int>();
            rotatingMolecules = new HashSet<int>();
            cachedRenderSettings = new Dictionary<int, MoleculeRenderSettings>();
            cachedFrameNumbers = new Dictionary<int, int?>();
            loadingFile = false;

            meshQuality = Settings.DefaultMeshQuality;
            autoMeshQuality = Settings.DefaultAutoMeshQuality;
        }

        public void LoadMolecule(int moleculeID, string filePath, MoleculeRenderSettings settings) {
            StartCoroutine(LoadMoleculeStructure(moleculeID, filePath, settings));
        }

        public IEnumerator LoadMolecule(int moleculeID, string structureFilePath, string trajectoryFilePath, MoleculeRenderSettings settings) {

            yield return StartCoroutine(LoadMoleculeStructure(moleculeID, structureFilePath, settings));
            LoadMoleculeTrajectory(moleculeID, trajectoryFilePath);
        }

        public IEnumerator LoadMoleculeStructure(int moleculeID, string filePath, MoleculeRenderSettings settings) {

            if (molecules.ContainsKey(moleculeID)) {

                MoleculeEvents.RaiseRenderMessage("Error Loading Molecule: already loaded", true);
                yield break;
            }

            if (loadingFile) {
                MoleculeEvents.RaiseRenderMessage("Can't Load Molecule: another molecule currently loading", true);
                yield break;
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            MoleculeEvents.RaiseRenderMessage("Loading Structure File: " + filePath, false);

            loadingFile = true;
            cacheRenderSettings(moleculeID, settings, null);
            int oldAtomMeshQuality = this.meshQuality;

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
            yield return null;

            if (primaryStructure != null) {

                GameObject moleculeGO = GameObject.Instantiate(MoleculePrefab);
                moleculeGO.transform.parent = this.transform;
                moleculeGO.SetActive(true);

                Molecule molecule = moleculeGO.GetComponent<Molecule>();
                molecule.Initialise(primaryStructure, settings);
                molecule.AutoRotateSpeed = moleculeAutoRotateSpeed;
                molecules.Add(moleculeID, molecule);

                // check to see if the meshQuality needs to change given the new primary structure
                updateMeshQuality();

                yield return StartCoroutine(molecule.Render(settings, meshQuality));
                MoleculeEvents.RaiseMoleculeLoaded(moleculeID, Path.GetFileName(filePath), primaryStructure);
            }

            if (oldAtomMeshQuality != meshQuality) {
                reRenderMolecules();
            }

            loadingFile = false;
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
                MoleculeEvents.RaiseTrajectoryLoaded(moleculeID, filePath, trajectory.FrameCount());
            }
        }

        public void UpdateMoleculeRenderSettings(int moleculeID, MoleculeRenderSettings settings, int? frameNumber = null) {

            cacheRenderSettings(moleculeID, settings, frameNumber);

            if (molecules.ContainsKey(moleculeID)) {
                StartCoroutine(molecules[moleculeID].Render(settings, meshQuality, frameNumber));
            }
        }

        public void UpdateMeshQuality(bool autoMeshQuality, int newMeshQuality) {

            int oldAtomMeshQuality = this.meshQuality;
            this.meshQuality = newMeshQuality;
            this.autoMeshQuality = autoMeshQuality;

            updateMeshQuality(); // could change meshQuality, depending on autoMeshQuality value

            if (oldAtomMeshQuality != meshQuality) {
                reRenderMolecules();
            }
        }

        public void ShowMolecule(int moleculeID) {

            if (molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].Show();
            }
        }

        public void HideMolecule(int moleculeID) {

            if (molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].Hide();
            }
        }

        public void SetMoleculeSelected(int moleculeID, bool selected) {

            if (molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].EnableInput(selected);
            }
        }

        public SerializableTransform GetMoleculeTransform(int moleculeID) {

            if (molecules.ContainsKey(moleculeID)) {
                return new SerializableTransform(molecules[moleculeID].transform);
            }

            return null;
        }

        public void SetMoleculeTransform(int moleculeID, SerializableTransform moleculeTransform) {

            if (molecules.ContainsKey(moleculeID) && moleculeTransform != null) {

                molecules[moleculeID].transform.position = moleculeTransform.Position;
                molecules[moleculeID].transform.rotation = moleculeTransform.Rotation;
                molecules[moleculeID].transform.localScale = moleculeTransform.Scale;
            }
        }

        public void SetAutoRotateSpeed(float speed) {

            moleculeAutoRotateSpeed = speed;

            foreach (Molecule molecule in molecules.Values) { 
                molecule.AutoRotateSpeed = speed;
            }
        }

        public void RemoveMolecule(int moleculeID) {

            if (molecules.ContainsKey(moleculeID)) {

                GameObject.Destroy(molecules[moleculeID].gameObject);

                molecules.Remove(moleculeID);

                if (selectedMolecules.Contains(moleculeID)) {
                    selectedMolecules.Remove(moleculeID);
                }

                if (rotatingMolecules.Contains(moleculeID)) {
                    rotatingMolecules.Remove(moleculeID);
                }

                updateMeshQuality();
            }
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

        private void cacheRenderSettings(int moleculeID, MoleculeRenderSettings renderSettings, int? frameNumber) {

            if (cachedRenderSettings.ContainsKey(moleculeID)) {
                cachedRenderSettings[moleculeID] = renderSettings;
            }
            else {
                cachedRenderSettings.Add(moleculeID, renderSettings);
            }

            if (cachedFrameNumbers.ContainsKey(moleculeID)) {
                cachedFrameNumbers[moleculeID] = frameNumber;
            }
            else {
                cachedFrameNumbers.Add(moleculeID, frameNumber);
            }
        }

        private void updateMeshQuality() {

            if (autoMeshQuality) {

                int totalAtomCount = 0;
                foreach (Molecule molecule in molecules.Values) {
                    totalAtomCount += molecule.PrimaryStructure.AtomCount();
                }

                if (totalAtomCount > Settings.LowMeshQualityThreshold) {
                    meshQuality = Settings.LowMeshQualityValue;
                }
                else {
                    meshQuality = Settings.DefaultMeshQuality;
                }
            }
        }

        private void reRenderMolecules() {

            foreach (KeyValuePair<int, Molecule> molecule in molecules) {

                int moleculeID = molecule.Key;

                MoleculeRenderSettings settings = null;
                if(cachedRenderSettings.ContainsKey(moleculeID)) {
                    settings = cachedRenderSettings[moleculeID];
                }
                else {
                    settings = MoleculeRenderSettings.Default();
                }

                int? frameNumber = null;
                if (cachedFrameNumbers.ContainsKey(moleculeID)) {
                    frameNumber = cachedFrameNumbers[moleculeID];
                }

                StartCoroutine(molecules[moleculeID].Render(settings, meshQuality, frameNumber));
            }
        }
    }
}
