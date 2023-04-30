using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using XVNML.XVNMLUtility.Tags;

namespace XVNML2U
{
    public enum CastGraphicMode
    {
        Image,
        Sprite,
        Live2D,
        Other
    }

    public sealed class CastEntity : MonoBehaviour
    {
        [SerializeField, Header("General")]
        public string associateWithName;

        [SerializeField, Header("Graphics")]
        CastGraphicMode graphicMode;

        //We'll have the UI change based on the mode in the future
        [SerializeField] SpriteRenderer spriteViewer;
        [SerializeField] UnityEngine.UI.Image imageViewer;

        [SerializeField, Header("Audio")]
        AudioSource voiceBox;

        //Data
        public SortedDictionary<string, Sprite> PortraitLibrary = new();
        public SortedDictionary<string, AudioClip> VoiceLibrary = new();

        void Awake()
        {
            voiceBox ??= GetComponent<AudioSource>();
            spriteViewer ??= GetComponent<SpriteRenderer>();
            imageViewer ??= GetComponent<UnityEngine.UI.Image>();
        }

        public void GenerateAndAddToPortraitLibrary(ReadOnlySpan<char> name, byte[] data)
        {
            if (data == null) return;
            if (data.Length == 0) return;
            Texture2D tex2D = XVNMLModule.ProcessTextureData(data);
            Sprite newSprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
            newSprite.name = name.ToString();
            PortraitLibrary.Add(name.ToString(), newSprite);
        }

        public void GenerateAndAddToVoiceLibrary(ReadOnlySpan<char> name, string path)
        {
            if (path == string.Empty) return;
            using (UnityWebRequest requestAudio = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                if (requestAudio == null) return;

                UnityWebRequestAsyncOperation operation = requestAudio.SendWebRequest();
                while (!operation.isDone) continue;

                if(requestAudio.result == UnityWebRequest.Result.ConnectionError || 
                    requestAudio.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Failed to load audio.");
                    return;
                }

                AudioClip newClip = DownloadHandlerAudioClip.GetContent(requestAudio);
                newClip.name = name.ToString();
                VoiceLibrary.Add(name.ToString(), newClip);
                return;
            }
        }

        internal void ChangeExpression(string name)
        {
            if (PortraitLibrary == null) return;
            switch (graphicMode)
            {
                case CastGraphicMode.Image:
                    if (imageViewer == null) return;
                    if (PortraitLibrary.ContainsKey(name) == false)
                    {
                        imageViewer.sprite = null;
                        return;
                    }
                    imageViewer.sprite = PortraitLibrary[name] ?? null;
                    return;
                case CastGraphicMode.Sprite:
                    if (spriteViewer == null) return;
                    if (PortraitLibrary.ContainsKey(name) == false)
                    {
                        spriteViewer.sprite = null;
                        return;
                    }
                    spriteViewer.sprite = PortraitLibrary[name] ?? null;
                    return;
                case CastGraphicMode.Live2D:
                    throw new NotImplementedException();
                case CastGraphicMode.Other:
                    return;
                default:
                    return;
            }
        }

        internal void ChangeVoice(string name)
        {
            if (VoiceLibrary == null) return;
            if (name == string.Empty || name == null) return;
            if (VoiceLibrary.ContainsKey(name) == false) return;
            voiceBox.clip = VoiceLibrary[name];
            voiceBox.Play();
        }

        internal void Construct(Cast cast)
        {
            ProducePortraitLibrary(cast.Portraits);
            ProduceVoiceLibrary(cast.Voices);
            gameObject.name = $"{cast.TagName} [ActiveCastMember]";
        }

        private void ProduceVoiceLibrary(Voice[] voices)
        {
            for(int i = 0; i < voices.Length; i++)
            {
                if (voices[i] == null) return;
                if (voices[i].audioTarget == null) return;
                GenerateAndAddToVoiceLibrary(voices[i].TagName, voices[i].audioTarget.GetAudioTargetPath());
            }
        }

        private void ProducePortraitLibrary(Portrait[] portraits)
        {
            for(int i = 0; i < portraits.Length; i++)
            {
                GenerateAndAddToPortraitLibrary(portraits[i].TagName, portraits[i].imageTarget.GetImageData());
            }
        }
    }
}
