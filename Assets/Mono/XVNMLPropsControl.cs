#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring the field as nullable
#nullable enable

using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XVNML.Utilities.Tags;

namespace XVNML2U.Mono
{
    public class XVNMLPropsControl : Singleton<XVNMLPropsControl>
    {
        [SerializeField, Header("Transform")]
        private RectTransform _rectTransform;

        [SerializeField, Header("Configurations")]
        private bool _enablePooling;

        [SerializeField]
        private int _maxPropCount = 32;

        private readonly static SortedDictionary<string, Sprite> ImageMapping = new();
        private readonly static List<PropEntity> CachedProps = new();
        private static Vector3 SetScale = new(1, 1, 1);
        private static int ModuleWidth = 0;
        private static int ModuleHeight = 0;

        static int PoolIndex = 0;

        public static TransitionMode PropTransitionMode { get; private set; }
        public static float TransitionDuration { get; private set; }

        private static readonly Color Transparent = new(1, 1, 1, 0);

        internal static void Init(XVNMLModule module)
        {
            if (IsNull) return;

            if (Instance._enablePooling) SetPropEntities();

            Instance._rectTransform = Instance.GetComponent<RectTransform>();
            ModuleWidth = module.Root!.GetParameterValue<int>("screenWidth")!;
            ModuleHeight = module.Root!.GetParameterValue<int>("screenHeight")!;

            Instance._rectTransform.sizeDelta = new Vector2(ModuleWidth, ModuleHeight);

            ImageDefinitions imageDefinitions = module.Get<ImageDefinitions>()!;

            GenerateImageMapping(imageDefinitions!);
        }

        private static void SetPropEntities()
        {
            for(int i = 0; i < Instance._maxPropCount; i++)
            {
                GameObject propObject = new();
                propObject.AddComponent<UnityEngine.UI.Image>();
                PropEntity propEntityComponent = propObject.AddComponent<PropEntity>();
                propObject.name = $"PropEntity [Empty]";
                propObject.transform.parent = Instance._rectTransform;
                propObject.transform.localScale = new Vector3(1, 1, 1);
                CachedProps.Add(propEntityComponent);
            }
        }

        private static void GenerateImageMapping(ImageDefinitions imageDefinitions)
        {
            if (imageDefinitions == null) return;
            if (imageDefinitions.Images == null) return;
            Image[] images = imageDefinitions.Images;

            foreach(var image in images)
            {
                if (image == null) continue;
                if (image.GetImageData() == null) continue;
                Texture2D? texture = XVNMLModule.ProcessTextureData(image.GetImageData());
                Sprite imageSprite = Sprite.Create(texture, new Rect(0, 0, texture!.width, texture.height), new Vector2(0.5f, 0.5f), 100);
                ImageMapping.Add(image.TagName!, imageSprite);
            }
        }

        internal static void LoadImage(string imageName, Anchoring horizontalAnchoring, Anchoring verticalAnchoring, int xOffset = 0, int yOffset = 0)
        {
            int xPos = 0;
            int yPos = 0;

            switch (horizontalAnchoring)
            {
                case Anchoring.Left:
                    xPos = 0;
                    break;
                case Anchoring.Center:
                    xPos = ModuleWidth / 2;
                    break;
                case Anchoring.Right:
                    xPos = ModuleWidth;
                    break;
                default:
                    break;
            }

            switch (verticalAnchoring)
            {
                case Anchoring.Left:
                    yPos = 0;
                    break;
                case Anchoring.Center:
                    yPos = (ModuleHeight / 2) * -1;
                    break;
                case Anchoring.Right:
                    yPos = ModuleHeight * -1;
                    break;
                default:
                    break;
            }

            xPos += xOffset;
            yPos += yOffset;

            LoadImage(imageName, xPos, yPos);
        }

        internal static void LoadImage(string imageName, int x, int y)
        {
            if (ImageMapping == null || ImageMapping.Count == 0) return;
            if (ImageMapping.ContainsKey(imageName) == false) return;

            Debug.Log($"Loading {imageName}...");

            if (Instance._enablePooling)
            {
                HandlePooling(imageName, x, y);
                return;
            }

            GameObject newImageObject = new();
            UnityEngine.UI.Image imageComponent = newImageObject.AddComponent<UnityEngine.UI.Image>();
            PropEntity propEntityComponent = newImageObject.AddComponent<PropEntity>();
            imageComponent.sprite = ImageMapping[imageName];
            propEntityComponent.SetGraphic(imageComponent.sprite, Color.white);
            newImageObject.name = $"${imageName} [Runtime Image]";

            newImageObject.transform.parent = Instance._rectTransform;
            newImageObject.transform.localScale = SetScale;
            newImageObject.transform.localPosition = new Vector3(x, y, 0);

            DoPropTransition(propEntityComponent);

            CachedProps.Add(propEntityComponent);
        }

        private static void HandlePooling(string imageName, int x, int y)
        {
            PropEntity? result = SearchForFree();
            if (result == null) return;

            result.SetGraphic(ImageMapping[imageName], Color.white);

            result.transform.localScale = SetScale;
            result.gameObject.transform.localPosition = new Vector3(x,y,0);

            result.gameObject.name = $"${imageName} [Runtime Image]";

            DoPropTransition(result);
        }

        internal static void UnloadImage(string imageName)
        {
            PropEntity target = CachedProps.Where(prop => prop.gameObject.name.Contains(imageName)).FirstOrDefault();
            if (target == null) return;
            DoPropTransition(target, () =>
            {
                target.Clear();
                target.gameObject.name = "PropEntity [Empty]";
            });
        }

        private static PropEntity? SearchForFree()
        {
            var objectFound = false;
            var limit = Instance._maxPropCount;
            var counter = 0;

            PropEntity? result = null;

            while(counter < limit && objectFound == false)
            {
                PoolIndex = Mathf.Clamp(PoolIndex, 0, limit-1);


                PropEntity current = CachedProps[PoolIndex];
                if (current.IsViewingGraphic)
                {
                    counter++;
                    PoolIndex++;
                    continue;
                }

                
                result = current;
                objectFound = result != null;
                PoolIndex++;
                counter++;
            }

            if (result == null)
            {
                Debug.LogError("All Images are current occupied. Either increase the Prop Capacity or check that you are unloading images that are not in use.");
                return null;
            }

            return result;
        }

        internal static void SetPropScale(int xScale, int yScale, int zScale = 1)
        {
            SetScale = new Vector3(xScale, yScale, zScale);
        }

        internal static void SetPropTransitionMode(TransitionMode transitionMode)
        {
            PropTransitionMode = transitionMode;
        }

        private static void DoPropTransition(PropEntity entity, TweenCallback? callback = null)
        {
            Transform propTransform = entity.transform;
            DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions>? tweening = null;
            var distance = 0.25f;
            switch (PropTransitionMode)
            {
                case TransitionMode.Instant:
                    entity.PropGraphic.color = Color.white;
                    return;
                case TransitionMode.FadeIn:
                    entity.PropGraphic.color = Transparent;
                    tweening = entity.PropGraphic.DOFade(1, TransitionDuration);
                    break;
                case TransitionMode.FadeOut:
                    entity.PropGraphic.DOFade(0, TransitionDuration);
                    break;
                case TransitionMode.FadeInFromLeft:
                    entity.PropGraphic.color = Transparent;
                    entity.transform.localPosition = new Vector3(propTransform.localPosition.x - distance, propTransform.localPosition.y, 1);
                    entity.transform.DOMoveX( distance, TransitionDuration);
                    tweening = entity.PropGraphic.DOFade(1, TransitionDuration);
                    break;
                case TransitionMode.FadeInFromRight:
                    entity.PropGraphic.color = Transparent;
                    entity.transform.localPosition = new Vector3(propTransform.localPosition.x + distance, propTransform.localPosition.y, 1);
                    entity.transform.DOMoveX(-distance, TransitionDuration);
                    tweening = entity.PropGraphic.DOFade(1, TransitionDuration);
                    break;
                case TransitionMode.FadeInFromTop:
                    entity.PropGraphic.color = Transparent;
                    entity.transform.localPosition = new Vector3(propTransform.localPosition.x, propTransform.localPosition.y - distance, 1);
                    entity.transform.DOMoveY(distance, TransitionDuration);
                    tweening = entity.PropGraphic.DOFade(1, TransitionDuration);
                    break;
                case TransitionMode.FadeInFromBottom:
                    entity.PropGraphic.color = Transparent;
                    entity.transform.localPosition = new Vector3(propTransform.localPosition.x, propTransform.localPosition.y + distance, 1);
                    entity.transform.DOMoveY(-distance, TransitionDuration);
                    tweening = entity.PropGraphic.DOFade(1, TransitionDuration);
                    break;
                case TransitionMode.FadeOutToLeft:
                    entity.transform.DOMoveX(distance, TransitionDuration);
                    tweening = entity.PropGraphic.DOFade(0, TransitionDuration);
                    break;
                case TransitionMode.FadeOutToRight:
                    entity.transform.DOMoveX(-distance, TransitionDuration);
                    tweening = entity.PropGraphic.DOFade(0, TransitionDuration);
                    break;
                case TransitionMode.FadeOutToTop:
                    entity.transform.DOMoveY(-distance, TransitionDuration);
                    tweening = entity.PropGraphic.DOFade(0, TransitionDuration);
                    break;
                case TransitionMode.FadeOutToBottom:
                    entity.transform.DOMoveY(distance, TransitionDuration);
                    tweening = entity.PropGraphic.DOFade(0, TransitionDuration);
                    break;
                default:
                    return;
            }

            if (tweening == null)  return;
            if (callback == null) return;
            tweening.onComplete += callback;
        }

        internal static void SetTransitionDuration(float duration)
        {
            TransitionDuration = duration;
        }
    }
}
