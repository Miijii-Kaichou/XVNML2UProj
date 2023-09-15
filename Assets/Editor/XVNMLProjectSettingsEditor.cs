using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace XVNML2U.Editor
{
#if UNITY_EDITOR
    public sealed class XVNMLProjectSettingsEditor : EditorWindow
    {
        private static XVNMLProjectSettingsEditor Instance;

        [MenuItem("Window/XVNML2U/Project Settings")]
        public static void OpenProjectSettingsWindow()
        {
            Instance ??= GetWindow<XVNMLProjectSettingsEditor>();
            Instance.titleContent = new GUIContent("XVNML Project Settings");
        }

        public void CreateGUI()
        {
            rootVisualElement.Add(new Label("Hewwoooo!!"));
        }
    }
#endif
}