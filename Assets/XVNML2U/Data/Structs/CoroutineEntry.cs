using System.Collections;

namespace XVNML2U
{
    public struct CoroutineEntry
    {
        public int EntryID { get; }
        public IEnumerator Coroutine { get; }

        public CoroutineEntry(IEnumerator coroutine, int entryID)
        {
            Coroutine = coroutine;
            EntryID = entryID;
        }
    }
}