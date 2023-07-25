using XVNML.Core.Tags;

namespace XVNML2U.Tags
{
    [AssociateWithTag("questInfo", typeof(Quest), TagOccurance.PragmaLocalOnce, true)]
    public sealed class QuestInfo : TagBase
    {
        protected override string[] AllowedParameters => new[]
        {
            "content"
        };

        public string content;

        public override void OnResolve(string fileOrigin)
        {
            base.OnResolve(fileOrigin);
            content = GetParameterValue<string>(AllowedParameters[0]) ?? value.ToString();
        }
    }
}
