using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XVNML2U.Mono
{
    public sealed class DialogueUI : MonoBehaviour
    {
        [SerializeField] bool _useMasking = false;
        [SerializeField] Vector4 _maskingBounds = Vector4.zero;
        [SerializeField] RectMask2D _dialogueMask;
        [SerializeField] TextMeshProUGUI _dialogueText;

        private void OnValidate()
        {
            if (_useMasking == false) return;
            _dialogueText.margin = -_maskingBounds;
            _dialogueMask.padding = _maskingBounds;
        }
    }
}
