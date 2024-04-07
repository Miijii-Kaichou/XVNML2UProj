using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace XVNML2U.Editor
{
    static class XVNMLSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateXVNMLSettingsProvider()
        {
            var provider = new SettingsProvider("Project/XVNML2U", SettingsScope.Project)
            {
                label = "XVNML2U",

                guiHandler = (searchContext) =>
                {
                    var settings = XVNMLProjectSettings.GetSerializedSettings();
                    DrawIMGUI(settings);
                },

                keywords = new HashSet<string>(new[] {"Allow", "VS", "Code", "Editing", "Disable", "Usage", "Tokenizer", "Channel", "Limit", "Source", "Directory", "Path"})
            };

            return provider;
        }

        private static void DrawIMGUI(SerializedObject settings)
        {
            SerializedProperty sp_allowVSCodeEditing = settings.FindProperty("_allowVSCodeEditing");
            SerializedProperty sp_disableUsageOfTokenizer = settings.FindProperty("_disableUsageOfTokenizer");
            SerializedProperty sp_channelLimit = settings.FindProperty("_CDPChannelLimit");

            GUIStyle boldLabelStyle = new GUIStyle(EditorStyles.boldLabel);

            EditorGUILayout.LabelField("General", boldLabelStyle);
            EditorGUILayout.PropertyField(sp_allowVSCodeEditing, new GUIContent("Allow VS Code Editing"));
            EditorGUILayout.PropertyField(sp_disableUsageOfTokenizer, new GUIContent("Disable Usage of Tokenizer"));
            EditorGUILayout.PropertyField(sp_channelLimit, new GUIContent("CDP Channel Limit"));
        }
    }
}
#endif