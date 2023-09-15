#nullable enable

using System;
using XVNML2U.Data;
using XVNML2U.Mono;

namespace XVNML2U
{
    public class ActionSender : ISendAction
    {
        public void SendNewAction(Func<WCResult> function)
        {
            XVNMLActionScheduler.SendNewAction(function);
        }
    }
}
