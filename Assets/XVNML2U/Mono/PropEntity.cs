using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XVNML2U.Mono
{
    public class PropEntity : MonoBehaviour
    {
        private const string NullImageResourcePath = "Images/NullImage";
        private Image propGraphic;

        public bool IsViewingGraphic => propGraphic.sprite != _nullImage;
        private Sprite _nullImage;

        public Image PropGraphic => propGraphic;

        private void Awake()
        {
            propGraphic ??= GetComponent<Image>();
            _nullImage = Resources.Load<Sprite>(NullImageResourcePath);
            SetGraphic(_nullImage, Color.white);
        }

        public void SetGraphic(Sprite graphic, Color rgba, bool preserveAspect = false, Image.Type type = Image.Type.Simple)
        {
            propGraphic.sprite = graphic;
            propGraphic.color = rgba;
            propGraphic.preserveAspect = preserveAspect;
            propGraphic.type = type;
        }

        public void Clear()
        {
            propGraphic.sprite = _nullImage;
        }
    }
}