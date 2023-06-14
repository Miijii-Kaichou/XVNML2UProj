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

        [Macro("set_sound_volume")]
        private static void SetMusicVolumeMacro(MacroCallInfo info, int volume)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.SetVolume(info.process.ID, volume);
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
        private static void OneShotMacro(MacroCallInfo info, string audioName, int volume)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].SendNewAction(() =>
            {
                XVNMLAudioController.PlayOneShot(info.process.ID, audioName, volume);
                return WCResult.Ok();
            });
        }

        [Macro("hide_text_box")]
        private static void HideTextBox(MacroCallInfo info)
        {

        }

        [Macro("htb")]
        private static void HideTextBoxShortHand(MacroCallInfo info)
        {
            HideTextBox(info);
        }
    }
}
