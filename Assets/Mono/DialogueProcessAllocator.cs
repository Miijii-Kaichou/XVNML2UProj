using System.Collections.Generic;
using UnityEngine;
using XVNML.Utility.Dialogue;
using XVNML2U.Mono.Singleton;

namespace XVNML2U.Mono
{
    public sealed class DialogueProcessAllocator : Singleton<DialogueProcessAllocator>
    {
        [Tooltip("How many channels to allocate for " +
                 "Concurrent Dialogue Processes.")]
        public uint channelSize = 12;

        public static uint ChannelSize => Instance.channelSize;

        internal static XVNMLDialogueControl[] ProcessReference { get; private set; }

        private void OnEnable()
        {
            ProcessReference = new XVNMLDialogueControl[channelSize];
            Application.quitting += ApplicationClosing;
            DialogueWriter.AllocateChannels((int)channelSize);
        }

        private void ApplicationClosing()
        {
            DialogueWriter.ShutDown();
            ProcessReference = null;
        }

        internal static void Register(XVNMLDialogueControl control, uint channel)
        {
            ProcessReference[channel] = control;
        }
    }
}