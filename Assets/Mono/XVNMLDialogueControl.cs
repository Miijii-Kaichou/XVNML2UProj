#nullable enable

using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using XVNML.Core.Dialogue;
using XVNML.Core.Dialogue.Structs;
using XVNML.Input.Enums;
using XVNML.Utility.Dialogue;
using XVNML.XVNMLUtility;
using XVNML.XVNMLUtility.Tags;
using XVNML2U.Data;

namespace XVNML2U.Mono
{
    [DisallowMultipleComponent]
    public sealed class XVNMLDialogueControl : MonoActionSender
    {
        public enum ElementReferenceValueType
        {
            ID,
            Name
        }

        // We first need a reference to the XVNML Module that you want to pull a dialogue from
        [Header("Set Up")]
        [SerializeField] private XVNMLModule? module;
        [SerializeField] private bool runOnAwakeUp = false;
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

        [Header("Components")]
        [SerializeField] private XVNMLTextRenderer _castNameText;
        [SerializeField] private XVNMLTextRenderer _mainText;
        [SerializeField] private ConfirmMarker _confirmMarker;
        [SerializeField] private XVNMLPromptControl _promptUnitComponent;

        [Header("Unity Events"), Space(4)]
        [SerializeField] private UnityEvent _onPlay;
        [SerializeField] private UnityEvent _onFinish;
        [SerializeField] private UnityEvent _onChannelBlock;
        [SerializeField] private UnityEvent _onChannelUnblock;

        internal XVNMLModule? Module => module;
        internal XVNMLStage? Stage => stageObj;

        internal int DOMWidth => module!.Root!["screenWidth"].ToInt();
        internal int DOMHeight => module!.Root!["screenHeight"].ToInt();

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

        public bool IsBlocked { get; private set; }

        private AudioSource? _voiceAudioSource;
        private CastInfo _castInfo;
        private CanvasGroup? _canvasGroup;

        private bool _isHidden = false;
        private bool _isFinished = false;
        private const float InactiveAlpha = 0.0f;
        private const float ActiveAlpha = 1.0f;

        private void OnValidate()
        {
            _mainText ??= GetComponent<XVNMLTextRenderer>();
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
            module!.onModuleBuildProcessComplete += Initialize;            
            module!.Build();
        }

        private void Initialize(XVNMLObj obj)
        {
            _mainText ??= GetComponent<XVNMLTextRenderer>();
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

            DialogueWriter.OnLineStart![processChannel] += ResetCastFlags;
            DialogueWriter.OnLineSubstringChange![processChannel] += UpdateTextOutput;
            DialogueWriter.OnLinePause![processChannel] += dontDetain ? DontWait : WaitForMouseClick;

            DialogueWriter.OnPrompt![processChannel] += ShowPrompt;
            DialogueWriter.OnPromptResonse![processChannel] += ResponseToPromptSelection;

            DialogueWriter.OnCastChange![processChannel] += SetCastName;
            DialogueWriter.OnCastExpressionChange![processChannel] += SetCastExpression;
            DialogueWriter.OnCastVoiceChange![processChannel] += SetCastVoice;

            DialogueWriter.OnSceneChange![processChannel] += ManifestCurrentScene;

            DialogueWriter.OnChannelBlock![processChannel] += OnChannelBlock;
            DialogueWriter.OnChannelUnblock![processChannel] += OnChannelUnblock;

            DialogueWriter.OnDialogueFinish![processChannel] += OnFinish;

            PrepareActionSchedular();
            PrepareInputManager();
            PrepareCasts();
            PrepareAudioPool();
            PrepareScenes();

            DialogueWriter.Write(dialogue.dialogueOutput!, channel);

            _onPlay?.Invoke();
        }

        private void OnFinish(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (_isFinished) return WCResult.Ok();
                if (sender.ID != processChannel) return WCResult.Unknown();

                _mainText.Text = string.Empty;
                _onFinish?.Invoke();

                DialogueWriter.OnLineStart![processChannel] -= ResetCastFlags;
                DialogueWriter.OnLineSubstringChange![processChannel] -= UpdateTextOutput;
                DialogueWriter.OnLinePause![processChannel] -= dontDetain ? DontWait : WaitForMouseClick;

                DialogueWriter.OnPrompt![processChannel] -= ShowPrompt;
                DialogueWriter.OnPromptResonse![processChannel] -= ResponseToPromptSelection;

                DialogueWriter.OnCastChange![processChannel] -= SetCastName;
                DialogueWriter.OnCastExpressionChange![processChannel] -= SetCastExpression;
                DialogueWriter.OnCastVoiceChange![processChannel] -= SetCastVoice;

                DialogueWriter.OnSceneChange![processChannel] -= ManifestCurrentScene;

                DialogueWriter.OnChannelBlock![processChannel] -= OnChannelBlock;
                DialogueWriter.OnChannelUnblock![processChannel] -= OnChannelUnblock;

                DialogueWriter.OnDialogueFinish![processChannel] -= OnFinish;

                _isFinished = true;

                return WCResult.Ok();
            });
        }

        internal void SetTextMotions(params string[] motions)
        {
            SendNewAction(() =>
            {
                for (int i = 0; i < motions.Length; i++)
                {
                    _mainText.AddNewMotion(TextMotionRegistry.GetMotion(motions[i]));
                }

                return WCResult.Ok();
            });
        }

        internal void ClearMotions()
        {
            SendNewAction(() =>
            {
                _mainText.ClearMotions();
                return WCResult.Ok();
            });
        }

        private void OnChannelUnblock(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.ID != processChannel) return WCResult.Unknown();
                IsBlocked = false;
                _onChannelUnblock?.Invoke();
                return WCResult.Ok();
            });
        }

        private void OnChannelBlock(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.ID != processChannel) return WCResult.Unknown();
                IsBlocked = true;
                _onChannelBlock?.Invoke();
                return WCResult.Ok();
            });
        }

        private void ResetCastFlags(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                _confirmMarker.OnAccept();
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

                _confirmMarker.OnPending();

                if (XVNMLInputManager.OnInputActive(module, InputEvent.PROCEED) && sender.ID == 0)
                {
                    _confirmMarker.OnAccept();
                    return NextLine(sender);
                }
                return WCResult.Unknown();

            });
        }

        private WCResult NextLine(DialogueWriterProcessor sender)
        {
            DialogueWriter.MoveNextLine(sender);
            _mainText.Text = sender.DisplayingContent;
            return WCResult.Ok();
        }

        private void UpdateTextOutput(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.ID != processChannel) return WCResult.Unknown();

                _mainText.Text = sender.DisplayingContent;

                if (_mainText.IsTextOverflowing) _mainText.PageToDisplay++;
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
                if (_castNameText == null) return WCResult.Ok();

                _castInfo = sender.CurrentCastInfo.Value;

                var name = _castInfo.name ?? string.Empty;

                if (_castNameText.Text == name) return WCResult.Ok();

                _castNameText.Text = name;

                if (_castInfo.name == null) return WCResult.Ok();

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
                _mainText.Text = sender.DisplayingContent;
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

        private void PrepareInputManager()
        {
            if (module == null) return;
            XVNMLInputManager.Init(module);
        }

        private void PrepareActionSchedular()
        {
            if (module == null) return;
            XVNMLActionScheduler.Init();
        }

        private void RunDialogueInGroup(DialogueGroup group)
        {
            if (dialogueReferenceType == ElementReferenceValueType.ID)
            {
                RunDialogue(group[dialogueGroupReferenceValue.Parse<int>()], processChannel);
                return;
            }

            RunDialogue(group[dialogueGroupReferenceValue.Parse<string>()], processChannel);
        }
    }
}