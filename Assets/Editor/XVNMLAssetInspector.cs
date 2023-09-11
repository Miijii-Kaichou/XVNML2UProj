using System;
using System.Collections.Generic;
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
        private int _enableWrapping = 0;
        private int _zoomLevel = 1;
        private int _maxZoomLevel = 4;

        private SerializedProperty xvnmlContent;
        private XVNMLAsset fileAsset;

        private Vector2 scrollPosition = Vector2.zero;

        private int _totalPages = 0;
        private int _currentPage = 1;

        private List<string> _pageContent = new List<string>();

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
                Process.Start(associatedPath, fileAsset.filePath);
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

        private void Process_Exited(object sender, System.EventArgs e)
        {
            var process = sender as Process;
            Console.Write($"VSCode Process {process.SessionId} has exited.");
        }

        private void DrawImporterGUI()
        {
            fileAsset = serializedObject.targetObject as XVNMLAsset;
            fileAsset.ReadContentFromFile();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
            EditorGUI.BeginDisabledGroup(_onEditMode == 0);
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
