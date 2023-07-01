using System.Collections;
using TMPro;

namespace XVNML2U
{
    interface ITextMotion
    {
        public virtual float Duration
        {
            get
            {
                return 1f;
            }
        }

        TextMeshProUGUI TMP_Text { get; set; }

        void OnMotionStart();
        void OnMotion();
        void OnMotionEnd();

        internal IEnumerator TextMotionCycle();
    }
}
