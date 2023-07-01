#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace XVNML2U.Mono
{
    public static class TextMotionRegistry
    {
        private static readonly SortedDictionary<string, BaseTextMotion> TextMotions = new();

        internal static void BeginRegistrationProcess()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(BaseTextMotion)))
                    .DoForEvery(t => Activator.CreateInstance(t));
        }

        internal static void Register(BaseTextMotion motion)
        {
            var reactionName = motion.GetName();
            TextMotions.Add(reactionName, motion);
            Debug.Log($"Registered {reactionName} with {motion}...");
        }

        internal static BaseTextMotion? GetMotion(string motionName)
        {
            if (TextMotions.ContainsKey(motionName) == false) return null;
            var motion = TextMotions[motionName];
            return motion;
        }
    }
}
