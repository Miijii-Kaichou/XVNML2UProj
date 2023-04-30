using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XVNML.Core.Dialogue.Structs;
using XVNML.XVNMLUtility.Tags;

namespace XVNML2U
{
    public class CastController : MonoBehaviour
    {
        private SortedDictionary<string, CastEntity> castObjectMap = new();

        internal void ChangeExpression(CastInfo castInfo)
        {
            string castName = castInfo.name;
            int id = -1;

            if (castObjectMap.ContainsKey(castName) == false) return;
            var target = castObjectMap[castName];

            if (target == null) return;

            try
            {

                id = castInfo.expression == null ? -1 : Convert.ToInt32(castInfo.expression);
            }
            catch
            {
                target.ChangeExpression(castInfo.expression);
            }
            finally
            {
                if (id != -1) target.ChangeExpression(id);
            }
        }

        internal void ChangeVoice(CastInfo castInfo)
        {
            string castName = castInfo.name;
            int id = -1;

            if (castObjectMap.ContainsKey(castName) == false) return;
            var target = castObjectMap[castName];

            if (target == null) return;

            try
            {
                id = castInfo.voice == null ? -1 : Convert.ToInt32(castInfo.voice);
            }
            catch
            {
                target.ChangeVoice(castInfo.voice);
            }
            finally
            {
                if (id != -1) target.ChangeVoice(id);
            }
        }

        internal void Init(Cast[] castMembers)
        {
            for (int i = 0; i < castMembers.Length; i++)
            {
                if (castMembers[i] == null) return;
                if (castObjectMap.ContainsKey(castMembers[i].TagName) == false) return;
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