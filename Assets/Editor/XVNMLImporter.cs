#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace XVNML2U.Mono.Core
{
    [ScriptedImporter(1, "xvnml")]
    public sealed class XVNMLImporter : ScriptedImporter
    {
        public AssetImportContext StoredContext { get; private set; }
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var xvnml = ScriptableObject.CreateInstance<XVNMLAsset>();
            var name = Path.GetFileName(ctx.assetPath);
            xvnml.filePath = ctx.assetPath;
            xvnml.ReadContentFromFile();

            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/xvnml_file_icon.png");
            EditorGUIUtility.SetIconForObject(xvnml, icon);
            ctx.AddObjectToAsset(name, xvnml);
            ctx.SetMainObject(xvnml);
            StoredContext = ctx;
        }
    }
}
#endif