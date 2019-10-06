
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractionSettings {

        public bool ShowClosestInteractionsOnly { get; set; }
        public bool HighlightInteracingAtoms { get; set; }
        public bool RenderInteractionLines { get; set; }

        public static MolecularInteractionSettings Default() {

            return new MolecularInteractionSettings {

                ShowClosestInteractionsOnly = true,
                HighlightInteracingAtoms = true,
                RenderInteractionLines = true,
            };
        }

        public override string ToString() {

            return
                "ShowClosestInteractionsOnly: " + ShowClosestInteractionsOnly + "\n" +
                "HighlightInteracingAtoms: " + HighlightInteracingAtoms + "\n" +
                "RenderInteractionLines: " + RenderInteractionLines;
        }
    }
}
