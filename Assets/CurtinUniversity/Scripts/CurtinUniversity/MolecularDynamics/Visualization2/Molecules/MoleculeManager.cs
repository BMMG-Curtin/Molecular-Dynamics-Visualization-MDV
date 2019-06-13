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

            // TODO
        }

        public void UpdateMoleculeRenderSettings(int moleculeID, MoleculeRenderSettings settings) {

            if(molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].RenderSettings = settings;
            }
        }

        public void ShowMolecule(int moleculeID, bool show) {

            if (molecules.ContainsKey(moleculeID)) {
                molecules[moleculeID].gameObject.SetActive(show);
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
                molecule.RenderSettings = settings;
                molecule.PrimaryStructure = primaryStructure;

                molecules.Add(moleculeID, molecule);

                MoleculeEvents.RaiseMoleculeLoaded(moleculeID, Path.GetFileName(filePath), primaryStructure.Title);
            }

            loadingFile = false;
        }
    }
}
