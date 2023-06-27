#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XVNML2U.Data;

namespace XVNML2U.Mono
{
    internal interface ISendAction
    {
        void SendNewAction(Func<WCResult> function);
    }

    /// <summary>
    /// MonoBehaviour variant of ActionSender
    /// </summary>
    public class MonoActionSender : MonoBehaviour, ISendAction
    {
        public void SendNewAction(Func<WCResult> function)
        {
            XVNMLActionScheduler.ActionQueue?.Enqueue(function);
        }
    }

    public class ActionSender : ISendAction
    {
        public void SendNewAction(Func<WCResult> function)
        {
            XVNMLActionScheduler.ActionQueue?.Enqueue(function);
        }
    }

    public sealed class XVNMLActionScheduler : Singleton<XVNMLActionScheduler>
    {
        public static Queue<Func<WCResult>>? ActionQueue { get; private set; }

        private void OnEnable()
        {
            ActionQueue = new Queue<Func<WCResult>>();
            StartCoroutine(QueueCycle());
        }

        private IEnumerator QueueCycle()
        {
            bool errorEncountered = false;

            while (true)
            {
                errorEncountered = ProcessActions(errorEncountered);

                if (errorEncountered) yield break;
                yield return null;
            }
        }

        private bool ProcessActions(bool errorEncountered)
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
    }
}
