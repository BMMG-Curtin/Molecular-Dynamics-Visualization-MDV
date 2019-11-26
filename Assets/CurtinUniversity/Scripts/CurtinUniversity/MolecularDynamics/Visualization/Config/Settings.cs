using UnityEngine;
using System.IO;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public static class Settings {

        // molecule loaded on start - adjustable in config file, see below
        public static bool LoadMoleculeOnStart { get; private set; }
        public static string LoadMoleculeFileName { get; private set; }

        // Colors for residue highlighting. 
        // Uses RGB values ranging from 0-255
        public static Color32 ResidueColour1 { get; private set; }
        public static Color32 ResidueColour2 { get; private set; }
        public static Color32 ResidueColour3 { get; private set; }
        public static Color32 ResidueColour4 { get; private set; }
        public static Color32 ResidueColour5 { get; private set; }
        public static Color32 ResidueColour6 { get; private set; }
        public static Color32 ResidueColour7 { get; private set; }
        public static Color32 ResidueColour8 { get; private set; }
        public static Color32 ResidueColour9 { get; private set; }
        public static Color32 ResidueColour10 { get; private set; }
        public static Color32 ResidueColour11 { get; private set; }
        public static Color32 ResidueColour12 { get; private set; }
        public static Color32 ResidueColour13 { get; private set; }
        public static Color32 ResidueColour14 { get; private set; }
        public static Color32 ResidueColour15 { get; private set; }
        // the default needs to be different than all the above colours - it's used in checks to see if an above color is selected
        public static Color32 ResidueColourDefault { get; private set; }

        // general render settings
        public static bool FlipZCoordinates { get; private set; }
        public static float ModelHoverHeight { get; private set; }
        public static Vector3 ModelCentre { get; private set; }
        public static float ModelBoxEdgeWidthDefault { get; private set; }

        // primary structure render settings
        public static float MaxAtomScale { get; private set; }
        public static float MinAtomScale { get; private set; }
        public static float DefaultAtomScale { get; private set; }
        public static float MaxBondScale { get; private set; }
        public static float MinBondScale { get; private set; }
        public static float DefaultBondScale { get; private set; }
        public static float CPKScaleFactor { get; private set; }
        public static string[] MeshQualityValues { get; private set; }
        public static int DefaultMeshQuality { get; private set; }
        public static int LowMeshQualityThreshold { get; private set; }
        public static int LowMeshQualityValue { get; private set; }
        public static bool DefaultAutoMeshQuality { get; private set; }

        // secondary structure settings
        public static Color32 AlphaHelixColour { get; private set; }
        public static Color32 BetaSheetColour { get; private set; }
        public static Color32 BetaBridgeColour { get; private set; }
        public static Color32 BendColour { get; private set; }
        public static Color32 TurnColour { get; private set; }
        public static Color32 FiveHelixColour { get; private set; }
        public static Color32 ThreeHelixColour { get; private set; }
        public static Color32 CoilColour { get; private set; }

        // trajectory settings
        public static int MinFrameAnimationSpeed { get; private set; }
        public static int MaxFrameAnimationSpeed { get; private set; }
        public static int DefaultFrameAnimationSpeed { get; private set; }
        public static float MinSecondsBetweenFrames { get; private set; }
        public static float MaxSecondsBetweenFrames { get; private set; }
        public static int MaxTrajectoryFrames { get; private set; }

        // box settings
        public static bool UseFileSimulationBox { get; private set; } // will use simulation box in structure file (is exists) instead of calculating bounding box from primary structure
        public static bool CalculateBoxEveryFrame { get; private set; } // recalculate bounding box for each frame based on coordinate shifts

        // user interface settings
        public static string StartMessage { get; private set; }
        public static int MinMouseSpeed { get; private set; }
        public static int MaxMouseSpeed { get; private set; }
        public static int DefaultMouseSpeed { get; private set; }
        public static int MouseSpeedMultiplier { get; private set; }

        // molecule file settings
        public static string[] ValidExtensions { get; private set; }
        public static string GromacsFileExtension { get; private set; }
        public static string[] StructureFileExtensions { get; private set; }
        public static string[] TrajectoryFileExtensions { get; private set; }
        public static string SettingsFileExtension { get; private set; }
        public static string StrideExecutablePath { get; private set; }
        public static string TmpFilePath { get; private set; }

        // molecule input settings
        public static int MinMoleculeMovementSpeed { get; private set; }
        public static int MaxMoleculeMovementSpeed { get; private set; }
        public static int DefaultMoleculeMovementSpeed { get; private set; }

        // lighting setttings
        public static int DefaultLightIntensity { get; private set; }
        public static int MaxLightIntensity { get; private set; }
        public static int MinLightIntensity { get; private set; }

        // animation setttings
        public static int DefaultAutoRotateSpeed { get; private set; }
        public static int MaxAutoRotateSpeed { get; private set; }
        public static int MinAutoRotateSpeed { get; private set; }

        // debug settings
        public static bool DebugMessages = true; // for the debug messages in the UI console
        public static bool DebugFlag = false; // used to swicth changes at runtime for debugging purposes

        // System Information
        public static int NumberOfProcessorCores = 6;

        public static void Load() {

            loadDefaults();
            loadFromSettingsFile();
        }

        private static void loadDefaults() {

            LoadMoleculeOnStart = false;
            LoadMoleculeFileName = "";

            ResidueColour1 = new Color32(255, 0, 0, 255);	  // red1
            ResidueColour2 = new Color32(255, 65, 65, 255);   // red2
            ResidueColour3 = new Color32(255, 125, 125, 255); // red3
            ResidueColour4 = new Color32(255, 195, 195, 255); // red4
            ResidueColour5 = new Color32(220, 220, 220, 255); // grey
            ResidueColour6 = new Color32(0, 0, 255, 255);	  // blue1
            ResidueColour7 = new Color32(65, 65, 255, 255);	  // blue2
            ResidueColour8 = new Color32(125, 125, 255, 255); // blue3
            ResidueColour9 = new Color32(195, 195, 255, 255); // blue4
            ResidueColour10 = new Color32(0, 255, 191, 255);  // cyan
            ResidueColour11 = new Color32(0, 255, 0, 255);    // green
            ResidueColour12 = new Color32(128, 255, 0, 255);  // lime
            ResidueColour13 = new Color32(255, 255, 0, 255);  // yellow
            ResidueColour14 = new Color32(255, 128, 0, 255);  // orange
            ResidueColour15 = new Color32(191, 0, 255, 255);  // purple

            ResidueColourDefault = new Color32(150, 150, 150, 255);   // grey (default needs to be different than any above)

            ModelHoverHeight = 2.5f;
            ModelCentre = new Vector3(0, 0, 0);
            ModelBoxEdgeWidthDefault = 0.05f;

            MaxAtomScale = 3f;
            MinAtomScale = 0.1f;
            DefaultAtomScale = 1f;
            MaxBondScale = 3f;
            MinBondScale = 0.1f;
            DefaultBondScale = 1f;
            CPKScaleFactor = 0.5f;
            MeshQualityValues = new[] { "Low", "Medium", "High" };
            DefaultMeshQuality = 1; // Index to the above meshQualityValues array = 2;
            LowMeshQualityThreshold = 30000; // over this amount of model atoms, app will change mesh quality to low
            LowMeshQualityValue = 0;
            DefaultAutoMeshQuality = true;

            AlphaHelixColour = new Color32(0, 0, 255, 1);     // blue
            BetaSheetColour = new Color32(255, 0, 0, 1);     // red
            BetaBridgeColour = new Color32(0, 0, 0, 1);       // black
            BendColour = new Color32(0, 128, 0, 1);     // green
            TurnColour = new Color32(255, 255, 0, 1);   // yellow
            FiveHelixColour = new Color32(128, 0, 128, 1);   // purple
            ThreeHelixColour = new Color32(128, 128, 128, 1); // grey
            CoilColour = new Color32(255, 255, 255, 1); // white

            MinFrameAnimationSpeed = 1;
            MaxFrameAnimationSpeed = 5;
            DefaultFrameAnimationSpeed = 3;
            MinSecondsBetweenFrames = 0.025f;
            MaxSecondsBetweenFrames = 0.25f;
            MaxTrajectoryFrames = 9999;

            StartMessage = "Welcome to Molecular Dynamics Visualization";
            MinMouseSpeed = 1;
            MaxMouseSpeed = 10;
            DefaultMouseSpeed = 5;
            MouseSpeedMultiplier = 5;

            MinMoleculeMovementSpeed = 1;
            MaxMoleculeMovementSpeed = 10;
            DefaultMoleculeMovementSpeed = 5;

            ValidExtensions = new[] { ".xyz", ".gro", ".pdb", ".xtc", ".dcd" };
            GromacsFileExtension = ".gro";
            StructureFileExtensions = new[] { ".gro", ".xyz", ".pdb" };
            TrajectoryFileExtensions = new[] { ".xtc", ".dcd" };
            SettingsFileExtension = ".mdv";
            StrideExecutablePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + @"stride_WIN32.exe";
            TmpFilePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar;

            DefaultLightIntensity = 5;
            MaxLightIntensity = 10;
            MinLightIntensity = 1;

            DefaultAutoRotateSpeed = 5;
            MaxAutoRotateSpeed = 10;
            MinAutoRotateSpeed = 1;
    }

        private static void loadFromSettingsFile() {

            Config.LoadConfig();

            // Start Molecule
            if (Config.KeyExists("LoadMoleculeOnStart"))
                LoadMoleculeOnStart = Config.GetBool("LoadMoleculeOnStart");
            if (Config.KeyExists("LoadMoleculeFileName"))
                LoadMoleculeFileName = Config.GetString("LoadMoleculeFileName");

            // Mesh Quality Settings
            if (Config.KeyExists("LowMeshQualityThreshold"))
                LowMeshQualityThreshold = Config.GetInt("LowMeshQualityThreshold");

            if (Config.KeyExists("NumberOfProcessorCores"))
                NumberOfProcessorCores = Config.GetInt("NumberOfProcessorCores");

            Debug.Log("Processor Cores: " + NumberOfProcessorCores);
        }
    }
}
