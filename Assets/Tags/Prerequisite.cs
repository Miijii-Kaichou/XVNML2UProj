using XVNML.Core.Tags;

namespace XVNML2U.Tags
{
    [AssociateWithTag("prerequisite", typeof(QuestPrerequisites), TagOccurance.Multiple, true)]
    public sealed class Prerequisite : TagBase
    {
        protected override string[] AllowedParameters => new[]
        {
            "category",
            "questID"
        };

        public string category;
        public string quest;

        public override void OnResolve(string fileOrigin)
        {
            base.OnResolve(fileOrigin);
            category = GetParameterValue<string>(AllowedParameters[0]);
            quest = GetParameterValue<string>(AllowedParameters[1]);
        }
    }
}
