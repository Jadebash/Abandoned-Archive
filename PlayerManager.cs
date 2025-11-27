using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
	public static PlayerManager Instance;

	public InputSystemUIInputModule inputSystem;

	public GameObject ui;

	private List<GameObject> playersList = new List<GameObject>();

	[HideInInspector]
	public PlayerInput lastActivePlayerInput;

	public static GameObject[] players
	{
		get
		{
			if (Instance == null)
			{
				return new GameObject[0];
			}
			return Instance.playersList.ToArray();
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public static GameObject ClosestPlayer(Vector3 position)
	{
		if (Instance == null)
		{
			return null;
		}
		GameObject result = null;
		float num = float.MaxValue;
		GameObject[] array = players;
		foreach (GameObject gameObject in array)
		{
			float num2 = Vector3.Distance(gameObject.transform.position, position);
			if (num2 < num)
			{
				num = num2;
				result = gameObject;
			}
		}
		return result;
	}

	public SpellCasting ClosestPlayerSpellCaster(Vector3 position)
	{
		GameObject gameObject = null;
		float num = float.MaxValue;
		GameObject[] array = players;
		foreach (GameObject gameObject2 in array)
		{
			float num2 = Vector3.Distance(gameObject2.transform.position, position);
			if (num2 < num)
			{
				num = num2;
				gameObject = gameObject2;
			}
		}
		return gameObject.GetComponent<SpellCasting>();
	}

	public void AddNewPlayer(PlayerInput player)
	{
		if (players.Length == 0)
		{
			PlayerInputManager.instance.DisableJoining();
		}
		if (SaveManager.Instance != null && SaveManager.Instance.currentSave != null && !string.IsNullOrEmpty(SaveManager.Instance.currentSave.keybindOverrides))
		{
			player.actions.LoadBindingOverridesFromJson(SaveManager.Instance.currentSave.keybindOverrides);
		}
		playersList.Add(player.gameObject);
		Health component = player.GetComponent<Health>();
		SceneManager.MoveGameObjectToScene(player.gameObject, base.gameObject.scene);
		if (component == null)
		{
			Debug.LogError("CRITICAL: Player " + player.gameObject.name + " is missing Health component! This will cause game-breaking issues. Player cannot take damage, die, or function properly.");
		}
		else
		{
			PlayerInput[] array = Object.FindObjectsOfType<PlayerInput>();
			for (int i = 0; i < array.Length; i++)
			{
				Health component2 = array[i].GetComponent<Health>();
				if (component2 != null && component2 != component)
				{
					component2.forceSyncHealths.Add(component);
					component.forceSyncHealths.Add(component2);
				}
			}
		}
		if (player.gameObject.name.Contains("Clone"))
		{
			SpriteRenderer component3 = player.transform.Find("Model").GetComponent<SpriteRenderer>();
			if (component3 != null)
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				component3.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat("_OutlineThickness", 1f);
				float num = 0f;
				float num2 = 0f;
				switch (Random.Range(0, 3))
				{
				case 0:
					num = 0f;
					num2 = 1f;
					break;
				case 1:
					num = 1f;
					num2 = 0f;
					break;
				}
				Color value = new Color(1.5f, num * 1.5f, num2 * 1.5f, 1f);
				materialPropertyBlock.SetColor("_OutlineColor", value);
				component3.SetPropertyBlock(materialPropertyBlock);
			}
		}
		TipManager.HideTip();
	}

	public void SetUIControlAuthority(GameObject player)
	{
		Debug.Log("Setting Control Authority to " + player.name);
		inputSystem.actionsAsset = player.GetComponent<PlayerInput>().actions;
		lastActivePlayerInput = player.GetComponent<PlayerInput>();
	}

	public void RemovePlayer(PlayerInput player)
	{
		playersList.Remove(player.gameObject);
	}

	public void ClearPlayers()
	{
		playersList.Clear();
	}
}
