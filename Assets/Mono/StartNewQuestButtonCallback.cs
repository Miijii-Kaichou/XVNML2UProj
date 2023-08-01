using UnityEngine;
using XVNML2U.Mono;

namespace XVNML2U.Mono
{
    public class StartNewQuestButtonCallback : MonoBehaviour
    {
        private readonly string[] QuestStuff = new[]
        {
            "QuestLogTest0",
            "QuestLogTest1"
        };

        public void StartNewQuest(XVNMLDialogueControl control)
        {
            if (control.IsActive) return;
            control.Play(QuestStuff[0]);
        }

        public void StartNewSideQuest(XVNMLDialogueControl control)
        {
            if (control.IsActive) return;
            control.Play(QuestStuff[1]);
        }
    }
}
