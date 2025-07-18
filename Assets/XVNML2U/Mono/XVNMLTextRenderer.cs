using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace XVNML2U.Mono
{
    public sealed class XVNMLTextRenderer : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _target;

        [Header("Unity Events")]
        public UnityEvent _onTextChange;

        internal string Text
        {
            get
            {
                return _target.text;
            }
            set
            {
                var previousText = _target.text;
                _target.text = value;
                if (previousText.Equals(value)) return;
                _onTextChange?.Invoke();
            }
        }

        internal bool IsTextOverflowing
        {
            get
            {
                return _target.isTextOverflowing;
            }
        }

        internal int PageToDisplay
        {
            get
            {
                return _target.pageToDisplay;
            }
            set
            {
                _target.pageToDisplay = value;
            }
        }

        [ExecuteInEditMode]
        public void SetTarget(TextMeshProUGUI tmpText)
        {
            _target = tmpText;
        }

        internal void Refresh()
        {
            _target.ForceMeshUpdate();
        }

        internal TextMeshProUGUI Root
        {
            get
            {
                return _target;
            }
        }
    }
}
