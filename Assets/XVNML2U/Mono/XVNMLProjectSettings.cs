using UnityEditor;
using UnityEngine;


namespace XVNML2U.Mono
{
    public sealed class XVNMLProjectSettings : ScriptableObject
    {
        public const string k_XVNMLProjectSettingsPath = "Assets/XVNML2UProjectSettings.asset";

        // Initialize default project settings here
        [SerializeField]
        private bool _checkForUpdatesOnStartUp = true;
        public bool CheckForUpdatesOnStartUp => _checkForUpdatesOnStartUp;

        [SerializeField]
        private bool _allowVSCodeEditing;
        public bool AllowVSCodeEditing => _allowVSCodeEditing;

        [SerializeField]
        private bool _disableTokenizer;
        public bool DisableTokenizer => _disableTokenizer;

        [SerializeField]
        private uint _defaultInterval = 1;
        public uint ThreadRate => _defaultInterval;

        [SerializeField, Range(1, 12)]
        private int _CDPChannelLimit;
        public int CDPChannelLimit => _CDPChannelLimit;

        [SerializeField]
        private bool _receiveLogs = false;
        public bool ReceiveLogs => _receiveLogs;

        [SerializeField]
        private bool _enableVerbose = true;
        public bool EnableVerbose => _enableVerbose;

        [SerializeField]
        private bool _enableWarning = true;
        public bool EnableWarning => _enableWarning;

        [SerializeField]
        private bool _enableError = true;
        public bool EnableError => _enableError;

        [SerializeField]
        private bool _pauseGamePlayOnError = false;
        public bool PauseGamePlayOnError => _pauseGamePlayOnError;

        internal static XVNMLProjectSettings GetOrCreateSettings()
        {
            XVNMLProjectSettings settings = null;

#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<XVNMLProjectSettings>(k_XVNMLProjectSettingsPath);
#endif

            if (settings == null)
            {
                settings = CreateInstance<XVNMLProjectSettings>();

                // Set your default setting values here
                settings._checkForUpdatesOnStartUp = true;
                settings._allowVSCodeEditing = true;
                settings._disableTokenizer = true;
                
                settings._defaultInterval = 1;
                settings._CDPChannelLimit = 12;

                settings._receiveLogs = true;
                settings._enableVerbose = true;
                settings._enableWarning = true;
                settings._enableError = true;
                settings._pauseGamePlayOnError = true;
#if UNITY_EDITOR
                AssetDatabase.CreateAsset(settings, k_XVNMLProjectSettingsPath);
                AssetDatabase.SaveAssets();
#endif
            }
            return settings;
        }

#if UNITY_EDITOR
        public static SerializedObject GetSerializedSettings()
        {
            XVNML2USettingsUtil.Set(GetOrCreateSettings());
            return new SerializedObject(GetOrCreateSettings());
        }
#endif
    }
}
