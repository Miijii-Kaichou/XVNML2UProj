using XVNML.Core.Tags;

namespace XVNML2U.Tags
{
    [AssociateWithTag("taskList", typeof(Quest), TagOccurance.PragmaLocalOnce, true)]
    public sealed class TaskList : TagBase
    {
        private Task[] _items;
        public Task[] Items => _items;

        public override void OnResolve(string fileOrigin)
        {
            base.OnResolve(fileOrigin);
            _items = Collect<Task>();
        }
    }
}
