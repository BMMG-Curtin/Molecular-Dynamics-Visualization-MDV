using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class ApplicationSettings : MonoBehaviour {

        public Toggle GroundToggle;
        public Toggle ShadowsToggle;
        public Toggle LightsToggle;

        public Text AtomMeshQualityText;
        public Text BondMeshQualityText;

        public Text MouseSpeedText;
        public Text GUICursorSpeedText;

        private SceneSettings sceneSettings;

        public void SetSceneSettings(SceneSettings settings) {

            sceneSettings = settings;
            reloadUIControls();
        }

        private void reloadUIControls() {

            GroundToggle.isOn = sceneSettings.ShowGround;
            ShadowsToggle.isOn = sceneSettings.ShowShadows;
            LightsToggle.isOn = sceneSettings.LightsOn;
        }

        public void OnGroundToggleChanged() {

            Debug.Log("Ground Toggle Clicked. Value: " + GroundToggle.isOn);

            sceneSettings.ShowGround = GroundToggle.isOn;
            UserInterfaceEvents.RaiseOnSceneSettingsUpdated(sceneSettings);
        }

        public void OnShadowsToggleChanged() {

            sceneSettings.ShowShadows = ShadowsToggle.isOn;
            UserInterfaceEvents.RaiseOnSceneSettingsUpdated(sceneSettings);
        }

        public void OnLightsToggleChanged() {

            sceneSettings.LightsOn= LightsToggle.isOn;
            UserInterfaceEvents.RaiseOnSceneSettingsUpdated(sceneSettings);
        }


        //public void IncreaseAtomMeshQuality() {

        //    if (currentAtomMeshIndex < Settings.AtomMeshQualityValues.Length - 1) {
        //        currentAtomMeshIndex++;
        //        AtomMeshQualityText.text = Settings.AtomMeshQualityValues[currentAtomMeshIndex];
        //        Settings.AtomMeshQuality = currentAtomMeshIndex;
        //        StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
        //    }

        //    PlayerPrefs.SetInt(playerPrefsAtomMeshQualityKey, Settings.AtomMeshQuality);
        //}

        //public void DecreaseAtomMeshQuality() {

        //    if (currentAtomMeshIndex > 0) {
        //        currentAtomMeshIndex--;
        //        AtomMeshQualityText.text = Settings.AtomMeshQualityValues[currentAtomMeshIndex];
        //        Settings.AtomMeshQuality = currentAtomMeshIndex;
        //        StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
        //    }

        //    PlayerPrefs.SetInt(playerPrefsAtomMeshQualityKey, Settings.AtomMeshQuality);
        //}

        //public void IncreaseBondMeshQuality() {

        //    if (currentBondMeshIndex < Settings.BondMeshQualityValues.Length - 1) {
        //        currentBondMeshIndex++;
        //        BondMeshQualityText.text = Settings.BondMeshQualityValues[currentBondMeshIndex];
        //        Settings.BondMeshQuality = currentBondMeshIndex;
        //        StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
        //    }

        //    PlayerPrefs.SetInt(playerPrefsBondMeshQualityKey, Settings.BondMeshQuality);
        //}

        //public void DecreaseBondMeshQuality() {

        //    if (currentBondMeshIndex > 0) {
        //        currentBondMeshIndex--;
        //        BondMeshQualityText.text = Settings.BondMeshQualityValues[currentBondMeshIndex];
        //        Settings.BondMeshQuality = currentBondMeshIndex;
        //        StartCoroutine(SceneManager.instance.ReloadModelView(true, false));
        //    }

        //    PlayerPrefs.SetInt(playerPrefsBondMeshQualityKey, Settings.BondMeshQuality);
        //}

        //public void InreaseMouseSpeed() {
        //    SceneManager.instance.InputManager.SetSceneMouseSpeed(Settings.SceneMouseCursorSpeed + 1);
        //    MouseSpeedText.text = Settings.SceneMouseCursorSpeed.ToString();
        //}

        //public void DecreaseMouseSpeed() {
        //    SceneManager.instance.InputManager.SetSceneMouseSpeed(Settings.SceneMouseCursorSpeed - 1);
        //    MouseSpeedText.text = Settings.SceneMouseCursorSpeed.ToString();
        //}

        //public void InreaseGUICursorSpeed() {
        //    SceneManager.instance.InputManager.SetGUICursorSpeed(Settings.GUIMouseCursorSpeed + 1);
        //    GUICursorSpeedText.text = Settings.GUIMouseCursorSpeed.ToString();
        //}

        //public void DecreaseGUICursorSpeed() {
        //    SceneManager.instance.InputManager.SetGUICursorSpeed(Settings.GUIMouseCursorSpeed - 1);
        //    GUICursorSpeedText.text = Settings.GUIMouseCursorSpeed.ToString();
        //}
    }
}
