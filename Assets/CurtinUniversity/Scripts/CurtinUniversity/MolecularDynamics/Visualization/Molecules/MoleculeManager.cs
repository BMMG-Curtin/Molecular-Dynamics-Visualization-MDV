using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeManager : MonoBehaviour {

        [SerializeField]
        private MolecularInteractions moleculeInterations;

        [SerializeField]
        private GameObject MoleculePrefab;

        private Dictionary<int, Molecule> molecules;

        private Dictionary<int, MoleculeRenderSettings> cachedRenderSettings;
        private Dictionary<int, int?> cachedFrameNumbers;

        private Dictionary<int, SerializableTransform> defaultMoleculeTransforms;

        private bool loadingStructure;
        private bool loadingTrajectory;

        GeneralSettings generalSettings;

        private void Awake() {

            molecules = new Dictionary<int, Molecule>();
            cachedRenderSettings = new Dictionary<int, MoleculeRenderSettings>();
            cachedFrameNumbers = new Dictionary<int, int?>();
            defaultMoleculeTransforms = new Dictionary<int, SerializableTransform>();
            loadingStructure = false;
            loadingTrajectory = false;

            generalSettings = GeneralSettings.Default();
        }

        public void LoadMolecule(int moleculeID, string filePath, MoleculeRenderSettings settings) {
            StartCoroutine(LoadMoleculeStructure(moleculeID, filePath, settings));
        }

        public IEnumerator LoadMolecule(int moleculeID, string structureFilePath, string trajectoryFilePath, MoleculeRenderSettings settings) {

            yield return StartCoroutine(LoadMoleculeStructure(moleculeID, structureFilePath, settings));
            yield return StartCoroutine(LoadMoleculeTrajectory(moleculeID, trajectoryFilePath));
        }

        public IEnumerator LoadMoleculeStructure(int moleculeID, string filePath, MoleculeRenderSettings settings) {

            if (molecules.ContainsKey(moleculeID)) {

                MoleculeEvents.RaiseRenderMessage("Error Loading Molecule: already loaded", true);
                yield break;
            }

            if (loadingStructure) {
                MoleculeEvents.RaiseRenderMessage("Can't Load Molecule: another molecule currently loading", true);
                yield break;
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            MoleculeEvents.RaiseRenderMessage("Loading Structure File: " + filePath, false);

            loadingStructure = true;
            cacheRenderSettings(moleculeID, settings, null);
            int oldAtomMeshQuality = generalSettings.MeshQuality;

            PrimaryStructure primaryStructure = null;
            string loadException = null;

            Thread thread = new Thread(() => {

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
                    loadException = ex.Message;
                }
            });

            thread.Start();

            while (thread.IsAlive) {
                yield return null;
            }

            if (loadException != null) {

                Debug.Log("Error Loading Structure File: " + loadException);
                MoleculeEvents.RaiseRenderMessage("Error Loading Structure File: " + loadException, true);
                loadingStructure = false;
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
                molecule.Initialise(moleculeID, primaryStructure, settings);
                molecule.AutoRotateSpeed = generalSettings.AutoRotateSpeed;
                molecule.SetInputSensitivity(generalSettings.MoleculeInputSensitivity);
                molecule.SetSpaceNavigatorControlEnabled(generalSettings.SpaceNavigatorMoleculeControlEnabled);
                molecules.Add(moleculeID, molecule);

                // check to see if the meshQuality needs to change given the new primary structure
                updateMeshQuality();

                molecule.SetRenderSettings(settings);
                yield return StartCoroutine(molecule.Render(generalSettings.MeshQuality));
                MoleculeEvents.RaiseMoleculeLoaded(moleculeID, Path.GetFileName(filePath), primaryStructure);
            }

            if (oldAtomMeshQuality != generalSettings.MeshQuality) {
                reRenderMolecules();
            }

            SaveCurrentMoleculeTransformAsDefault(moleculeID);

            loadingStructure = false;
        }

        public void LoadTrajectory(int moleculeID, string filePath) {
            StartCoroutine(LoadMoleculeTrajectory(moleculeID, filePath));
        }

        public IEnumerator LoadMoleculeTrajectory(int moleculeID, string filePath) {

            if (!molecules.ContainsKey(moleculeID)) {

                MoleculeEvents.RaiseRenderMessage("Can't load molecule trajectory. No molecule found.", true);
                yield break;
            }

            if (loadingTrajectory) {
                MoleculeEvents.RaiseRenderMessage("Can't Load Trajectory: another trajectory currently loading", true);
                yield break;
            }

            PrimaryStructure primaryStructure = molecules[moleculeID].PrimaryStructure;
            int atomCount = loadTrajectoryAtomCount(filePath);

            if (atomCount != primaryStructure.AtomCount()) {

                MoleculeEvents.RaiseRenderMessage("Trajectory atom count [" + atomCount + " doesn't match loaded structure atom count [" + primaryStructure.AtomCount() + "]", true);
                yield break;
            }

            MoleculeEvents.RaiseRenderMessage("Loading trajectory. Please wait", false);

            loadingTrajectory = true;

            PrimaryStructureTrajectory trajectory = null;
            string loadException = null;

            Thread thread = new Thread(() => {

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
                    loadException = ex.Message;
                }
            });

            thread.Start();

            while (thread.IsAlive) {
                yield return null;
            }

            if (loadException != null) {

                MoleculeEvents.RaiseRenderMessage("Error Loading Trajectory File: " + loadException, true);
                loadingTrajectory = false;
                yield break;
            }

            if (trajectory != null) {

                molecules[moleculeID].SetTrajectory(trajectory);
                MoleculeEvents.RaiseTrajectoryLoaded(moleculeID, filePath, trajectory.FrameCount());
            }

            loadingTrajectory = false;
        }

        public void UpdateMoleculeRenderSettings(int moleculeID, MoleculeRenderSettings settings, int? frameNumber = null) {

            cacheRenderSettings(moleculeID, settings, frameNumber);

            if (molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].SetRenderSettings(settings);
                StartCoroutine(molecules[moleculeID].Render(generalSettings.MeshQuality, frameNumber));
            }

            if (moleculeInterations.Active) {

                if (moleculeInterations.Molecule1.ID == moleculeID) {
                    moleculeInterations.SetMolecule1RenderSettings(settings);
                }
                else if (moleculeInterations.Molecule2.ID == moleculeID) {
                    moleculeInterations.SetMolecule2RenderSettings(settings);
                }
            }
        }

        public void UpdateGeneralSettings(GeneralSettings newSettings) {

            // meshquality settings
            int oldAtomMeshQuality = generalSettings.MeshQuality;
            generalSettings = newSettings;

            updateMeshQuality(); // could change meshQuality, depending on autoMeshQuality value

            if (oldAtomMeshQuality != generalSettings.MeshQuality) {
                reRenderMolecules();
            }

            // autorotate speed
            foreach (Molecule molecule in molecules.Values) {

                molecule.AutoRotateSpeed = generalSettings.AutoRotateSpeed;
                molecule.SetSpaceNavigatorControlEnabled(generalSettings.SpaceNavigatorMoleculeControlEnabled);
                molecule.SetInputSensitivity(generalSettings.MoleculeInputSensitivity);
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


        public void SaveCurrentMoleculeTransformAsDefault(int moleculeID) {

            if (defaultMoleculeTransforms.ContainsKey(moleculeID)) {
                defaultMoleculeTransforms[moleculeID] = GetMoleculeTransform(moleculeID);
            }
            else {
                defaultMoleculeTransforms.Add(moleculeID, GetMoleculeTransform(moleculeID));
            }
        }

        public void LoadDefaultMoleculeTransform(int moleculeID) {

            if(!defaultMoleculeTransforms.ContainsKey(moleculeID)) {
                return;
            }

            SetMoleculeTransform(moleculeID, defaultMoleculeTransforms[moleculeID]);
        }

        public BoundingBox GetMoleculeBoundingBox(int moleculeID) {

            if (!molecules.ContainsKey(moleculeID)) {
                return null;
            }

            return molecules[moleculeID].GetBoundingBox();
        }

        public void RemoveMolecule(int moleculeID) {

            if (molecules.ContainsKey(moleculeID)) {

                GameObject.Destroy(molecules[moleculeID].gameObject);

                molecules.Remove(moleculeID);

                if(defaultMoleculeTransforms.ContainsKey(moleculeID)) {
                    defaultMoleculeTransforms.Remove(moleculeID);
                }

                // check if the molecule was being monitored. If so, shut down monitoring
                if(moleculeInterations.Active && (moleculeInterations.Molecule1.ID == moleculeID || moleculeInterations.Molecule2.ID == moleculeID)) {
                    moleculeInterations.StopMonitoring();
                }

                updateMeshQuality();
            }
        }

        public void StartMonitoringInteractions(int molecule1ID, int molecule2ID, MolecularInteractionSettings interactionSettings, MoleculeRenderSettings molecule1Settings, MoleculeRenderSettings molecule2Settings) {

            if(!molecules.ContainsKey(molecule1ID)) {

                MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. Molecule " + molecule1ID + " not found", true);
                return;
            }

            if (!molecules.ContainsKey(molecule2ID)) {

                MoleculeEvents.RaiseInteractionsMessage("Can't monitor interactions. Molecule " + molecule2ID + " not found", true);
                return;
            }

            moleculeInterations.StartMonitoring(molecules[molecule1ID], molecules[molecule2ID], interactionSettings, molecule1Settings, molecule2Settings);
        }

        public void StopMonitoringInteractions() {
            moleculeInterations.StopMonitoring();
        }

        public void UpdateMolecularInteractionSettings(MolecularInteractionSettings settings) {
            moleculeInterations.SetMolecularInteractionSettings(settings);
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

            if (generalSettings.AutoMeshQuality) {

                int totalAtomCount = 0;
                foreach (Molecule molecule in molecules.Values) {
                    totalAtomCount += molecule.PrimaryStructure.AtomCount();
                }

                if (totalAtomCount > Settings.LowMeshQualityThreshold) {
                    generalSettings.MeshQuality = Settings.LowMeshQualityValue;
                }
                else {
                    generalSettings.MeshQuality = Settings.DefaultMeshQuality;
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

                molecules[moleculeID].SetRenderSettings(settings);
                StartCoroutine(molecules[moleculeID].Render(generalSettings.MeshQuality, frameNumber));
            }
        }
    }
}
