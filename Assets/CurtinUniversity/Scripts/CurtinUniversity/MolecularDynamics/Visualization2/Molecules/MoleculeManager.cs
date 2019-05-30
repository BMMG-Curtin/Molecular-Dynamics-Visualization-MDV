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

        public void LoadMolecule(int moleculeID, string fileName, MoleculeRenderSettings settings) {

            if(molecules.ContainsKey(moleculeID)) {

                MoleculeEvents.RaiseOnRenderMessage("Error Loading Structure File: already loaded", true);
                return;
            }

            if (!loadingFile) {
                StartCoroutine(loadMolecule(moleculeID, fileName, settings));
            }
        }

        public void LoadMoleculeTrajectory(int moleculeID, string fileName) {

            // TODO
        }

        public void UpdateMoleculeRenderSettings(int moleculeID, MoleculeRenderSettings settings) {

            if(molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].MoleculeRenderSettings = settings;
            }
        }

        private IEnumerator loadMolecule(int moleculeID, string fileName, MoleculeRenderSettings renderSettings) {

            loadingFile = true;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            MoleculeEvents.RaiseOnRenderMessage("Loading Structure File: " + fileName, false);
            yield return new WaitForSeconds(0.05f);

            PrimaryStructure primaryStructure = null;

            try {
                if (fileName.EndsWith(".gro")) {
                    primaryStructure = GROStructureParser.GetStructure(fileName);
                }
                else if (fileName.EndsWith(".xyz")) {
                    primaryStructure = XYZStructureParser.GetStructure(fileName);
                }
                else if (fileName.EndsWith(".pdb")) {
                    primaryStructure = PDBStructureParser.GetPrimaryStructure(fileName);
                }
            }
            catch (FileParseException ex) {

                Debug.Log("Error Loading Structure File: " + ex.Message);
                MoleculeEvents.RaiseOnRenderMessage("Error Loading Structure File: " + ex.Message, true);
                loadingFile = false;
                yield break;
            }


            watch.Stop();
            MoleculeEvents.RaiseOnRenderMessage("Structure File Load Complete [" + watch.ElapsedMilliseconds + "ms]", false);
            yield return new WaitForSeconds(0.05f);

            if (primaryStructure != null) {

                GameObject moleculeGO = GameObject.Instantiate(MoleculePrefab);
                moleculeGO.transform.parent = this.transform;
                moleculeGO.SetActive(true);

                Molecule molecule = moleculeGO.GetComponent<Molecule>();
                molecule.MoleculeRenderSettings = renderSettings;
                molecule.PrimaryStructure = primaryStructure;

                molecules.Add(moleculeID, molecule);

                MoleculeEvents.RaiseOnLoadedMolecule();
            }

            loadingFile = false;
        }
    }
}
