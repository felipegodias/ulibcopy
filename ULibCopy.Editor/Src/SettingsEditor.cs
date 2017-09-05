using UnityEditor;
using UnityEngine;

namespace ULibCopy.Editor {

    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor {

        private const string kSolutionsField = "solutions";

        private Vector2 scrollPosition;

        private SerializedProperty solutionsProperty;

        private void OnEnable() {
            this.solutionsProperty = this.serializedObject.FindProperty(kSolutionsField);
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Solutions", EditorStyles.centeredGreyMiniLabel);

            int indexToDelete = -1;

            for (int i = 0; i < this.solutionsProperty.arraySize; i++) {
                EditorGUILayout.BeginHorizontal();
                SerializedProperty solutionProperty = this.solutionsProperty.GetArrayElementAtIndex(i);
                string solutionPath = solutionProperty.stringValue;
                string[] splitedSolutionPath = solutionPath.Split('/');
                string solutionName = splitedSolutionPath[splitedSolutionPath.Length - 1].Split('.')[0];
                solutionPath = EditorGUILayout.TextField(solutionName, solutionPath);
                bool browseSolutionPathButtonClicked = GUILayout.Button("Browse", EditorStyles.miniButton,
                    GUILayout.Width(50));
                if (browseSolutionPathButtonClicked) {
                    solutionPath = EditorUtility.OpenFilePanel("Solution Path", solutionPath, "sln");
                }
                solutionProperty.stringValue = solutionPath;

                bool deleteButtonClicked = GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(50));
                if (deleteButtonClicked) {
                    indexToDelete = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (indexToDelete != -1) {
                this.solutionsProperty.DeleteArrayElementAtIndex(indexToDelete);
            }

            bool addSolutionButtonClicked = GUILayout.Button("Add New Solution");
            if (addSolutionButtonClicked) {
                string newSolutionPath = EditorUtility.OpenFilePanel("Solution Path", EditorApplication.applicationPath,
                    "sln");
                if (newSolutionPath.EndsWith(".sln")) {
                    int indexToAddElement = this.solutionsProperty.arraySize;
                    this.solutionsProperty.InsertArrayElementAtIndex(indexToAddElement);
                    this.solutionsProperty.GetArrayElementAtIndex(indexToAddElement).stringValue = newSolutionPath;
                }
            }

            EditorGUILayout.EndVertical();

            this.serializedObject.ApplyModifiedProperties();
        }

    }

}