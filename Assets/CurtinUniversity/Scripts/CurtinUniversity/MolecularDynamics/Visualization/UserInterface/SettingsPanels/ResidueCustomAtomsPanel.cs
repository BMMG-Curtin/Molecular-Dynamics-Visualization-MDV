using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueCustomAtomsPanel : MonoBehaviour {

        public void Initialise() {




            // create atom option buttons

            //List<string> atomNames = new List<string>();
            //foreach(KeyValuePair<int, Atom> atom in residue.Atoms) {
            //    if(!atomNames.Contains(atom.Value.Name)) {
            //        atomNames.Add(atom.Value.Name);
            //    }
            //}

            //foreach(string atomName in atomNames) {

            //    GameObject atomOptionsButton = GameObject.Instantiate(AtomNameButtonPrefab);
            //    atomOptionsButton.transform.SetParent(AtomNameListContentPanel.transform);
            //    AtomNameButton buttonScript = atomOptionsButton.GetComponent<AtomNameButton>();

            //    if(renderSettings.AtomDisplayOptions.ContainsKey(atomName)) {
            //        renderSettings.AtomDisplayOptions.Add(atomName, new AtomRenderSettings(atomName, Settings.ResidueColourDefault));
            //    }

            //    buttonScript.Initialise(renderSettings.AtomDisplayOptions[atomName]);
            //}
        }
    }
}
