#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XVNML2U.Data;

namespace XVNML2U.Mono
{

    public sealed class XVNMLActionScheduler : Singleton<XVNMLActionScheduler>
    {
        public static Queue<Func<WCResult>>? ActionQueue { get; private set; }

        private static bool IsInitialzed = false;

        internal static void Init()
        {
            if (IsInitialzed) return;
            ActionQueue = new Queue<Func<WCResult>>();
            Instance.StartCoroutine(QueueCycle());
            IsInitialzed = true;
        }

        private static IEnumerator QueueCycle()
        {
            bool errorEncountered = false;

            while (true)
            {
                errorEncountered = ProcessActions(errorEncountered);

                if (errorEncountered) yield break;
                yield return null;
            }
        }

        private static bool ProcessActions(bool errorEncountered)
        {
            if (ActionQueue?.Count > 0)
            {
                for (int i = 0; i < ActionQueue?.Count; i++)
                {
                    var action = new Func<WCResult>(() => WCResult.Unknown());
                    var result = WCResult.Unknown();

                    ActionQueue?.TryDequeue(out action);

                    if (action == null) continue;
                    if ((result = action.Invoke()) == WCResult.Unknown()) ActionQueue?.Enqueue(action);
                    if (result != WCResult.Error() && result.Message != string.Empty)
                    {
                        Debug.Log(result.Message);
                    }

                    if (result == WCResult.Error())
                    {
                        Debug.LogError(result.Message);
                        errorEncountered = true;
                        break;
                    }
                }
            }

            return errorEncountered;
        }

        public static void SendNewAction(Func<WCResult> function)
        {
            if (ActionQueue?.Count == 0) return; 
            ActionQueue?.Enqueue(function);
        }
    }
}
