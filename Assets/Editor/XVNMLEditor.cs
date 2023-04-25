using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace XVNML2U.Mono.Core
{
#if UNITY_EDITOR
    public sealed class XVNMLEditor : EditorWindow
    {
        private static XVNMLEditor Instance;

        [MenuItem("Window/XVNML2U/XVNML Editor")]
        public static void OpenTextEditorWindow()
        {
            Instance ??= GetWindow<XVNMLEditor>();
            Instance.titleContent = new GUIContent("");
        }

        public void CreateGUI()
        {
            rootVisualElement.Add(new Label("Hewwo"));
        }
    }
#endif
}