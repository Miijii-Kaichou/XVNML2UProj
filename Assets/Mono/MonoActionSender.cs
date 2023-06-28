#nullable enable

using System;
using UnityEngine;
using XVNML2U.Data;

namespace XVNML2U.Mono
{
    /// <summary>
    /// MonoBehaviour variant of ActionSender
    /// </summary>
    public class MonoActionSender : MonoBehaviour, ISendAction
    {
        public void SendNewAction(Func<WCResult> function)
        {
            XVNMLActionScheduler.SendNewAction(function);
        }
    }
}
