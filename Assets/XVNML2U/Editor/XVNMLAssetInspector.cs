using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using XVNML2U.FileSupport;

namespace XVNML2U.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(XVNMLAsset))]
    public sealed class XVNMLAssetInspector : UnityEditor.Editor
    {
        private bool _editMode = false;
        private bool IsAtCharacterLimit
        {
            get
            {
                return xvnmlContent.stringValue.Length > MaxCharacterLength;
            }
        }

        public bool ContentDiffPresent
        {
            get
            {
                return xvnmlContent.stringValue.SequenceEqual(_overrideContent) == false;
            }
        }

        private SerializedProperty xvnmlContent;
        private XVNMLAsset fileAsset;
        private Vector2 scrollPosition = Vector2.zero;
        private Process _activeProcess;

        private string _overrideContent;
        private bool _enableWrapping;
        private bool _showVSCodeButton;

        private const int MaxCharacterLength = 16383;

        public void OnEnable()
        {
            _showVSCodeButton = CheckIfVSCodeExists();

            if (_showVSCodeButton == false && EditorUtility.GetDialogOptOutDecision(DialogOptOutDecisionType.ForThisMachine, "vsCodeRemainderOptOut") == false)
            {
                EditorUtility.DisplayDialog("Visual Studio Code not Detected...",
                    "We've noticed that you don't have Visual Studio Code installed. It's recommended so you can have a" +
                    "better experience with XVNML.", "Install VSCode", "Continue Without VSCode", DialogOptOutDecisionType.ForThisMachine, "vsCodeRemainderOptOut");
            }

            fileAsset = serializedObject.targetObject as XVNMLAsset;
            fileAsset.ReadContentFromFile();
            xvnmlContent = serializedObject.FindProperty("content");
            _overrideContent = xvnmlContent.stringValue.Replace("\r", string.Empty);
        }

        private bool CheckIfVSCodeExists()
        {
            Process vsCodeVersionCheckProcess = new();

            vsCodeVersionCheckProcess.StartInfo.CreateNoWindow = true;
            vsCodeVersionCheckProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            vsCodeVersionCheckProcess.StartInfo.FileName = "code";
            vsCodeVersionCheckProcess.StartInfo.Arguments = "--version";

            vsCodeVersionCheckProcess.Start();

            vsCodeVersionCheckProcess.WaitForExit();
            return vsCodeVersionCheckProcess.ExitCode == 0;
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            const float FullAlpha = 255;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            string editString = _editMode ? "Finish" : "Edit";

            EditorGUI.BeginDisabledGroup(IsAtCharacterLimit);
            if (GUILayout.Button($"{editString} XVNML"))
            {
                if (ContentDiffPresent)
                {
                    var result = EditorUtility.DisplayDialogComplex(
                        "Finishing Editing",
                        $"Would you like to apply changes to {fileAsset.name}?",
                        "Apply Changes",
                        "Cancel",
                        "Continue Without Changes");

                    if (result == 0)
                    {
                        fileAsset.OverrideContent(_overrideContent);
                        _editMode = !_editMode;
                        return;
                    }

                    if (result == 1)
                    {
                        Repaint();
                        return;
                    }

                    GUI.FocusControl(null);
                    _overrideContent = xvnmlContent.stringValue;
                }

                _editMode = !_editMode;
                Repaint();
            }
            EditorGUI.EndDisabledGroup();

            TryDisplayVSCodeButton(FullAlpha);

            EditorGUILayout.EndHorizontal(); ;

            if (IsAtCharacterLimit)
            {
                EditorGUILayout.Space(2);
                EditorGUILayout.HelpBox("Can not edit within editor because document surpasses the number of characters allowed...", MessageType.Warning);
            }

            EditorGUILayout.Space();

            DrawImporterGUI();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(ContentDiffPresent == false || IsAtCharacterLimit);
            if (GUILayout.Button("Revert", EditorStyles.miniButtonLeft, GUILayout.Width(85)))
            {
                _overrideContent = xvnmlContent.stringValue;
                Repaint();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply", EditorStyles.miniButtonRight, GUILayout.Width(85)))
            {
                fileAsset.OverrideContent(_overrideContent);
                Repaint();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);

            serializedObject.ApplyModifiedProperties();
        }

        private void TryDisplayVSCodeButton(float FullAlpha)
        {
            if (_showVSCodeButton)
            {
                GUIStyle vscButtonStyle = new(GUI.skin.button);

                Color originalColor = GUI.backgroundColor;
                Color vscBlueColor = new(35f / FullAlpha, 221f / FullAlpha, 242f / FullAlpha, FullAlpha / FullAlpha);

                GUI.backgroundColor = vscBlueColor;

                if (GUILayout.Button("View in VSCode", vscButtonStyle))
                {
                    OpenVSCode();
                }

                GUI.backgroundColor = originalColor;
            }
        }

        private void OpenVSCode()
        {
            //TODO: Start Process with argument.
            _ = HasAssociatedExecutable(fileAsset.filePath, out string associatedPath);

            _activeProcess = new Process();
            _activeProcess.StartInfo.FileName = associatedPath;
            _activeProcess.StartInfo.Arguments = fileAsset.filePath;
            _activeProcess.Start();
        }

        private void DrawImporterGUI()
        {
            EditorStyles.textArea.wordWrap = _enableWrapping;
            EditorStyles.textArea.richText = false;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
            EditorGUI.BeginDisabledGroup(_editMode == false);
            _overrideContent = EditorGUILayout.TextArea(xvnmlContent == null ? string.Empty : _overrideContent
                , EditorStyles.textArea
                , GUILayout.ExpandWidth(true)
                , GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(5);
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

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
#endif
}
