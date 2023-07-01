using System.Collections;
using TMPro;
using UnityEngine;
using XVNML2U.Mono;

namespace XVNML2U
{
    public abstract class BaseTextMotion : ITextMotion
    {
        private ITextMotion Instance => this;

        protected string Name => GetType().Name.TakeOff("TextMotion");

        public virtual float Duration => Instance.Duration;

        [SerializeField]
        private TextMeshProUGUI tmp_text;
        public TextMeshProUGUI TMP_Text
        {
            get { return tmp_text; }
            set { tmp_text = value; }
        }

        protected BaseTextMotion()
        {
            TextMotionRegistry.Register(this);
        }

        public void DoTextMotion()
        {
            CoroutineHandler.Execute(Instance.TextMotionCycle());
        }
        
        public string GetName() => Name;

        public virtual void OnMotion() { }
        public virtual void OnMotionStart() { }
        public virtual void OnMotionEnd() { }

        IEnumerator ITextMotion.TextMotionCycle()
        {
            var time = 0f;
            var endTime = Duration;

            OnMotionStart();

            while (time < endTime)
            {
                time += Time.deltaTime;

                OnMotion();

                yield return null;
            }

            OnMotionEnd();
        }
    }
}
