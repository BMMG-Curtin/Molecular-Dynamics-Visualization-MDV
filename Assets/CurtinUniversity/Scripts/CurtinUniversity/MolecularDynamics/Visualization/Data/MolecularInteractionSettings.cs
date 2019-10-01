
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractionSettings {

        public bool CalculateClosestInteractionsOnly { get; set; }
        public bool RenderClosestInteractionsOnly { get; set; }
        public bool HighlightInteracingAtoms { get; set; }
        public bool RenderInteractionLines { get; set; }

        public static MolecularInteractionSettings Default() {

            return new MolecularInteractionSettings {

                CalculateClosestInteractionsOnly = true,
                RenderClosestInteractionsOnly = true,
                HighlightInteracingAtoms = true,
                RenderInteractionLines = true,
            };
        }

        public override string ToString() {

            return
                "CalculateClosestInteractionsOnly: " + CalculateClosestInteractionsOnly + "\n" +
                "RenderClosestInteractionsOnly: " + RenderClosestInteractionsOnly + "\n" +
                "HighlightInteracingAtoms: " + HighlightInteracingAtoms + "\n" +
                "RenderInteractionLines: " + RenderInteractionLines + "\n";
        }
    }
}
