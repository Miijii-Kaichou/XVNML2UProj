using System;
using System.IO;
using UnityEditor;
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

        private const string IconPath = "Assets/Editor Default Resources/Icons/xvnml_file_icon.png";
        private int m_InstanceID;

        /// <summary>
        /// HashCode based on the name of the asset.
        /// </summary>
        private int _hashCode;
        public int HashCode => (m_InstanceID + filePath).GetHashCode();

        /// <summary>
        /// Original file path of this asset
        /// </summary>A
        [HideInInspector] public string filePath;

        [HideInInspector] public XVNMLObj top;

        public string content;

        public Action<XVNMLObj> assetBuildCompleted;

        private void OnValidate()
        {
        #if UNITY_EDITOR
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
            EditorGUIUtility.SetIconForObject(this, icon);
        #endif
        }

        public void Build()
        {
            top = XVNMLObj.Create(filePath);
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        internal void ReadContentFromFile()
        {
            using StreamReader streamReader = new StreamReader(filePath);
            content = streamReader.ReadToEnd();
        }
    } 
}