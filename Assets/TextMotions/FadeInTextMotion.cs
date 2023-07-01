using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace XVNML2U
{

    public class FadeInTextMotion : BaseTextMotion
    {
        private TMP_CharacterInfo lastCharacterInfo;

        public override float Duration => 3f;
        private float time = 0f;

        public override void OnMotionStart()
        {
            TMP_Text.ForceMeshUpdate();
            var textInfo = TMP_Text.textInfo;
            var lastCharacterIndex = textInfo.characterCount;
            lastCharacterInfo = textInfo.characterInfo[lastCharacterIndex];
            lastCharacterInfo.scale = 2f;
            TMP_Text.textInfo.characterInfo[lastCharacterIndex] = lastCharacterInfo;
            TMP_Text.UpdateVertexData();
        }
    }
}
