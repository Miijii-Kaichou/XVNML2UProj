using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using XVNML.Core.Tags;
using XVNML.Utility.Diagnostics;
using XVNML.XVNMLUtility;
using XVNML2U.Data;
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

            Texture2D tex2D = new(2, 2, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            if (tex2D.LoadImage(data) == false) return null;

            tex2D.filterMode = FilterMode.Bilinear;

            tex2D.wrapMode = TextureWrapMode.Clamp;
            tex2D.wrapModeU = TextureWrapMode.Clamp;
            tex2D.wrapModeV = TextureWrapMode.Clamp;
            tex2D.wrapModeW = TextureWrapMode.Clamp;

            tex2D.Apply();

            return tex2D;
        }

        public static AudioClip? ProcessAudioClip(string path)
        {
            if (path == string.Empty) return null;
            var extension = path.Split('.', StringSplitOptions.RemoveEmptyEntries)[1];
            var requestedAudioType = AudioType.UNKNOWN;

            if (extension == "mp2" || extension == "mp3") requestedAudioType = AudioType.MPEG;
            if (extension == "m4a") requestedAudioType = AudioType.AUDIOQUEUE;
            if (extension == "wav") requestedAudioType = AudioType.WAV;
            if (extension == "it") requestedAudioType = AudioType.IT;
            if (extension == "mod") requestedAudioType = AudioType.MOD;
            if (extension == "xm") requestedAudioType = AudioType.XM;
            if (extension == "aiff") requestedAudioType = AudioType.AIFF;
            if (extension == "ogg") requestedAudioType = AudioType.OGGVORBIS;
            
            using (UnityWebRequest requestAudio = UnityWebRequestMultimedia.GetAudioClip(path, requestedAudioType))
            {
                if (requestAudio == null) return null;

                UnityWebRequestAsyncOperation operation = requestAudio.SendWebRequest();
                while (!operation.isDone) continue;

                if (requestAudio.result == UnityWebRequest.Result.ConnectionError ||
                    requestAudio.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Failed to load audio.");
                    return null;
                }

                return DownloadHandlerAudioClip.GetContent(requestAudio);
            }
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
