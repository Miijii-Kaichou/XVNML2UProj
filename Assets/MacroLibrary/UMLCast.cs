#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove used parameters

using XVNML.Utilities.Macros;
using XVNML2U.Data;
using XVNML2U.Mono;

namespace XVNML2U
{
    [MacroLibrary(typeof(UMLCast))]
    internal class UMLCast : ActionSender
    {
        private static UMLCast Instance => new();

        [Macro("cast_enters_from")]
        [Macro("cstef")]
        private static void CastEntersFromMacro(MacroCallInfo info, string side)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.HaveCastEnterFrom(side.Parse<EnterSide>());
        }

        [Macro("cue_cast")]
        [Macro("cast")]
        private static void CueCastMacro(MacroCallInfo info, string anchoring)
        {
            var name = info.process.CurrentCastInfo!.Value.name!;
            CueCastMacro(info, name, anchoring, 0);
        }

        [Macro("cue_cast")]
        [Macro("cast")]
        private static void CueCastMacro(MacroCallInfo info, string anchoring, uint offset)
        {
            var name = info.process.CurrentCastInfo!.Value.name!;
            CueCastMacro(info, name, anchoring, offset);
        }

        [Macro("cue_cast")]
        [Macro("cast")]
        private static void CueCastMacro(MacroCallInfo info, string name, string anchoring)
        {
            CueCastMacro(info, name, anchoring, 0);
        }

        [Macro("cue_cast")]
        [Macro("cast")]
        private static void CueCastMacro(MacroCallInfo info, string name, string anchoring, uint offset)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.PositionCast(info, name, anchoring.Parse<Anchoring>(), offset);
        }

        [Macro("move_cast")]
        [Macro("mcst")]
        private static void MoveCastMacro(MacroCallInfo info, int units)
        {
            var name = info.process.CurrentCastInfo?.name!;
            MoveCastMacro(info, name, units);
        }

        [Macro("move_cast")]
        [Macro("mcst")]
        private static void MoveCastMacro(MacroCallInfo info, string name, int units)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.MoveCast(info, name, units);
        }

        [Macro("react")]
        private static void ReactMacro(MacroCallInfo info, string reactionName)
        {
            var castName = info.process.CurrentCastInfo?.name!;
            ReactMacro(info, castName, reactionName);
        }

        [Macro("react")]
        private static void ReactMacro(MacroCallInfo info, string castName, string reactionName)
        {
            Instance.SendNewAction(() =>
            {
                ReactionRegistry.DoReaction(reactionName, castName);
                return WCResult.Ok();
            });
        }

        [Macro("set_cast_motion_duration")]
        [Macro("scstmd")]
        private static void SetCastMotionDurationMacro(MacroCallInfo info, float motionDuration)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.SetCastMotionDuration(motionDuration);
        }

        [Macro("set_cast_motion")]
        [Macro("scstm")]
        private static void SetCastMotionMacro(MacroCallInfo info, string motionType)
        {
            DialogueProcessAllocator.ProcessReference[info.process.ID].Stage!.SetCastMotion(motionType.Parse<CastMotionType>());
        }
    }
}

#pragma warning restore IDE0060 // Remove used parameters
#pragma warning restore IDE0051 // Remove unused private members