#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using XVNML.Core.Dialogue.Structs;
using XVNML.Utilities.Tags;

namespace XVNML2U.Mono
{
    public sealed class SceneController : MonoBehaviour
    {
        public UnityEngine.UI.Image[]? sceneRenderers;

        private Sprite? _nullImage;
        private readonly SortedDictionary<string, Sprite> _sceneMap = new();
        private List<Scene>? _scenes;

        // Start is called before the first frame update
        void Awake()
        {
            var sceneLayers = GetComponentsInChildren<UnityEngine.UI.Image>();
            sceneRenderers = new UnityEngine.UI.Image[sceneLayers.Length];
            sceneRenderers = sceneLayers;

            _nullImage = Resources.Load<Sprite>("Images/NullImage");
            _scenes = new List<Scene>();
        }

        internal void ChangeScene(SceneInfo sceneInfo)
        {
            string? sceneName = sceneInfo.name;
            string? transition = sceneInfo.transition;
            int layer = sceneInfo.layer;

            if (_sceneMap.ContainsKey(sceneName!) == false) return;
            var target = _sceneMap[sceneName!];

            sceneRenderers![layer].sprite = target;
        }

        internal void ClearScene(SceneInfo sceneInfo)
        {
            string? sceneName = sceneInfo.name;
            int layer = sceneInfo.layer;

            if (sceneName == "{all_active}")
            {
                sceneRenderers
                    .Where(sr => sr.sprite != _nullImage)
                    .DoForEvery(sr => sr.sprite = _nullImage);
                return;
            }

            if (sceneName == "{active}")
            {
                sceneRenderers
                    .Where(sr => sr.sprite != _nullImage)
                    .FirstOrDefault(sr => sr.sprite = _nullImage);

                return;
            }

            if (layer > sceneRenderers!.Length - 1) layer = sceneRenderers.Length - 1;
            if (_sceneMap.ContainsKey(sceneName ?? string.Empty) == false)
            {
                sceneRenderers[layer].sprite = _nullImage;
                return;
            }

            if (sceneName != null)
            {
                var target = _sceneMap[sceneName];
                sceneRenderers
                    .Where(sr => sr.sprite == target)
                    .ToArray()[layer].sprite = _nullImage;
                return;
            }
        }

        internal void Init(Scene[] scenes)
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                GenerateSceneImageAndAddToMap(scenes[i]);
            }
        }

        private void GenerateSceneImageAndAddToMap(Scene scene)
        {
            if (scene == null) return;
            if (scene.imageTarget == null || scene.imageTarget.GetImageTargetPath() == string.Empty) return;
            Texture2D? tex2D = XVNMLModule.ProcessTextureData(scene.imageTarget.GetImageData());
            _scenes?.Add(scene);
            _sceneMap.Add(scene?.TagName!, Sprite.Create(tex2D, new Rect(0, 0, tex2D!.width, tex2D.height), new Vector2(0.5f, 0.5f)));
        }

    }
}
