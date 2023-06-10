using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XVNML2U
{
    public sealed class ConfirmMarker : MonoBehaviour
    {
        [SerializeField]
        private Sprite confirmMarkerGraphic;

        private Image confirmMarkerRenderer;

        private void Awake()
        {
           confirmMarkerRenderer ??= GetComponent<Image>();
            if (confirmMarkerGraphic == null) return;
            confirmMarkerRenderer.sprite = confirmMarkerGraphic;
        }

        private void OnEnable()
        {
               
        }

        private void OnDisable()
        {
            
        }
    }
}
