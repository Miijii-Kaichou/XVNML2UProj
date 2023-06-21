using System.Collections;
using System.Reflection;
using UnityEngine;
using XVNML2U.Data;

namespace XVNML2U.Mono.CastReactions
{
    [SerializeField]
    public abstract class BaseCastReaction : MonoBehaviour, ICastReaction
    {
        private ICastReaction Instance => this;

        protected string Name => GetType().Name.TakeOff("CastReaction");

        public virtual float Duration => Instance.Duration;

        protected CastEntity Cast { get; private set; }

        internal void DoReaction(CastEntity target)
        {
            Cast = target;
            StartCoroutine(Instance.ReactionCycle());
        }

        public string GetName() => Name;

        IEnumerator ICastReaction.ReactionCycle()
        {
            var time = 0f;
            var endTime = Duration;

            OnReactionStart();

            while (time < endTime)
            {
                time += Time.deltaTime;

                OnReaction();

                yield return null;
            }

            OnReactionEnd();
        }

        public virtual void OnReaction() { }
        public virtual void OnReactionStart() { }
        public virtual void OnReactionEnd() { }
    }
}
