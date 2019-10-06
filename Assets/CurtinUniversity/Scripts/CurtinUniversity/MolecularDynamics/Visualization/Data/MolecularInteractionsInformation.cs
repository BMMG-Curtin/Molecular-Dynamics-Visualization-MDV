
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MolecularInteractionsInformation {

        public int TotalInteractions;
        public int TotalAttractiveInteractions;
        public int TotalStableInteractions;
        public int TotalRepulsiveInteractions;

        public double SummedInteractionForce;
        public double SummedAttractionForce;
        public double SummedRepulsionForce;

        public double SummedLennardJonesForce;
        public double SummedLennardJonesAttractionForce;
        public double SummedLennardJonesRepulsionForce;

        public double SummedElectrostaticForce;
        public double SummedElectrostaticAttractionForce;
        public double SummedElectrostaticRepulsionForce;



        public MolecularInteractionsInformation() {

            TotalInteractions = 0;
            TotalAttractiveInteractions = 0;
            TotalStableInteractions = 0;
            TotalRepulsiveInteractions = 0;

            SummedInteractionForce = 0;
            SummedAttractionForce = 0;
            SummedRepulsionForce = 0;

            SummedLennardJonesForce = 0;
            SummedLennardJonesAttractionForce = 0;
            SummedLennardJonesRepulsionForce = 0;

            SummedElectrostaticForce = 0;
            SummedElectrostaticAttractionForce = 0;
            SummedElectrostaticRepulsionForce = 0;
        }

        public override string ToString() {

            return
                "Stable Interactions: " + TotalStableInteractions + "\n" +
                "Attractive Interactions: " + TotalAttractiveInteractions + "\n" +
                "Repulsive Interactions: " + TotalRepulsiveInteractions + "\n" +
                "Total Interactions: " + TotalInteractions + "\n\n" +

                "Total Force: " + SummedInteractionForce.ToString("N2") + "\n" +
                "Attraction Force: " + SummedAttractionForce.ToString("N2") + "\n" +
                "Repulsion Force: " + SummedRepulsionForce.ToString("N2") + "\n\n" +

                "VDW Force: " + SummedLennardJonesForce.ToString("N2") + "\n" +
                "VDW Attraction: " + SummedLennardJonesAttractionForce.ToString("N2") + "\n" +
                "VDW Repulsion: " + SummedLennardJonesRepulsionForce.ToString("N2") + "\n\n" +

                "Electrostatic Force: " + SummedElectrostaticForce.ToString("N2") + "\n" +
                "Electrostatic Attraction: " + SummedElectrostaticAttractionForce.ToString("N2") + "\n" +
                "Electrostatic Repulsion: " + SummedElectrostaticRepulsionForce.ToString("N2") + "\n";
        }
    }
}
