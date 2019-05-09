using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class SceneSettings {

        public bool ShowGround { get; set; }
        public bool ShowShadows { get; set; }
        public bool ShowLights { get; set; } // turn off spotlighting, ambient light only
        public bool ShowLightGlobes { get; set; } // light globes in scene, not actual lighting

        public static int AtomMeshQuality { get; set; }
        public static int BondMeshQuality { get; set; }

        public SceneSettings() {

            ShowGround = true;
            ShowShadows = true;
            ShowLights = true;
            ShowLightGlobes = true;
        }
    }
}
