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
            NewTask
        }

        
        [SerializeField] QuestAnimations questLogAnimation;
        [SerializeField] QuestTaskAnimations questTaskAnimation;

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

        private void SendNewBroadcastPayload(QuestLog log, BroadcastPayloadType type)
        {
            switch (type)
            {
                case BroadcastPayloadType.NewQuest:
                    _broadcastEventQueue.Enqueue((log, BroadcastNewQuest));
                    return;
                case BroadcastPayloadType.NewTask:
                    _broadcastEventQueue.Enqueue((log, BroadcastNewTask));
                    break;
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

                yield return new WaitUntil(() => _isBusy == false);

                _isBusy = true;
                var action = _broadcastEventQueue.Dequeue();
                var log = action.log;
                action.callback.Invoke(log);

                yield return null;
            }
        }

        void BroadcastNewQuest(QuestLog log)
        {
            questLogAnimation.DoAnimation(log);
        }


        void BroadcastNewTask(QuestLog log)
        {
            questTaskAnimation.DoAnimation(log);
        }

        public void SetBroadcasterAsNotBusy() => _isBusy = false;
    }
}
