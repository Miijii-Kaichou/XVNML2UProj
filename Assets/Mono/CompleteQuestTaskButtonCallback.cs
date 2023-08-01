using UnityEngine;

namespace XVNML2U.Mono
{
    public class CompleteQuestTaskButtonCallback : MonoBehaviour
    {
        public void CompleteMainQuestTask()
        {
            var mainQuest = XVNMLQuestSystem.QuestControl[("_main", "Amongst_the_Mysterious_Lights")];
            if (mainQuest.Complete) return;
            mainQuest.CompleteCurrentTask();
        }

        public void CompleteSideQuestTask()
        {
            var sideQuest = XVNMLQuestSystem.QuestControl[("_side", "Very_Strange_Stalactites")];
            if (sideQuest.Complete) return;
            sideQuest.CompleteCurrentTask();
        }
    }
}
