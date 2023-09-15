#pragma warning disable IDE0051 // Remove unused private members
#nullable enable

using UnityEngine;
using XVNML.Core.Dialogue.Structs;
using XVNML.Utilities.Macros;
using XVNML2U.Data;
using XVNML2U.Mono;

namespace XVNML2U
{
    [MacroLibrary(typeof(UMLScenes))]
    internal class UMLScenes : ActionSender
    {
        private static UMLScenes Instance => new();

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
    }
}

#pragma warning restore IDE0051 // Remove unused private members