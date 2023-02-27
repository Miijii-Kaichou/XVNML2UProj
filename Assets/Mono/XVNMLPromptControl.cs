using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XVNML.Core.Dialogue;
using XVNML.XVNMLUtility.Tags;

namespace XVNML2U.Mono
{
    public class XVNMLPromptControl : MonoBehaviour
    {
        [SerializeField]
        VerticalLayoutGroup content;

        public void SetPrompts(DialogueLine prompt)
        {
            if (prompt.Mode is not XVNML.Core.Dialogue.Enums.DialogueLineMode.Prompt) return;

        }
    }
}