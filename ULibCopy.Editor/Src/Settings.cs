using UnityEditor;
using UnityEngine;

namespace ULibCopy.Editor {

    public class Settings : ScriptableObject {

        private static Settings instance;

        [SerializeField] private string[] solutions;

        public static Settings Instance {
            get {
                if (instance != null) {
                    return instance;
                }

                string[] guids = AssetDatabase.FindAssets("ULibCopy.Editor");
                string dllPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                string assetPath = dllPath.Replace("ULibCopy.Editor.dll", "Settings.asset");
                instance = AssetDatabase.LoadAssetAtPath<Settings>(assetPath);
                if (instance != null) {
                    return instance;
                }

                instance = CreateInstance<Settings>();
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.Refresh();
                return instance;
            }
        }

        public string[] Solutions => this.solutions;

        public string MonoPath => EditorApplication.applicationContentsPath + "/MonoBleedingEdge/bin/mono.exe";

        public string Pdb2MdbPath => EditorApplication.applicationContentsPath +
                                     "/MonoBleedingEdge/lib/mono/4.5/pdb2mdb.exe";

    }

}