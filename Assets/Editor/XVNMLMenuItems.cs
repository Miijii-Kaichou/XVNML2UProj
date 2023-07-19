using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XVNML2U.Mono;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace XVNML2U
{
    public sealed class XVNMLMenuItems : Editor
    {

        static readonly ColorBlock DefaultResponseControlButtonColorBlock = new()
        {
            normalColor = Color.black + new Color(0, 0, 0, 0.5f),
            highlightedColor = Color.white,
            pressedColor = new Color(0.5503293f, 0.7139516f, 0.7830189f, 1),
            selectedColor = new Color(0.5039605f, 0.6303246f, 0.7075472f, 1),
            colorMultiplier = 1,
            fadeDuration = 0.1f,
        };

        static readonly Color WhiteSemiTransparentColor = new Color(1, 1, 1, 0.5f);


        [MenuItem("GameObject/XVNML2U/Stage/Stage (Empty)")]
        static void AddNewEmptyXVNMLStageObject()
        {
            GameObject newStageObject = new();
            newStageObject.AddComponent<XVNMLStage>();
            newStageObject.name = "XVNMLStage (Empty)";
            
            FinalizeObjectCreation(ref newStageObject);
        }


        [MenuItem("GameObject/XVNML2U/Stage/Stage")]
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

        [MenuItem("GameObject/XVNML2U/XVNML Module")]
        static void AddNewXVNMLModuleObject()
        {
            GameObject newXVNMLModuleObject = new();
            newXVNMLModuleObject.AddComponent<XVNMLModule>();
            newXVNMLModuleObject.AddComponent<CoroutineHandler>();

            newXVNMLModuleObject.name = "XVNML Module (Empty)";

            FinalizeObjectCreation(ref newXVNMLModuleObject);
        }

        [MenuItem("GameObject/XVNML2U/UI/Text Renderer")]
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

        [MenuItem("GameObject/XVNML2U/Audio/AudioController")]
        static void AddNewAudioControllerObject()
        {
            GameObject newXVNMLAudioControllerObject = new();
            newXVNMLAudioControllerObject.AddComponent<XVNMLAudioController>();

            newXVNMLAudioControllerObject.name = "XVNMLAudioController";

            FinalizeObjectCreation(ref newXVNMLAudioControllerObject);
        }

        [MenuItem("GameObject/XVNML2U/Singleton/Input Manager")]
        static void AddNewInputManagerObject()
        {
            GameObject newXVNMLInputManagerObject = new();
            newXVNMLInputManagerObject.AddComponent<XVNMLInputManager>();

            newXVNMLInputManagerObject.name = "XVNMLInputManager";

            FinalizeObjectCreation(ref newXVNMLInputManagerObject);
        }

        [MenuItem("GameObject/XVNML2U/Singleton/XVNML Log Listener")]
        static void AddNewXVNMLogListenerObject()
        {
            GameObject newXVNMLLogListenerObject = new();
            newXVNMLLogListenerObject.AddComponent<XVNMLLogListener>();

            newXVNMLLogListenerObject.name = "XVNMLLogListener";

            FinalizeObjectCreation(ref newXVNMLLogListenerObject);
            
        }

        [MenuItem("GameObject/XVNML2U/Dialogue/Dialogue Control")]
        static void AddNewXVNMLDialogueControl()
        {
            GameObject newXVNMLDialogueControlObject = new();
            newXVNMLDialogueControlObject.AddComponent<XVNMLDialogueControl>();

            newXVNMLDialogueControlObject.name = "XVNMLDialogueController";

            FinalizeObjectCreation(ref newXVNMLDialogueControlObject);
            
        }

        [MenuItem("GameObject/XVNML2U/Singleton/XVNML Action Scheduler")]
        static void AddNewXVNMLActionScheduler()
        {
            GameObject newXVNMLActionSchedulerObject = new();
            newXVNMLActionSchedulerObject.AddComponent<XVNMLActionScheduler>();

            newXVNMLActionSchedulerObject.name = "XVNMLActionScheduler";

            FinalizeObjectCreation(ref newXVNMLActionSchedulerObject);
        }

        [MenuItem("GameObject/XVNML2U/Singleton/Dialogue Writer Allocator")]
        static void AddNewXVNMLDialogueWriterAllocator()
        {
            GameObject newDialogueWriterAllocator = new();
            newDialogueWriterAllocator.AddComponent<DialogueProcessAllocator>();

            newDialogueWriterAllocator.name = "XVNMLDialogueWriterAllocator";

            FinalizeObjectCreation(ref newDialogueWriterAllocator);
            
        }

        [MenuItem("GameObject/XVNML2U/Prompts/Prompt Control (Empty)")]
        static void AddNewEmptyXVNMLPromptControl()
        {
            GameObject newEmptyPromptControlObject = new();
            newEmptyPromptControlObject.AddComponent<XVNMLPromptControl>();

            newEmptyPromptControlObject.name = "XVNMLPromptControl (Empyt)";

            FinalizeObjectCreation(ref newEmptyPromptControlObject);
        }

        [MenuItem("GameObject/XVNML2U/Prompts/Prompt Control")]
        static void AddNewXVNMLPromptControl()
        {
            GameObject newPromptControlObject = new();
            newPromptControlObject.AddComponent<XVNMLPromptControl>();

            GameObject content = new();
            RectTransform rectTransformComponent = content.AddComponent<RectTransform>();
            content.AddComponent<CanvasRenderer>();
            VerticalLayoutGroup vlgComponent = content.AddComponent<VerticalLayoutGroup>();

            vlgComponent.padding = new RectOffset(200, 200, 0, 0);
            vlgComponent.spacing = 24;

            var contentLength = 12;

            StringBuilder sb = new StringBuilder();

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

        [MenuItem("GameObject/XVNML2U/Prompts/Response Control (Empty)")]
        static void AddNewEmptyResponseControl()
        {
            GameObject newEmptyResponseControl = new();
            newEmptyResponseControl.AddComponent<ResponseControl>();

            newEmptyResponseControl.name = "ResponseControl (Empyt)";

            FinalizeObjectCreation(ref newEmptyResponseControl);
        }

        [MenuItem("GameObject/XVNML2U/Prompts/Response Control")]
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

        [MenuItem("GameObject/XVNML2U/UI/Confirm Marker")]
        static void AddNewConfirmMarker()
        {

            
        }

        [MenuItem("GameObject/XVNML2U/Scene/SceneController")]
        static void AddNewSceneController()
        {

            
        }

        [MenuItem("GameObject/XVNML2U/Cast/CastController")]
        static void AddNewCastController()
        {

            
        }

        [MenuItem("GameObject/XVNML2U/Cast/Cast Entity")]
        static void AddNewCastEntity()
        {

            
        }

        [MenuItem("GameObject/XVNML2U/Kits/Basic Module Kit")]
        static void AddNewBasicModuleKit()
        {

            
        }

        [MenuItem("GameObject/XVNML2U/Kits/VN Module Kit")]
        static void AddNewVNModuleKit()
        {

            
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
                EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
                EditorWindow.focusedWindow.SendEvent(new Event { keyCode = KeyCode.F2, type = EventType.KeyDown });
            };
        }
    }
}
