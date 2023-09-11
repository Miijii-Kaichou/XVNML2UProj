using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private SerializedProperty xvnmlContent;
        private XVNMLAsset fileAsset;

        private Vector2 scrollPosition = Vector2.zero;

        private Process _activeProcess;

        public void OnEnable()
        {
            xvnmlContent = serializedObject.FindProperty("content");
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("View in VSCode"))
            {
                //TODO: Start Process with argument.
                _ = HasAssociatedExecutable(fileAsset.filePath, out string associatedPath);

                _activeProcess = new Process();
                _activeProcess.StartInfo.FileName = associatedPath;
                _activeProcess.StartInfo.Arguments = fileAsset.filePath;
                _activeProcess.Start();
            }

            if (GUILayout.Button("Ping"))
            {
                EditorGUIUtility.PingObject(target);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            DrawImporterGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawImporterGUI()
        {
            fileAsset = serializedObject.targetObject as XVNMLAsset;
            fileAsset.ReadContentFromFile();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextArea(xvnmlContent == null ? string.Empty : xvnmlContent.stringValue
                 , GUILayout.ExpandWidth(true)
                 , GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(10);
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
