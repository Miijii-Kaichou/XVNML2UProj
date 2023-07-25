using XVNML.Core.Tags;
using XVNML.Utilities;
using XVNML.Utilities.Tags;

namespace XVNML2U.Tags
{
    [AssociateWithTag("questCategory", new[] { typeof(Source), typeof(QuestDefinitions) }, TagOccurance.Multiple, true)]
    public sealed class QuestCategory : UserDefined
    {
        private static string QuestDirectory = @"/Quests/Categories/";

        protected override string[] AllowedParameters => new[]
        {
            "src"
        };

        protected override string[] AllowedFlags => new[]
        {
            "isDefault"
        };

        private Quest[] _quests;
        public Quest[] Quests => _quests;

        private bool _isDefault;
        public bool IsDefault => _isDefault;

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
            _isDefault = HasFlag(AllowedFlags[0]);
            _quests = Collect<Quest>();
        }

        private void OnSourceCreation(XVNMLObj obj)
        {
            QuestCategory category = obj.Root.SearchElement<QuestCategory>(TagName);
            if (category == null) return;
            _isDefault = HasFlag(AllowedFlags[0]);
            _quests = category.Collect<Quest>();
        }
    }
}
