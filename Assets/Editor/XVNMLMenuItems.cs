using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XVNML2U.Mono;

namespace XVNML2U.Editor
{
#if UNITY_EDITOR
    public sealed class XVNMLMenuItems : UnityEditor.Editor
    {
        private const string HierarchyMenuItemPath = "Window/General/Hierarchy";
        private const string NullImageResourcePath = "Images/NullImage";
        private const string BasicModuleKitPrefabResourcePath = "Assets/Resources/Kits/BasicModuleKit.prefab";
        private const string VNModuleKitPrefabResourcePath = "Assets/Resources/Kits/VNModuleKit.prefab";
        static readonly ColorBlock DefaultResponseControlButtonColorBlock = new()
        {
            normalColor = Color.black + new Color(0, 0, 0, 0.5f),
            highlightedColor = Color.white,
            pressedColor = new Color(0.5503293f, 0.7139516f, 0.7830189f, 1),
            selectedColor = new Color(0.5039605f, 0.6303246f, 0.7075472f, 1),
            colorMultiplier = 1,
            fadeDuration = 0.1f,
        };

        static readonly Color WhiteSemiTransparentColor = new(1, 1, 1, 0.5f);

        [MenuItem("GameObject/XVNML2U/General/Module", priority = 80)]
        static void AddNewXVNMLModuleObject()
        {
            GameObject newXVNMLModuleObject = new();
            newXVNMLModuleObject.AddComponent<XVNMLModule>();
            newXVNMLModuleObject.AddComponent<CoroutineHandler>();

            newXVNMLModuleObject.name = "XVNML Module (Empty)";

            FinalizeObjectCreation(ref newXVNMLModuleObject);
        }

        [MenuItem("GameObject/XVNML2U/General/Input Manager", priority = 80)]
        static void AddNewInputManagerObject()
        {
            GameObject newXVNMLInputManagerObject = new();
            newXVNMLInputManagerObject.AddComponent<XVNMLInputManager>();

            newXVNMLInputManagerObject.name = "XVNMLInputManager";

            FinalizeObjectCreation(ref newXVNMLInputManagerObject);
        }

        [MenuItem("GameObject/XVNML2U/General/Action Scheduler", priority = 80)]
        static void AddNewXVNMLActionScheduler()
        {
            GameObject newXVNMLActionSchedulerObject = new();
            newXVNMLActionSchedulerObject.AddComponent<XVNMLActionScheduler>();

            newXVNMLActionSchedulerObject.name = "XVNMLActionScheduler";

            FinalizeObjectCreation(ref newXVNMLActionSchedulerObject);
        }

        [MenuItem("GameObject/XVNML2U/General/Dialogue Writer Allocator", priority = 80)]
        static void AddNewXVNMLDialogueWriterAllocator()
        {
            GameObject newDialogueWriterAllocator = new();
            DialogueProcessAllocator dialogueProcessAllocatorComponent = newDialogueWriterAllocator.AddComponent<DialogueProcessAllocator>();

            dialogueProcessAllocatorComponent.channelSize = 1;

            newDialogueWriterAllocator.name = "XVNMLDialogueWriterAllocator";

            FinalizeObjectCreation(ref newDialogueWriterAllocator);

        }

        [MenuItem("GameObject/XVNML2U/General/Log Listener", priority = 80)]
        static void AddNewXVNMLogListenerObject()
        {
            GameObject newXVNMLLogListenerObject = new();
            newXVNMLLogListenerObject.AddComponent<XVNMLLogListener>();

            newXVNMLLogListenerObject.name = "XVNMLLogListener";

            FinalizeObjectCreation(ref newXVNMLLogListenerObject);

        }

        [MenuItem("GameObject/XVNML2U/Stage/Stage (Empty)", priority = 81)]
        static void AddNewEmptyXVNMLStageObject()
        {
            GameObject newStageObject = new();
            newStageObject.AddComponent<XVNMLStage>();
            newStageObject.name = "XVNMLStage (Empty)";
            
            FinalizeObjectCreation(ref newStageObject);
        }


        [MenuItem("GameObject/XVNML2U/Stage/Stage", priority = 81)]
        static void AddNewXVNMLStageObject()
        {
            GameObject newStageObject = new();
            XVNMLStage stageComponent = newStageObject.AddComponent<XVNMLStage>();

            GameObject newCastController = new();
            newCastController.transform.parent = newStageObject.transform;
            CastController castControllerComponent = newCastController.AddComponent<CastController>();
            newCastController.name = "CastController";
            stageComponent.SetCastController(castControllerComponent);

            GameObject newSceneController = new();
            newSceneController.transform.parent = newStageObject.transform;
            SceneController sceneControllerComponent = newSceneController.AddComponent<SceneController>();
            newSceneController.name = "SceneController";
            stageComponent.SetSceneController(sceneControllerComponent);

            newStageObject.name = "XVNMLStage";

            FinalizeObjectCreation(ref newStageObject);
        }

        [MenuItem("GameObject/XVNML2U/Text/Text Renderer", priority = 81)]
        static void AddNewXVNMLTextRendererObject()
        {
            GameObject newXVNMLTextRendererObject = new();
            XVNMLTextRenderer textRendererComponent = newXVNMLTextRendererObject.AddComponent<XVNMLTextRenderer>();

            GameObject tmpText = new();

            TextMeshProUGUI textMeshComponent = tmpText.AddComponent<TextMeshProUGUI>();

            textRendererComponent.SetTarget(textMeshComponent);

            tmpText.transform.parent = textRendererComponent.transform;

            newXVNMLTextRendererObject.name = "XVNMLTextRenderer";

            FinalizeObjectCreation(ref newXVNMLTextRendererObject);
        }

        [MenuItem("GameObject/XVNML2U/Audio/AudioController", priority = 81)]
        static void AddNewAudioControllerObject()
        {
            GameObject newXVNMLAudioControllerObject = new();
            newXVNMLAudioControllerObject.AddComponent<XVNMLAudioController>();

            newXVNMLAudioControllerObject.name = "XVNMLAudioController";

            FinalizeObjectCreation(ref newXVNMLAudioControllerObject);
        }

        [MenuItem("GameObject/XVNML2U/Dialogue/Dialogue Control", priority = 81)]
        static void AddNewXVNMLDialogueControl()
        {
            GameObject newXVNMLDialogueControlObject = new();
            newXVNMLDialogueControlObject.AddComponent<XVNMLDialogueControl>();

            newXVNMLDialogueControlObject.name = "XVNMLDialogueController";

            FinalizeObjectCreation(ref newXVNMLDialogueControlObject);
            
        }

        [MenuItem("GameObject/XVNML2U/Prompts/Prompt Control (Empty)", priority = 81)]
        static void AddNewEmptyXVNMLPromptControl()
        {
            GameObject newEmptyPromptControlObject = new();
            newEmptyPromptControlObject.AddComponent<XVNMLPromptControl>();

            newEmptyPromptControlObject.name = "XVNMLPromptControl (Empyt)";

            FinalizeObjectCreation(ref newEmptyPromptControlObject);
        }

        [MenuItem("GameObject/XVNML2U/Prompts/Prompt Control", priority = 81)]
        static void AddNewXVNMLPromptControl()
        {
            GameObject newPromptControlObject = new();
            newPromptControlObject.AddComponent<XVNMLPromptControl>();

            GameObject content = new();
            content.AddComponent<CanvasRenderer>();
            VerticalLayoutGroup vlgComponent = content.AddComponent<VerticalLayoutGroup>();

            vlgComponent.padding = new RectOffset(200, 200, 0, 0);
            vlgComponent.spacing = 24;

            var contentLength = 12;

            StringBuilder sb = new();

            for(int i = 0; i < contentLength; i++)
            {
                GameObject responseControlObject = new();
                Button buttonComponent = responseControlObject.AddComponent<Button>();
                ResponseControl responseControlComponent = responseControlObject.AddComponent<ResponseControl>();

                buttonComponent.colors = DefaultResponseControlButtonColorBlock;

                GameObject graphicObject = new();
                Image imageComponent = graphicObject.AddComponent<Image>();

                imageComponent.color = WhiteSemiTransparentColor;

                GameObject tmpTextObject = new();
                TextMeshProUGUI textMeshProComponent = tmpTextObject.AddComponent<TextMeshProUGUI>();

                textMeshProComponent.fontSize = 36;
                textMeshProComponent.horizontalAlignment = HorizontalAlignmentOptions.Center;
                textMeshProComponent.verticalAlignment = VerticalAlignmentOptions.Middle;

                string text = sb.Append("Prompt Response ")
                                .Append("[")
                                .Append(i)
                                .Append("]")
                                .ToString();

                textMeshProComponent.text = text;

                sb.Clear();

                tmpTextObject.transform.parent = responseControlObject.transform;
                graphicObject.transform.parent = responseControlObject.transform;

                responseControlComponent.SetButton(buttonComponent);

                responseControlObject.name = "ResponseControl";

                responseControlObject.transform.parent = content.transform;
            }

            content.transform.parent = newPromptControlObject.transform;

            newPromptControlObject.name = "XVNMLPromptControl";

            FinalizeObjectCreation(ref newPromptControlObject);
        }

        [MenuItem("GameObject/XVNML2U/Prompts/Response Control (Empty)", priority = 81)]
        static void AddNewEmptyResponseControl()
        {
            GameObject newEmptyResponseControl = new();
            newEmptyResponseControl.AddComponent<ResponseControl>();

            newEmptyResponseControl.name = "ResponseControl (Empty)";

            FinalizeObjectCreation(ref newEmptyResponseControl);
        }

        [MenuItem("GameObject/XVNML2U/Prompts/Response Control", priority = 81)]
        static void AddNewResponseControl()
        {
            GameObject responseControlObject = new();
            Button buttonComponent = responseControlObject.AddComponent<Button>();
            ResponseControl responseControlComponent = responseControlObject.AddComponent<ResponseControl>();

            buttonComponent.colors = DefaultResponseControlButtonColorBlock;

            GameObject graphicObject = new();
            Image imageComponent = graphicObject.AddComponent<Image>();

            imageComponent.color = WhiteSemiTransparentColor;

            GameObject tmpTextObject = new();
            TextMeshProUGUI textMeshProComponent = tmpTextObject.AddComponent<TextMeshProUGUI>();

            textMeshProComponent.fontSize = 36;
            textMeshProComponent.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textMeshProComponent.verticalAlignment = VerticalAlignmentOptions.Middle;

            textMeshProComponent.text = "Prompt Response";

            tmpTextObject.transform.parent = responseControlObject.transform;
            graphicObject.transform.parent = responseControlObject.transform;
            responseControlComponent.SetButton(buttonComponent);

            responseControlObject.name = "ResponseControl";

            FinalizeObjectCreation(ref responseControlObject);
        }

        [MenuItem("GameObject/XVNML2U/Graphics/Confirm Marker", priority = 81)]
        static void AddNewConfirmMarker()
        {
            GameObject newConfirmMarkerObject = new();
            ConfirmMarker confirmMarkerComponent = newConfirmMarkerObject.AddComponent<ConfirmMarker>();
            Image graphic = newConfirmMarkerObject.AddComponent<Image>();

            confirmMarkerComponent.SetGraphic(graphic);

            newConfirmMarkerObject.name = "ConfirmMarker";

            FinalizeObjectCreation(ref  newConfirmMarkerObject);
        }

        [MenuItem("GameObject/XVNML2U/Scene/SceneController", priority = 81)]
        static void AddNewSceneController()
        {
            GameObject newSceneControllerObject = new();
            newSceneControllerObject.AddComponent<SceneController>();

            int layerCount = 5;

            for(int i = 0; i < layerCount; i++)
            {
                GameObject layerObject = new();
                Image imageComponent = layerObject.AddComponent<Image>();
                imageComponent.sprite = Resources.Load<Sprite>(NullImageResourcePath);
                imageComponent.raycastTarget = false;
                imageComponent.maskable = false;
                imageComponent.type = Image.Type.Simple;
                imageComponent.useSpriteMesh = false;
                imageComponent.preserveAspect = false;
                imageComponent.color = Color.white;

                imageComponent.transform.parent = newSceneControllerObject.transform;
            }

            newSceneControllerObject.name = "SceneController";

            FinalizeObjectCreation(ref newSceneControllerObject);
        }

        [MenuItem("GameObject/XVNML2U/Cast/CastController (Empty)", priority = 81)]
        static void AddNewEmptyCastController()
        {
            GameObject newEmptyCastControllerObject = new();
            newEmptyCastControllerObject.AddComponent<CastController>();

            newEmptyCastControllerObject.name = "CastController";

            FinalizeObjectCreation(ref newEmptyCastControllerObject);
        }

        [MenuItem("GameObject/XVNML2U/Cast/CastController", priority = 81)]
        static void AddNewCastController()
        {
            int castCount = 12;

            GameObject newCastControllerObject = new();
            newCastControllerObject.AddComponent<CastController>();

            StringBuilder sb = new();

            for(int i = 0; i < castCount; i++)
            {
                GameObject castEntityObject = new();
                AudioSource castEntityVoiceBox = castEntityObject.AddComponent<AudioSource>();
                Image castEntityImage = castEntityObject.AddComponent<Image>();
                RectTransform castEntityTransform = castEntityObject.AddComponent<RectTransform>();

                castEntityImage.sprite = Resources.Load<Sprite>(NullImageResourcePath);
                castEntityImage.raycastTarget = false;
                castEntityImage.maskable = false;
                castEntityImage.type = Image.Type.Simple;
                castEntityImage.color = Color.white;
                castEntityImage.useSpriteMesh = false;
                castEntityImage.preserveAspect = true;

                castEntityTransform.localPosition = new Vector3(-1325, 0);
                castEntityTransform.sizeDelta = new Vector2(700, 2300);

                CastEntity castEntityComponent = castEntityObject.AddComponent<CastEntity>();
                castEntityComponent.loadingMode = LoadingMode.External;
                castEntityComponent.graphicMode = CastGraphicMode.Image;
                castEntityComponent.SetVoiceBox(castEntityVoiceBox);

                var newName = sb.Append("Cast")
                    .Append("[")
                    .Append(i)
                    .Append("]")
                    .ToString();

                castEntityObject.name = newName;

                sb.Clear();

                castEntityObject.transform.parent = newCastControllerObject.transform;
            }

            newCastControllerObject.name = "CastController";

            FinalizeObjectCreation(ref newCastControllerObject);
        }

        [MenuItem("GameObject/XVNML2U/Cast/Cast Entity", priority = 81)]
        static void AddNewCastEntity()
        {
            GameObject newCastEntityObject = new();
            AudioSource castEntityVoiceBox = newCastEntityObject.AddComponent<AudioSource>();
            Image castEntityImage = newCastEntityObject.AddComponent<Image>();
            RectTransform castEntityTransform = newCastEntityObject.AddComponent<RectTransform>();

            castEntityImage.sprite = Resources.Load<Sprite>(NullImageResourcePath);
            castEntityImage.raycastTarget = false;
            castEntityImage.maskable = false;
            castEntityImage.type = Image.Type.Simple;
            castEntityImage.color = Color.white;
            castEntityImage.useSpriteMesh = false;
            castEntityImage.preserveAspect = true;

            castEntityTransform.localPosition = new Vector3(-1325, 0);
            castEntityTransform.sizeDelta = new Vector2(700, 2300);

            CastEntity castEntityComponent = newCastEntityObject.AddComponent<CastEntity>();
            castEntityComponent.loadingMode = LoadingMode.External;
            castEntityComponent.graphicMode = CastGraphicMode.Image;
            castEntityComponent.SetVoiceBox(castEntityVoiceBox);

            newCastEntityObject.name = "CastEntity";

            FinalizeObjectCreation(ref newCastEntityObject);
        }

        [MenuItem("GameObject/XVNML2U/Props/XVNMLPropsController (Empty)", priority = 81)]
        static void AddNewEmptyXVNMLPropController()
        {
            GameObject newPropControllerObject = new();
            newPropControllerObject.AddComponent<XVNMLPropsControl>();

            newPropControllerObject.name = "PropController (Empty)";

            FinalizeObjectCreation(ref newPropControllerObject);
        }

        [MenuItem("GameObject/XVNML2U/Props/PropEntity", priority = 81)]
        static void AddNewPropEntity()
        {
            GameObject newPropEntityObject = new();
            newPropEntityObject.AddComponent<PropEntity>();

            newPropEntityObject.name = "PropEntity";

            FinalizeObjectCreation(ref newPropEntityObject);
        }

        [MenuItem("GameObject/XVNML2U/Kits/Basic Module Kit", priority = 81)]
        static void AddNewBasicModuleKit()
        {
            GameObject newBasicModuleKitObject = PrefabUtility.LoadPrefabContents(BasicModuleKitPrefabResourcePath);
            newBasicModuleKitObject = Instantiate(newBasicModuleKitObject);
            newBasicModuleKitObject.name = "BasicModuleKit";
            FinalizeObjectCreation(ref newBasicModuleKitObject);
        }

        [MenuItem("GameObject/XVNML2U/Kits/VN Module Kit", priority = 81)]
        static void AddNewVNModuleKit()
        {
            GameObject newVNModuleKitObject = PrefabUtility.LoadPrefabContents(VNModuleKitPrefabResourcePath);
            newVNModuleKitObject = Instantiate(newVNModuleKitObject);
            newVNModuleKitObject.name = "VNModuleKit";
            FinalizeObjectCreation(ref newVNModuleKitObject);
        }

        private static void FinalizeObjectCreation(ref GameObject obj)
        {
            Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);

            Selection.activeObject = obj;

            InitializeRenameMode();
        }

        private static void InitializeRenameMode()
        {
            EditorApplication.delayCall += () =>
            {
                EditorApplication.ExecuteMenuItem(HierarchyMenuItemPath);
                EditorWindow.focusedWindow.SendEvent(new Event { keyCode = KeyCode.F2, type = EventType.KeyDown });
            };
        }
    }
#endif
}
