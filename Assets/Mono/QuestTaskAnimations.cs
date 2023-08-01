using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using XVNML2U.Data;


namespace XVNML2U.Mono
{
    public class QuestTaskAnimations : MonoBehaviour, IQuestLogAnimation
    {
        [SerializeField]
        QuestLabel _questTaskLabel;

        private float _localMoveTime = 1f;
        private float _canvasFadeDuration = 1f;

        [Header("Animation Callbacks")]
        [SerializeField] UnityEvent<QuestLog> _onAnimationStart;
        [SerializeField] UnityEvent<QuestLog> _onAnimationEnd;

        public UnityEvent<QuestLog> OnAnimationStart => _onAnimationStart;

        public UnityEvent<QuestLog> OnAnimationEnd => _onAnimationEnd;

        public void DoAnimation(QuestLog questLog)
        {
            OnAnimationStart?.Invoke(questLog);
            DoNewTaskAnimation(questLog);
        }

        private void DoNewTaskAnimation(QuestLog questLog)
        {
            _questTaskLabel.LabelText = questLog.TaskTitle;

            var initialY = _questTaskLabel.transform.localPosition.y;

            _questTaskLabel.transform.DOLocalMoveY(initialY+(-64), _localMoveTime);
            _questTaskLabel.CanvasGroup.DOFade(1, _canvasFadeDuration);

            _questTaskLabel.transform.DOLocalMoveY(initialY+0, _localMoveTime).SetDelay(5);

            Tween lastTween = _questTaskLabel.CanvasGroup.DOFade(0, _canvasFadeDuration).SetDelay(5);

            lastTween.onComplete = delegate() { InvokeOnAnimationEnd(questLog); } ;
        }

        private void InvokeOnAnimationEnd(QuestLog questLog)
        {
            OnAnimationEnd?.Invoke(questLog);
        }
    }
}
