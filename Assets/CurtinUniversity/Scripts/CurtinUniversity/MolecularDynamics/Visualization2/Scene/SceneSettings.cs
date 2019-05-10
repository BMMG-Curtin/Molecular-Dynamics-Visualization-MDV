using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public struct SceneSettings {

        public bool ShowGround { get; set; }
        public bool ShowShadows { get; set; }
        public bool LightsOn { get; set; } 

        public static SceneSettings Default() {

            return new SceneSettings {
                ShowGround = true,
                ShowShadows = true,
                LightsOn = true,
            };
        }
    }
}
