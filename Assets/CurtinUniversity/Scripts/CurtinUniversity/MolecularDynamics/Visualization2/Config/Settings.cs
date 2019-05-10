using UnityEngine;
using System.IO;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public enum Platform {
        Desktop,
        SteamVR,
    }

    public static class Settings {

        public static bool Loaded = false;

        // public Platform DisplayPlatform;

        public static bool HideHardwareMouseCursor = true;

        /// USER ADJUSTABLE SETTINGS (either through UI or config file)

        public static bool EnablePrimaryStructure { get; set; }
        public static bool ShowBonds { get; set; }
        public static bool ShowAtoms { get; set; }
        public static bool ShowStandardResidues { get; set; }
        public static bool ShowNonStandardResidues { get; set; }
        public static bool ShowMainChains { get; set; }
        public static bool ShowSimulationBox { get; set; }
        public static MolecularRepresentation Representation { get; set; }
        public static bool EnableSecondaryStructure { get; set; }
        public static bool ShowHelices { get; set; }
        public static bool ShowSheets { get; set; }
        public static bool ShowTurns { get; set; }
        public static bool ShowGround { get; set; }
        public static bool ShowShadows { get; set; }
        public static bool ShowLights { get; set; } // turn off spotlighting, ambient light only
        public static bool ShowLightGlobes { get; set; } // light globes in scene, not actual lighting
        public static bool ModelRotate { get; set; }
        public static bool ToggleStructures { get; set; } // determines if enabling primary structure disables the secondary 
        public static bool SmoothRibbons { get; set; }
        public static float MaxModelScale { get; set; }
        public static float MinModelScale { get; set; }
        public static float DefaultModelScale { get; set; }
        public static float MaxAtomScale { get; set; }
        public static float MinAtomScale { get; set; }
        public static float DefaultAtomScale { get; set; }
        public static float MaxBondScale { get; set; }
        public static float MinBondScale { get; set; }
        public static float DefaultBondScale { get; set; }
        public static float MaxBondLength { get; set; }
        public static bool GenerateBonds = true;
        public static bool GenerateBondsOnModelLoad = true;
        public static bool RecalculateBondsOnNewFrame = false;
        public static int MaxFrameAnimationSpeed { get; set; }
        public static int FrameAnimationSpeed { get; set; }
        public static float MaxSecondsBetweenFrames { get; set; }
        public static int DefaultTrajectoryStartFrame { get; set; }
        public static int MinTrajectoryStartFrame { get; set; }
        public static int MaxTrajectoryStartFrame { get; set; }
        public static int DefaultTrajectoryFrameCount { get; set; }
        public static int MinTrajectoryFrameCount { get; set; }
        public static int MaxTrajectoryFrameCount { get; set; }
        public static int DefaultTrajectoryFrameFrequency { get; set; }
        public static int MinTrajectoryFrameFrequency { get; set; }
        public static int MaxTrajectoryFrameFrequency { get; set; }
        public static string[] AtomMeshQualityValues { get; set; }
        public static string[] BondMeshQualityValues { get; set; }
        public static int AtomMeshQuality { get; set; }
        public static int BondMeshQuality { get; set; }
        public static int DefaultAtomMeshQuality { get; set; }
        public static int DefaultBondMeshQuality { get; set; }
        public static int LowMeshQualityThreshold { get; set; }
        public static int LowMeshQualityValue { get; set; }
        public static int SceneMouseCursorSpeed { get; set; }
        public static int GUIMouseCursorSpeed { get; set; }
        public static float UIDistance { get; set; }
        public static float MinUIDistance { get; set; }
        public static float MaxUIDistance { get; set; }
        public static float UIDistanceStep { get; set; }
        // Trajectory color values. Colour values loaded from colour file are in 0-1 range (HSV base).
        public static float MinColourHue { get; set; }
        public static float MaxColourHue { get; set; }
        public static float MinColourValue { get; set; }
        public static float MidColourValue { get; set; }
        public static float MaxColourValue { get; set; }
        public static int ColourBands { get; set; }
        public static float HighlightedBondColourHue { get; set; }

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
        public static int MinSceneMouseCursorSpeed { get; set; }
        public static int MaxSceneMouseCursorSpeed { get; set; }
        public static int DefaultSceneMouseCursorSpeed { get; set; }
        public static int SceneMouseCursorSpeedMultiplier { get; set; }
        public static int MinGUIMouseCursorSpeed { get; set; }
        public static int MaxGUIMouseCursorSpeed { get; set; }
        public static int DefaultGUIMouseCursorSpeed { get; set; }
        public static int GUIMouseCursorSpeedMultiplier { get; set; }
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
        public static float DefaultLightHaloSize { get; set; }
        public static float GeneralLightBrightness { get; set; }
        public static string StrideExecutablePath { get; set; }
        public static string TmpFilePath { get; set; }

        public static Resolution DefaultResolution = new Resolution();

        public static void Load() {

            loadDefaultApplicationSettings();
            loadConfigSettings();

            DefaultResolution.width = 1280;
            DefaultResolution.height = 768;

            Loaded = true;
        }

        // Set settings defaults. 
        // Some settings will be overwritten by the config file values loaded below but left here in case config file fields are mistyped/missing
        // Some settings will be ignored due to stored player prefs loaded by classes using the settings
        private static void loadDefaultApplicationSettings() {

            //DisplayPlatform = Platform.Desktop;
            EnablePrimaryStructure = true;
            ShowAtoms = false;
            ShowBonds = true;
            ShowStandardResidues = true;
            ShowNonStandardResidues = true;
            ShowMainChains = false;
            ShowSimulationBox = false;
            EnableSecondaryStructure = false;
            ShowHelices = true;
            ShowSheets = true;
            ShowTurns = true;
            Representation = MolecularRepresentation.CPK;
            ShowGround = true;
            ShowShadows = true;
            ShowLights = true;
            ModelRotate = true;
            ToggleStructures = false;
            SmoothRibbons = true;
            MaxModelScale = 3f;
            MinModelScale = 0.1f;
            DefaultModelScale = 1f;
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
            DefaultTrajectoryStartFrame = 0;
            MinTrajectoryStartFrame = 0;
            MaxTrajectoryStartFrame = 1000;
            DefaultTrajectoryFrameCount = 100;
            MinTrajectoryFrameCount = 1;
            MaxTrajectoryFrameCount = 1000;
            DefaultTrajectoryFrameFrequency = 1;
            MinTrajectoryFrameFrequency = 1;
            MaxTrajectoryFrameFrequency = 1000;
            MaxFrameAnimationSpeed = 10;
            FrameAnimationSpeed = 5;
            MaxSecondsBetweenFrames = 0.5f;
            GUIMouseCursorSpeed = 5;
            MinGUIMouseCursorSpeed = 1;
            MaxGUIMouseCursorSpeed = 10;
            GUIMouseCursorSpeedMultiplier = 4;
            SceneMouseCursorSpeed = 5;
            MinSceneMouseCursorSpeed = 1;
            MaxSceneMouseCursorSpeed = 10;
            SceneMouseCursorSpeedMultiplier = 30;
            CPKScaleFactor = 0.5f;
            FlipZCoordinates = true;
            ModelHoverHeight = 2.5f;
            ModelCentre = new Vector3(0, 0, 0);
            ModelBoxEdgeWidthDefault = 0.05f;
            UseFileSimulationBox = false;
            CalculateBoxEveryFrame = false;
            MinColourHue = 360f / 360f; // red
            MaxColourHue = 205f / 360f; // blue
            MinColourValue = 0.3f;
            MidColourValue = 0.5f;
            MaxColourValue = 0.7f;
            ColourBands = 5;
            HighlightedBondColourHue = 360f / 360f;
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
            UIDistance = 1f;
            MinUIDistance = 0.9f;
            MaxUIDistance = 2f;
            UIDistanceStep = 0.1f;
            StartMessage = "Welcome to the Molecular Dynamics Viewer";
            ValidExtensions = new[] { ".xyz", ".gro", ".pdb", ".xtc", ".dcd", ".col" };
            GromacsFileExtension = ".gro";
            StructureFileExtensions = new[] { ".gro", ".xyz", ".pdb" };
            TrajectoryFileExtensions = new[] { ".xtc", ".dcd" };
            ColourFileExtensions = new[] { ".col" };
            DefaultFillLightBrightness = 0.2f;
            DefaultSpotLightBrightness = 0.3f;
            DefaultMainLightBrightness = 0.3f;
            DefaultAreaLightBrightness = 0.3f;
            DefaultLightHaloSize = 0.3f;
            GeneralLightBrightness = 1f;

            StrideExecutablePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + @"stride_WIN32.exe";
            TmpFilePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar;

            Debug.Log("StrideExePath: " + StrideExecutablePath + ", TMPFilePath: " + TmpFilePath);
        }

        /// load default settings from config file
        /// some settings may be overwritten in relevant classes by stored player preferences 
        private static void loadConfigSettings() {

            Config.LoadConfig();

            //// platform option
            //if (Config.KeyExists("Platform")) {
            //    switch (Config.GetString("Platform").ToUpper()) {
            //        case "DESKTOP":
            //            DisplayPlatform = Platform.Desktop;
            //            break;
            //        case "STEAMVR":
            //            DisplayPlatform = Platform.SteamVR;
            //            break;
            //    }
            //}

            //Debug.Log("Display Platform: " + DisplayPlatform.ToString());

            // colour options
            if (Config.KeyExists("MinColourHue"))
                MinColourHue = Config.GetFloat("MinColourHue");
            if (Config.KeyExists("MaxColourHue"))
                MaxColourHue = Config.GetFloat("MaxColourHue");
            if (Config.KeyExists("MinColourValue"))
                MinColourValue = Config.GetFloat("MinColourValue");
            if (Config.KeyExists("MidColourValue"))
                MidColourValue = Config.GetFloat("MidColourValue");
            if (Config.KeyExists("MaxColourValue"))
                MaxColourValue = Config.GetFloat("MaxColourValue");
            if (Config.KeyExists("ColourBands"))
                ColourBands = Config.GetInt("ColourBands");
            if (Config.KeyExists("HighlightedBondColourHue"))
                HighlightedBondColourHue = Config.GetFloat("HighlightedBondColourHue");

            // bond options
            if (Config.KeyExists("GenerateBonds"))
                GenerateBonds = Config.GetBool("GenerateBonds");
            if (Config.KeyExists("MaxBondLength"))
                MaxBondLength = Config.GetFloat("MaxBondLength");
            if (Config.KeyExists("GenerateBondsOnModelLoad"))
                GenerateBondsOnModelLoad = Config.GetBool("GenerateBondsOnModelLoad");
            if (Config.KeyExists("RecalculateBondsOnNewFrame"))
                RecalculateBondsOnNewFrame = Config.GetBool("RecalculateBondsOnNewFrame");

            // trajectory options
            if (Config.KeyExists("DefaultTrajectoryStartFrame"))
                DefaultTrajectoryStartFrame = Config.GetInt("DefaultTrajectoryStartFrame");
            if (Config.KeyExists("MinTrajectoryStartFrame"))
                MinTrajectoryStartFrame = Config.GetInt("MinTrajectoryStartFrame");
            if (Config.KeyExists("MaxTrajectoryStartFrame"))
                MaxTrajectoryStartFrame = Config.GetInt("MaxTrajectoryStartFrame");
            if (Config.KeyExists("DefaultTrajectoryFrameCount"))
                DefaultTrajectoryFrameCount = Config.GetInt("DefaultTrajectoryFrameCount");
            if (Config.KeyExists("MinTrajectoryFrameCount"))
                MinTrajectoryFrameCount = Config.GetInt("MinTrajectoryFrameCount");
            if (Config.KeyExists("MaxTrajectoryFrameCount"))
                MaxTrajectoryFrameCount = Config.GetInt("MaxTrajectoryFrameCount");
            if (Config.KeyExists("DefaultTrajectoryFrameFrequency"))
                DefaultTrajectoryFrameFrequency = Config.GetInt("DefaultTrajectoryFrameFrequency");
            if (Config.KeyExists("MinTrajectoryFrameFrequency"))
                MinTrajectoryFrameFrequency = Config.GetInt("MinTrajectoryFrameFrequency");
            if (Config.KeyExists("MaxTrajectoryFrameFrequency"))
                MaxTrajectoryFrameFrequency = Config.GetInt("MaxTrajectoryFrameFrequency");

            // lighting options
            if (Config.KeyExists("GeneralLightBrightness"))
                GeneralLightBrightness = Config.GetFloat("GeneralLightBrightness");
            if (Config.KeyExists("DefaultFillLightBrightness"))
                DefaultFillLightBrightness = Config.GetFloat("DefaultFillLightBrightness");
            if (Config.KeyExists("DefaultSpotLightBrightness"))
                DefaultSpotLightBrightness = Config.GetFloat("DefaultSpotLightBrightness");
            if (Config.KeyExists("DefaultMainLightBrightness"))
                DefaultMainLightBrightness = Config.GetFloat("DefaultMainLightBrightness");
            if (Config.KeyExists("DefaultAreaLightBrightness"))
                DefaultAreaLightBrightness = Config.GetFloat("DefaultAreaLightBrightness");

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
