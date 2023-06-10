using System;
using UnityEngine;
using XVNML.Core.Dialogue.Structs;
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
    }
}
