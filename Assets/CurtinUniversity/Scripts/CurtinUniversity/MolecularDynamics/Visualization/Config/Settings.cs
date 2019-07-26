using UnityEngine;
using System.IO;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public enum Platform {
        Desktop,
        SteamVR,
    }

    public static class Settings {

        public static bool HideHardwareMouseCursor = true;

        public static bool SmoothRibbons { get; set; }

        public static float MaxAtomScale { get; set; }
        public static float MinAtomScale { get; set; }
        public static float DefaultAtomScale { get; set; }
        public static float MaxBondScale { get; set; }
        public static float MinBondScale { get; set; }
        public static float DefaultBondScale { get; set; }
        public static float MaxBondLength { get; set; }

        public static bool RecalculateBondsOnNewFrame = false;

        public static int MinFrameAnimationSpeed { get; set; }
        public static int MaxFrameAnimationSpeed { get; set; }
        public static float MinSecondsBetweenFrames { get; set; }
        public static float MaxSecondsBetweenFrames { get; set; }

        public static int MaxTrajectoryFrames { get; set; }

        public static string[] AtomMeshQualityValues { get; set; }
        public static string[] BondMeshQualityValues { get; set; }
        public static int AtomMeshQuality { get; set; }
        public static int BondMeshQuality { get; set; }
        public static int DefaultAtomMeshQuality { get; set; }
        public static int DefaultBondMeshQuality { get; set; }
        public static int LowMeshQualityThreshold { get; set; }
        public static int LowMeshQualityValue { get; set; }

        public static Color32 ResidueColour1 { get; set; }
        public static Color32 ResidueColour2 { get; set; }
        public static Color32 ResidueColour3 { get; set; }
        public static Color32 ResidueColour4 { get; set; }
        public static Color32 ResidueColour5 { get; set; }
        public static Color32 ResidueColour6 { get; set; }
        public static Color32 ResidueColour7 { get; set; }
        public static Color32 ResidueColour8 { get; set; }
        public static Color32 ResidueColour9 { get; set; }
        public static Color32 ResidueColour10 { get; set; }
        public static Color32 ResidueColourDefault { get; set; }

        /// NON USER SETTINGS (for adjustment before compile)

        public static bool DebugMessages = true; // for the debug messages in the UI console
        public static Color32 AlphaHelixColour { get; set; }
        public static Color32 BetaSheetColour { get; set; }
        public static Color32 BetaBridgeColour { get; set; }
        public static Color32 BendColour { get; set; }
        public static Color32 TurnColour { get; set; }
        public static Color32 FiveHelixColour { get; set; }
        public static Color32 ThreeHelixColour { get; set; }
        public static Color32 CoilColour { get; set; }
        public static bool DebugFlag = false; // used to swith changes at runtime for debugging purposes
        public static float CPKScaleFactor { get; set; }
        public static bool FlipZCoordinates { get; set; } // chemistry simulation coordinate systems are left handed, Unity coordinate system is right handed. This setting will flip z coordinates in the model views to compensate. 
        public static float ModelHoverHeight { get; set; }
        public static Vector3 ModelCentre { get; set; }
        public static float ModelBoxEdgeWidthDefault { get; set; }
        public static bool UseFileSimulationBox { get; set; } // will use simulation box in structure file (is exists) instead of calculating bounding box from primary structure
        public static bool CalculateBoxEveryFrame { get; set; } // recalculate bounding box for each frame based on coordinate shifts

        public static int MinMouseSpeed { get; set; }
        public static int MaxMouseSpeed { get; set; }
        public static int MouseSpeedMultiplier { get; set; }
        public static string StartMessage { get; set; }
        public static string[] ValidExtensions { get; set; }
        public static string GromacsFileExtension { get; set; }
        public static string[] StructureFileExtensions { get; set; }
        public static string[] TrajectoryFileExtensions { get; set; }
        public static string[] ColourFileExtensions { get; set; }
        public static float DefaultFillLightBrightness { get; set; }
        public static float DefaultSpotLightBrightness { get; set; }
        public static float DefaultMainLightBrightness { get; set; }
        public static float DefaultAreaLightBrightness { get; set; }
        public static string StrideExecutablePath { get; set; }
        public static string TmpFilePath { get; set; }

        public static void Load() {

            loadDefaultApplicationSettings();
            loadConfigSettings();
        }

        // Set settings defaults. 
        // Some settings will be overwritten by the config file values loaded below but left here in case config file fields are mistyped/missing
        // Some settings will be ignored due to stored player prefs loaded by classes using the settings
        private static void loadDefaultApplicationSettings() {

            MaxAtomScale = 3f;
            MinAtomScale = 0.1f;
            DefaultAtomScale = 1f;
            MaxBondScale = 3f;
            MinBondScale = 0.1f;
            DefaultBondScale = 1f;
            MaxBondLength = 0.155f;
            AtomMeshQualityValues = new[] { "Cuboid", "Low", "Medium", "High" };
            BondMeshQualityValues = new[] { "Cuboid", "Low", "Medium", "High" };
            DefaultAtomMeshQuality = 2; // Index to the above meshQualityValues array
            DefaultBondMeshQuality = 2; // Index to the above meshQualityValues array
            AtomMeshQuality = DefaultAtomMeshQuality;
            BondMeshQuality = DefaultBondMeshQuality;
            LowMeshQualityThreshold = 30000; // over this amount of model atoms, app will change mesh quality to low
            LowMeshQualityValue = 1;

            MaxTrajectoryFrames = 9999;

            MinFrameAnimationSpeed = 1;
            MaxFrameAnimationSpeed = 5;
            MinSecondsBetweenFrames = 0.1f;
            MaxSecondsBetweenFrames = 0.5f;
            
            MinMouseSpeed = 1;
            MaxMouseSpeed = 10;
            MouseSpeedMultiplier = 3;

            CPKScaleFactor = 0.5f;
            FlipZCoordinates = true;
            ModelHoverHeight = 2.5f;
            ModelCentre = new Vector3(0, 0, 0);
            ModelBoxEdgeWidthDefault = 0.05f;
            
            AlphaHelixColour = new Color32(0, 0, 255, 1);     // blue
            BetaSheetColour = new Color32(255, 0, 0, 1);     // red
            BetaBridgeColour = new Color32(0, 0, 0, 1);       // black
            BendColour = new Color32(0, 128, 0, 1);     // green
            TurnColour = new Color32(255, 255, 0, 1);   // yellow
            FiveHelixColour = new Color32(128, 0, 128, 1);   // purple
            ThreeHelixColour = new Color32(128, 128, 128, 1); // grey
            CoilColour = new Color32(255, 255, 255, 1); // white
            ResidueColour1 = new Color32(255, 0, 0, 255);   // red
            ResidueColour2 = new Color32(255, 128, 0, 255); // orange
            ResidueColour3 = new Color32(255, 255, 0, 255); // yellow
            ResidueColour4 = new Color32(128, 255, 0, 255); // lime
            ResidueColour5 = new Color32(0, 255, 0, 255);   // green
            ResidueColour6 = new Color32(0, 255, 191, 255); // cyan
            ResidueColour7 = new Color32(0, 191, 255, 255); // blue
            ResidueColour8 = new Color32(0, 0, 255, 255);   // dark blue
            ResidueColour9 = new Color32(191, 0, 255, 255); // purple
            ResidueColour10 = new Color32(255, 0, 191, 255); // pink
            ResidueColourDefault = ResidueColour1;

            StartMessage = "Welcome to the Molecular Dynamics Viewer";
            ValidExtensions = new[] { ".xyz", ".gro", ".pdb", ".xtc", ".dcd" };
            GromacsFileExtension = ".gro";
            StructureFileExtensions = new[] { ".gro", ".xyz", ".pdb" };
            TrajectoryFileExtensions = new[] { ".xtc", ".dcd" };
            ColourFileExtensions = new[] { ".col" };
            DefaultFillLightBrightness = 0.2f;
            DefaultSpotLightBrightness = 0.3f;
            DefaultMainLightBrightness = 0.3f;
            DefaultAreaLightBrightness = 0.3f;

            StrideExecutablePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + @"stride_WIN32.exe";
            TmpFilePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar;

            Debug.Log("StrideExePath: " + StrideExecutablePath + ", TMPFilePath: " + TmpFilePath);
        }

        /// load default settings from config file
        /// some settings may be overwritten in relevant classes by stored player preferences 
        private static void loadConfigSettings() {

            Config.LoadConfig();

            // bond options
            if (Config.KeyExists("MaxBondLength"))
                MaxBondLength = Config.GetFloat("MaxBondLength");

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
