using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using XVNML.XVNMLUtility;
using XVNML2U.Configuration;

namespace XVNML2U.Mono.Editor
{
    [CustomEditor(typeof(XVNMLAsset))]
    public class XVNMLAssetScriptableObjectInspector : UnityEditor.Editor
    {
        SerializedProperty xvnmlAssetProperty;

        private string content;
        private Vector2 scrollPosition = Vector2.zero;
        private bool useExternalTool;
        private XVNMLAsset asset;
        public const string FileExtension = ".xvnml";
        private string path;
        FileSystemWatcher watcher;

        private void OnEnable()
        {
            xvnmlAssetProperty ??= serializedObject.FindProperty("asset");
            asset = (serializedObject.targetObject as XVNMLAsset);
        }

        public override void OnInspectorGUI()
        {
            OnXVNMLInspectorGUI();
            serializedObject.ApplyModifiedProperties();

        }


        private void OnXVNMLInspectorGUI()
        {
            content = asset.content;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(xvnmlAssetProperty, new GUIContent("XVNML File"), GUILayout.ExpandWidth(true));
            // Options
            if (GUILayout.Button("Edit"))
            {
                // TODO: Open External Tool if prompted
                if (useExternalTool)
                {
                    ProcessStartInfo processStart = new(XVNMLProjectSettings.ExternalEditorPath, AssetDatabase.GetAssetPath(xvnmlAssetProperty.objectReferenceValue ?? null));
                    Process.Start(processStart);
                    return;
                }

                // TODO: Otherwise, open Build-In XVNML Editor
            }

            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                path = RefreshMaterial();
                serializedObject.ApplyModifiedProperties();
            }


            EditorGUI.BeginDisabledGroup(true);
            content = EditorGUILayout.TextArea(content, GUILayout.MinHeight(750));
            EditorGUI.EndDisabledGroup();
        }

        private string RefreshMaterial()
        {
            var path = AssetDatabase.GetAssetPath(xvnmlAssetProperty.objectReferenceValue ?? null);

            if (path != string.Empty && path.Contains(xvnmlAssetProperty.objectReferenceValue.name + FileExtension))
            {
                asset = (serializedObject.targetObject as XVNMLAsset);

                // TODO: Validate .xvnml file before parsing information
                using StreamReader sm = new(path);

                asset.content = sm.ReadToEnd();
                content = asset.content;

                asset.filePath = path;
                asset.root = XVNMLObj.Create(path);
            }

            return path;
        }
    }
}