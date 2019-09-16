
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractionSettings {

        public bool CalculateClosestInteractionsOnly { get; set; }
        public bool RenderClosestInteractionsOnly { get; set; }


        public static MolecularInteractionSettings Default() {

            return new MolecularInteractionSettings {

                CalculateClosestInteractionsOnly = true,
                RenderClosestInteractionsOnly = true,
            };
        }

        public override string ToString() {

            return
                "CalculateClosestInteractionsOnly: " + CalculateClosestInteractionsOnly + "\n" +
                "RenderClosestInteractionsOnly: " + RenderClosestInteractionsOnly + "\n";
        }
    }
}
