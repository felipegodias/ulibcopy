using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ULibCopy.Editor {

    public static class Hotkeys {

        [MenuItem("ULibCopy/Settings")]
        public static void SelectSettings() {
            Settings settings = Settings.Instance;
            Selection.activeObject = settings;
        }

        [MenuItem("ULibCopy/Import %&i")]
        public static void ImportLibraries() {
            Settings settings = Settings.Instance;
            string monoPath = settings.MonoPath;
            string pdb2MdbPath = settings.Pdb2MdbPath;

            string[] solutions = settings.Solutions;

            foreach (string solution in solutions) {
                string[] splitedSolutionPath = solution.Split('/');
                string solutionName = splitedSolutionPath[splitedSolutionPath.Length - 1];

                string solutionDirectory = solution.Remove(solution.Length - solutionName.Length,
                    solutionName.Length);

                solutionName = solutionName.Split('.')[0];

                if (!Directory.Exists(solutionDirectory)) {
                    throw new DirectoryNotFoundException(solutionDirectory);
                }

                if (Directory.Exists(Application.dataPath + "/Plugins/" + solutionName)) {
                    Directory.Delete(Application.dataPath + "/Plugins/" + solutionName, true);
                }

                string[] directories = Directory.GetDirectories(solutionDirectory);
                foreach (string directory in directories) {
                    string debugBinDirectory = directory + "/bin/Debug";
                    if (!Directory.Exists(debugBinDirectory)) {
                        continue;
                    }

                    string pathToCopyFiles = solutionName;

                    if (directory.Contains("Editor")) {
                        pathToCopyFiles += "/Editor";
                    }
                    pathToCopyFiles = Application.dataPath + "/Plugins/" + pathToCopyFiles;

                    if (!Directory.Exists(pathToCopyFiles)) {
                        Directory.CreateDirectory(pathToCopyFiles);
                    }
                    CopyFilesFromTo(debugBinDirectory, pathToCopyFiles, monoPath, pdb2MdbPath);
                }
            }

            AssetDatabase.Refresh();
        }

        private static void CopyFilesFromTo(string from, string to, string monoPath, string pdb2MdbPath) {
            string[] files = Directory.GetFiles(from);
            if (!Directory.Exists(to)) {
                Directory.CreateDirectory(to);
            }

            string dllFile = null;
            string pdbFile = null;
            for (int i = 0; i < files.Length; i++) {
                EditorUtility.DisplayProgressBar("Copying Files", files[i], (float) i / files.Length);
                FileInfo file = new FileInfo(files[i]);
                string copyPath = string.Format("{0}/{1}", to, file.Name);
                file.CopyTo(copyPath);
                if (copyPath.EndsWith(".dll")) {
                    dllFile = copyPath;
                }
                if (copyPath.EndsWith(".pdb")) {
                    pdbFile = copyPath;
                }
            }

            if (dllFile != null) {
                EditorUtility.DisplayProgressBar("Runing PDB to MDB",
                    "Coverting the files from " + to + " from PDB to MDB.", 1);
                RunPDB2MDB(dllFile, monoPath, pdb2MdbPath);
            }

            if (pdbFile != null) {
                File.Delete(pdbFile);
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        private static void RunPDB2MDB(string path, string monoPath, string pdb2MdbPath) {
            string arguments = string.Format("\"{0}\" {1}", pdb2MdbPath, path);
            Process process = Process.Start(monoPath, arguments);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.WaitForExit();
        }

    }

}