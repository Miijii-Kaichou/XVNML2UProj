using System.Collections;

namespace XVNML2U.Data
{
    public interface ICastReaction
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
