using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Model.Analysis;
using CurtinUniversity.MolecularDynamics.Model.FileParser;
using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public delegate void OnLoadedMolecule();
    public delegate void OnLoadMoleculeMessage(string message, bool error);
    
    public class Molecules : MonoBehaviour {

        [SerializeField]
        private GameObject MoleculePrefab;

        private Dictionary<int, Molecule> molecules;

        private bool loadingFile;

        private void Awake() {
            loadingFile = false;
        }

        public void LoadMolecule(string fileName, MoleculeRenderSettings settings, OnLoadMoleculeMessage message, OnLoadedMolecule onLoaded) {

            if (!loadingFile) {
                StartCoroutine(loadMolecule(fileName, settings,  message, onLoaded));
            }
        }

        public void LoadMoleculeTrajectory(int moleculeID, string fileName, OnLoadMoleculeMessage message, OnLoadedMolecule onLoaded) {

        }

        public void SetMolecularRepresentation(int moleculeID, string fileName, OnLoadMoleculeMessage message) {

        }

        private IEnumerator loadMolecule(string fileName, MoleculeRenderSettings renderSettings, OnLoadMoleculeMessage message, OnLoadedMolecule moleculeLoaded) {

            loadingFile = true;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            message("Loading Structure File: " + fileName, false);
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
                message("Error Loading Structure File: " + ex.Message, true);
                loadingFile = false;
                yield break;
            }


            watch.Stop();
            message("Structure File Load Complete [" + watch.ElapsedMilliseconds + "ms]", false);
            yield return new WaitForSeconds(0.05f);

            if (primaryStructure != null) {

                GameObject moleculeGO = GameObject.Instantiate(MoleculePrefab);
                moleculeGO.transform.parent = this.transform;

                Molecule molecule = moleculeGO.GetComponent<Molecule>();
                molecule.PrimaryStructure = primaryStructure;
                molecule.MoleculeRenderSettings = renderSettings;

                // callback
                if (moleculeLoaded != null) {
                    moleculeLoaded();
                }
            }

            loadingFile = false;
        }
    }
}
