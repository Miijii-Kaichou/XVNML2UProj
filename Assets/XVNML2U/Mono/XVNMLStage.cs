using System;
using UnityEngine;
using XVNML.Core.Dialogue;
using XVNML.Core.Dialogue.Structs;
using XVNML.Utilities.Dialogue;
using XVNML.Utilities.Macros;
using XVNML.Utilities.Tags;

namespace XVNML2U.Mono
{
    [DisallowMultipleComponent]
    public sealed class XVNMLStage : MonoBehaviour
    {
        [SerializeField]
        private SceneController sceneController;

        [SerializeField]
        private CastController castController;

        internal void InitializeCastController(Cast[] castMembers)
        {
            castController.Init(castMembers);
        }

        internal void InitializeSceneController(Scene[] scenes)
        {
            sceneController.Init(scenes);
        }

        internal void ChangeScene(SceneInfo currentSceneInfo)
        {
            sceneController.ChangeScene(currentSceneInfo);
        }

        internal void ClearScene(SceneInfo currentSceneInfo)
        {
            sceneController.ClearScene(currentSceneInfo);
        }
        
        internal void ChangeExpression(CastInfo castInfo)
        {
            castController.ChangeExpression(castInfo);
        }

        internal void ChangeExpression(string castName, string value)
        {
            castController.ChangeExpression(new CastInfo() { name = castName, expression = value});
        }

        internal void ChangeVoice(CastInfo castInfo)
        {
            castController.ChangeVoice(castInfo);
        }

        internal void ChangeVoice(string castName, string value)
        {
            castController.ChangeVoice(new CastInfo() { name = castName, expression = value});
        }

        internal void PositionCast(MacroCallInfo info, string name, Anchoring anchoring, uint offset)
        {
            castController.PositionCast(info.process, name, anchoring, (int)offset);
        }

        internal void MoveCast(MacroCallInfo info, string name, int offset)
        {
            castController.PositionCast(info.process, name, offset);
        }

        internal void SetCastMotion(CastMotionType castMotionType)
        {
            castController.SetCastMotion(castMotionType);
        }

        internal void SetCastMotionDuration(float motionDuration)
        {
            castController.SetCastMotionDuration(motionDuration);
        }

        internal void HaveCastEnterFrom(EnterSide side)
        {
            castController.HaveCastEnterFrom(side);
        }

        [ExecuteInEditMode]
        public void SetCastController(CastController castControllerComponent)
        {
            castController = castControllerComponent;
        }

        [ExecuteInEditMode]
        public void SetSceneController(SceneController sceneControllerComponent)
        {
            sceneController = sceneControllerComponent;
        }
    }
}
