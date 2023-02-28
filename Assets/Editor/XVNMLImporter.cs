#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using XVNML.XVNMLUtility;

namespace XVNML2U.Mono.Editor
{
    [ScriptedImporter(1, "xvnml", AllowCaching = true)]
    public sealed class XVNMLImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var xvnml = new XVNMLAsset();
            xvnml.filePath = assetPath;
            Debug.Log(xvnml.top.Root.TagName);
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/xvnml_file_icon.png");
            EditorGUIUtility.SetIconForObject(xvnml, icon);
            ctx.AddObjectToAsset(xvnml.top.Root.TagName, xvnml);
            ctx.SetMainObject(xvnml);
        }
    }
}

#endif