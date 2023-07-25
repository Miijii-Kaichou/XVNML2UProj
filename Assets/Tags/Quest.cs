using UnityEngine;
using XVNML.Core.Tags;
using XVNML.Utilities;
using XVNML.Utilities.Tags;

namespace XVNML2U.Tags
{
    [AssociateWithTag("quest", new[] { typeof(QuestDefinitions), typeof(QuestCategory) }, TagOccurance.Multiple, true)]
    public sealed class Quest : TagBase
    {
        private static string QuestDirectory = @"/Quests/";

        private string _description;
        public string Description => _description;

        private string _title;
        public string Title => _title;

        private QuestPrerequisites _prerequisites;
        public QuestPrerequisites Prerequisites => _prerequisites;

        private TaskList _objectives;
        public TaskList Objectives => _objectives;

        protected override string[] AllowedParameters => new[]
        {
            "src",
            "description",
            "title"
        };

        public override void OnResolve(string fileOrigin)
        {
            base.OnResolve(fileOrigin);

            var source = GetParameterValue<string>(AllowedParameters[0]);

            if (source != null)
            {
                if (source == "nil") return;
                XVNMLObj.Create(fileOrigin + QuestDirectory + source, OnSourceCreation);
                return;
            }

            _prerequisites = GetElement<QuestPrerequisites>();
            _objectives = GetElement<TaskList>();
            _description = GetParameterValue<string>(AllowedParameters[1]) ?? value.ToString();
            _title = GetParameterValue<string>(AllowedParameters[2]);
        }

        private void OnSourceCreation(XVNMLObj obj)
        {
            var root = obj.Root;
            _prerequisites = root.GetElement<QuestPrerequisites>();
            _objectives = root.GetElement<TaskList>();
            _description = GetParameterValue<string>(AllowedParameters[1]) ?? root.GetElement<QuestInfo>().content;
            _title = GetParameterValue<string>(AllowedParameters[2]);
        }
    }
}
