using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XVNML.Core.Dialogue;

namespace XVNML2U.Mono
{
    [DisallowMultipleComponent]
    public class XVNMLPromptControl : MonoBehaviour
    {
        [SerializeField]
        VerticalLayoutGroup content;

        // Buttons
        private ResponseControl[] Buttons;

        internal void SetPrompts(DialogueWriterProcessor sender)
        {
            content.gameObject.SetActive(true);

            Buttons ??= content.GetComponentsInChildren<ResponseControl>();

            Clear();
           
            var responses = sender.FetchPrompts().Keys.ToArray();
            if (Buttons == null) return;
            for(int i = 0; i < responses.Length; i++)
            {
                var button = Buttons[i];
                var response = responses[i];

                if (button.gameObject.activeInHierarchy == false)
                {
                    button.gameObject.SetActive(true);
                    SetButton(button, response, i, () => { sender.JumpToStartingLineFromResponse(response); });
                }
            }
        }

        internal void Clear()
        {
            foreach(var control in Buttons)
            {
                control.Clear();
                control.onClick = null;
                control.gameObject.SetActive(false);
            }
        }

        private void SetButton(ResponseControl responseControl, string response, int index, Action onClick)
        {
            responseControl.SetText(response);
            responseControl.AssignIndex(index);
            responseControl.onClick += onClick;
        }

        internal void SetContent(VerticalLayoutGroup vlg)
        {
            content = vlg;
        }
    }
}