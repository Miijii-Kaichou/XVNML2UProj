using XVNML2U.Mono;

namespace XVNML2U
{
    public static class XVNMLProjectSettingsContainer
    {
        public static XVNMLProjectSettings ActiveProjectSettings { get; private set; }

        internal static void Set(XVNMLProjectSettings settings)
        {
            ActiveProjectSettings = settings;
        }
    }
}
