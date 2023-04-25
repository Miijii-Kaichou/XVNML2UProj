using System;
using System.Linq;
using UnityEngine;
using XVNML2U.Mono;
using XVNML2U.Mono.Singleton;

namespace XVNML2U
{
    public sealed class XVNMLHandler : Singleton<XVNMLHandler>
    {
        [SerializeField]
        private XVNMLAsset[] xvnmlList;

        public XVNMLAsset GetXVNML(int index)
        {
            return xvnmlList[index];
        }

        public XVNMLAsset GetXVNML(ReadOnlySpan<char> name)
        {
            var namestring = name.ToString();
            return xvnmlList
                .Where(xvnml => xvnml.name.Equals(namestring))
                .FirstOrDefault();
        }
    }
}