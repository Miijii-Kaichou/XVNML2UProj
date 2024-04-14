using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace XVNML2U.Editor
{
#if UNITY_EDITOR

    public sealed class XVNMLProjectMenuItems : UnityEditor.Editor
    {
        private static string FileTemplateSource = "Assets/Resources/XVNML2U/Templates/";
        private static bool FileTemplateSourceUpdated = false;
        
        private static readonly StringBuilder StringBuilder = new StringBuilder();

        private const string FileMenuPath = "Assets/Create/XVNML2U/";
        
        private const string XVNMLFileExtension = ".xvnml";
        private const string CSharpFileExtension = ".cs";
        private const string XVNMLTemplateExtension = XVNMLFileExtension + ".txt";
        private const string CSharpTemplateExtension = CSharpFileExtension + ".txt";
        
        private const string TemplatePath = "Resources/Templates";


        [MenuItem(FileMenuPath + "XVNML/Proxy", priority = 81)]
        private static void CreateXVNMLProxyFile()
        {
            GenerateXVNMLTemplate("NewProxy");
        }

        [MenuItem(FileMenuPath + "XVNML/Source", priority = 81)]
        private static void CreateXVNMLSourceFile()
        {
            GenerateXVNMLTemplate("NewSource");
        }

        [MenuItem(FileMenuPath + "C#/MacroLibrary", priority = 82)]
        private static void CreateMacroLibrary()
        {
            GenerateCSharpTemplate("NewMacroLibrary");
        }

        [MenuItem(FileMenuPath + "C#/User-Defined Tag", priority = 82)]
        private static void CreateCustomTagScript()
        {
            GenerateCSharpTemplate("NewUDTag");
        }

        [MenuItem(FileMenuPath + "C#/Reaction", priority = 83)]
        private static void CreateReactionScript()
        {
            GenerateCSharpTemplate("NewReaction");
        }

        [MenuItem(FileMenuPath + "C#/Runtime Environment Model", priority = 83)]
        private static void CreateRuntimeEnvironmentModel()
        {
            GenerateCSharpTemplate("NewRuntimeEnvironmentModel");
        }

        #region Helper Methods
        private static void GenerateXVNMLTemplate(string name)
        {
            ProvideUpdatedTemplateLocation();

            StringBuilder.Append(FileTemplateSource)
                .Append('/')
                .Append(name)
                .Append(XVNMLTemplateExtension);

            string target = StringBuilder.ToString();
            string defaultFileName = name + XVNMLFileExtension;

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(target, defaultFileName);
            StringBuilder.Clear();
        }

        private static void GenerateCSharpTemplate(string name)
        {
            ProvideUpdatedTemplateLocation();

            StringBuilder.Append(FileTemplateSource)
               .Append('/')
               .Append(name)
               .Append(CSharpTemplateExtension);

            string target = StringBuilder.ToString();
            string defaultFileName = name + CSharpFileExtension;

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(target, defaultFileName);
            StringBuilder.Clear();
        }

        private static void ProvideUpdatedTemplateLocation()
        {
            var dataPath = Application.dataPath;

            if (FileTemplateSourceUpdated) return;

            FileTemplateSource = XVNMLDirectoryNavigator.GoToDirectory(TemplatePath);

            FileTemplateSourceUpdated = true;
        } 
        #endregion
    }
#endif
}