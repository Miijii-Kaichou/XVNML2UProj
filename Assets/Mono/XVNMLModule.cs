using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using XVNML.Core.Tags;
using XVNML.Utility.Dialogue;
using XVNML.XVNMLUtility;

#nullable enable
namespace XVNML2U.Mono
{
    [DisallowMultipleComponent]
    public sealed class XVNMLModule : MonoBehaviour
    {
        [SerializeField, Tooltip("XVNML Entry Path")]
        private XVNMLAsset _main;

        internal Action<XVNMLObj> onModuleBuildProcessComplete;

        internal TagBase? Root => _main.top!.Root;

        public void Build()
        {
            ReactionRegistry.BeginRegistrationProcess();

            Application.quitting += ShutDown;
            
            #if UNITY_EDITOR
            EditorApplication.quitting += ShutDown;
            EditorApplication.playModeStateChanged += EvaluatePlayModeState;
            #endif

            _main.Build(onModuleBuildProcessComplete);
        }

        private void EvaluatePlayModeState(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingPlayMode)
                DialogueWriter.ShutDown();
        }

        private void ShutDown()
        {
            DialogueWriter.ShutDown();

            Application.quitting -= ShutDown;

            #if UNITY_EDITOR
            EditorApplication.quitting -= ShutDown;
            EditorApplication.playModeStateChanged -= EvaluatePlayModeState;
            #endif
        }

        public T? Get<T>(int index) where T : TagBase
        {
            return Root?.GetElement<T>(index);
        }

        public T? Get<T>(string path) where T : TagBase
        {
            return Root?.GetElement<T>(path);
        }

        public T? Get<T>() where T : TagBase
        {
            return Root?.GetElement<T>();
        }

        public static Texture2D? ProcessTextureData(byte[]? data)
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
    }
}
#nullable disable