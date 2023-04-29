using System;
using System.Collections;
using System.IO;
using UnityEngine;

using XVNML.Core.Tags;
using XVNML.Utility.Diagnostics;
using XVNML.XVNMLUtility;
using XVNML2U.Mono;

namespace XVNML2U
{
    public sealed class XVNMLModule : MonoBehaviour
    {
        [SerializeField, Tooltip("XVNML Entry Path")]
        private XVNMLAsset _main;

        [SerializeField]
        private bool ReceiveLogs = false;

        private void OnEnable()
        {
            _main.Build();
        }
        
        private void Start()
        {
            //if (ReceiveLogs == false) return;
            //StartCoroutine(LoggerListenerCycle());
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

        public static Texture2D ProcessTextureData(byte[] data)
        {
            if (data == null) return null;
            if (data.Length == 0) return null;
            Texture2D tex2D = new Texture2D(2, 2);
            if (tex2D.LoadImage(data) == false) return null;
            tex2D.Apply();
            return tex2D;
        }

        public IEnumerator LoggerListenerCycle()
        {
            while(true)
            {
                XVNMLLogger.CollectLog(out XVNMLLogMessage msg);
                if (msg == null)
                {
                    yield return null;
                    continue;
                }
                switch (msg.Level)
                {
                    case XVNMLLogLevel.Standard:
                        Debug.Log(msg.Message);
                        break;
                    case XVNMLLogLevel.Warning:
                        Debug.LogWarning(msg.Message);
                        break;
                    case XVNMLLogLevel.Error:
                        Debug.LogError(msg.Message);
                        break;
                    default:
                        break;
                }
                yield return null;
            }
        }
    }
}
