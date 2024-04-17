using UnityEngine;
using XVNML.Core.Tags;
using XVNML.Utilities.Tags;
using XVNML2U;

[AssociateWithTag("gameDataGroup", typeof(Proxy), TagOccurance.PragmaOnce, true)]
public sealed class GameDataGroupTag : UserDefined
{
	protected override string[] AllowedParameters => new[]
	{
		nameof(saveFilePath),
	};

	protected override string[] AllowedFlags => new[]
	{
		nameof(isMutable)
	};

	
	// Create your tag members here...
	public string saveFilePath;
    public bool isMutable;

	private GameDataTag[] _gameData;

    public override void OnResolve(string fileOrigin)
	{
		// Always call this, or default property for tag will not function
		base.OnResolve(fileOrigin);
		
		saveFilePath = GetParameterValue<string>(nameof(saveFilePath));
		isMutable = HasFlag(nameof(isMutable));

		_gameData = Collect<GameDataTag>();
	}

	public void Save()
	{
		if (!isMutable) return;
		_gameData.DoForEvery(data => data.WriteValueToPlayerPrefs());
        PlayerPrefs.Save();
        Debug.Log("Game Data Saved");
	}

	public void Load()
	{
		if (!isMutable) return;
		_gameData.DoForEvery(data => data.ReadValueFromPlayerPrefs());
        Debug.Log("Loading Game Data");
	}
}