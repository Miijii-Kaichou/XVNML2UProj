using UnityEditor;
using UnityEngine;
using XVNML2U.Mono.Core;

namespace XVNML2U
{
    [CustomEditor(typeof(XVNMLImporter))]
    public sealed class XVNMLImporterInspector : Editor
    {
        SerializedProperty ctxProperty;

        public void OnEnable()
        {
            ctxProperty = serializedObject.FindProperty("StoredContent");
        }

        public override void OnInspectorGUI()
        {  
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Edit XVNML"))
            {
                //TODO: Check in XVNML2U Project Settings if 
                //edit with external tool is enabled.
            }
        }
    }
}
