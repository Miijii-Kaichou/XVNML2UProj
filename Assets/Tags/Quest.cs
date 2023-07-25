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

        private QuestPrerequisites _prerequisites;
        public QuestPrerequisites Prerequisites => _prerequisites;

        private TaskList _objectives;
        public TaskList Objectives => _objectives;

        protected override string[] AllowedParameters => new[]
        {
            "src",
            "description"
        };

        public override void OnResolve(string fileOrigin)
        {
            base.OnResolve(fileOrigin);

            var source = GetParameterValue<string>(AllowedParameters[0]);

            if (source != null)
            {
                Debug.Log("Hwoow");
                if (source == "nil") return;
                XVNMLObj.Create(QuestDirectory + source, OnSourceCreation);
                return;
            }

            _prerequisites = GetElement<QuestPrerequisites>();
            _objectives = GetElement<TaskList>();
            _description = GetParameterValue<string>(AllowedParameters[1]) ?? value.ToString();
        }

        private void OnSourceCreation(XVNMLObj obj)
        {
            var root = obj.Root;
            _prerequisites = root.GetElement<QuestPrerequisites>();
            _objectives = root.GetElement<TaskList>();
            _description = GetParameterValue<string>(AllowedParameters[1]) ?? root.GetElement<QuestInfo>().content;
        }
    }

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
