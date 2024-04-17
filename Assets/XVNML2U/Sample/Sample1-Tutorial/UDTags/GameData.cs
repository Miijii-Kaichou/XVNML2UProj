using System;
using UnityEngine;
using XVNML.Core.Extensions;
using XVNML.Core.Tags;
using XVNML.Utilities.Tags;
using XVNML2U;

[AssociateWithTag("gameData", typeof(GameDataGroupTag), TagOccurance.Multiple, true)]
public sealed class GameDataTag : UserDefined
{
	protected override string[] AllowedParameters => new[]
	{
		nameof(key),
		nameof(value),
		nameof(valueType)
	};
	
	// Create your tag members here...
	public string key;
	public string valueType;

	private Type type;

	public override void OnResolve(string fileOrigin)
	{
		// Always call this, or default property for tag will not function
		base.OnResolve(fileOrigin);
		
		key = GetParameterValue<string>(nameof(key));
		type = value.DetermineValueType();
	}

	public void WriteValueToPlayerPrefs()
	{
		if (type.Equals(typeof(int)))
		{
			PlayerPrefs.SetInt(key, value.ToInt());
			return;
		}

        if (type.Equals(typeof(float)))
        {
			PlayerPrefs.SetFloat(key, value.ToFloat());
            return;
        }

        if (type.Equals(typeof(string)))
        {
			PlayerPrefs.SetString(key, value.ToString());
            return;
        }
    }

	public void ReadValueFromPlayerPrefs()
	{
        if (type.Equals(typeof(int)))
        {
            PlayerPrefs.GetInt(key);
            return;
        }

        if (type.Equals(typeof(float)))
        {
            PlayerPrefs.GetFloat(key);
            return;
        }

        if (type.Equals(typeof(string)))
        {
            PlayerPrefs.GetString(key);
            return;
        }
    }
}