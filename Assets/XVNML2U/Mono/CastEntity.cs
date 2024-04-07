using System;
using UnityEngine;
using XVNML.Core.Dialogue;
using XVNML.Utilities.Tags;

namespace XVNML2U.Mono
{
    public sealed class CastEntity : MonoBehaviour
    {
        private const int MainDialogueControl = 0;

        [Header("Graphics")]
        public CastGraphicMode graphicMode;

        [Header("Loading Mode")]
        public LoadingMode loadingMode = LoadingMode.External;

        //We'll have the UI change based on the mode in the future
        [SerializeField] SpriteRenderer spriteViewer;
        [SerializeField] UnityEngine.UI.Image imageViewer;

        [SerializeField, Header("Audio")]
        AudioSource voiceBox;

        //Data
        public ElementMediaLibrary<Sprite> PortraitLibrary = new();
        public ElementMediaLibrary<(bool perCharacter, AudioClip clip)> VoiceLibrary = new();

        void Awake()
        {
            voiceBox ??= GetComponent<AudioSource>();
            spriteViewer ??= GetComponent<SpriteRenderer>();
            imageViewer ??= GetComponent<UnityEngine.UI.Image>();
        }

        public void GenerateAndAddToPortraitLibrary(int id, ReadOnlySpan<char> name, byte[] data)
        {
            if (data == null) return;
            if (data.Length == 0) return;

            Texture2D tex2D = XVNMLModule.ProcessTextureData(data);

            if (tex2D == null) return;

            Sprite newSprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.Tight, Vector4.zero, false);
            newSprite.name = name.ToString();

            PortraitLibrary.Add(id, name.ToString(), newSprite);
        }

        public void GenerateAndAddToVoiceLibrary(int id, ReadOnlySpan<char> name, string path, bool isPerCharacter = false)
        {
            AudioClip newClip = XVNMLModule.ProcessAudioClip(path);
            newClip.name = name.ToString();
            VoiceLibrary.Add(id, name.ToString(), (isPerCharacter, newClip));
            return;
        }

        internal void ChangeExpression(int id)
        {
            if (PortraitLibrary == null) return;
            ChangeExpression(PortraitLibrary.GetStringKey(id));
        }

        internal void ChangeExpression(string name)
        {
            if (PortraitLibrary == null) return;
            switch (graphicMode)
            {
                case CastGraphicMode.Image:
                    if (imageViewer == null) return;
                    if (PortraitLibrary.ContainsName(name) == false)
                    {
                        imageViewer.sprite = null;
                        return;
                    }
                    imageViewer.sprite = PortraitLibrary[name] ?? null;
                    return;
                case CastGraphicMode.Sprite:
                    if (spriteViewer == null) return;
                    if (PortraitLibrary.ContainsName(name) == false)
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

        internal void ChangeVoice(int id)
        {
            if (VoiceLibrary == null) return;
            ChangeVoice(VoiceLibrary.GetStringKey(id));
        }

        internal void ChangeVoice(string name)
        {
            if (VoiceLibrary == null) return;
            if (name == string.Empty || name == null) return;
            if (VoiceLibrary.ContainsName(name) == false) return;
            
            voiceBox.clip = VoiceLibrary[name].clip;

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
            if (voices == null) return;

            for (int i = 0; i < voices.Length; i++)
            {
                if (voices[i] == null) return;
                if (voices[i].audioTarget == null) return;
                GenerateAndAddToVoiceLibrary(voices[i].TagID.Value, voices[i].TagName, voices[i].audioTarget.GetAudioTargetPath());
            }
        }

        private void ProducePortraitLibrary(Portrait[] portraits)
        {
            if (portraits == null) return;

            for (int i = 0; i < portraits.Length; i++)
            {
                GenerateAndAddToPortraitLibrary(portraits[i].TagID.Value, portraits[i].TagName, portraits[i].imageTarget.GetImageData());
            }
        }

        [ExecuteInEditMode]
        public void SetVoiceBox(AudioSource castEntityVoiceBox)
        {
            voiceBox = castEntityVoiceBox;
        }
    }
}
