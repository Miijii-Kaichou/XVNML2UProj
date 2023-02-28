using UnityEngine;
using XVNML.XVNMLUtility;

namespace XVNML2U.Mono
{
    public sealed class XVNMLAsset : ScriptableObject
    {
        /// <summary>
        /// Instance ID of the XVNML Asset
        /// </summary>
        public int InstanceID
        {
            get
            {
                if (m_InstanceID == 0)
                    m_InstanceID = GetInstanceID();

                return m_InstanceID;
            }
        }
        private int m_InstanceID;

        /// <summary>
        /// HashCode based on the name of the asset.
        /// </summary>
        public int hashCode;

        /// <summary>
        /// Original file path of this asset
        /// </summary>
        [HideInInspector]public string filePath;

        [HideInInspector] public XVNMLObj top;

        [HideInInspector] public string content;

        public void Build()
        {
            top = XVNMLObj.Create(filePath);
        }
    } 
}