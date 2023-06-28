#nullable enable

using System;

namespace XVNML2U.Data
{
    internal interface ISendAction
    {
        void SendNewAction(Func<WCResult> function);
    }
}
