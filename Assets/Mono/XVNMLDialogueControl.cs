#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

using XVNML.Core.Dialogue;
using XVNML.Core.Dialogue.Structs;
using XVNML.Utility.Dialogue;
using XVNML.XVNMLUtility;
using XVNML.XVNMLUtility.Tags;
using XVNML2U.Assets.Extensions;
using XVNML2U.Data;

namespace XVNML2U.Mono
{
    [DisallowMultipleComponent]
    public sealed class XVNMLDialogueControl : MonoBehaviour
    {
        public enum ElementReferenceValueType
        {
            ID,
            Name
        }

        [SerializeField] private bool _isFinished = false;

        // We first need a reference to the XVNML Module that you want to pull a dialogue from
        [Header("Set Up")]
        [SerializeField] private XVNMLModule? module;
        [SerializeField] private bool runOnAwakeUp = false;
        [SerializeField] private bool clearScreenOnFinish = false;
        [SerializeField] private bool dontDetain = false;
        [SerializeField] private int processChannel = 0;
        [SerializeField] private AudioClip tickSound;
        [SerializeField] private XVNMLStage stageObj;

        // This is just going to be a normal object,
        // but we're expecting a number or a string.
        [Header("Dialogue Target")]
        [SerializeField, Tooltip("Set a number or name of the dialogue" +
            "you want to access")]
        private string dialogueReferenceValue;
        [SerializeField] private ElementReferenceValueType dialogueReferenceType = ElementReferenceValueType.ID;

        // Lastly, have an optional string that references the
        // dialogue group
        [Header("Dialogue Group Target")]
        [SerializeField] private string dialogueGroupReferenceValue;
        [SerializeField] private ElementReferenceValueType dialogueGroupReferenceType = ElementReferenceValueType.ID;

        [Header("TextMeshPro/Styling")]
        [SerializeField] private TextMeshProUGUI nameOutput;
        [SerializeField] private TextMeshProUGUI bodyOutput;
        [SerializeField] private ConfirmMarker confirmMarker;

        [Header("Prompt Unit Component")]
        [SerializeField] private XVNMLPromptControl _promptUnitComponent;

        private bool _castChanging = false;
        private Queue<Func<WCResult>>? outputProcessQueue;
        private AudioSource? _voiceAudioSource;
        private CastInfo _castInfo;
        private CanvasGroup? _canvasGroup;
        private bool _isHidden = false;

        internal XVNMLModule? Module => module;
        internal XVNMLStage? Stage => stageObj;
        internal int DOMWidth => module!.Main.top!.Root!["screenWidth"].ToInt();
        internal int DOMHeight => module!.Main.top!.Root!["screenHeight"].ToInt();
        internal bool IsHidden
        {
            get
            {
                return _isHidden;
            }
            set
            {
                _isHidden = value;
                if (_canvasGroup == null) return;

                SendNewAction(() =>
                {
                    _canvasGroup.alpha = _isHidden ? InactiveAlpha : ActiveAlpha;
                    return WCResult.Ok();
                });
            }
        }

        private const float InactiveAlpha = 0.0f;
        private const float ActiveAlpha = 1.0f;

        private void OnValidate()
        {
            bodyOutput ??= GetComponent<TextMeshProUGUI>();
            _canvasGroup ??= GetComponent<CanvasGroup>();

            if (tickSound == null) return;
            if (_voiceAudioSource != null) return;
            if (gameObject.GetComponent<AudioSource>() == null)
            {
                _voiceAudioSource = gameObject.AddComponent<AudioSource>();
                return;
            }

            _voiceAudioSource = gameObject.GetComponent<AudioSource>();
        }

        private void Start()
        {
            DialogueProcessAllocator.Register(this, (uint)processChannel);
            module!.onModuleBuildProcessComplete = Initialize;
            module!.Build();
        }

        private void Initialize(XVNMLObj obj)
        {
            bodyOutput ??= GetComponent<TextMeshProUGUI>();
            _canvasGroup ??= GetComponent<CanvasGroup>();
            if (runOnAwakeUp == false) return;

            Play();
        }

        public void Play()
        {
            if (module == null) return;

            _isFinished = false;

            if (processChannel < 0)
            {
                Debug.LogError("Process Channel can not be less than zero.");
                return;
            }

            if (processChannel > DialogueProcessAllocator.ChannelSize - 1)
            {
                Debug.LogError($"Process Channel can not be greater than channels allocated: Total Channels Allocated: {DialogueProcessAllocator.ChannelSize}");
                return;
            }

            if (dialogueReferenceValue == string.Empty)
                dialogueReferenceValue = 0.ToString();

            if (dialogueGroupReferenceValue != string.Empty)
            {
                DialogueGroup? group = null;

                if (dialogueGroupReferenceType == ElementReferenceValueType.ID)
                {
                    group = module.Get<DialogueGroup>(Convert.ToInt32(dialogueGroupReferenceValue));
                    if (group == null) return;

                    RunDialogueInGroup(group);
                    return;
                }

                Assert.IsNull(dialogueReferenceValue.ToString());
                if (group == null) return;

                group = module.Get<DialogueGroup>(dialogueGroupReferenceValue.ToString());
                RunDialogueInGroup(group!);
                return;
            }

            RunDialogue();
        }

        public void Play(int dialogueIndex)
        {
            dialogueReferenceValue = dialogueIndex.ToString();
        }

        public void Play(int dialogueIndex, int dialogueGroup)
        {
            dialogueReferenceValue = dialogueIndex.ToString();
            dialogueGroupReferenceValue = dialogueGroup.ToString();
        }

        public void Play(int dialogueIndex, string dialogueGroup)
        {
            dialogueReferenceValue = dialogueIndex.ToString();
            dialogueGroupReferenceValue = dialogueGroup;
        }

        public void Play(string dialogueName)
        {
            dialogueReferenceValue = dialogueName;
        }

        public void Play(string dialogueName, int dialogueGroup)
        {
            dialogueReferenceValue = dialogueName;
        }

        public void Play(string dialogueName, string dialogueGroup)
        {
            dialogueReferenceValue = dialogueName;
            dialogueGroupReferenceValue = dialogueGroup;
        }

        public void Continue(DialogueWriterProcessor process)
        {
            DialogueWriter.MoveNextLine(process);
        }

        public void SetForChannel(int channel)
        {
            processChannel = channel;
        }

        private void RunDialogue()
        {
            if (module == null) return;

            if (dialogueReferenceType == ElementReferenceValueType.ID)
            {
                RunDialogue(module.Get<Dialogue>(Convert.ToInt32(dialogueReferenceValue)), processChannel);
                return;
            }

            RunDialogue(module.Get<Dialogue>(dialogueReferenceValue.ToString()), processChannel);
        }

        private void RunDialogue(Dialogue? dialogue, int channel)
        {
            if (dialogue == null)
            {
                Debug.LogError($"Failed to run dialogue for channel {channel}");
                return;
            }

            dontDetain = dialogue.DoNotDetain;
            outputProcessQueue = new Queue<Func<WCResult>>();

            DialogueWriter.OnLineStart![processChannel] += ResetCastFlags;
            DialogueWriter.OnLineSubstringChange![processChannel] += UpdateTextOutput;
            DialogueWriter.OnLinePause![processChannel] += dontDetain ? DontWait : WaitForMouseClick;

            DialogueWriter.OnPrompt![processChannel] += ShowPrompt;
            DialogueWriter.OnPromptResonse![processChannel] += ResponseToPromptSelection;

            DialogueWriter.OnCastChange![processChannel] += SetCastName;
            DialogueWriter.OnCastExpressionChange![processChannel] += SetCastExpression;
            DialogueWriter.OnCastVoiceChange![processChannel] += SetCastVoice;

            DialogueWriter.OnSceneChange![processChannel] += ManifestCurrentScene;

            DialogueWriter.OnDialogueFinish![processChannel] += OnFinish;

            PrepareCasts();
            PrepareScenes();
            PrepareAudioPool();

            DialogueWriter.Write(dialogue.dialogueOutput!, channel);

            StartCoroutine(QueueCycle());
        }

        private void OnFinish(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (_isFinished) return WCResult.Ok();
                if (sender.ID != processChannel) return WCResult.Unknown();

                bodyOutput.text = string.Empty;

                DialogueWriter.OnLineStart![processChannel] -= ResetCastFlags;
                DialogueWriter.OnLineSubstringChange![processChannel] -= UpdateTextOutput;
                DialogueWriter.OnLinePause![processChannel] -= dontDetain ? DontWait : WaitForMouseClick;

                DialogueWriter.OnPrompt![processChannel] -= ShowPrompt;
                DialogueWriter.OnPromptResonse![processChannel] -= ResponseToPromptSelection;

                DialogueWriter.OnCastChange![processChannel] -= SetCastName;
                DialogueWriter.OnCastExpressionChange![processChannel] -= SetCastExpression;
                DialogueWriter.OnCastVoiceChange![processChannel] -= SetCastVoice;

                DialogueWriter.OnSceneChange![processChannel] -= ManifestCurrentScene;

                DialogueWriter.OnDialogueFinish![processChannel] -= OnFinish;

                _isFinished = true;

                if (clearScreenOnFinish == false) return WCResult.Ok();
                if (_canvasGroup == null) return WCResult.Ok();

                _canvasGroup.alpha = InactiveAlpha;

                return WCResult.Ok();
            });
        }

        private void ResetCastFlags(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                _castChanging = false;
                confirmMarker.gameObject.SetActive(false);
                SetCastName(sender);
                return WCResult.Ok();
            });
        }

        private void DontWait(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.ID != processChannel) return WCResult.Unknown();
                return NextLine(sender);
            });
        }

        private void WaitForMouseClick(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.ID != processChannel) return WCResult.Unknown();

                if (sender.IsPass)
                {
                    return NextLine(sender);
                }

                confirmMarker.gameObject.SetActive(true);

                if (Input.GetMouseButtonDown(0) && sender.ID == 0)
                {
                    confirmMarker.gameObject.SetActive(false);
                    return NextLine(sender);
                }
                return WCResult.Unknown();

            });
        }

        private WCResult NextLine(DialogueWriterProcessor sender)
        {
            DialogueWriter.MoveNextLine(sender);
            bodyOutput.text = sender.DisplayingContent;
            return WCResult.Ok();
        }

        private void UpdateTextOutput(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.ID != processChannel) return WCResult.Unknown();

                bodyOutput.text = sender.DisplayingContent;

                if (bodyOutput.isTextOverflowing) bodyOutput.pageToDisplay++;
                if (tickSound == null) return WCResult.Ok();

                _voiceAudioSource?.PlayOneShot(tickSound);
                return WCResult.Ok();
            });
        }

        private void SetCastName(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                ;
                if (sender.CurrentCastInfo == null) return WCResult.Ok();
                if (nameOutput == null) return WCResult.Ok();

                _castInfo = sender.CurrentCastInfo.Value;

                if (_castInfo.name == null) return WCResult.Ok();
                if (nameOutput.text == _castInfo.name) return WCResult.Ok();

                nameOutput.text = _castInfo.name;

                SetCastExpression(sender);
                SetCastVoice(sender);

                return WCResult.Ok();
            });
        }


        private void SetCastExpression(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.CurrentCastInfo == null) return WCResult.Ok();

                _castInfo = sender.CurrentCastInfo.Value;
                if (_castInfo.name == null) return WCResult.Ok();

                Stage?.ChangeExpression(_castInfo);

                return WCResult.Ok();
            });
        }

        private void SetCastVoice(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.CurrentCastInfo == null) return WCResult.Ok();

                _castInfo = sender.CurrentCastInfo.Value;
                if (_castInfo.name == null) return WCResult.Ok();

                Stage?.ChangeVoice(_castInfo);

                return WCResult.Ok();
            });
        }

        private void ManifestCurrentScene(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.CurrentSceneInfo == null) return WCResult.Ok();
                stageObj.ChangeScene(sender.CurrentSceneInfo.Value);
                return WCResult.Ok();
            });
        }
        private void ResponseToPromptSelection(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                bodyOutput.text = sender.DisplayingContent;
                _promptUnitComponent.Clear();
                return WCResult.Ok();
            });
        }

        private void ShowPrompt(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                _promptUnitComponent.SetPrompts(sender);
                return WCResult.Ok();
            });
        }

        private void PrepareScenes()
        {
            if (module == null) return;
            if (stageObj == null) return;

            SceneDefinitions definitions = module.Get<SceneDefinitions>()!;

            if (definitions == null) return;
            if (definitions.Scenes == null) return;
            if (definitions.Scenes.Length == 0) return;

            stageObj.InitializeSceneController(definitions.Scenes);
        }

        private void PrepareCasts()
        {
            if (module == null) return;
            if (stageObj == null) return;

            CastDefinitions definitions = module.Get<CastDefinitions>()!;

            if (definitions == null) return;
            if (definitions.CastMembers == null) return;
            if (definitions.CastMembers.Length == 0) return;

            stageObj.InitializeCastController(definitions.CastMembers);
        }

        private void PrepareAudioPool()
        {
            if (module == null) return;

            AudioDefinitions definitions = module.Get<AudioDefinitions>()!;

            if (definitions == null) return;
            if (definitions.AudioCollection == null) return;
            if (definitions.AudioCollection.Length == 0) return;

            XVNMLAudioController.Init(definitions.AudioCollection);
        }

        private void RunDialogueInGroup(DialogueGroup group)
        {
            if (dialogueReferenceType == ElementReferenceValueType.ID)
            {
                RunDialogue(group.GetDialogue(Convert.ToInt32(dialogueGroupReferenceValue)), processChannel);
                return;
            }

            RunDialogue(group.GetDialogue(dialogueGroupReferenceValue.ToString()), processChannel);
        }



        private IEnumerator QueueCycle()
        {
            bool errorEncountered = false;

            if (_canvasGroup != null && _canvasGroup.alpha == InactiveAlpha)
                _canvasGroup.alpha = ActiveAlpha;

            while (_isFinished == false)
            {
                if (outputProcessQueue?.Count > 0)
                {
                    for (int i = 0; i < outputProcessQueue?.Count; i++)
                    {
                        var action = new Func<WCResult>(() => WCResult.Unknown());
                        var result = WCResult.Unknown();

                        outputProcessQueue?.TryDequeue(out action);

                        if (action == null) continue;

                        while ((result = action.Invoke()) == WCResult.Unknown())
                        {
                            yield return null;
                            continue;
                        }

                        if (result != WCResult.Error() && result.Message != string.Empty)
                        {
                            Debug.Log(result.Message);
                        }

                        if (result == WCResult.Error())
                        {
                            Debug.LogError(result.Message);
                            errorEncountered = true;
                            break;
                        }
                    }
                }

                if (errorEncountered) yield break;

                yield return null;
            }
        }

        internal void SendNewAction(Func<WCResult> function)
        {
            outputProcessQueue?.Enqueue(function);
        }
    }
}