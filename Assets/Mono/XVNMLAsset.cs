using UnityEngine;
using XVNML.XVNMLUtility;

[CreateAssetMenu(fileName = "New XVNML Asset", menuName = "XVNML/XVNML Asset")]
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
    public string filePath;

    public Object asset;

    public XVNMLObj root;

    public string content;
}