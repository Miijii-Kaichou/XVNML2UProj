using System;
using UnityEngine;
using XVNML.Core.Dialogue.Structs;
using XVNML.Utility.Macros;
using XVNML.XVNMLUtility.Tags;

namespace XVNML2U
{
    public class XVNMLStage : MonoBehaviour
    {
        [SerializeField]
        private SceneController sceneController;

        [SerializeField]
        private CastController castController;

        internal void ChangeScene(SceneInfo currentSceneInfo)
        {
            sceneController.ChangeScene(currentSceneInfo);
        }
        
        internal void ChangeExpression(CastInfo castInfo)
        {
            castController.ChangeExpression(castInfo);
        }

        internal void ChangeVoice(CastInfo castInfo)
        {
            castController.ChangeVoice(castInfo);
        }

        internal void InitializeCastController(Cast[] castMembers)
        {
            castController.Init(castMembers);
        }

        internal void InitializeSceneController(Scene[] scenes)
        {
            sceneController.Init(scenes);
        }

        internal void PositionCast(MacroCallInfo info, string name, Anchoring anchoring, uint offset)
        {
            castController.PositionCast(info.process, name, anchoring, (int)offset);
        }

        internal void SetCastMotion(CastMotionType castMotionType)
        {
            castController.SetCastMotion(castMotionType);
        }

        internal void SetCastMotionDuration(float motionDuration)
        {
            castController.SetCastMotionDuration(motionDuration);
        }
    }
}
