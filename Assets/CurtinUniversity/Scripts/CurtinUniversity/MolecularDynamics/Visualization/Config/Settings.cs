using UnityEngine;
using System.IO;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public static class Settings {

        // settings adjustable in config file

        public static bool LoadMoleculeOnStart { get; private set; } = false;
        public static string LoadMoleculeFileName { get; private set; } = "";

        public static Color32 ResidueColour1 { get; private set; } = new Color32(255, 0, 0, 255);   // red
        public static Color32 ResidueColour2 { get; private set; } = new Color32(255, 128, 0, 255); // orange
        public static Color32 ResidueColour3 { get; private set; } = new Color32(255, 255, 0, 255); // yellow
        public static Color32 ResidueColour4 { get; private set; } = new Color32(128, 255, 0, 255); // lime
        public static Color32 ResidueColour5 { get; private set; } = new Color32(0, 255, 0, 255);   // green
        public static Color32 ResidueColour6 { get; private set; } = new Color32(0, 255, 191, 255); // cyan
        public static Color32 ResidueColour7 { get; private set; } = new Color32(0, 191, 255, 255); // blue
        public static Color32 ResidueColour8 { get; private set; } = new Color32(0, 0, 255, 255);   // dark blue
        public static Color32 ResidueColour9 { get; private set; } = new Color32(191, 0, 255, 255); // purple
        public static Color32 ResidueColour10 { get; private set; } = new Color32(255, 0, 191, 255); // pink
        public static Color32 ResidueColourDefault { get; private set; } = new Color32(255, 0, 0, 255);   // red


        // these settings are contained to this class and will not be overridden by the config file

        // general render settings

        public static bool FlipZCoordinates { get; private set; }           = true; // chemistry simulation coordinate systems are left handed, Unity coordinate system is right handed. This setting will flip z coordinates in the model views to compensate. 
        public static float ModelHoverHeight { get; private set; }          = 2.5f;
        public static Vector3 ModelCentre { get; private set; }             = new Vector3(0, 0, 0);
        public static float ModelBoxEdgeWidthDefault { get; private set; }  = 0.05f;

        // primary structure render settings

        public static float MaxAtomScale { get; private set; }      = 3f;
        public static float MinAtomScale { get; private set; }      = 0.1f;
        public static float DefaultAtomScale { get; private set; }  = 1f;
        public static float MaxBondScale { get; private set; }      = 3f;
        public static float MinBondScale { get; private set; }      = 0.1f;
        public static float DefaultBondScale { get; private set; }  = 1f;
        public static float CPKScaleFactor { get; private set; }    = 0.5f;

        public static string[] AtomMeshQualityValues { get; private set; }  = new[] { "Low", "Medium", "High" };
        public static string[] BondMeshQualityValues { get; private set; }  = new[] { "Low", "Medium", "High" };
        public static int DefaultAtomMeshQuality { get; private set; }      = 1; // Index to the above meshQualityValues array = 2;
        public static int DefaultBondMeshQuality { get; private set; }      = 1; // Index to the above meshQualityValues array = 2;
        public static int LowMeshQualityThreshold { get; private set; }     = 30000; // over this amount of model atoms, app will change mesh quality to low
        public static int LowMeshQualityValue { get; private set; }         = 0;

        // secondary structure settings

        public static Color32 AlphaHelixColour { get; private set; } = new Color32(0, 0, 255, 1);     // blue
        public static Color32 BetaSheetColour { get; private set; }  = new Color32(255, 0, 0, 1);     // red
        public static Color32 BetaBridgeColour { get; private set; } = new Color32(0, 0, 0, 1);       // black
        public static Color32 BendColour { get; private set; }       = new Color32(0, 128, 0, 1);     // green
        public static Color32 TurnColour { get; private set; }       = new Color32(255, 255, 0, 1);   // yellow
        public static Color32 FiveHelixColour { get; private set; }  = new Color32(128, 0, 128, 1);   // purple
        public static Color32 ThreeHelixColour { get; private set; } = new Color32(128, 128, 128, 1); // grey
        public static Color32 CoilColour { get; private set; }       = new Color32(255, 255, 255, 1); // white

        // trajectory settings

        public static int MinFrameAnimationSpeed { get; private set; }      = 1;
        public static int MaxFrameAnimationSpeed { get; private set; }      = 5;
        public static float MinSecondsBetweenFrames { get; private set; }   = 0.1f;
        public static float MaxSecondsBetweenFrames { get; private set; }   = 0.5f;
        public static int MaxTrajectoryFrames { get; private set; }         = 9999;

        // box settings

        public static bool UseFileSimulationBox { get; private set; } // will use simulation box in structure file (is exists) instead of calculating bounding box from primary structure
        public static bool CalculateBoxEveryFrame { get; private set; } // recalculate bounding box for each frame based on coordinate shifts

        // user interface settings

        public static string StartMessage { get; private set; }      = "Welcome to the Molecular Dynamics Viewer";
        public static int MinMouseSpeed { get; private set; }        = 1;
        public static int MaxMouseSpeed { get; private set; }        = 10;
        public static int MouseSpeedMultiplier { get; private set; } = 3;

        // molecule file settings

        public static string[] ValidExtensions { get; private set; }          = new[] { ".xyz", ".gro", ".pdb", ".xtc", ".dcd" };
        public static string GromacsFileExtension { get; private set; }       = ".gro";
        public static string[] StructureFileExtensions { get; private set; }  = new[] { ".gro", ".xyz", ".pdb" };
        public static string[] TrajectoryFileExtensions { get; private set; } = new[] { ".xtc", ".dcd" };
        public static string StrideExecutablePath { get; private set; }       = Application.streamingAssetsPath + Path.DirectorySeparatorChar + @"stride_WIN32.exe";
        public static string TmpFilePath { get; private set; }                = Application.streamingAssetsPath + Path.DirectorySeparatorChar;

        // lighting setttings

        public static float DefaultFillLightBrightness { get; private set; } = 0.2f;
        public static float DefaultSpotLightBrightness { get; private set; } = 0.3f;
        public static float DefaultMainLightBrightness { get; private set; } = 0.3f;
        public static float DefaultAreaLightBrightness { get; private set; } = 0.3f;
        
        // debug settings

        public static bool DebugMessages = true; // for the debug messages in the UI console
        public static bool DebugFlag = false; // used to swicth changes at runtime for debugging purposes


        // load default settings from config file
        // some settings may be overwritten in relevant classes by stored player preferences 
        public static void Load() {

            Config.LoadConfig();

            // Start Molecule
            if (Config.KeyExists("LoadMoleculeOnStart"))
                LoadMoleculeOnStart = Config.GetBool("LoadMoleculeOnStart");
            if (Config.KeyExists("LoadMoleculeFileName")) 
                LoadMoleculeFileName = Config.GetString("LoadMoleculeFileName");

            // residue custom colours
            if (Config.KeyExists("ResidueColour1"))
                ResidueColour1 = Config.GetRGB("ResidueColour1");
            if (Config.KeyExists("ResidueColour2"))
                ResidueColour2 = Config.GetRGB("ResidueColour2");
            if (Config.KeyExists("ResidueColour3"))
                ResidueColour3 = Config.GetRGB("ResidueColour3");
            if (Config.KeyExists("ResidueColour4"))
                ResidueColour4 = Config.GetRGB("ResidueColour4");
            if (Config.KeyExists("ResidueColour5"))
                ResidueColour5 = Config.GetRGB("ResidueColour5");
            if (Config.KeyExists("ResidueColour6"))
                ResidueColour6 = Config.GetRGB("ResidueColour6");
            if (Config.KeyExists("ResidueColour7"))
                ResidueColour7 = Config.GetRGB("ResidueColour7");
            if (Config.KeyExists("ResidueColour8"))
                ResidueColour8 = Config.GetRGB("ResidueColour8");
            if (Config.KeyExists("ResidueColour9"))
                ResidueColour9 = Config.GetRGB("ResidueColour9");
            if (Config.KeyExists("ResidueColour10"))
                ResidueColour10 = Config.GetRGB("ResidueColour10");
            if (Config.KeyExists("ResidueColourDefault"))
                ResidueColourDefault = Config.GetRGB("ResidueColourDefault");
        }
    }
}
