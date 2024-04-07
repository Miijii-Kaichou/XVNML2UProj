#nullable enable

using System;
using XVNML2U.Data;
using XVNML2U.Mono;

namespace XVNML2U
{
    public class ActionSender<Object> : ISendAction where Object : class, new()
    {
        protected static Object I => new();
        
        public void SendNewAction(Func<WCResult> function)
        {
            XVNMLActionScheduler.SendNewAction(function);
        }
    }
}
