using System;
using UnityEngine;
using XVNML.Utility.Macros;
using XVNML2U.Assets.Extensions;
using XVNML2U.Data;
using XVNML2U.Mono;

namespace XVNML2U
{
    [MacroLibrary(typeof(UnityMacroLibrary))]
    internal static class UnityMacroLibrary
    {
        [Macro("debug")]
        private static void DebugLogMacro(MacroCallInfo info, string message)
        {
            Debug.Log(message);
        }

        [Macro("play_sound")]
        private static void PlayMacro(MacroCallInfo info)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.Play(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("pause_sound")]
        private static void PauseMacro(MacroCallInfo info)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.Pause(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("set_sound")]
        private static void SetMusicMacro(MacroCallInfo info, string audioName)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.SetMusic(info.process.ID, audioName);
                return WCResult.Ok();
            });
        }

        [Macro("set_volume")]
        private static void SetMusicVolumeMacro(MacroCallInfo info, uint volume)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.SetVolume(info.process.ID, (int)volume);
                return WCResult.Ok();
            });
        }

        [Macro("enable_sound_loop")]
        private static void EnableLoopMacro(MacroCallInfo info)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.EnableLoop(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("disable_sound_loop")]
        private static void disableLoopMacro(MacroCallInfo info)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.DisableLoop(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("play_one_shot")]
        private static void OneShotMacro(MacroCallInfo info, string audioName, uint volume)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.PlayOneShot(info.process.ID, audioName, (int)volume);
                return WCResult.Ok();
            });
        }

        [Macro("htb")]
        private static void HideTextBoxShortHand(MacroCallInfo info)
        {
            HideTextBox(info);
        }

        [Macro("hide_text_box")]
        private static void HideTextBox(MacroCallInfo info)
        {
            var control = DialogueProcessAllocator.ProcessReference[info.process.ID];
            control.IsHidden = true;
        }

        [Macro("stb")]
        private static void ShowTextBoxShortHand(MacroCallInfo info)
        {
            ShowTextBox(info);
        }

        [Macro("show_text_box")]
        private static void ShowTextBox(MacroCallInfo info)
        {
            var control = DialogueProcessAllocator.ProcessReference[info.process.ID];
            control.IsHidden = false;
        }

        [Macro("cue_cast")]
        private static void CueCastMacro(MacroCallInfo info, string name, string anchoring, uint offset)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage.PositionCast(info, name, anchoring.Parse<Anchoring>(), offset);
        }

        [Macro("move_cast")]
        private static void MoveCastMacro(MacroCallInfo info, string name, uint units, uint isNegative)
        {
            var sign = isNegative == 0 ? 1 : -1;
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage.MoveCast(info, name, (int)units * sign);
        }

        [Macro("set_cast_motion")]
        private static void SetCastMotionMacro(MacroCallInfo info, string motionType)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage.SetCastMotion(motionType.Parse<CastMotionType>());
        }

        [Macro("set_cast_motion_duration")]
        private static void SetCastMotionDurationMacro(MacroCallInfo info, float motionDuration)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage.SetCastMotionDuration(motionDuration);
        }

        [Macro("cast_enters_from")]
        private static void CastEntersFromMacro(MacroCallInfo info, string side)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage.HaveCastEnterFrom(side.Parse<EnterSide>());
        }
    }
}
