namespace XVNML2U
{
    public static class XVNML2USettingsUtil
    {
        private static XVNMLProjectSettings _activeProjectSettings;
        public static XVNMLProjectSettings ActiveProjectSettings
        {
            get
            {
                if (_activeProjectSettings == null)
                    _activeProjectSettings = XVNMLProjectSettings.GetOrCreateSettings();
                return _activeProjectSettings;
            }
        }

        internal static void Set(XVNMLProjectSettings settings)
        {
            _activeProjectSettings = settings;
        }
    }
}
