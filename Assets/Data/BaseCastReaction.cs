using System.Collections;
using UnityEngine;
using XVNML2U.Data;

namespace XVNML2U.Mono
{
    public abstract class BaseCastReaction : ICastReaction
    {
        private ICastReaction Instance => this;

        protected string Name => GetType().Name.TakeOff("CastReaction");

        public virtual float Duration => Instance.Duration;

        protected CastEntity Cast { get; private set; }

        protected BaseCastReaction()
        {
            ReactionRegistry.Register(this);
        }

        internal void DoReaction(CastEntity target)
        {
            Cast = target;
            CoroutineHandler.Execute(Instance.ReactionCycle());
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
