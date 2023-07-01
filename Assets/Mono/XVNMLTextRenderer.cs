using Codice.CM.SEIDInfo;
using System;
using System.Collections.Generic;
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

        private List<BaseTextMotion> textMotions = new();

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
                if (textMotions.Count != 0) textMotions?.DoForEvery(PlayTextMotion);
                _onTextChange?.Invoke();
            }
        }

        private object PlayTextMotion(BaseTextMotion motion)
        {
            motion.DoTextMotion();
            return motion;
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

        internal void AddNewMotion(BaseTextMotion? newMotion)
        {
            if (newMotion == null) return;

            newMotion.TMP_Text = _target;
            newMotion.DoTextMotion();

            textMotions.Add(newMotion);
        }

        internal void ClearMotions()
        {
            textMotions.Clear();
        }
    }
}
