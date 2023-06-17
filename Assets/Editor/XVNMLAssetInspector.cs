using UnityEditor;
using UnityEngine;
using XVNML2U.Mono;

namespace XVNML2U
{
#if UNITY_EDITOR
    [CustomEditor(typeof(XVNMLAsset))]
    public sealed class XVNMLAssetInspector : Editor
    {
        SerializedProperty xvnmlContent;
        public void OnEnable()
        {
            xvnmlContent = serializedObject.FindProperty("content");
        }

        public override void OnInspectorGUI()
        {
            DrawImporterGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawImporterGUI()
        {
            (serializedObject.targetObject as XVNMLAsset).ReadContentFromFile();

            EditorStyles.textField.wordWrap = true;
            EditorGUILayout.TextArea(xvnmlContent == null ? string.Empty : xvnmlContent.stringValue
                , GUILayout.ExpandWidth(true)
                , GUILayout.ExpandHeight(true)
                , GUILayout.MinHeight(500));
        }
    }
#endif
}
