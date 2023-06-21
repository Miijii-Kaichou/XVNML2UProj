using System;
using System.Collections;
using UnityEngine;
using XVNML2U.Mono.CastReactions;

namespace XVNML2U.Data
{
    interface ICastReaction
    {
        public virtual float Duration
        {
            get
            {
                return 1f;
            }
        }

        void OnReaction();
        void OnReactionStart();
        void OnReactionEnd();

        internal IEnumerator ReactionCycle();
    }
}
