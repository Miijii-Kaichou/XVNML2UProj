#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Unity objects hould not use null propagation.

using DG.Tweening;
using UnityEngine;
using XVNML.Utilities.Macros;
using XVNML2U.Data;
using XVNML2U.Mono;

namespace XVNML2U
{
    [MacroLibrary(typeof(UMLControl))]
    internal class UMLControl : ActionSender
    {
        private static UMLControl Instance => new();

        [Macro("debug")]
        private static void DebugLogMacro(MacroCallInfo info, string message)
        {
            Debug.Log(message);
        }

        [Macro("disable_sound_loop")]
        [Macro("dsndl")]
        private static void DisableLoopMacro(MacroCallInfo info)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLAudioController.DisableLoop(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("enable_sound_loop")]
        [Macro("esndl")]
        private static void EnableLoopMacro(MacroCallInfo info)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLAudioController.EnableLoop(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("htb")]
        [Macro("hide_text_box")]
        private static void HideTextBox(MacroCallInfo info)
        {
            var control = DialogueProcessAllocator.ProcessReference[info.process.ID];
            control.IsHidden = true;
        }

        [Macro("play_sound_effect")]
        [Macro("play_sfx")]
        [Macro("psfx")]
        private static void OneShotMacro(MacroCallInfo info, string audioName, uint volume)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.PlayOneShot(info.process.ID, audioName, (int)volume);
                return WCResult.Ok();
            });
        }

        [Macro("pause_sound")]
        [Macro("pssnd")]
        private static void PauseMacro(MacroCallInfo info)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLAudioController.Pause(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("play_sound")]
        [Macro("plsnd")]
        private static void PlayMacro(MacroCallInfo info)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLAudioController.Play(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("set_sound")]
        [Macro("ssnd")]
        private static void SetMusicMacro(MacroCallInfo info, string audioName)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLAudioController.SetMusic(info.process.ID, audioName);
                return WCResult.Ok();
            });
        }

        [Macro("set_sound_volume")]
        [Macro("ssndv")]
        private static void SetMusicVolumeMacro(MacroCallInfo info, uint volume)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLAudioController.SetVolume(info.process.ID, (int)volume);
                return WCResult.Ok();
            });
        }

        [Macro("loop_sound")]
        [Macro("loop")]
        private static void SetSoundLoopMacro(MacroCallInfo info, bool loop)
        {
            Instance.SendNewAction(() =>
            {
                if (loop)
                {
                    XVNMLAudioController.EnableLoop(info.process.ID);
                    return WCResult.Ok();
                }

                XVNMLAudioController.DisableLoop(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("stb")]
        [Macro("show_text_box")]
        private static void ShowTextBox(MacroCallInfo info)
        {
            var control = DialogueProcessAllocator.ProcessReference[info.process.ID];
            control.IsHidden = false;
        }

        [Macro("load_prop")]
        [Macro("prop")]
        private static void LoadPropImageMacro(MacroCallInfo info, string imageName, int x, int y)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLPropsControl.LoadImage(imageName, x, -y);
                return WCResult.Ok();
            });
        }

        [Macro("load_prop")]
        [Macro("prop")]
        private static void LoadPropImageMacro(MacroCallInfo info, string imageName, string horizontalAnchoring, string verticalAnchoring)
        {
            LoadPropImageMacro(info, imageName, horizontalAnchoring, verticalAnchoring, 0, 0);
        }

        [Macro("load_prop")]
        [Macro("prop")]
        private static void LoadPropImageMacro(MacroCallInfo info, string imageName, string horizontalAnchoring, string verticalAnchoring, int xOffset, int yOffset)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLPropsControl.LoadImage(imageName, horizontalAnchoring.Parse<Anchoring>(), verticalAnchoring.Parse<Anchoring>(), xOffset, yOffset);
                return WCResult.Ok();
            });
        }

        [Macro("set_prop_scale")]
        [Macro("spscl")]
        private static void SetPropImageScaleMacro(MacroCallInfo info, int xScale, int yScale)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLPropsControl.SetPropScale(xScale, yScale);
                return WCResult.Ok();
            });
        }

        [Macro("clear_prop")]
        [Macro("clrp")]
        private static void ClearPropImageMacro(MacroCallInfo info, string imageName)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLPropsControl.UnloadImage(imageName);
                return WCResult.Ok();
            });
        }

        [Macro("set_prop_loading_mode")]
        [Macro("spldm")]
        private static void SetPropLoadingModeMacro(MacroCallInfo info, string mode)
        {
            TransitionMode transitionMode = mode.Parse<TransitionMode>();
            Instance.SendNewAction(() =>
            {
                XVNMLPropsControl.SetPropTransitionMode(transitionMode);
                return WCResult.Ok();
            });
        }

        [Macro("set_prop_loading_duration")]
        [Macro("spldd")]
        private static void SetPropLoadingDurationMacro(MacroCallInfo info, float duration)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLPropsControl.SetTransitionDuration(duration);
                return WCResult.Ok();
            });
        }

        [Macro("camera_shake")]
        [Macro("cmrsk")]
        private static void CameraShakeMacro(MacroCallInfo info, float duration)
        {
            CameraShakeMacro(info, duration, 3);
        }

        [Macro("camera_shake")]
        [Macro("cmrsk")]
        private static void CameraShakeMacro(MacroCallInfo info, float duration, float strength)
        {
            var processID = info.process.ID;
            Instance.SendNewAction(() =>
            {
                Camera moduleCamera = DialogueProcessAllocator.ProcessReference[processID].Module!.Camera;
                moduleCamera.DOShakePosition(duration, strength);
                return WCResult.Ok();
            });
        }

        [Macro("camera_shake")]
        [Macro("cmrsk")]
        private static void CameraShakeMacro(MacroCallInfo info, float duration, string configString)
        {
            var processID = info.process.ID;

            string[] data = configString.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

            float strength = data[0].ToFloat(3);
            int vibrato = data[1].ToInt(10);
            float randomness = data[2].ToFloat(90);
            bool fadeOut = data[3].ToBool(true);
            ShakeRandomnessMode mode = data[4].Parse<ShakeRandomnessMode>();

            Instance.SendNewAction(() =>
            {
                Camera moduleCamera = DialogueProcessAllocator.ProcessReference[processID].Module!.Camera;
                moduleCamera.DOShakePosition(duration, strength, vibrato, randomness, fadeOut, mode);
                return WCResult.Ok();
            });
        }
    }
}

#pragma warning restore IDE0060 // Unity objects hould not use null propagation.
#pragma warning restore IDE0051 // Remove unused private members