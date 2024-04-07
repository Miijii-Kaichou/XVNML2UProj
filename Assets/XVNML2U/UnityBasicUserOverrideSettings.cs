using UnityEngine;
using XVNML.Core.Tags.UserOverrides;
using XVNML.Utilities.Tags;
using XVNML2U;

public class UnityBasicUserOverrideSettings : MonoBehaviour, IUserOverrideSettings
{ 
    public void Apply()
    {
        UserOverrideManager.IncludeAsAllowedFlag<Dialogue>("enableMigration");
        UserOverrideManager.IncludeAsAllowedFlag<Dialogue>("enableGroupMigration");
        UserOverrideManager.IncludeAsAllowedFlag<Dialogue>("collectFiles");
        UserOverrideManager.IncludeAsAllowedFlag<Dialogue>("textSpeedIsControlledExternally");

        UserOverrideManager.IncludeAsAllowedFlag<DialogueGroup>("enableMigration");
        UserOverrideManager.IncludeAsAllowedFlag<DialogueGroup>("enableGroupMigration");
        UserOverrideManager.IncludeAsAllowedFlag<DialogueGroup>("collectFiles");
        UserOverrideManager.IncludeAsAllowedFlag<DialogueGroup>("actAsSceneController");
    }
}
