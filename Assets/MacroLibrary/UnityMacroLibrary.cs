using UnityEngine;
using XVNML.Utility.Macros;

namespace XVNML2U
{
    [MacroLibrary(typeof(UnityMacroLibrary))]
    public sealed class UnityMacroLibrary
    {
        [Macro("debug")]
        private static void DebugLogMacro(MacroCallInfo info, string message)
        {
            Debug.Log(message);
        }
    }
}
