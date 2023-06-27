using System.Collections.Generic;
using UnityEngine;
using XVNML2U.Mono;

namespace XVNML2U.Mono
{
    [DisallowMultipleComponent]
    public sealed class ReactionRegistry : Singleton<ReactionRegistry>
    {
        private SortedDictionary<string, BaseCastReaction> _castReactions = new SortedDictionary<string, BaseCastReaction>();

        internal void OnEnable()
        {
            BaseCastReaction[] reactionRegistryList = GetComponents<BaseCastReaction>();

            for(int i = 0; i < reactionRegistryList.Length; i++)
            {
                var reaction = reactionRegistryList[i];
                var reactionName = reaction.GetName();
                _castReactions.Add(reactionName, reaction);
            }
        }

        internal static void DoReaction(string reactionName, string castName)
        {
            CastEntity target = CastController.Use(castName);
            Instance._castReactions[reactionName].DoReaction(target);
        }
    }
}
