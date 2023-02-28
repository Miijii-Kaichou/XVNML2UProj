using System.IO;
using UnityEngine;

using XVNML.Core.Tags;
using XVNML.XVNMLUtility;
using XVNML2U.Mono;

namespace XVNML2U
{
    public sealed class XVNMLModule : MonoBehaviour
    {
        [SerializeField, Tooltip("XVNML Entry Path")]
        private XVNMLAsset _main;

        private void Awake()
        {
            _main.Build();
        }

        public T Get<T>(int index) where T : TagBase
        {
            return _main.top.Root.GetElement<T>(index);
        }

        public T Get<T>(string path) where T : TagBase
        {
            return _main.top.Root.GetElement<T>(path);
        }

        public T Get<T>() where T : TagBase
        {
            return _main.top.Root.GetElement<T>();
        }
    }
}
