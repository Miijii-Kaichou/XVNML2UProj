using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XVNML.Input.Enums;
using XVNML.XVNMLUtility;
using XVNML.XVNMLUtility.Tags;

namespace XVNML2U.Mono
{
    public sealed class XVNMLInputManager : Singleton<XVNMLInputManager>
    {
        [SerializeField]
        private XVNMLModule _module;

        private static readonly SortedDictionary<InputEvent, List<VirtualKey>> VKPurposeMap = new();
        private static KeycodeDefinitions AttachedKeycodeDefinitions;
        private static bool IsInitialized = false;

        private void Start()
        {
            _module.onModuleBuildProcessComplete += OnModuleReady;
        }

        private void OnModuleReady(XVNMLObj dom)
        {
            KeycodeDefinitions def = dom.Root.GetElement<KeycodeDefinitions>();
            if (def == null) return;

            Keycode[] keycodes = dom.Root.GetElement<KeycodeDefinitions>().KeyCodes;
            if (keycodes.Length == 0) return;
        
            AttachedKeycodeDefinitions = def;
            Initialize(keycodes);
        }

        private static void Initialize(Keycode[] keycodes)
        {
            if (IsInitialized) return;

            for (int i = 0; i < keycodes.Length; i++)
            {
                Keycode code = keycodes[i];

                if (VKPurposeMap.ContainsKey(code.purpose))
                {
                    VKPurposeMap[code.purpose].Add(code.vkey);
                    continue;
                }

                VKPurposeMap.Add(code.purpose, new List<VirtualKey> { code.vkey });
            }

            IsInitialized = true;
        }

        public static bool KeyPressed(VirtualKey key)
        {
            return Input.GetKeyDown((KeyCode)key);
        }

        public static bool KeyHold(VirtualKey key)
        {
            return Input.GetKey((KeyCode)key);
        }

        public static bool KeyReleased(VirtualKey key)
        {
            return Input.GetKeyUp((KeyCode)key);
        }

        public static bool KeyPressed(string key)
        {
            return Input.GetKeyDown((KeyCode)AttachedKeycodeDefinitions.GetElement<Keycode>(key).vkey);
        }

        public static bool KeyHold(string key)
        {
            return Input.GetKey((KeyCode)AttachedKeycodeDefinitions.GetElement<Keycode>(key).vkey);
        }

        public static bool KeyReleased(string key)
        {
            return Input.GetKeyUp((KeyCode)AttachedKeycodeDefinitions.GetElement<Keycode>(key).vkey);
        }

        public static bool OnInput(InputEvent purpose)
        {
            var validInput = VKPurposeMap.ContainsKey(purpose);
            if (validInput == false) return validInput;
            
            var vkList = VKPurposeMap[purpose];
            if (vkList.Count == 1) return KeyHold(VKPurposeMap[purpose].First());

            foreach(var vk in vkList)
            {
                if (KeyHold(vk)) return true;
            }

            return false;
        }

        public static bool OnInputActive(InputEvent purpose)
        {
            var validInput = VKPurposeMap.ContainsKey(purpose);
            if (validInput == false) return validInput;

            var vkList = VKPurposeMap[purpose];
            if (vkList.Count == 1) return KeyPressed(VKPurposeMap[purpose].First());

            foreach (var vk in vkList)
            {
                if (KeyPressed(vk)) return true;
            }

            return false;
        }

        public static bool OnInputRelease(InputEvent purpose)
        {
            var validInput = VKPurposeMap.ContainsKey(purpose);
            if (validInput == false) return validInput;

            var vkList = VKPurposeMap[purpose];
            if (vkList.Count == 1) return KeyReleased(VKPurposeMap[purpose].First());

            foreach (var vk in vkList)
            {
                if (KeyReleased(vk)) return true;
            }

            return false;
        }
    }
}
