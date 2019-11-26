using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    // Utility class. Read settings from a config file and provides methods to access the settings.
    public static class Config {

        private static string configFilePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + @"config.txt";
        private static Dictionary<string, string> settings = new Dictionary<string, string>();

        private static String comment = "//";
        private static Char separator = ':';

        public static void LoadConfig() {

            try {
                string[] lines = File.ReadAllLines(configFilePath);

                foreach (string line in lines) {

                    if (line.Trim() == "" || line.StartsWith(comment)) {
                        continue;
                    }

                    string[] setting = line.Split(separator);
                    if (setting.Length < 2) {
                        continue;
                    }

                    string[] value = setting[1].Trim().Split(new string[] { comment }, StringSplitOptions.None);

                    settings.Add(setting[0].Trim(), value[0].Trim());
                }
            }
            catch (Exception) {
                Debug.Log("Error loading config file at: " + configFilePath);
            }
        }

        public static bool KeyExists(string key) {
            return settings.ContainsKey(key);
        }

        public static string GetString(string key) {
            string value = null;
            settings.TryGetValue(key, out value);
            return value;
        }

        public static float GetFloat(string key) {
            float value = 0f;
            if (settings.ContainsKey(key)) {
                float.TryParse(GetString(key), out value);
            }

            return value;
        }

        public static int GetInt(string key) {
            int value = 0;
            if (settings.ContainsKey(key)) {
                int.TryParse(GetString(key), out value);
            }
            return value;
        }

        public static bool GetBool(string key) {
            string value = GetString(key);
            if (value.ToUpper().Equals("TRUE")) {
                return true;
            }
            else {
                return false;
            }
        }

        public static Color GetRGB(string key) {

            Color errorColour = Color.black;

            try {

                string colorStr = GetString(key);
                string[] rgbValues = colorStr.Split(',');

                if (rgbValues == null || rgbValues.Length < 3) {
                    return errorColour;
                }

                int red = int.Parse(rgbValues[0].Trim());
                int blue = int.Parse(rgbValues[1].Trim());
                int green = int.Parse(rgbValues[2].Trim());

                if (red < 0 || red > 255 || blue < 0 || blue > 255 || green < 0 || green > 255) {
                    return errorColour;
                }

                return new Color((float)red / 255f, (float)blue / 255f, (float)green / 255f);
            }
            catch (Exception) {
                // do nothing, fall through
            }

            return errorColour;
        }

        public static string AllSettings() {

            string output = "";

            int i = 0;

            foreach (KeyValuePair<string, string> setting in settings) {
                i++;
                output += "[[" + i + "] " + setting.Key + ": " + setting.Value + "]";
            }

            return output;
        }
    }
}
