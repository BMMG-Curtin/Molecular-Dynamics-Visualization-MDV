using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// Manages the loading of molecule data and the rendering of molecules.
    /// </summary>
    public class MoleculeManager : MonoBehaviour {

        [SerializeField]
        private MolecularInteractions moleculeInterations;

        [SerializeField]
        private GameObject MoleculePrefab;

        private Dictionary<int, Molecule> molecules;
        private int? selectedMolecule;

        private Dictionary<int, SerializableTransform> defaultMoleculeTransforms;

        private bool loadingStructure;
        private bool loadingTrajectory;

        private bool moleculeMovementEnabled;

        GeneralSettings generalSettings;

        private void Awake() {

            molecules = new Dictionary<int, Molecule>();
            selectedMolecule = null;

            defaultMoleculeTransforms = new Dictionary<int, SerializableTransform>();
            loadingStructure = false;
            loadingTrajectory = false;

            moleculeMovementEnabled = true;

            generalSettings = GeneralSettings.Default();
        }

        public void LoadMolecule(int moleculeID, string filePath, MoleculeRenderSettings settings) {
            StartCoroutine(LoadMoleculeStructure(moleculeID, filePath, settings));
        }

        public IEnumerator LoadMolecule(int moleculeID, string structureFilePath, string trajectoryFilePath, MoleculeRenderSettings settings) {

            yield return StartCoroutine(LoadMoleculeStructure(moleculeID, structureFilePath, settings));

            if(molecules.ContainsKey(moleculeID)) {
                yield return StartCoroutine(LoadMoleculeTrajectory(moleculeID, trajectoryFilePath));
            }
        }

        // Loads and renders a molecule using a structure file path and render settings
        public IEnumerator LoadMoleculeStructure(int moleculeID, string filePath, MoleculeRenderSettings settings) {

            if (molecules.ContainsKey(moleculeID)) {

                MoleculeEvents.RaiseShowMessage("Error Loading Molecule: already loaded", true);
                MoleculeEvents.RaiseOnMoleculeLoadFailed(moleculeID);
                yield break;
            }

            if (loadingStructure) {
                MoleculeEvents.RaiseShowMessage("Can't Load Molecule: another molecule currently loading", true);
                MoleculeEvents.RaiseOnMoleculeLoadFailed(moleculeID);
                yield break;
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            MoleculeEvents.RaiseShowMessage("Loading Structure File: " + filePath, false);

            loadingStructure = true;
            int oldAtomMeshQuality = generalSettings.MeshQuality;

            Debug.Log("Loading structure");

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

            Debug.Log("Finished Loading structure");

            if (loadException != null) {

                Debug.Log("Error Loading Structure File: " + loadException);
                MoleculeEvents.RaiseShowMessage("Error Loading Structure File: " + loadException, true);
                MoleculeEvents.RaiseOnMoleculeLoadFailed(moleculeID);
                loadingStructure = false;
                yield break;
            }

            watch.Stop();
            MoleculeEvents.RaiseShowMessage("Structure File Load Complete [" + watch.ElapsedMilliseconds + "ms]", false);
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

        // Loads a molecule trajectory. Will only work if molecule is already loaded
        public IEnumerator LoadMoleculeTrajectory(int moleculeID, string filePath) {

            if (!molecules.ContainsKey(moleculeID)) {

                MoleculeEvents.RaiseShowMessage("Can't load molecule trajectory. No molecule found.", true);
                yield break;
            }

            if (loadingTrajectory) {
                MoleculeEvents.RaiseShowMessage("Can't Load Trajectory: another trajectory currently loading", true);
                yield break;
            }

            PrimaryStructure primaryStructure = molecules[moleculeID].PrimaryStructure;
            int atomCount = loadTrajectoryAtomCount(filePath);

            if (atomCount != primaryStructure.AtomCount()) {

                MoleculeEvents.RaiseShowMessage("Trajectory atom count [" + atomCount + " doesn't match loaded structure atom count [" + primaryStructure.AtomCount() + "]", true);
                yield break;
            }

            MoleculeEvents.RaiseShowMessage("Loading trajectory. Please wait", false);

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

                MoleculeEvents.RaiseShowMessage("Error Loading Trajectory File: " + loadException, true);
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

            if (molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].SetRenderSettings(settings);
                molecules[moleculeID].SetFrameNumber(frameNumber);
                StartCoroutine(molecules[moleculeID].Render(generalSettings.MeshQuality));
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

        // Enables molecule movement input controls for the given molecule.
        // All other molecule movement controls are disabled
        public void SetMoleculeSelected(int moleculeID, bool selected) {

            if (molecules.ContainsKey(moleculeID)) {

                if(selected && moleculeMovementEnabled) {
                    molecules[moleculeID].EnableInput(true);
                }
                else {
                    molecules[moleculeID].EnableInput(false);
                }
            }

            if (selected) {
                selectedMolecule = moleculeID;
            }
            else {
                selectedMolecule = null;
            }
        }

        // This allows temporary disable of molecule movement.
        // This is used to stop molecule movement when user interface is accepting text input.
        public void EnableMoleculeInput(bool enable) {

            moleculeMovementEnabled = enable;

            foreach (Molecule molecule in molecules.Values) {
                molecule.EnableInput(false);
            }

            if (moleculeMovementEnabled && selectedMolecule != null && molecules.ContainsKey((int)selectedMolecule)) {
                molecules[(int)selectedMolecule].EnableInput(true);
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

                // check if the molecule was being monitored for interactions. If so, shut down monitoring
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
                MoleculeEvents.RaiseShowMessage("Error Loading Trajectory File: " + ex.Message, true);
            }

            return count;
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

                StartCoroutine(molecules[moleculeID].Render(generalSettings.MeshQuality));
            }
        }
    }
}
