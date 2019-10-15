using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public enum ConsoleMessageType {
        Banner,
        Standard,
        Warning,
        Error
    }

    public class MessageConsole : MonoBehaviour {
        
        [SerializeField]
        private TextMeshProUGUI bannerMessage;

        [SerializeField]
        private TextMeshProUGUI consoleContent;

        [SerializeField]
        private ScrollRect scrollRect;

        public bool Silent { get; set; }
        public bool ShowBanner { get; set; }

        public string BannerFPS { set { bannerFPS = value; newBanner = true; } }
        public string BannerBuildTime { set { bannerBuildTime = value; newBanner = true; } }
        // public string BannerModelInfo { set { bannerModelInfo = value; newBanner = true; } }

        private SceneManager sceneManager;

        private string bannerFPS = "";
        private string bannerBuildTime = "";
        // private string bannerModelInfo = "";

        private int maxConsoleMessages = 50;

        private struct ConsoleMessage {
            public string message;
            public ConsoleMessageType type;
        }
        private List<ConsoleMessage> messages = new List<ConsoleMessage>();

        private static readonly Dictionary<ConsoleMessageType, Color> messageTypeColors = new Dictionary<ConsoleMessageType, Color> {
            { ConsoleMessageType.Banner, Color.green },
            { ConsoleMessageType.Standard, Color.white },
            { ConsoleMessageType.Warning, Color.yellow },
            { ConsoleMessageType.Error, Color.cyan },
        };

        private bool newBanner = true;
        private bool newMessages = true;

        void Awake() {

            ShowBanner = false;
            if (Settings.DebugMessages)
                ShowBanner = true;

            ShowError(Settings.StartMessage);
        }

        void LateUpdate() {

            if (newBanner) {
                updateBanner();
                newBanner = false;
            }

            if (newMessages) {
                StartCoroutine(updateConsole());
                newMessages = false;
            }
        }

        public void ShowMessage(string message) {
            showMessage(message, ConsoleMessageType.Standard);
        }

        public void ShowWarning(string message) {
            showMessage(message, ConsoleMessageType.Warning);
        }

        public void ShowError(string message) {
            showMessage(message, ConsoleMessageType.Error);
        }

        private void showMessage(string message, ConsoleMessageType type) {

            if (!Silent) {

                messages.Add(new ConsoleMessage {
                    message = message,
                    type = type,
                });

                trimExcessMessages();

                newMessages = true;
            }
        }

        private void trimExcessMessages() {

            if (messages.Count <= maxConsoleMessages) {
                return;
            }

            messages.RemoveRange(0, messages.Count - maxConsoleMessages);
        }

        private void updateBanner() {

            if (ShowBanner) {

                bannerMessage.text = "";

                if (bannerBuildTime != null && bannerBuildTime != "")
                    bannerMessage.text += bannerBuildTime + "ms build";

                if (bannerFPS != null) {

                    if (bannerMessage.text != null && bannerMessage.text != "")
                        bannerMessage.text += ", ";

                    bannerMessage.text += bannerFPS + " fps";
                }
            }
            else
                bannerMessage.text = "";
        }

        private IEnumerator updateConsole() {

            string consoleText = "";

            for(int i=0; i < messages.Count; i++) {
                ConsoleMessage message = messages[i];
                consoleText += "<color=" + HTMLColor(messageTypeColors[message.type]) + ">" + message.message + "</color>";
                if(i < messages.Count - 1) {
                    consoleText += "\n";
                }
            }

            consoleContent.text = consoleText;

            // wait a frame or veritalNormalizedPosition wont work correctly
            yield return null;

            scrollRect.verticalNormalizedPosition = 0f;
        }

        private string HTMLColor(Color color) {
            return string.Format("#{0:X2}{1:X2}{2:X2}", (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }
    }
}
