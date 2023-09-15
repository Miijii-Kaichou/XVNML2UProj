#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using XVNML2U.FileSupport;

namespace XVNML2U.Editor
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

            IncludeNecessaryExtensions();

            SaveAndReimport();
        }

        private void IncludeNecessaryExtensions()
        {
            var necessaryExt = new[]
            {
                ".xvnml",
                ".json",
                ".ha"
            };

            var extensionsList = EditorSettings.projectGenerationUserExtensions.ToList();

            foreach (var ext in necessaryExt)
            {
                if (EditorSettings.projectGenerationUserExtensions.Contains(ext)) continue;
                extensionsList.Add(ext);
            }

            EditorSettings.projectGenerationUserExtensions = extensionsList.ToArray();
        }
    }
}
#endif