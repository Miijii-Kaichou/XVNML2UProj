using UnityEditor;

namespace XVNML2U.Editor
{
#if UNITY_EDITOR

    public sealed class XVNMLProjectMenuItems : UnityEditor.Editor
    {
        private const string FileMenuPath = "Assets/Create/XVNML File/";
        private const string FileTemplateSource = "Assets/Resources/XVNML File Templates/";

        [MenuItem(FileMenuPath + "Proxy", priority = 81)]
        public static void CreateXVNMLProxyFile()
        {
            string templatePath = FileTemplateSource + $"/NewProxy.xvnml.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewProxy.xvnml");
        }

        [MenuItem(FileMenuPath + "Source", priority = 81)]
        public static void CreateXVNMLSourceFile()
        {
            string templatePath = FileTemplateSource + $"/NewSource.xvnml.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewSource.xvnml");
        }
    }
#endif
}