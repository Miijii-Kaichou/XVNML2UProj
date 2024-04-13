using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using XVNML2U.FileSupport;

namespace XVNML2U.Editor
{
#if UNITY_EDITOR
    public sealed class XVNMLProjectSettings : ScriptableObject
    {
        public const string k_XVNMLProjectSettingsPath = "Assets/XVNML2UProjectSettings.asset";

        // Initialize default project settings here
        [SerializeField]
        private bool _allowVSCodeEditing;

        [SerializeField]
        private bool _disableUsageOfTokenizer;

        [SerializeField]
        private int _CDPChannelLimit;


        internal static XVNMLProjectSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<XVNMLProjectSettings>(k_XVNMLProjectSettingsPath);
            if (settings == null)
            {
                settings = CreateInstance<XVNMLProjectSettings>();

                // Set your default setting values here
                settings._allowVSCodeEditing = true;
                settings._disableUsageOfTokenizer = true;
                settings._CDPChannelLimit = 12;

                AssetDatabase.CreateAsset(settings, k_XVNMLProjectSettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
#endif
}
