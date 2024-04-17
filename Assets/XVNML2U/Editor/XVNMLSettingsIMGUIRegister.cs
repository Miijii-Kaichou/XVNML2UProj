using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using XVNML2U.Mono;

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
            SerializedProperty sp_checkForUpdates = settings.FindProperty("_checkForUpdates");
            
            SerializedProperty sp_defaultInterval = settings.FindProperty("_defaultInterval");
            SerializedProperty sp_channelLimit = settings.FindProperty("_CDPChannelLimit");

            SerializedProperty sp_allowVSCodeEditing = settings.FindProperty("_allowVSCodeEditing");
            SerializedProperty sp_disableTokenizer = settings.FindProperty("_disableTokenizer");

            SerializedProperty sp_receiveLogs = settings.FindProperty("_receiveLogs");
            SerializedProperty sp_enableVerbose = settings.FindProperty("_enableVerbose");
            SerializedProperty sp_enableWarning = settings.FindProperty("_enableWarning");
            SerializedProperty sp_enableError = settings.FindProperty("_enableError");
            SerializedProperty sp_pauseGamePlayOnError = settings.FindProperty("_pauseGamePlayOnError");

            GUIStyle boldLabelStyle = new GUIStyle(EditorStyles.boldLabel);

            EditorGUILayout.LabelField("General", boldLabelStyle);
            EditorGUILayout.PropertyField(sp_checkForUpdates, new GUIContent("Check For Updates"));
            EditorGUILayout.PropertyField(sp_allowVSCodeEditing, new GUIContent("Allow VS Code Editing"));
            EditorGUILayout.PropertyField(sp_disableTokenizer, new GUIContent("Disable Tokenizer"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Dialogue Writer", boldLabelStyle);
            EditorGUILayout.PropertyField(sp_defaultInterval, new GUIContent("Default Interval"));
            EditorGUILayout.PropertyField(sp_channelLimit, new GUIContent("CDP Channel Limit"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Log Listener", boldLabelStyle);
            EditorGUILayout.PropertyField(sp_receiveLogs, new GUIContent("Receive Logs"));
            
            EditorGUI.BeginDisabledGroup(!sp_receiveLogs.boolValue);
            EditorGUILayout.PropertyField(sp_enableVerbose, new GUIContent("Enable Verbose"));
            EditorGUILayout.PropertyField(sp_enableWarning, new GUIContent("Enable Warning"));
            EditorGUILayout.PropertyField(sp_enableError, new GUIContent("Enable Error"));
            EditorGUILayout.PropertyField(sp_pauseGamePlayOnError, new GUIContent("Pause On Error"));
            EditorGUI.EndDisabledGroup();

            settings.ApplyModifiedProperties();
            settings.Update();
        }
    }
}
#endif