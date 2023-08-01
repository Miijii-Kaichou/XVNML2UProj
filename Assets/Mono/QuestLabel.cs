using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XVNML2U.Mono
{
    public sealed class QuestLabel : MonoBehaviour
    {
        [SerializeField]
        private Image _labelGraphic;

        [SerializeField]
        private TextMeshProUGUI _labelTextComponent;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        public string LabelText
        {
            get
            {
                return _labelTextComponent.text;
            }
            set
            {
                _labelTextComponent.text = value;
            }
        }

        public float labelAlpha
        {
            get
            {
                return _canvasGroup.alpha;
            }
            set
            {
                _canvasGroup.alpha = value;
            }
        }

        public CanvasGroup CanvasGroup => _canvasGroup;

        private void Awake()
        {
            _canvasGroup ??= GetComponent<CanvasGroup>();
            _labelGraphic ??= GetComponent<Image>();
        }
    }
}
