using System;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractionsRenderer : MonoBehaviour {

        [SerializeField]
        private GameObject atomPrefeb;

        private List<GameObject> displayedMolecules;

        private void Awake() {
            displayedMolecules = new List<GameObject>();
        }

        public void ShowInteractingMolecule(Vector3 moleculePosition, List<Vector3> atomPositions) {

            GameObject molecule = new GameObject();
            molecule.transform.position = moleculePosition;
            molecule.transform.SetParent(transform);

            GameObject moleculeCentre = GameObject.Instantiate(atomPrefeb);
            moleculeCentre.transform.position = Vector3.zero;
            moleculeCentre.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            moleculeCentre.transform.SetParent(molecule.transform, false);


            //if(atomPositions != null && atomPositions.Count > 0) {
            //    Debug.Log("Rendering position: " + atomPositions[0]);
            //}

            foreach (Vector3 position in atomPositions) {


                GameObject atomGO = GameObject.Instantiate(atomPrefeb);
                atomGO.transform.position = position;
                atomGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                atomGO.transform.SetParent(molecule.transform, true);
            }

            displayedMolecules.Add(molecule);
        }

        public void ClearAtoms() {

            foreach(GameObject molecule in displayedMolecules) {
                GameObject.Destroy(molecule);
            }

            displayedMolecules = new List<GameObject>();
        }
    }
}
