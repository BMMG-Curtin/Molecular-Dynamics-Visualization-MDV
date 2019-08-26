using UnityEngine;
using System.IO;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public static class Settings {

        // molecule loaded on start - adjustable in config file, see below
        public static bool LoadMoleculeOnStart { get; private set; } = false;
        public static string LoadMoleculeFileName { get; private set; } = "";

        // Colors for residue highlighting. 
        // Uses RGB values ranging from 0-255
        public static Color32 ResidueColour1  { get; private set; } = new Color32 (255, 0, 0, 255);		// red1
        public static Color32 ResidueColour2  { get; private set; } = new Color32 (255, 65, 65, 255);   // red2
        public static Color32 ResidueColour3  { get; private set; } = new Color32 (255, 125, 125, 255); // red3
        public static Color32 ResidueColour4  { get; private set; } = new Color32 (255, 195, 195, 255); // red4
        public static Color32 ResidueColour5  { get; private set; } = new Color32 (255, 255, 0, 255); 	// yellow
        public static Color32 ResidueColour6  { get; private set; } = new Color32 (220, 220, 220, 255);	// grey
        public static Color32 ResidueColour7  { get; private set; } = new Color32 (195, 195, 255, 255); // blue1
        public static Color32 ResidueColour8  { get; private set; } = new Color32 (125, 125, 255, 255);	// blue2
        public static Color32 ResidueColour9  { get; private set; } = new Color32 (65, 65, 255, 255);	// blue3
        public static Color32 ResidueColour10 { get; private set; } = new Color32 (0, 0, 255, 255);	    // blue4
        public static Color32 ResidueColour11 { get; private set; } = new Color32 (255, 128, 0, 255);	// orange
        public static Color32 ResidueColour12 { get; private set; } = new Color32 (128, 255, 0, 255);	// lime
        public static Color32 ResidueColour13 { get; private set; } = new Color32 (0, 255, 0, 255);		// green
        public static Color32 ResidueColour14 { get; private set; } = new Color32 (0, 255, 191, 255); 	// cyan
        public static Color32 ResidueColour15 { get; private set; } = new Color32 (191, 0, 255, 255);	// purple
        // the default needs to be different than all the above colours - it's used in checks to see if an above color is selected
        public static Color32 ResidueColourDefault { get; private set; } = new Color32(150, 150, 150, 255);   // grey

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
        public static string[] MeshQualityValues { get; private set; }  = new[] { "Low", "Medium", "High" };
        public static int DefaultMeshQuality { get; private set; }      = 1; // Index to the above meshQualityValues array = 2;
        public static int LowMeshQualityThreshold { get; private set; } = 30000; // over this amount of model atoms, app will change mesh quality to low
        public static int LowMeshQualityValue { get; private set; }     = 0;
        public static bool DefaultAutoMeshQuality { get; private set; } = true;

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
        public static string MDVSettingsFileExtension            = ".mdv";
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

            // Mesh Quality Settings
            if (Config.KeyExists("LowMeshQualityThreshold"))
                LowMeshQualityThreshold = Config.GetInt("LowMeshQualityThreshold");
        }
    }
}
