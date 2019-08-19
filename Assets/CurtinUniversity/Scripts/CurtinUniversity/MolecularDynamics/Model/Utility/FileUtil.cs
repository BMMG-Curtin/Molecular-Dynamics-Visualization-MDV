using System;
using System.IO;

namespace CurtinUniversity.MolecularDynamics.Model {

    public static class FileUtil {

        public static void CopyFile(string inputFilePath, string outputFilePath) {

            using (StreamReader sr = new StreamReader(inputFilePath)) {
                using (StreamWriter sw = new StreamWriter(outputFilePath)) {
                    string content = sr.ReadToEnd();
                    sw.Write(content);
                }
            }
        }

        public static void DeleteFile(string fileName) {
            File.Delete(fileName);
        }
    }
}
