
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractionSettings {

        public bool HighlightInteracingAtoms { get; set; }
        public bool RenderInteractionLines { get; set; }
        public bool ShowAttractiveInteractions { get; set; }
        public bool ShowStableInteractions { get; set; }
        public bool ShowRepulsiveInteractions { get; set; }

        public static MolecularInteractionSettings Default() {

            return new MolecularInteractionSettings {

                HighlightInteracingAtoms = true,
                RenderInteractionLines = true,
                ShowAttractiveInteractions = true,
                ShowStableInteractions = true,
                ShowRepulsiveInteractions = true,
            };
        }

        public override string ToString() {

            return
                "HighlightInteracingAtoms: " + HighlightInteracingAtoms + "\n" +
                "RenderInteractionLines: " + RenderInteractionLines + "\n" +
                "ShowAttractiveInteractions: " + ShowAttractiveInteractions + "\n" +
                "ShowStableInteractions: " + ShowStableInteractions + "\n" +
                "ShowRepulsiveInteractions: " + ShowRepulsiveInteractions;
        }
    }
}
