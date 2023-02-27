using TMPro;
using UnityEngine;
using XVNML2U;

namespace XVNML2U.Mono
{
    public class BasicVNObject : MonoBehaviour
    {
        [SerializeField]
        XVNMLModule XVNMLModule;

        [SerializeField]
        public GameObject dialogueView;

        [SerializeField]
        public GameObject promptView;

        [SerializeField]
        public GameObject controlView;
    } 
}
