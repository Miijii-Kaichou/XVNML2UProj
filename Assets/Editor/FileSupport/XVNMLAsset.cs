using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using XVNML.Utilities;

#nullable enable
namespace XVNML2U.FileSupport
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

        private const string IconPath = "Assets/Editor/Editor Default Resources/Icons/xvnml_file_icon.png";
        private int m_InstanceID;

        /// <summary>
        /// HashCode based on the name of the asset.
        /// </summary>
        public int HashCode => (m_InstanceID + filePath).GetHashCode();

        /// <summary>
        /// Original file path of this asset
        /// </summary>A
        [HideInInspector] public string? filePath;

        [HideInInspector] public XVNMLObj? top;

        [HideInInspector] public string? content;

        private void OnEnable()
        {
            OnValidate();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
            EditorGUIUtility.SetIconForObject(this, icon);
            filePath = AssetDatabase.GetAssetPath(this);
#endif
        }

        public void Build(Action<XVNMLObj>? onBuildFinished, bool allowCacheUsageAndGeneration = false)
        {
            if (allowCacheUsageAndGeneration == false)
            {
                XVNMLObj.Create(filePath!, top =>
                {
                    this.top = top;
                    onBuildFinished?.Invoke(top!);
                }, allowCacheUsageAndGeneration);
                return;
            }

            XVNMLObj.UseOrCreate(filePath!, top =>
            {
                this.top = top;
                onBuildFinished?.Invoke(top!);
            });
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public void ReadContentFromFile()
        {
            using StreamReader streamReader = new(filePath);
            content = streamReader
            .ReadToEnd()
            .Replace("\r", string.Empty);
            
            streamReader.Close();
        }

        public void OverrideContent(string overrideContent)
        {
            using StreamWriter streamWriter = new(filePath);
            content = overrideContent;
            streamWriter.Write(content);
            streamWriter.Close();
        }
    } 
}
#nullable disable