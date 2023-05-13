#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

using XVNML.Core.Dialogue;
using XVNML.Core.Dialogue.Structs;
using XVNML.Utility.Dialogue;
using XVNML.XVNMLUtility.Tags;

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
        [SerializeField] private bool dontDetain = false;
        [SerializeField] private int processChannel = 0;
        [SerializeField] private AudioClip tickSound;
        [SerializeField] private XVNMLStage stageObj;

        // This is just going to be a normal object,
        // but we're expecting a number or a string.
        [Header("Dialogue Target")]
        [SerializeField, Tooltip("Set a number or name of the dialogue" +
            "you want to access")]
        string dialogueReferenceValue;
        [SerializeField] ElementReferenceValueType dialogueReferenceType = ElementReferenceValueType.ID;

        // Lastly, have an optional string that references the
        // dialogue group
        [Header("Dialogue Group Target")]
        [SerializeField] string dialogueGroupReferenceValue;
        [SerializeField] ElementReferenceValueType dialogueGroupReferenceType = ElementReferenceValueType.ID;

        [SerializeField, Header("TextMeshPro")]
        private TextMeshProUGUI nameOuput;

        [SerializeField]
        private TextMeshProUGUI bodyOutput;

        [SerializeField, Header("Prompt Unit Component")]
        private XVNMLPromptControl _promptUnitComponent;

        private Queue<Func<WCResult>>? outputProcessQueue;

        private AudioSource voiceAudioSource;
        private CastInfo castInfo;

        private void OnValidate()
        {
            bodyOutput ??= GetComponent<TextMeshProUGUI>();

            if (tickSound == null) return;
            if (voiceAudioSource != null) return;
            if (gameObject.GetComponent<AudioSource>() == null)
            {
                voiceAudioSource = gameObject.AddComponent<AudioSource>();
                return;
            }
            voiceAudioSource = gameObject.GetComponent<AudioSource>();
        }

        private void Start()
        {
            bodyOutput ??= GetComponent<TextMeshProUGUI>();
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

            if (processChannel > DialogueStartUpAllocation.ChannelSize - 1)
            {
                Debug.LogError($"Process Channel can not be greater than channels allocated: Total Channels Allocated: {DialogueStartUpAllocation.ChannelSize}");
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
                    RunDialogueInGroup(group);
                    return;
                }
                Assert.IsNull(dialogueReferenceValue.ToString());
                group = module.Get<DialogueGroup>(dialogueGroupReferenceValue.ToString());
                RunDialogueInGroup(group);
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

            DialogueWriter.OnLineStart![processChannel] += ManifestSpeakingCast;
            DialogueWriter.OnLineSubstringChange![processChannel] += UpdateTextOutput;
            DialogueWriter.OnLinePause![processChannel] += dontDetain ? DontWait : WaitForMouseClick;

            DialogueWriter.OnPrompt![processChannel] += ShowPrompt;
            DialogueWriter.OnPromptResonse![processChannel] += ResponseToPromptSelection;

            DialogueWriter.OnCastChange![processChannel] += ManifestSpeakingCast;
            DialogueWriter.OnCastExpressionChange![processChannel] += ManifestSpeakingCast;
            DialogueWriter.OnCastVoiceChange![processChannel] += ManifestSpeakingCast;

            DialogueWriter.OnDialogueFinish![processChannel] += OnFinish;

            PrepareCasts();
            PrepareScenes();

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

                DialogueWriter.OnLineStart![processChannel] -= ManifestSpeakingCast;
                DialogueWriter.OnLineSubstringChange![processChannel] -= UpdateTextOutput;
                DialogueWriter.OnLinePause![processChannel] -= dontDetain ? DontWait : WaitForMouseClick;

                DialogueWriter.OnPrompt![processChannel] -= ShowPrompt;
                DialogueWriter.OnPromptResonse![processChannel] -= ResponseToPromptSelection;

                DialogueWriter.OnCastChange![processChannel] -= ManifestSpeakingCast;
                DialogueWriter.OnCastExpressionChange![processChannel] -= ManifestSpeakingCast;
                DialogueWriter.OnCastVoiceChange![processChannel] -= ManifestSpeakingCast;

                DialogueWriter.OnDialogueFinish![processChannel] -= OnFinish;


                _isFinished = true;

                return WCResult.Ok();
            });
        }

        private void DontWait(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.ID != processChannel) return WCResult.Unknown();
                DialogueWriter.MoveNextLine(sender);
                return WCResult.Ok();
            });
        }

        private void WaitForMouseClick(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.ID != processChannel) return WCResult.Unknown();
                if (Input.GetMouseButtonDown(0) && sender.ID == 0)
                {
                    Debug.Log("Proceed");
                    DialogueWriter.MoveNextLine(sender);
                    bodyOutput.text = sender.DisplayingContent;
                    return WCResult.Ok();
                }
                return WCResult.Unknown();
            });
        }


        private void UpdateTextOutput(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.ID != processChannel) return WCResult.Unknown();
                bodyOutput.text = sender.DisplayingContent;
                if (bodyOutput.isTextOverflowing) bodyOutput.pageToDisplay++;
                if (tickSound == null) return WCResult.Ok();
                voiceAudioSource.PlayOneShot(tickSound);
                return WCResult.Ok();
            });
        }

        private void ManifestSpeakingCast(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (sender.CurrentCastInfo == null) return WCResult.Ok();
                if (nameOuput == null) return WCResult.Ok();
                castInfo = sender.CurrentCastInfo.Value;
                nameOuput.text = castInfo.name;
                stageObj.ChangeExpression(castInfo);
                stageObj.ChangeVoice(castInfo);
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
            SceneDefinitions definitions = module.Get<SceneDefinitions>();
            if (definitions == null) return;
            if (definitions.Scenes == null) return;
            if (definitions.Scenes.Length == 0) return;
            stageObj.InitializeSceneController(definitions.Scenes);

        }

        private void PrepareCasts()
        {
            if (module == null) return;
            if (stageObj == null) return;
            CastDefinitions definitions = module.Get<CastDefinitions>();
            if (definitions == null) return;
            if (definitions.CastMembers == null) return;
            if (definitions.CastMembers.Length == 0) return;
            stageObj.InitializeCastController(definitions.CastMembers);
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
            while (_isFinished == false)
            {
                while (outputProcessQueue?.Count < 1)
                {
                    yield return null;
                    continue;
                }

                var action = new Func<WCResult>(() => WCResult.Unknown());
                var result = WCResult.Unknown();

                outputProcessQueue?.TryDequeue(out action);

                if (action == null)
                {
                    yield return null;
                    continue;
                }

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
                }

                yield return null;
            }
        }

        private void SendNewAction(Func<WCResult> function)
        {
            outputProcessQueue?.Enqueue(function);
        }
    }
}