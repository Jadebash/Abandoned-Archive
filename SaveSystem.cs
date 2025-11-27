using System.IO;
using UnityEngine;

public static class SaveSystem
{
	public static void SaveGame(Save newSave)
	{
		if (newSave != null)
		{
			string contents = SaveToString(newSave);
			File.WriteAllText(Application.persistentDataPath + "/save.json", contents);
		}
	}

	public static Save LoadGame(string saveName = "save")
	{
		string text = Application.persistentDataPath + "/" + saveName + ".json";
		if (File.Exists(text))
		{
			return JsonUtility.FromJson<Save>(File.ReadAllText(Application.persistentDataPath + "/" + saveName + ".json"));
		}
		Debug.Log("Save file not found at " + text);
		return null;
	}

	public static string SaveToString(Save save)
	{
		return JsonUtility.ToJson(save);
	}
}
