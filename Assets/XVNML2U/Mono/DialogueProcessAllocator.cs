using System;
using UnityEngine;
using XVNML.Utilities.Dialogue;

namespace XVNML2U.Mono
{
    public sealed class DialogueProcessAllocator : Singleton<DialogueProcessAllocator>
    {
        public static int ChannelSize;

        internal static XVNMLDialogueControl[] ProcessReference { get; private set; }

        private bool _isInitialized = false;

        private static void Initialize()
        {
            ChannelSize = XVNML2USettingsUtil.ActiveProjectSettings.CDPChannelLimit;
            ProcessReference = new XVNMLDialogueControl[ChannelSize];
            Application.quitting += ApplicationClosing;
            DialogueWriter.AllocateChannels(ChannelSize);
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
            ChannelSize = XVNML2USettingsUtil.ActiveProjectSettings.CDPChannelLimit;
            DialogueWriter.AllocateChannels(ChannelSize);
        }

        public int GetChannelSize() => ChannelSize;
    }
}