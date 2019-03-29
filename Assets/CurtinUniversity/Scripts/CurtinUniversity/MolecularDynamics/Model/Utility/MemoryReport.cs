
namespace CurtinUniversity.MolecularDynamics.Visualization.Utility {

    public static class MemoryReport {

        public static long GetMemoryUsage() {
            return System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
        }
    }
}
