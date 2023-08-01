using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using XVNML2U.Data;

namespace XVNML2U.Mono
{
    interface IQuestLogAnimation
    {
        UnityEvent<QuestLog> OnAnimationStart { get; }
        UnityEvent<QuestLog> OnAnimationEnd { get; }
        
        void DoAnimation(QuestLog questLog);
    }

    public class QuestAnimations : MonoBehaviour, IQuestLogAnimation
    {
        [SerializeField]
        QuestLabel _newQuestLabel;

        [Header("Animation Callbacks")]
        [SerializeField] UnityEvent<QuestLog> _onAnimationStart;
        [SerializeField] UnityEvent<QuestLog> _onAnimationEnd;

        private float _localMoveTime = 1f;
        private float _canvasFadeDuration = 1f;

        public UnityEvent<QuestLog> OnAnimationStart => _onAnimationStart;
        public UnityEvent<QuestLog> OnAnimationEnd => _onAnimationEnd;

        public void DoAnimation(QuestLog questLog)
        {
            OnAnimationStart.Invoke(questLog);
            DoNewQuestAnimation(questLog);
        }

        private void DoNewQuestAnimation(QuestLog questLog)
        {
            _newQuestLabel.LabelText = questLog.questName;

            var initialY = _newQuestLabel.transform.localPosition.y;

            _newQuestLabel.transform.DOLocalMoveY(initialY+20, _localMoveTime);
            _newQuestLabel.CanvasGroup.DOFade(1, _canvasFadeDuration);

            _newQuestLabel.transform.DOLocalMoveY(initialY+(-20), _localMoveTime).SetDelay(5);

            Tween lastTween = _newQuestLabel.CanvasGroup.DOFade(0, _canvasFadeDuration).SetDelay(5);

            lastTween.onComplete = delegate() { InvokeOnAnimationEnd(questLog); };
        }

        private void InvokeOnAnimationEnd(QuestLog questLog)
        {
            OnAnimationEnd.Invoke(questLog);
        }
    }
}
