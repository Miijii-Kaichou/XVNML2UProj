using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XVNML2U.Mono
{
    public sealed class ResponseControl : MonoBehaviour
    {
        [SerializeField]
        private Button _button;


        private Color _originalColor;
        private TextMeshProUGUI _buttonText;
        private int _index = -1;

        public Action onClick;

        private void OnEnable()
        {
            _button ??= GetComponent<Button>();
            if (_button == null) return;
            _button.interactable = true;
            _buttonText = _button.GetComponentInChildren<TextMeshProUGUI>();
            _originalColor = _button.targetGraphic.color;
            _button.onClick.AddListener(OnClickEvent);
        }

        internal void AssignIndex(int index)
        {
            _index = index;
        }

        internal void Clear()
        {
            _button.interactable = false;
            _buttonText.text = string.Empty;
            _index = -1;
            _button.onClick.RemoveListener(OnClickEvent);
        }

        internal void SetText(string responseString)
        {
            _buttonText.text = responseString;
        }

        private void OnClickEvent()
        {
            onClick?.Invoke();
            Clear();
        }

        [ExecuteInEditMode]
        public void SetButton(Button buttonComponent)
        {
            throw new NotImplementedException();
        }
    }
}
