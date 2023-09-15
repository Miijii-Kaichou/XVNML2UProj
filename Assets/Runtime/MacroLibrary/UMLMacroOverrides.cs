#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable UNT0008 // Unity objects hould not use null propagation.
#nullable enable

using XVNML.Utilities.Macros;
using XVNML2U.Data;
using XVNML2U.Mono;

namespace XVNML2U
{
    [MacroLibrary(typeof(UMLMacroOverrides))]
    internal class UMLMacroOverrides : ActionSender
    {
        private static UMLMacroOverrides Instance => new();

        [Macro("expression")]
        [Macro("portrait")]
        [Macro("exp")]
        [Macro("port")]
        private static void SetCastExpressionMacro(MacroCallInfo info, string castName, string value)
        {
            var stage = DialogueProcessAllocator.ProcessReference[info.process.ID].Stage;
            Instance.SendNewAction(() =>
            {
                stage?.ChangeExpression(castName, value);
                return WCResult.Ok();
            });
        }

        [Macro("expression")]
        [Macro("portrait")]
        [Macro("exp")]
        [Macro("port")]
        private static void SetCastExpressionMacro(MacroCallInfo info, string castName, int value)
        {
            SetCastExpressionMacro(info, castName, value.ToString());
        }

        [Macro("voice")]
        [Macro("vo")]
        private static void SetCastVoice(MacroCallInfo info, string castName, string value)
        {
            var processRef = DialogueProcessAllocator.ProcessReference[info.process.ID];
            var stage = processRef.Stage;
            processRef.SendNewAction(() =>
            {
                stage?.ChangeVoice(castName, value);
                return WCResult.Ok();
            });
        }

        [Macro("voice")]
        [Macro("vo")]
        internal static void SetCastVoice(MacroCallInfo info, string castName, int value)
        {
            SetCastVoice(info, castName, value.ToString());
        }
    }
}

#pragma warning restore UNT0008 // Unity objects hould not use null propagation.
#pragma warning restore IDE0051 // Remove unused private members