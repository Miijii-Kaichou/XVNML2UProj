using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XVNML2U.Mono
{
    public sealed class ConfirmMarker : MonoBehaviour
    {
        [SerializeField]
        private Sprite confirmMarkerGraphic;

        [Header("Unity Events")]
        [SerializeField] private UnityEvent _onShow;
        [SerializeField] private UnityEvent _onHide;


        private Image confirmMarkerRenderer;

        private void Awake()
        {
           confirmMarkerRenderer ??= GetComponent<Image>();
            if (confirmMarkerGraphic == null) return;
            confirmMarkerRenderer.sprite = confirmMarkerGraphic;
        }

        internal void OnPending() => _onShow?.Invoke();

        internal void OnAccept() => _onHide?.Invoke();

        [ExecuteInEditMode]
        public void SetGraphic(Image graphic)
        {
            confirmMarkerRenderer = graphic;
        }
    }
}
