using System.IO;
using UnityEngine;

using XVNML.Core.Tags;
using XVNML.XVNMLUtility;

namespace XVNML2U
{
    public sealed class XVNMLModule : MonoBehaviour
    {
        [SerializeField, Tooltip("XVNML Entry Path")]
        private string entryPath;
        [SerializeField]
        private bool initializeOnWakeUp = false;

        private XVNMLObj _main;

        private void Awake()
        {
            if (initializeOnWakeUp == false) return;
            Initialize();
        }

        public void Initialize()
        {
            var projectPath = Application.streamingAssetsPath + @"/" + entryPath;
            _main = XVNMLObj.Create(projectPath);
            Debug.Log(_main.Root.TagName);
        }

        public T Get<T>(int index) where T : TagBase
        {
            return _main.Root.GetElement<T>(index);
        }

        public T Get<T>(string path) where T : TagBase
        {
            return _main.Root.GetElement<T>(path);
        }

        public T Get<T>() where T : TagBase
        {
            return _main.Root.GetElement<T>();
        }
    }
}
