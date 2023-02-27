using System;
using XVNML.Core.Dialogue;

namespace XVNML2U.Data
{
    public struct WCResult
    {
        enum ResultState
        {
            Unknown,
            Ok,
            Error
        }
        public string Message { get; private set; }

        ResultState state;
        WCResult(ReadOnlySpan<char> msg, ResultState sta) { Message = msg.ToString(); state = sta; }
        public static WCResult Ok() => new WCResult(string.Empty, ResultState.Ok);
        public static WCResult Ok(ReadOnlySpan<char> msg) => new WCResult(msg, ResultState.Ok);
        public static WCResult Error() => new WCResult(string.Empty, ResultState.Error);
        public static WCResult Error(ReadOnlySpan<char> msg) => new WCResult(msg, ResultState.Error);
        public static WCResult Unknown() => new WCResult(string.Empty, ResultState.Unknown);
        public static WCResult Unknown(ReadOnlySpan<char> msg) => new WCResult(msg, ResultState.Unknown);

        public override bool Equals(object obj)
        {
            return obj is WCResult result &&
                   Message == result.Message &&
                   state == result.state;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Message, state);
        }

        public static bool operator ==(WCResult a, WCResult b)
        {
            return a.state.Equals(b.state);
        }

        public static bool operator !=(WCResult a, WCResult b)
        {
            return a.state.Equals(b.state) == false;
        }
    }
}