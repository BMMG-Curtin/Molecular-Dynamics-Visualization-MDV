using System;
using System.Collections.Generic;
using System.Diagnostics;

using CurtinUniversity.MolecularDynamics.Model.Model;
using CurtinUniversity.MolecularDynamics.Model.FileParser;
using CurtinUniversity.MolecularDynamics.Model.FileCreator;

namespace CurtinUniversity.MolecularDynamics.Model.Examples {

    public static class SecondaryStructureTrajectoryTest {

        private static string inputFilePath = @"D:\Molecule Examples\";
        private static string inputFileName = @"membrane_protein.gro";
        private static string trajectoryFileName = @"membrane_protein_sim.xtc";

        private static string strideExePath = @"D:\Stride\stride_WIN32.exe";
        private static string tmpFilePath = @"D:\tmp\";

        public static void Run() {


            // get the primary structure
            Console.WriteLine("Parsing GRO File");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            PrimaryStructure primaryStructure = GROStructureParser.GetStructure(inputFilePath + inputFileName);
            stopWatch.Stop();

            Console.WriteLine("Structure Parsing Complete [" + stopWatch.ElapsedMilliseconds + " ms]");

            // get the structure trajectory
            int atomCount = XTCTrajectoryParser.GetAtomCount(inputFilePath + trajectoryFileName);
            if (atomCount == primaryStructure.AtomCount()) {
                Console.WriteLine("Atom Count matches.");
            }
            else {
                Console.WriteLine("Atom count doesnt match between primary structure and trajectory");
            }

            Stopwatch stopWatch2 = new Stopwatch();
            stopWatch2.Start();
            PrimaryStructureTrajectory trajectory = XTCTrajectoryParser.GetTrajectory(inputFilePath + trajectoryFileName, 0, 100, 1);
            stopWatch2.Stop();

            Console.WriteLine("Trajectory Parsing Complete [" + stopWatch2.ElapsedMilliseconds + " ms]");

            // get the secondary structure for the base primary structure

            Stopwatch stopWatch3 = new Stopwatch();
            stopWatch3.Start();
            SecondaryStructure secondaryStructure = SecondaryStructure.CreateFromPrimaryStructure(primaryStructure, strideExePath, tmpFilePath);
            Console.WriteLine("Main Secondary Structure Parsing Complete [" + stopWatch3.ElapsedMilliseconds + " ms]");

            Console.WriteLine(secondaryStructure);

            // get the secondary structure for a frame of the trajectory
            Stopwatch stopWatch4 = new Stopwatch();
            stopWatch4.Start();

            SecondaryStructureTrajectory secondaryStructureTrajectory = new SecondaryStructureTrajectory(primaryStructure, trajectory, strideExePath, tmpFilePath);
            SecondaryStructure secondaryStructure2 = secondaryStructureTrajectory.GetStructure(50);

            Console.WriteLine("Secondary Structure Parsing for frame 50 Complete [" + stopWatch3.ElapsedMilliseconds + " ms]");

            Console.WriteLine(secondaryStructure2);
        }
    }
}
