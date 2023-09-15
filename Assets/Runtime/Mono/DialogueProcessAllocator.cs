using System;
using UnityEngine;
using XVNML.Utilities.Dialogue;

namespace XVNML2U.Mono
{
    public sealed class DialogueProcessAllocator : Singleton<DialogueProcessAllocator>
    {
        [Tooltip("How many channels to allocate for " +
                 "Concurrent Dialogue Processes."), Range(1,12)]
        public uint channelSize = 12;

        public static uint ChannelSize => Instance.channelSize;

        internal static XVNMLDialogueControl[] ProcessReference { get; private set; }

        private bool _isInitialized = false;

        private static void Initialize()
        {
            int size = (int)Instance.channelSize;
            ProcessReference = new XVNMLDialogueControl[size];
            Application.quitting += ApplicationClosing;
            DialogueWriter.AllocateChannels(size);
        }

        private static void ApplicationClosing()
        {
            DialogueWriter.ShutDown();
            ProcessReference = null;
        }

        internal static void Register(XVNMLDialogueControl control, uint channel)
        {
            if (Instance._isInitialized == false) Initialize();
            ProcessReference[channel] = control;
        }

        internal static void Refresh()
        {
            var size = (int)Instance.channelSize;
            DialogueWriter.AllocateChannels(size);
        }
    }
}