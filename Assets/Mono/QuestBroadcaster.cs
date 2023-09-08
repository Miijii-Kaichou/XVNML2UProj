using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XVNML2U.Data;

namespace XVNML2U.Mono
{
    public class QuestBroadcaster : MonoBehaviour
    {
        public enum BroadcastPayloadType
        {
            NewQuest,
            NewTask,
            QuestComplete
        }

        
        [SerializeField] QuestAnimations questLogAnimation;
        [SerializeField] QuestTaskAnimations questTaskAnimation;
        [SerializeField] QuestAnimations questCompleteAnimation;

        private Queue<(QuestLog log, Action<QuestLog> callback)> _broadcastEventQueue = new();
        private bool _isActive;
        private bool _isBusy = false;

        // Start is called before the first frame update
        void Start()
        {
            _isActive = true;
            XVNMLQuestSystem.OnQuestInitilize.AddListener(BroadcastNewQuest);
            StartCoroutine(QuestBroadcastListener());
        }

        public void SendNewQuestBroadcast(QuestLog log)
        {
            SendNewBroadcastPayload(log, BroadcastPayloadType.NewQuest);
        }

        public void SendNewTaskBroadcast(QuestLog log)
        {
            SendNewBroadcastPayload(log, BroadcastPayloadType.NewTask);
        }

        public void SendQuestCompleteBroadcast(QuestLog log)
        {
            SendNewBroadcastPayload(log, BroadcastPayloadType.QuestComplete);
        }

        private void SendNewBroadcastPayload(QuestLog log, BroadcastPayloadType type)
        {
            switch (type)
            {
                case BroadcastPayloadType.NewQuest:
                    _broadcastEventQueue.Enqueue((log, BroadcastNewQuest));
                    return;
                case BroadcastPayloadType.NewTask:
                    _broadcastEventQueue.Enqueue((log, BroadcastNewTask));
                    return;
                 case BroadcastPayloadType.QuestComplete:
                    _broadcastEventQueue.Enqueue((log, BroadcastQuestComplete));
                    return;
                default:
                    break;
            }
        }


        IEnumerator QuestBroadcastListener()
        {
            while(_isActive)
            {
                if (_broadcastEventQueue.Count == 0)
                {
                    yield return null;
                    continue;
                }

                for (int i = 0; i < _broadcastEventQueue.Count; i++)
                {
                    yield return new WaitUntil(() => _isBusy == false);
                    _isBusy = true;
                    var action = _broadcastEventQueue.Dequeue();
                    var log = action.log;
                    action.callback.Invoke(log);
                }

                yield return null;
            }
        }

        private void BroadcastNewQuest(QuestLog log)
        {
            questLogAnimation.DoAnimation(log);
        }


        private void BroadcastNewTask(QuestLog log)
        {
            questTaskAnimation.DoAnimation(log);
        }

        private void BroadcastQuestComplete(QuestLog log)
        {
            questCompleteAnimation.DoAnimation(log);
        }
       
        public void SetBroadcasterAsNotBusy() => _isBusy = false;
    }
}
