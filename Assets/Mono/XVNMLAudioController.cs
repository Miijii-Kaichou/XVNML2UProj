using System.Collections.Generic;
using UnityEngine;
using XVNML.Utilities.Tags;


namespace XVNML2U.Mono
{
    [DisallowMultipleComponent]
    public sealed class XVNMLAudioController : Singleton<XVNMLAudioController>
    {
        // We'll have multiple audioSources for the channel processes.
        private static AudioSource[] AudioSources;

        private static SortedDictionary<string, AudioClip> AudioMap = new();
        // Data
        private static List<Audio> Audio;

        private const int FullVolume = 100;

        internal static void SetMusic(int channel, string audioName)
        {
            AudioSources[channel].clip = AudioMap[audioName];
        }

        internal static void SetVolume(int channel, int volume = 100)
        {
            AudioSources[channel].volume = ((float)volume / (float)FullVolume);
        }

        internal static void EnableLoop(int channel)
        {
            AudioSources[channel].loop = true;
        }

        internal static void DisableLoop(int channel)
        {
            AudioSources[channel].loop = false;
        }

        internal static void Play(int channel)
        {
            if (AudioSources[channel].clip == null)
            {
                Debug.LogWarning($"Sound for Channel {channel} was not set. Sound play is ignored.");
                return;
            }
            AudioSources[channel].Play();
        }

        internal static void Pause(int channel)
        {
            if (AudioSources[channel].clip == null)
            {
                Debug.LogWarning($"Sound for Channel {channel} was not set. Sound pause is ignored.");
                return;
            }
            AudioSources[channel].Pause();
        }

        internal static void PlayOneShot(int channel, string audioName, int volume)
        {
            AudioSources[channel].PlayOneShot(AudioMap[audioName], volume / FullVolume);
        }

        internal static void StopPlaying(int channel)
        {
            if (AudioSources[channel].clip == null)
            {
                Debug.LogWarning($"Sound for Channel {channel} was not set. Sound stop is ignored.");
                return;
            }
            AudioSources[channel].Stop();
        }

        internal static void Init(Audio[] audio)
        {
            if (IsNull) return;

            AudioSources = new AudioSource[DialogueProcessAllocator.ChannelSize];
            for (int i = 0; i < AudioSources.Length; i++)
            {
                AudioSources[i] = Instance.gameObject.AddComponent<AudioSource>();
                AudioSources[i].playOnAwake = false;
            }
            Audio = new List<Audio>();

            for (int i = 0; i < audio.Length; i++)
            {
                GenerateAudioAndAddToMap(audio[i]);
            }
        }

        private static void GenerateAudioAndAddToMap(Audio audio)
        {
            if (audio == null) return;
            if (audio.GetAudioTargetPath() == null) return;
            AudioClip clip = XVNMLModule.ProcessAudioClip(audio.GetAudioTargetPath());
            clip.name = audio.TagName;
            Audio.Add(audio);
            AudioMap.Add(audio.TagName, clip);
        }
    }
}
