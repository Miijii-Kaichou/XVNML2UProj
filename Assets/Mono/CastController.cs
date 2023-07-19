using System;
using System.Collections.Generic;

using UnityEngine;

using XVNML.Core.Dialogue;
using XVNML.Core.Dialogue.Structs;
using XVNML.Utilities.Tags;
using XVNML2U.Data;
using XVNML2U.Mono;

using DG.Tweening;

namespace XVNML2U.Mono
{
    public sealed class CastController : Singleton<CastController>
    {
        private SortedDictionary<string, CastEntity> castObjectMap = new();
        private bool _isInitialized = false;

        public CastMotionType CastMotion { get; private set; }
        public EnterSide CastEntersFrom { get; private set; }
        public float CastMotionDuration { get; private set; }

        private const int DefaultXPos = -1325;

        internal void ChangeExpression(CastInfo castInfo)
        {
            string castName = castInfo.name;

            if (castObjectMap.ContainsKey(castName) == false) return;

            var target = castObjectMap[castName];

            if (target == null) return;
            if (castInfo.expression == null) return;

            if (char.IsDigit(castInfo.expression[0]) && castInfo.expression.Length == 1)
            {
                int id = Convert.ToInt32(castInfo.expression);
                target.ChangeExpression(id);
                return;
            }

            target.ChangeExpression(castInfo.expression);
        }

        internal void ChangeVoice(CastInfo castInfo)
        {
            string castName = castInfo.name;

            if (castObjectMap.ContainsKey(castName) == false) return;

            var target = castObjectMap[castName];

            if (target == null) return;
            if (castInfo.voice == null) return;

            if (char.IsDigit(castInfo.voice[0]) && castInfo.voice.Length == 1)
            {
                int id = Convert.ToInt32(castInfo.voice);
                target.ChangeVoice(id);
                return;
            }

            target.ChangeVoice(castInfo.voice);
        }

        internal void PositionCast(DialogueWriterProcessor process, string name, Anchoring anchor, int offset)
        {
            if (castObjectMap.ContainsKey(name) == false) return;

            var target = castObjectMap[name];

            XVNMLDialogueControl control = DialogueProcessAllocator.ProcessReference[process.ID];

            var width = control.DOMWidth;
            var height = control.DOMHeight;

            var initialXPos = CastEntersFrom == EnterSide.Right ? DefaultXPos * -1 : DefaultXPos;
            var xPos = EvaluateAnchor(anchor, offset, width, height);

            control.SendNewAction(() =>
            {
                var transform = target.transform;
                
                target.transform.localPosition  = new Vector2(initialXPos, transform.localPosition.y);
                
                var startPosition = transform.localPosition;
                var targetPosition = new Vector2(xPos, transform.localPosition.y);

                if (CastMotion == CastMotionType.Instant) target.transform.localPosition = targetPosition;
                if (CastMotion == CastMotionType.Interpolation) target.transform.DOLocalPath(
                    new Vector3[2]
                    {
                        startPosition,
                        targetPosition
                    }, CastMotionDuration, pathMode: PathMode.Sidescroller2D);

                return WCResult.Ok();
            });
        }

        internal void PositionCast(DialogueWriterProcessor process, string name, int offset)
        {
            if (castObjectMap.ContainsKey(name) == false) return;

            var target = castObjectMap[name];

            XVNMLDialogueControl control = DialogueProcessAllocator.ProcessReference[process.ID];

            var width = control.DOMWidth;
            var height = control.DOMHeight;

            control.SendNewAction(() =>
            {
                var transform = target.transform;
                var startPosition = transform.localPosition;
                var targetPosition = new Vector2(offset, transform.localPosition.y);

                if (CastMotion == CastMotionType.Instant) target.transform.localPosition = targetPosition;
                if (CastMotion == CastMotionType.Interpolation) target.transform.DOLocalPath(
                    new Vector3[2]
                    {
                        startPosition,
                        targetPosition
                    }, CastMotionDuration, pathMode: PathMode.Sidescroller2D);
                return WCResult.Ok();
            });
        }

        private int EvaluateAnchor(Anchoring anchor, int offset, int width, int height)
        {
            var hPosition = -1;
            switch (anchor)
            {
                case Anchoring.Left:
                    hPosition -= width / 3;
                    break;
                case Anchoring.Center:
                    hPosition = 0;
                    break;
                case Anchoring.Right:
                    hPosition += width / 3;
                    break;
                default:
                    hPosition = width;
                    break;
            }
            return hPosition + offset;
        }

        internal void Init(Cast[] castMembers)
        {
            if (_isInitialized) return;

            var castEntityObjects = GetComponentsInChildren<CastEntity>();

            for (int i = 0; i < castMembers.Length; i++)
            {
                if (castMembers[i] == null) return;

                var castMember = castMembers[i];
                var castMemberName = castMember.TagName;
                var castEntity = castEntityObjects[i];

                castObjectMap.Add(castMemberName, castEntity);
                castObjectMap[castMemberName].Construct(castMember);
                castObjectMap[castMemberName].ChangeExpression(0);
            }

            _isInitialized = true;
        }

        internal void SetCastMotion(CastMotionType castMotionType)
        {
            CastMotion = castMotionType;
        }

        internal void SetCastMotionDuration(float duration)
        {
            CastMotionDuration = duration;
        }

        internal void HaveCastEnterFrom(EnterSide side)
        {
            CastEntersFrom = side;
        }

        internal static CastEntity Use(string name) => Instance.castObjectMap[name];
    }
}