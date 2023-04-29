using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XVNML.XVNMLUtility.Tags;

namespace XVNML2U
{
    public class SceneController : MonoBehaviour
    {
        public UnityEngine.UI.Image mainScene;

        private SortedDictionary<string, Sprite> sceneMap = new();

        //Data
        private List<Scene> _scenes;

        // Start is called before the first frame update
        void Awake()
        {
            mainScene = GetComponent<UnityEngine.UI.Image>();
        }

        void ChangeScene(string sceneName)
        {

        }

        void ChangeScene(string sceneName, Action<ChangeSceneOptions> options)
        {

        }

        void ChangeScene(int sceneId)
        {

        }

        void ChangeScene(int sceneId, Action<ChangeSceneOptions> options)
        {

        }

        internal void Init(XVNML.XVNMLUtility.Tags.Scene[] scenes)
        {
            for(int i = 0; i < scenes.Length; i++)
            {
                GenerateSceneImageAndAddToMap(scenes[i]);
            }
        }

        private void GenerateSceneImageAndAddToMap(Scene scene)
        {
            _scenes.Add(scene);
            sceneMap.Add(scene.TagName, Sprite.Create(XVNMLModule.ProcessTextureData(scene.imageTarget.GetImageData()), new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f)));
        } 
    }

    public class ChangeSceneOptions
    {

    }
}
