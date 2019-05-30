using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class MoleculesSettings : MonoBehaviour {

        public void OnloadFile1() {

            int id = 1;
            string filePath = @"D:\MolecularModels\MDV\Demo Files\Final Systems\AB.gro";
            MoleculeRenderSettings settings = MoleculeRenderSettings.Default();
            UserInterfaceEvents.RaiseOnLoadMolecule(id, filePath, settings);
        }

        public void OnloadFile2() {

            int id = 2;
            string filePath = @"D:\MolecularModels\MDV\Demo Files\protein+water.gro";
            MoleculeRenderSettings settings = MoleculeRenderSettings.Default();
            UserInterfaceEvents.RaiseOnLoadMolecule(id, filePath, settings);
        }

        public void OnDeleteFile1() {

            int id = 1;
            UserInterfaceEvents.RaiseOnRemoveMolecule(id);
        }

        public void OnDeleteFile2() {

            int id = 2;
            UserInterfaceEvents.RaiseOnRemoveMolecule(id);
        }
    }
}
