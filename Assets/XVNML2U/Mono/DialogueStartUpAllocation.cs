using UnityEngine;
using XVNML.Utility.Dialogue;
using XVNML2U.Mono.Singleton;

namespace XVNML2U.Mono
{
    public class DialogueStartUpAllocation : Singleton<DialogueStartUpAllocation>
    {
        [Tooltip("How many channels to allocate for " +
                 "Concurrent Dialogue Processes.")]
        public uint channelSize = 12;

        public static uint ChannelSize => Instance.channelSize;

        private void OnEnable()
        {
            Application.quitting += ApplicationClosing;
            DialogueWriter.AllocateChannels((int)channelSize);
        }

        private void ApplicationClosing()
        {
            DialogueWriter.ShutDown();
        }
    }
}