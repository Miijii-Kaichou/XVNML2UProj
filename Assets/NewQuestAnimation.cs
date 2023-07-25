using UnityEngine;
using DG.Tweening;

namespace XVNML2U
{
    public class NewQuestAnimation : MonoBehaviour
    {
        [SerializeField]
        QuestLabel _newQuestLabel;

        private float _localMoveTime = 1f;
        private float _canvasFadeDuration = 1f;

        public void Start()
        {
            XVNMLQuestSystem.OnQuestInitilize.AddListener(DoNewQuestAnimation);
        }

        private void DoNewQuestAnimation(QuestLog questLog)
        {
            SetNewQuestTitle(questLog.questName);
            Play();
        }

        public void SetNewQuestTitle(string title)
        {
            _newQuestLabel.LabelText = title;
        }

        public void Play()
        {
            _newQuestLabel.transform.DOLocalMoveY(0, _localMoveTime);
            _newQuestLabel.CanvasGroup.DOFade(1, _canvasFadeDuration);

            _newQuestLabel.transform.DOLocalMoveY(-10, _localMoveTime).SetDelay(5);
            _newQuestLabel.CanvasGroup.DOFade(0, _canvasFadeDuration).SetDelay(5);
        }
    }
}
