using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XVNML.Core.Dialogue.Structs;
using XVNML.XVNMLUtility.Tags;

namespace XVNML2U
{
    public class CastController : MonoBehaviour
    {
        private SortedDictionary<string, CastEntity> castObjectMap = new();

        //Data
        private List<Cast> _castMembers = new();

        internal void ChangeExpression(CastInfo castInfo)
        {
            var target = castObjectMap[castInfo.name];
            if (target == null) return;
            target.ChangeExpression(castInfo.expression);
        }

        internal void ChangeVoice(CastInfo castInfo)
        {
            var target = castObjectMap[castInfo.name];
            if (target == null) return;
            target.ChangeVoice(castInfo.voice);
        }

        internal void Init(Cast[] castMembers)
        {
            for(int i = 0; i < castMembers.Length; i++)
            {
                if (castMembers[i] == null) return;
                if (castObjectMap.ContainsKey(castMembers[i].TagName) == false) return;
                _castMembers.Add(castMembers[i]);
                castObjectMap[castMembers[i].TagName].Construct(castMembers[i]);
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            var objs = GetComponentsInChildren<CastEntity>();
            foreach (var obj in objs)
            {
                castObjectMap.Add(obj.associateWithName, obj);
            }
        }
    }
}