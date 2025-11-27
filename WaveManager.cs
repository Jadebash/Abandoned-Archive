using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
	public delegate void EnteredRoomCallback(DungeonRoom room);

	public delegate void WalkedRoomCallback(Transform room);

	public static WaveManager Instance;

	public static bool preventAutoSpellGive;

	private GameObject player;

	[HideInInspector]
	public bool startedWaves;

	[HideInInspector]
	public bool playerInRoom;

	public bool giveSpell;

	[HideInInspector]
	public DungeonRoom currentRoom;

	private float enteredRoomTimer;

	public float calmMusicAfter;

	public event EnteredRoomCallback OnEnteredRoom;

	public event WalkedRoomCallback OnWalkedRoom;

	private void Awake()
	{
		Instance = this;
		SceneManager.sceneLoaded += LoadedScene;
	}

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null && giveSpell && !preventAutoSpellGive && (FloorManager.Instance == null || FloorManager.Instance.currentLoop == 1))
		{
			GivePlayerSpell();
		}
		NotifySpellCastingInstances();
	}

	private void NotifySpellCastingInstances()
	{
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			if (instance != null)
			{
				instance.OnWaveManagerReady(this);
			}
		}
	}

	private void LoadedScene(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (scene.name == "Player")
		{
			player = GameObject.FindGameObjectWithTag("Player");
			if (giveSpell && !preventAutoSpellGive)
			{
				GivePlayerSpell();
			}
		}
	}

	private void GivePlayerSpell()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spell_Pickup", player.transform.position);
		Spell spell = null;
		if (FloorManager.Instance != null && !string.IsNullOrEmpty(FloorManager.Instance.startingSpellId))
		{
			spell = RunManager.GetSpellByID(FloorManager.Instance.startingSpellId);
			if (spell != null)
			{
				spell = Object.Instantiate(spell);
				Debug.Log("Giving same spell as before: " + FloorManager.Instance.startingSpellId);
			}
		}
		if (spell == null)
		{
			spell = RunManager.GetSpell(forceMain: false, forceStartingSpell: true);
			spell = Object.Instantiate(spell);
			if (spell != null && FloorManager.Instance != null)
			{
				FloorManager.Instance.startingSpellId = spell.GetType().ToString();
				Debug.Log("Getting new random spell: " + FloorManager.Instance.startingSpellId);
			}
		}
		player.GetComponent<SpellCasting>().stances[0].spellSlots[1].SetNewSpell(spell);
		giveSpell = false;
	}

	private void Update()
	{
		if (!playerInRoom)
		{
			enteredRoomTimer += Time.deltaTime;
			if (enteredRoomTimer > calmMusicAfter)
			{
				MusicManager.Calm();
			}
		}
	}

	public void EnteredRoom(DungeonRoom room)
	{
		currentRoom = room;
		playerInRoom = true;
		this.OnEnteredRoom?.Invoke(room);
		enteredRoomTimer = 0f;
	}

	public void WalkedRoom(Transform room)
	{
		this.OnWalkedRoom?.Invoke(room);
	}

	public void ExitedRoom(DungeonRoom room)
	{
		currentRoom = null;
		playerInRoom = false;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
