using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using XVNML.Utilities.Tags;
using XVNML2U.Mono;

namespace XVNML2U
{
    public class XVNMLPropsControl : Singleton<XVNMLPropsControl>
    {
        [SerializeField]
        private XVNMLModule _module;

        [SerializeField, Header("Transform")]
        private RectTransform _rectTransform;

        [SerializeField, Header("Configurations")]
        private bool _enablePooling;

        [SerializeField]
        private int _maxPropCount = 32;

        private static SortedDictionary<string, Sprite> ImageMapping = new();
        private static List<PropEntity> CachedProps = new();
        private static Vector3 SetScale = new Vector3(1, 1, 1);


        static int PoolIndex = 0;

        internal static void Init(XVNMLModule module)
        {
            if (module == null) return;

            if (Instance._enablePooling) SetPropEntities();

            Instance._rectTransform ??= Instance.GetComponent<RectTransform>();
            var moduleWidth = module.Root.GetParameterValue<int>("screenWidth");
            var moduleHeight = module.Root.GetParameterValue<int>("screenHeight");

            Instance._rectTransform.sizeDelta = new Vector2(moduleWidth, moduleHeight);

            ImageDefinitions imageDefinitions = module.Get<ImageDefinitions>();

            GenerateImageMapping(imageDefinitions);
            
            Instance._module = module;
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
                Texture2D? texture = XVNMLModule.ProcessTextureData(image.GetImageData());
                Sprite imageSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
                ImageMapping.Add(image.TagName, imageSprite);
            }
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
        }

        internal static void UnloadImage(string imageName)
        {
            PropEntity target = CachedProps.Where(prop => prop.gameObject.name.Contains(imageName)).FirstOrDefault();
            if (target == null) return;
            target.Clear();
            target.gameObject.transform.localPosition = Vector2.zero;
            target.gameObject.name = "PropEntity [Empty]";
        }

        private static PropEntity? SearchForFree()
        {
            var objectFound = false;
            var limit = Instance._maxPropCount;
            var counter = 0;

            PropEntity result = null;

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

        internal static void SetPropScale(int xScale, int yScale, int v)
        {
            SetScale = new Vector3(xScale, yScale, 1);
        }
    }
}
