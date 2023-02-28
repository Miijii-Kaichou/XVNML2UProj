#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace XVNML2U.Mono.Editor
{
    [ScriptedImporter(1, "xvnml", AllowCaching = true)]
    public sealed class XVNMLImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var xvnml = new XVNMLAsset();
            var name = Path.GetFileName(ctx.assetPath);
            xvnml.filePath = ctx.assetPath;
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/xvnml_file_icon.png");
            EditorGUIUtility.SetIconForObject(xvnml, icon);
            ctx.AddObjectToAsset(name, xvnml);
            ctx.SetMainObject(xvnml);
        }
    }
}
#endif