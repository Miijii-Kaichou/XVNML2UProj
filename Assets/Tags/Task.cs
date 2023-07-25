using XVNML.Core.Tags;

namespace XVNML2U.Tags
{
    [AssociateWithTag("task", typeof(TaskList), TagOccurance.Multiple, true)]
    public sealed class Task : TagBase
    {
        protected override string[] AllowedParameters => new[]
        {
            "title"
        };

        public string title;
        public override void OnResolve(string fileOrigin)
        {
            base.OnResolve(fileOrigin);
            title = GetParameterValue<string>(AllowedParameters[0]);
        }
    }
}
