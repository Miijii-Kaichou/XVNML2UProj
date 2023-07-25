#nullable enable
using UnityEngine;
using XVNML.Utilities.Macros;
using XVNML2U.Data;
using XVNML2U.Mono;
using XVNML.Core.Dialogue.Structs;
using DG.Tweening;

namespace XVNML2U
{
    [MacroLibrary(typeof(UnityMacroLibrary))]
    internal class UnityMacroLibrary : ActionSender
    {
        private static UnityMacroLibrary Instance => new UnityMacroLibrary();

        [Macro("debug")]
        private static void DebugLogMacro(MacroCallInfo info, string message)
        {
            Debug.Log(message);
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

        [Macro("htb")]
        [Macro("hide_text_box")]
        private static void HideTextBox(MacroCallInfo info)
        {
            var control = DialogueProcessAllocator.ProcessReference[info.process.ID];
            control.IsHidden = true;
        }

        [Macro("stb")]
        [Macro("show_text_box")]
        private static void ShowTextBox(MacroCallInfo info)
        {
            var control = DialogueProcessAllocator.ProcessReference[info.process.ID];
            control.IsHidden = false;
        }

        [Macro("cue_cast")]
        [Macro("cast")]
        private static void CueCastMacro(MacroCallInfo info, string anchoring)
        {
            var name = info.process.CurrentCastInfo!.Value.name!;
            CueCastMacro(info, name, anchoring, 0);
        }

        [Macro("cue_cast")]
        [Macro("cast")]
        private static void CueCastMacro(MacroCallInfo info, string anchoring, uint offset)
        {
            var name = info.process.CurrentCastInfo!.Value.name!;
            CueCastMacro(info, name, anchoring, offset);
        }

        [Macro("cue_cast")]
        [Macro("cast")]
        private static void CueCastMacro(MacroCallInfo info, string name, string anchoring)
        {
            CueCastMacro(info, name, anchoring, 0);
        }

        [Macro("cue_cast")]
        [Macro("cast")]
        private static void CueCastMacro(MacroCallInfo info, string name, string anchoring, uint offset)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.PositionCast(info, name, anchoring.Parse<Anchoring>(), offset);
        }

        [Macro("move_cast")]
        [Macro("mcst")]
        private static void MoveCastMacro(MacroCallInfo info, int units)
        {
            var name = info.process.CurrentCastInfo?.name!;
            MoveCastMacro(info, name, units);
        }

        [Macro("move_cast")]
        [Macro("mcst")]
        private static void MoveCastMacro(MacroCallInfo info, string name, int units)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.MoveCast(info, name, units);
        }

        [Macro("set_cast_motion")]
        [Macro("scstm")]
        private static void SetCastMotionMacro(MacroCallInfo info, string motionType)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.SetCastMotion(motionType.Parse<CastMotionType>());
        }

        [Macro("set_cast_motion_duration")]
        [Macro("scstmd")]
        private static void SetCastMotionDurationMacro(MacroCallInfo info, float motionDuration)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.SetCastMotionDuration(motionDuration);
        }

        [Macro("cast_enters_from")]
        [Macro("cstef")]
        private static void CastEntersFromMacro(MacroCallInfo info, string side)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.HaveCastEnterFrom(side.Parse<EnterSide>());
        }

        [Macro("react")]
        private static void ReactMacro(MacroCallInfo info, string reactionName)
        {
            var castName = info.process.CurrentCastInfo?.name!;
            ReactMacro(info, castName, reactionName);
        }

        [Macro("react")]
        private static void ReactMacro(MacroCallInfo info, string castName, string reactionName)
        {
            Instance.SendNewAction(() =>
            {
                ReactionRegistry.DoReaction(reactionName, castName);
                return WCResult.Ok();
            });
        }

        [Macro("use_scene")]
        [Macro("scene")]
        private static void UseSceneMacro(MacroCallInfo info, string sceneName)
        {
            UseSceneMacro(info, sceneName, 0);
        }

        [Macro("use_scene")]
        [Macro("scene")]
        private static void UseSceneMacro(MacroCallInfo info, string sceneName, int layer)
        {
            Instance.SendNewAction(() =>
            {
                SceneInfo newSceneInfo = new() { name = sceneName, layer = layer };
                DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.ChangeScene(newSceneInfo);
                return WCResult.Ok();
            });
        }

        [Macro("clear_all_scenes")]
        [Macro("clrascn")]
        private static void ClearAllScenesMacro(MacroCallInfo info)
        {
            ClearSceneMacro(info, "{all_active}", 0);
        }

        [Macro("clear_scene")]
        [Macro("clrscn")]
        private static void ClearSceneMacro(MacroCallInfo info)
        {
            ClearSceneMacro(info, "{active}", 0);
        }

        [Macro("clear_scene")]
        [Macro("clrscn")]
        private static void ClearSceneMacro(MacroCallInfo info, string sceneName)
        {
            Debug.Log("USE ME BEBE!!!!");
            ClearSceneMacro(info, sceneName, 0);
        }

        [Macro("clear_scene")]
        [Macro("clrscn")]
        private static void ClearSceneMacro(MacroCallInfo info, uint layerID)
        {
            ClearSceneMacro(info, null, layerID);
        }

        [Macro("clear_scene")]
        [Macro("clrscn")]
        private static void ClearSceneMacro(MacroCallInfo info, string? sceneName, uint layerID)
        {
            Instance.SendNewAction(() =>
            {
                SceneInfo newScene = new() { name = sceneName, layer = (int)layerID };
                DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.ClearScene(newScene);
                return WCResult.Ok();
            });
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
                XVNMLPropsControl.SetPropScale(xScale, yScale, 1);
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

        [Macro("new_quest")]
        private static void InitializeNewQuestMacro(MacroCallInfo callInfo, object questID)
        {
            InitializeNewQuestMacro(callInfo, null, questID);
        }

        [Macro("new_quest")]
        private static void InitializeNewQuestMacro(MacroCallInfo callInfo, object? questCategory, object questID)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLQuestSystem.InitializeQuest(questID.ToString(), questCategory?.ToString());
                return WCResult.Ok();
            });
        }

        [Macro("complete_task")]
        private static void CompleteTaskMacro(MacroCallInfo callInfo, object questCategory, object questID)
        {
            Instance.SendNewAction(() =>
            {
                XVNMLQuestSystem.CompleteCurrentTask(questID.ToString(), questCategory?.ToString());
                return WCResult.Ok();
            });
        }

        #region Standard Macro Overrides
        [Macro("expression")]
        [Macro("portrait")]
        [Macro("exp")]
        [Macro("port")]
        internal static void SetCastExpressionMacro(MacroCallInfo info, string castName, string value)
        {
            var stage = DialogueProcessAllocator.ProcessReference[info.process.ID].Stage;
            Instance.SendNewAction(() =>
            {
                stage?.ChangeExpression(castName, value);
                return WCResult.Ok();
            });
        }

        [Macro("expression")]
        [Macro("portrait")]
        [Macro("exp")]
        [Macro("port")]
        internal static void SetCastExpressionMacro(MacroCallInfo info, string castName, int value)
        {
            SetCastExpressionMacro(info, castName, value.ToString());
        }

        [Macro("voice")]
        [Macro("vo")]
        internal static void SetCastVoice(MacroCallInfo info, string castName, string value)
        {
            var processRef = DialogueProcessAllocator.ProcessReference[info.process.ID];
            var stage = processRef.Stage;
            processRef.SendNewAction(() =>
            {
                stage?.ChangeVoice(castName, value);
                return WCResult.Ok();
            });
        }

        [Macro("voice")]
        [Macro("vo")]
        internal static void SetCastVoice(MacroCallInfo info, string castName, int value)
        {
            SetCastVoice(info, castName, value.ToString());
        }
        #endregion
    }
}
