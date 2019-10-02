
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractionSettings {

        public bool ShowSimpleForces { get; set; }
        public bool ShowVDWForces { get; set; }
        public bool RenderClosestInteractionsOnly { get; set; }
        public bool HighlightInteracingAtoms { get; set; }
        public bool RenderInteractionLines { get; set; }

        public static MolecularInteractionSettings Default() {

            return new MolecularInteractionSettings {

                ShowSimpleForces = true,
                ShowVDWForces = true,
                RenderClosestInteractionsOnly = true,
                HighlightInteracingAtoms = true,
                RenderInteractionLines = true,
            };
        }

        public override string ToString() {

            return
                "CalculateSimpleForces: " + ShowSimpleForces + "\n" +
                "CalculateVDWForces: " + ShowVDWForces + "\n" +
                "CalculateClosestInteractionsOnly: " + RenderClosestInteractionsOnly + "\n" +
                "HighlightInteracingAtoms: " + HighlightInteracingAtoms + "\n" +
                "RenderInteractionLines: " + RenderInteractionLines;
        }
    }
}
