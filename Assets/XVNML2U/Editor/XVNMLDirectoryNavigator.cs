using System.IO;
using System.Linq;
using UnityEngine;

namespace XVNML2U.Editor
{
    internal static class XVNMLDirectoryNavigator
    {
        internal static string GoToDirectory(string path)
        {
            return Directory
                .GetDirectories(Application.dataPath, $"XVNML2U/{path}", SearchOption.AllDirectories)
                .First();
        }
    }
}