#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XVNML2U.Mono
{
    public static class ReactionRegistry
    {
        private static readonly SortedDictionary<string, BaseCastReaction> CastReactions = new();

        internal static void BeginRegistrationProcess()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(BaseCastReaction)))
                    .DoForEvery(t => Activator.CreateInstance(t));
        }

        internal static void Register(BaseCastReaction reaction)
        {
            var reactionName = reaction.GetName();
            CastReactions.Add(reactionName, reaction);
        }

        internal static void DoReaction(string reactionName, string castName)
        {
            CastEntity target = CastController.Use(castName);
            CastReactions[reactionName].DoReaction(target);
        }
    }
}
