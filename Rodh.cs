using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;

public class Rodh : MonoBehaviour
{
	public enum RoomState
	{
		Square = 0,
		Diamond = 1,
		Triangle = 2
	}

	private GameObject endingFloor;

	public GameObject room;

	public Vector3 roomPos;

	public Vector3 camPos;

	public Vector3 startPos;

	[HideInInspector]
	public Animator roomAnim;

	public List<GameObject> spearGlow;

	public List<GameObject> waterGlow;

	public GameObject spear;

	public Dialogue[] fightMessages;

	public GameObject greenLaser;

	public GameObject emeraldCircle;

	public GameObject largeEmeraldCircle;

	public GameObject greenBubble;

	public GameObject growingTriangle;

	public GameObject blueWisp;

	public GameObject water;

	public List<LocalisedString> speeches;

	private List<GameObject> textBoxes;

	public RoomState currentRoom;

	[HideInInspector]
	public bool ended;

	private List<AttackTelegraph> activeTelegraphs = new List<AttackTelegraph>();

	public AttackHelper attackHelper;

	private void Start()
	{
		textBoxes = new List<GameObject>();
		currentRoom = RoomState.Square;
		base.transform.parent = null;
		GameObject.Find("EndingFloor").SetActive(value: false);
		room = Object.Instantiate(room, roomPos, Quaternion.identity);
		startPos = room.transform.position + new Vector3(0f, 10f, 0f);
		base.transform.position = startPos;
		attackHelper = GetComponent<AttackHelper>();
		if (attackHelper == null)
		{
			attackHelper = base.gameObject.AddComponent<AttackHelper>();
		}
		attackHelper.FindPlayableSpaceBounds();
		GetComponent<Health>().OnDeath += Death;
		GameObject[] players = PlayerManager.players;
		for (int i = 0; i < players.Length; i++)
		{
			Movement component = players[i].GetComponent<Movement>();
			if (component != null)
			{
				component.SafeTeleport(room.transform.position);
				component.canTeleport = false;
			}
		}
		CameraController.Instance.targets.Add(room.transform);
		roomAnim = room.GetComponent<Animator>();
		foreach (Transform item in room.transform.Find("SpearGlows"))
		{
			spearGlow.Add(item.gameObject);
			item.gameObject.SetActive(value: false);
		}
		foreach (Transform item2 in room.transform.Find("WaterGlows"))
		{
			waterGlow.Add(item2.gameObject);
			item2.gameObject.SetActive(value: false);
		}
		foreach (Transform item3 in room.transform.Find("Speech"))
		{
			item3.gameObject.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
			textBoxes.Add(item3.gameObject);
		}
		RuntimeManager.StudioSystem.setParameterByName("Intensity", 100f);
	}

	private void Death(GameObject attacker)
	{
		ended = true;
		roomAnim.SetBool("Ending", value: true);
		GetComponent<Animator>().SetTrigger("Ending");
		roomAnim.gameObject.transform.position = new Vector3(0f, 0f, 0f);
		base.transform.position = room.transform.position;
		GameObject[] players = PlayerManager.players;
		for (int i = 0; i < players.Length; i++)
		{
			Movement component = players[i].GetComponent<Movement>();
			if (component != null)
			{
				component.canTeleport = true;
				component.SafeTeleport(room.transform.position - new Vector3(0f, 3f, 0f));
			}
		}
		BossManager.Instance.EndBossFight();
	}

	private void Update()
	{
		CameraController.Instance.gameObject.transform.GetChild(0).GetComponent<Camera>().orthographicSize = 10f;
		activeTelegraphs.RemoveAll((AttackTelegraph item) => item == null);
	}

	public void StartEnding()
	{
		Ending.Instance.KillRodhEnding();
	}

	public void startSpeech()
	{
		StartCoroutine(doSpeech());
		Debug.Log("StartSpeech");
	}

	private IEnumerator doSpeech()
	{
		if (speeches == null || speeches.Count == 0)
		{
			yield break;
		}
		int randSpeech = Random.Range(0, speeches.Count);
		if (randSpeech < 0 || randSpeech >= speeches.Count || string.IsNullOrEmpty(speeches[randSpeech].key))
		{
			Debug.LogWarning($"Rodh speech: Invalid speech index {randSpeech} or null/empty key. Skipping speech.");
			yield break;
		}
		Debug.Log("Rodh speech: Using speech key '" + speeches[randSpeech].key + "'");
		foreach (GameObject textBox in textBoxes)
		{
			TextLocaliserUI textLocaliserUI = textBox.GetComponent<TextLocaliserUI>();
			if (textLocaliserUI == null)
			{
				textLocaliserUI = textBox.AddComponent<TextLocaliserUI>();
			}
			if (!(textLocaliserUI != null))
			{
				continue;
			}
			textLocaliserUI.localisedString = speeches[randSpeech];
			TextMeshProUGUI tmpro = textBox.GetComponent<TextMeshProUGUI>();
			if (tmpro != null)
			{
				string value = textLocaliserUI.localisedString.value;
				if (string.IsNullOrEmpty(value))
				{
					Debug.LogWarning("Rodh speech: Failed to get localized value for key '" + speeches[randSpeech].key + "'. Using fallback text.");
					tmpro.text = "Rodh speaks...";
				}
				else
				{
					tmpro.text = value;
				}
				tmpro.color = new Color(1f, 1f, 1f, 0f);
				while (tmpro.color.a < 0.9f)
				{
					tmpro.color = new Color(1f, 1f, 1f, tmpro.color.a + 0.1f);
					yield return new WaitForSeconds(0.1f);
				}
				tmpro.color = new Color(1f, 1f, 1f, 1f);
			}
		}
		yield return new WaitForSeconds(5f);
		foreach (GameObject textBox2 in textBoxes)
		{
			TextMeshProUGUI tmpro = textBox2.GetComponent<TextMeshProUGUI>();
			if (tmpro != null)
			{
				while (tmpro.color.a > 0.1f)
				{
					tmpro.color = new Color(1f, 1f, 1f, tmpro.color.a - 0.1f);
					yield return new WaitForSeconds(0.1f);
				}
				tmpro.color = new Color(1f, 1f, 1f, 0f);
			}
		}
		yield return null;
	}

	public void doSpawnSpears(int randSpear)
	{
		if (!spearGlow[randSpear].activeSelf)
		{
			StartCoroutine(SpawnSpears(randSpear));
		}
	}

	public void doSpawnWaterGlow(int randWater)
	{
		if (!waterGlow[randWater].activeSelf)
		{
			StartCoroutine(SpawnWaterGlow(randWater));
		}
	}

	private IEnumerator SpawnSpears(int randSpear)
	{
		spearGlow[randSpear].SetActive(value: true);
		GameObject gameObject = new GameObject("AttackTelegraph");
		AttackTelegraph telegraph = gameObject.AddComponent<AttackTelegraph>();
		Vector3 right = spearGlow[randSpear].transform.right;
		telegraph.Initialize(spearGlow[randSpear].transform.position, right, 20f, 1f, 0.1f);
		telegraph.preserveAfterDuration = true;
		activeTelegraphs.Add(telegraph);
		yield return new WaitForSeconds(0.7f);
		GameObject gameObject2 = Object.Instantiate(spear, spearGlow[randSpear].transform.position, spearGlow[randSpear].transform.rotation);
		telegraph.AttachProjectile(gameObject2.transform);
		yield return new WaitForSeconds(0.25f);
		spearGlow[randSpear].SetActive(value: false);
	}

	private IEnumerator SpawnWaterGlow(int randWater)
	{
		waterGlow[randWater].SetActive(value: true);
		GameObject gameObject = new GameObject("AttackTelegraph");
		AttackTelegraph telegraph = gameObject.AddComponent<AttackTelegraph>();
		Vector3 right = waterGlow[randWater].transform.right;
		telegraph.Initialize(waterGlow[randWater].transform.position, right, 20f, 1f, 0.1f);
		telegraph.preserveAfterDuration = true;
		activeTelegraphs.Add(telegraph);
		yield return new WaitForSeconds(0.5f);
		GameObject gameObject2 = Object.Instantiate(spear, waterGlow[randWater].transform.position, waterGlow[randWater].transform.rotation);
		telegraph.AttachProjectile(gameObject2.transform);
		yield return new WaitForSeconds(0.25f);
		waterGlow[randWater].SetActive(value: false);
	}

	public void GreenBubbles()
	{
		if (attackHelper != null)
		{
			GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
			if (gameObject != null)
			{
				attackHelper.FindPlayableSpaceBounds(gameObject.transform.position);
			}
		}
		for (int i = 0; i < 2; i++)
		{
			Object.Instantiate(greenBubble, GetRandomPositionInBounds(), Quaternion.identity);
		}
	}

	private bool IsPositionValidDistance(Vector3 position, float minDistance)
	{
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject in players)
		{
			if (gameObject != null && Vector2.Distance(position, gameObject.transform.position) < minDistance)
			{
				return false;
			}
		}
		return true;
	}

	private Vector3 GetRandomPositionInBounds()
	{
		float num = 2f;
		int num2 = 0;
		Vector3 vector;
		do
		{
			vector = new Vector3(room.transform.position.x + Random.Range(-9f, 9f), room.transform.position.y + Random.Range(-9f, 9f), room.transform.position.z);
			if (attackHelper != null)
			{
				vector = attackHelper.ClampPositionToBounds(vector);
			}
			if (IsPositionValidDistance(vector, num))
			{
				return vector;
			}
			num *= 0.95f;
			num2++;
		}
		while (num2 <= 50);
		return vector;
	}

	private void OnDestroy()
	{
		foreach (AttackTelegraph activeTelegraph in activeTelegraphs)
		{
			if (activeTelegraph != null)
			{
				Object.Destroy(activeTelegraph.gameObject);
			}
		}
		activeTelegraphs.Clear();
	}

	public Vector3 ClampPositionToBounds(Vector3 position)
	{
		if (attackHelper != null)
		{
			return attackHelper.ClampPositionToBounds(position);
		}
		return position;
	}

	public void SetBubblesMovement(bool canMove)
	{
		GreenBubble[] array = Object.FindObjectsOfType<GreenBubble>();
		foreach (GreenBubble greenBubble in array)
		{
			if (greenBubble != null)
			{
				greenBubble.canMove = canMove;
			}
		}
	}

	public IEnumerator EnableBubbleMovementAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		SetBubblesMovement(canMove: true);
	}
}
