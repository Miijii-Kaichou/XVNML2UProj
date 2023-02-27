#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

using XVNML.Core.Dialogue;
using XVNML.Utility.Dialogue;
using XVNML.XVNMLUtility.Tags;

using XVNML2U.Data;

namespace XVNML2U.Mono
{
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
        private TextMeshProUGUI textOutput;
        private Queue<Func<WCResult>>? outputProcessQueue;

        private void OnValidate()
        {
            textOutput ??= GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            textOutput ??= GetComponent<TextMeshProUGUI>();
            if (runOnAwakeUp == false) return;
            Play();
        }

        private void OnFinish(DialogueWriterProcessor sender)
        {
            SendNewAction(() =>
            {
                if (_isFinished) return WCResult.Ok();
                if (sender.ID != processChannel) return WCResult.Unknown();
                textOutput.text = string.Empty;
                DialogueWriter.OnLineSubstringChange![processChannel] -= UpdateTextOutput;
                DialogueWriter.OnLinePause![processChannel] -= dontDetain ? DontWait : WaitForMouseClick;
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
                    textOutput.text = sender.DisplayingContent;
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
                textOutput.text = sender.DisplayingContent;
                return WCResult.Ok();
            });
        }

        public void Play()
        {
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

        private void RunDialogue()
        {
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
            DialogueWriter.OnLineSubstringChange![processChannel] += UpdateTextOutput;
            DialogueWriter.OnLinePause![processChannel] += dontDetain ? DontWait : WaitForMouseClick;
            DialogueWriter.OnDialogueFinish![processChannel] += OnFinish;
            DialogueWriter.Write(dialogue.dialogueOutput!, channel);
            StartCoroutine(QueueCycle());
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

        public void Continue(DialogueWriterProcessor process)
        {
            DialogueWriter.MoveNextLine(process);
        }

        public void SetForChannel(int channel)
        {
            processChannel = channel;
        }

        IEnumerator QueueCycle()
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