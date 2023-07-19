#nullable enable

using System;
using UnityEngine;

namespace XVNML2U.Data
{
    internal interface ISendAction
    {
        void SendNewAction(Func<WCResult> function);
    }
}
