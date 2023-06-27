using System.Collections.Generic;
using UnityEngine;

using XVNML.Core.Dialogue.Structs;
using XVNML.XVNMLUtility.Tags;

namespace XVNML2U.Mono
{
    public sealed class SceneController : MonoBehaviour
    {
        public UnityEngine.UI.Image mainScene;

        private SortedDictionary<string, Sprite> sceneMap = new();

        //Data
        private List<Scene> _scenes;

        // Start is called before the first frame update
        void Awake()
        {
            mainScene = GetComponent<UnityEngine.UI.Image>();
            _scenes = new List<Scene>();
        }

        internal void ChangeScene(SceneInfo sceneInfo)
        {
            string sceneName = sceneInfo.name;
            string transition = sceneInfo.transition;

            if (sceneMap.ContainsKey(sceneName) == false) return;
            var target = sceneMap[sceneName];

            mainScene.sprite = target;
        }

        internal void Init(Scene[] scenes)
        {
            for(int i = 0; i < scenes.Length; i++)
            {
                GenerateSceneImageAndAddToMap(scenes[i]);
            }
        }

        private void GenerateSceneImageAndAddToMap(Scene scene)
        {
            if (scene == null) return;
            if (scene.imageTarget == null || scene.imageTarget.GetImageTargetPath() == string.Empty) return;
            Texture2D tex2D = XVNMLModule.ProcessTextureData(scene.imageTarget.GetImageData());
            _scenes.Add(scene);
            sceneMap.Add(scene.TagName, Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f)));
        } 
    }
}
