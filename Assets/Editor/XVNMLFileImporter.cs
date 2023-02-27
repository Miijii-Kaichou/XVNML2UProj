using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace XVNML2U.Mono.Editor
{
    public sealed class XVNMLFileImporter : EditorWindow
    {
        private static XVNMLFileImporter Instance;

        [MenuItem("Window/XVNML2U/XVNML Importer")]
        public static void OpenImportorWindow()
        {
            Instance ??= GetWindow<XVNMLFileImporter>();
            Instance.titleContent = new GUIContent("XVNML File Importer");
        }

        public void CreateGUI()
        {
            rootVisualElement.Add(new Label("Hewwo"));
        }
    }
}
