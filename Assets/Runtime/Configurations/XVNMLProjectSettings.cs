using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XVNML2U.Configuration
{
    /// <summary>
    /// When Project Settings are saved, it'll generate a projSettings.json
    /// file for xVNML to use.
    /// </summary>
    public sealed class XVNMLProjectSettings : ScriptableObject
    {
        internal string version = Application.unityVersion;
        public static string Version { get; internal set; }

        internal bool allowExplicitPathReferencing = false;
        public static bool AllowExplicitPathReferencing { get; internal set; }

        internal string projectDirectory;
        public static string ProjectDirectory { get; internal set; }

        internal SortedDictionary<string, string> assetRootPaths = new SortedDictionary<string, string>();
        public static SortedDictionary<string, string> AssetRootPaths { get; internal set; }

        // Unity Specific Settings
        // (will fall under "Dependencies")
        internal string externalEditorPath;
        public static string ExternalEditorPath { get; internal set; }
    }
}
