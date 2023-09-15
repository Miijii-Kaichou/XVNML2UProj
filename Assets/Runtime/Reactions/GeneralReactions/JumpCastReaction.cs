using DG.Tweening;
using UnityEngine;

namespace XVNML2U.Mono.CastReactions
{
    public sealed class JumpCastReaction : BaseCastReaction
    {
        public override float Duration => 0.5f;
        
        private float _jumpHeight = 25;
        private int _vibrato = 0;
        private float _elasticity = 0;

        public override void OnReactionStart()
        {
            Cast.transform.DOPunchPosition(new Vector2(0, _jumpHeight), Duration, _vibrato, _elasticity);
        }
    }
}
