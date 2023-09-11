using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using XVNML2U.Mono;

namespace XVNML2U
{
#if UNITY_EDITOR
    [CustomEditor(typeof(XVNMLAsset))]
    public sealed class XVNMLAssetInspector : Editor
    {
        private int _onEditMode = 0;

        private SerializedProperty xvnmlContent;
        private XVNMLAsset fileAsset;

        private Vector2 scrollPosition = Vector2.zero;

        public void OnEnable()
        {
            xvnmlContent = serializedObject.FindProperty("content");
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            EditorGUILayout.BeginHorizontal();

            var editText = _onEditMode == 0 ? "Edit" : "Finish";

            if (GUILayout.Button($"{editText} XVNML"))
            {
                //TODO: Enable Text Box for XVNML to be Editted.
                _onEditMode = _onEditMode == 0 ? _onEditMode + 1 : 0;
                OnHeaderGUI();
                return;
            }

            EditorGUI.BeginDisabledGroup(_onEditMode == 0);
            if (GUILayout.Button("Save"))
            {
                //TODO: Update File Content and call DrawImportGUI again.
                return;
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Use VSCode"))
            {
                //TODO: Start Process with argument.
                _ = HasAssociatedExecutable(fileAsset.filePath, out string associatedPath);
                var process = Process.Start(associatedPath, fileAsset.filePath);
                process.Exited += Process_Exited;
                return;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            DrawImporterGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void Process_Exited(object sender, System.EventArgs e)
        {
            var process = sender as Process;
            Console.Write($"VSCode Process {process.SessionId} has exited.");
        }

        private void DrawImporterGUI()
        {
            fileAsset = serializedObject.targetObject as XVNMLAsset;
            fileAsset.ReadContentFromFile();

            EditorStyles.textField.wordWrap = true;
            EditorGUI.BeginDisabledGroup(_onEditMode == 0);
            EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.TextArea(xvnmlContent == null ? string.Empty : xvnmlContent.stringValue
                 , GUILayout.ExpandWidth(true)
                 , GUILayout.ExpandHeight(true)
                 , GUILayout.MinHeight(250)
                 , GUILayout.MaxHeight(500));
            EditorGUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUI.EndDisabledGroup();
        }

        static bool HasAssociatedExecutable(string path, out string associatedPath)
        {
            var executable = FindExecutable(path);
            associatedPath = executable;
            return !string.IsNullOrEmpty(executable);
        }

        private static string FindExecutable(string path)
        {
            var executable = new StringBuilder(2048);
            FindExecutable(path, string.Empty, executable);
            var returnString = executable?.ToString();
            return returnString;
        }

        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        private static extern long FindExecutable(string lpFile, string lpDirectory, StringBuilder lpResult);
    }
#endif
}
