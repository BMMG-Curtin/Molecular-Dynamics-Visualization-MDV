using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public enum ConsoleMessageType {
        Banner,
        Standard,
        Warning,
        Error
    }

    public class MessageConsole : MonoBehaviour {
        
        public Text BannerMessage;
        public Text ConsoleContent;
        public ScrollRect ScrollRect;

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

        struct ConsoleMessage {
            public string message;
            public ConsoleMessageType type;
        }
        private List<ConsoleMessage> messages = new List<ConsoleMessage>();

        private static readonly Dictionary<ConsoleMessageType, Color> messageTypeColors = new Dictionary<ConsoleMessageType, Color> {
        { ConsoleMessageType.Banner, Color.green },
        { ConsoleMessageType.Standard, Color.white },
        { ConsoleMessageType.Warning, Color.yellow },
        { ConsoleMessageType.Error, Color.red },
    };

        private bool newBanner = true;
        private bool newMessages = true;

        void Start() {

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
                updateConsole();
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

                BannerMessage.text = "";

                if (bannerBuildTime != null && bannerBuildTime != "")
                    BannerMessage.text += bannerBuildTime + "ms build";

                if (bannerFPS != null) {

                    if (BannerMessage.text != null && BannerMessage.text != "")
                        BannerMessage.text += ", ";

                    BannerMessage.text += bannerFPS + " fps";
                }
            }
            else
                BannerMessage.text = "";
        }

        private void updateConsole() {

            string consoleText = "";

            foreach (ConsoleMessage message in messages)
                consoleText += " <color=" + HTMLColor(messageTypeColors[message.type]) + ">" + message.message + "</color>\n";

            ConsoleContent.text = consoleText;
            ScrollRect.verticalNormalizedPosition = 0f;
        }

        private string HTMLColor(Color color) {
            return string.Format("#{0:X2}{1:X2}{2:X2}", (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }
    }
}
