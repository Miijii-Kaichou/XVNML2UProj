using XVNML.Core.Tags;

namespace XVNML2U.Tags
{
    [AssociateWithTag("questPrerequisites", typeof(Quest), TagOccurance.PragmaLocalOnce, true)]
    public sealed class QuestPrerequisites : TagBase
    {
        private Prerequisite[] _items;
        public Prerequisite[] Items => _items;
        public override void OnResolve(string fileOrigin)
        {
            base.OnResolve(fileOrigin);
            _items = Collect<Prerequisite>();
        }
    }
}
