using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XVNML.Input.Enums;
using XVNML.Utilities.Tags;

namespace XVNML2U.Mono
{
    public sealed class XVNMLInputManager : Singleton<XVNMLInputManager>
    {
        private static readonly Dictionary<XVNMLModule, SortedDictionary<InputEvent, List<VirtualKey>>> VKPurposeMap = new();
        private static readonly Dictionary<XVNMLModule, KeycodeDefinitions> AttachedKeycodeDefinitions = new();

        public static bool IsInitialized = false;

        public static void Init(XVNMLModule module)
        {
            if (IsNull) return;
            if (IsInitialized) return;

            var root = module.Root;

            KeycodeDefinitions def = root.GetElement<KeycodeDefinitions>();
            if (def == null)
            {
                Debug.LogWarning("There are now <keycodeDefinitions> element");
                return;
            }

            Keycode[] keycodes = root?.GetElement<KeycodeDefinitions>().KeyCodes;
            if (keycodes.Length == 0)
            {
                Debug.LogWarning("There are no <keycode> elements defined.");
                return;
            }

            AttachedKeycodeDefinitions[module] = def;

            VKPurposeMap.Add(module, new SortedDictionary<InputEvent, List<VirtualKey>>());

            SortedDictionary<InputEvent, List<VirtualKey>> targetInputKeyPairs = VKPurposeMap[module];

            for (int i = 0; i < keycodes.Length; i++)
            {
                Keycode code = keycodes[i];

                if (targetInputKeyPairs.ContainsKey(code.purpose))
                {
                    targetInputKeyPairs[code.purpose].Add(code.vkey);
                    continue;
                }

                targetInputKeyPairs.Add(code.purpose, new List<VirtualKey> { code.vkey });
            }

            IsInitialized = true;
        }

        public static bool KeyPressed(XVNMLModule module, VirtualKey key)
        {
            if (VKPurposeMap.ContainsKey(module) == false) return false;
            var modulesWithKey = VKPurposeMap[module].Where(ivp => ivp.Value.Contains(key));
            return modulesWithKey.Any() && Input.GetKeyDown((KeyCode)key);
        }

        public static bool KeyHold(XVNMLModule module, VirtualKey key)
        {
            if (VKPurposeMap.ContainsKey(module) == false) return false;
            var modulesWithKey = VKPurposeMap[module].Where(ivp => ivp.Value.Contains(key));
            return modulesWithKey.Any() && Input.GetKey((KeyCode)key);
        }

        public static bool KeyReleased(XVNMLModule module, VirtualKey key)
        {
            if (VKPurposeMap.ContainsKey(module) == false) return false;
            var modulesWithKey = VKPurposeMap[module].Where(ivp => ivp.Value.Contains(key));
            return modulesWithKey.Any() && Input.GetKeyUp((KeyCode)key);
        }

        public static bool KeyPressed(XVNMLModule module, string key)
        {
            if (VKPurposeMap.ContainsKey(module) == false) return false;
            return Input.GetKeyDown((KeyCode)AttachedKeycodeDefinitions[module].GetElement<Keycode>(key).vkey);
        }

        public static bool KeyHold(XVNMLModule module, string key)
        {
            if (VKPurposeMap.ContainsKey(module) == false) return false;
            return Input.GetKey((KeyCode)AttachedKeycodeDefinitions[module].GetElement<Keycode>(key).vkey);
        }

        public static bool KeyReleased(XVNMLModule module, string key)
        {
            if (VKPurposeMap.ContainsKey(module) == false) return false;
            return Input.GetKeyUp((KeyCode)AttachedKeycodeDefinitions[module].GetElement<Keycode>(key).vkey);
        }

        public static bool OnInput(XVNMLModule module, InputEvent purpose)
        {
            if (VKPurposeMap.ContainsKey(module) == false) return false;
            SortedDictionary<InputEvent, List<VirtualKey>> targetInputKeyPairs = VKPurposeMap[module];
            var validInput = VKPurposeMap[module].ContainsKey(purpose);
            if (validInput == false) return validInput;
            
            var vkList = VKPurposeMap[module][purpose];
            if (vkList.Count == 1)
            {
                return KeyHold(module, targetInputKeyPairs[purpose].First());
            }

            foreach (var vk in vkList)
            {
                if (KeyHold(module, vk)) return true;
            }

            return false;
        }

        public static bool OnInputActive(XVNMLModule module, InputEvent purpose)
        {
            if (VKPurposeMap.ContainsKey(module) == false) return false;
            SortedDictionary<InputEvent, List<VirtualKey>> targetInputKeyPairs = VKPurposeMap[module];
            var validInput = VKPurposeMap[module].ContainsKey(purpose);
            if (validInput == false) return validInput;

            var vkList = VKPurposeMap[module][purpose];
            if (vkList.Count == 1)
            {
                return KeyPressed(module, targetInputKeyPairs[purpose].First());
            }

            foreach (var vk in vkList)
            {
                if (KeyPressed(module, vk)) return true;
            }

            return false;
        }

        public static bool OnInputRelease(XVNMLModule module, InputEvent purpose)
        {
            if (VKPurposeMap.ContainsKey(module) == false) return false;
            SortedDictionary<InputEvent, List<VirtualKey>> targetInputKeyPairs = VKPurposeMap[module];
            var validInput = VKPurposeMap[module].ContainsKey(purpose);
            if (validInput == false) return validInput;

            var vkList = VKPurposeMap[module][purpose];
            if (vkList.Count == 1)
            {
                return KeyReleased(module, targetInputKeyPairs[purpose].First());
            }

            foreach (var vk in vkList)
            {
                if (KeyReleased(module, vk)) return true;
            }

            return false;
        }
    }
}
