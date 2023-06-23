using System;
using UnityEngine;
using XVNML.Utility.Macros;
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
        private static void DisableLoopMacro(MacroCallInfo info)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.DisableLoop(info.process.ID);
                return WCResult.Ok();
            });
        }

        [Macro("play_sfx")]
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
        private static void CueCastMacro(MacroCallInfo info, string anchoring)
        {
            var name = info.process.CurrentCastInfo.Value.name;
            CueCastMacro(info, name, anchoring, 0);
        }

        [Macro("cue_cast")]
        private static void CueCastMacro(MacroCallInfo info, string anchoring, uint offset)
        {
            var name = info.process.CurrentCastInfo.Value.name;
            CueCastMacro(info, name, anchoring, offset);
        }

        [Macro("cue_cast")]
        private static void CueCastMacro(MacroCallInfo info, string name, string anchoring)
        {
            CueCastMacro(info, name, anchoring, 0);
        }

        [Macro("cue_cast")]
        private static void CueCastMacro(MacroCallInfo info, string name, string anchoring, uint offset)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage.PositionCast(info, name, anchoring.Parse<Anchoring>(), offset);
        }

        [Macro("move_cast")]
        private static void MoveCastMacro(MacroCallInfo info, string name, int units)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage.MoveCast(info, name, units);
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

        [Macro("react")]
        private static void ReactMacro(MacroCallInfo info, string reactionName)
        {
            var castName = info.process.CurrentCastInfo?.name;
            ReactMacro(info, castName, reactionName);
        }

        [Macro("react")]
        private static void ReactMacro(MacroCallInfo info, string castName, string reactionName)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                ReactionRegistry.DoReaction(reactionName, castName);
                return WCResult.Ok();
            });
        }

        #region Standard Macro Overrides
        [Macro("exp")]
        internal static void SetCastExpressionMacroShortHand(MacroCallInfo info, string castName, string value)
        {
            SetCastExpressionMacro(info, castName, value);
        }

        [Macro("exp")]
        internal static void SetCastExpressionMacroShortHand(MacroCallInfo info, string castName, int value)
        {
            SetCastExpressionMacro(info, castName, value);
        }

        [Macro("expression")]
        internal static void SetCastExpressionMacro(MacroCallInfo info, string castName, string value)
        {
            var processRef = DialogueProcessAllocator.ProcessReference[info.process.ID];
            var stage = processRef.Stage;
            processRef.SendNewAction(() =>
            {
                stage.ChangeExpression(castName, value);
                return WCResult.Ok();
            });
        }

        [Macro("expression")]
        internal static void SetCastExpressionMacro(MacroCallInfo info, string castName, int value)
        {
            SetCastExpressionMacro(info, castName, value.ToString());
        }

        [Macro("vo")]
        internal static void SetCastVoiceShortHand(MacroCallInfo info, string castName, string value)
        {
            SetCastVoice(info, castName, value);
        }

        [Macro("vo")]
        internal static void SetCastVoiceShortHand(MacroCallInfo info, string castName, int value)
        {
            SetCastVoice(info, castName, value);
        }

        [Macro("voice")]
        internal static void SetCastVoice(MacroCallInfo info, string castName, string value)
        {
            var processRef = DialogueProcessAllocator.ProcessReference[info.process.ID];
            var stage = processRef.Stage;
            processRef.SendNewAction(() =>
            {
                stage.ChangeVoice(castName, value);
                return WCResult.Ok();
            });
        }

        [Macro("voice")]
        internal static void SetCastVoice(MacroCallInfo info, string castName, int value)
        {
            SetCastVoice(info, castName, value.ToString());
        }
        #endregion
    }
}
