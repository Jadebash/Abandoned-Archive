using System;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnarchyManager : MonoBehaviour
{
	public List<AnarchyPhase> phases = new List<AnarchyPhase>();

	public float phaseLength = 57f;

	private int currentPhase;

	public EventReference musicEvent;

	public GameObject light;

	[Header("UI")]
	public Transform collectedRelicsParent;

	public GameObject collectedRelicPrefab;

	private List<Relic> relicsToGive;

	private float timer;

	private float totalAnarchyTimer;

	private float checkTimer = 0.5f;

	[Header("Tutorial")]
	public bool forceTutorial;

	public GameObject tutorialObjects;

	public GameObject killCrystal;

	public Dialogue stanceHint;

	public LocalisedString switchStanceTip;

	public LocalisedString useBothStancesTip;

	private bool needsToKillCrystal;

	private float needsToKillCrystalTimer;

	[Header("Closing Walls")]
	public GameObject wallColliderObject;

	public float wallCloseSpeed = 0.1f;

	public float minRoomSize = 2f;

	[Header("Fade Sprites")]
	public GameObject topFade;

	public GameObject bottomFade;

	public GameObject leftFade;

	public GameObject rightFade;

	[Header("Playable Space")]
	public BoxCollider2D playableSpace;

	private EdgeCollider2D wallCollider;

	private Vector2[] initialColliderPoints;

	private float initialRoomSize;

	private Vector3 initialTopPos;

	private Vector3 initialBottomPos;

	private Vector3 initialLeftPos;

	private Vector3 initialRightPos;

	private Vector2 initialPlayableSpaceSize;

	private bool bossRoomIndicatorWasVisible;

	public void NewStancePickup()
	{
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			instance.stances[1].enabled = true;
		}
		DialogueSystem.Instance.StartDialogue(stanceHint);
		DialogueSystem.Instance.OnFinishDialogue += NewStance;
	}

	public void NewStance(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= NewStance;
		killCrystal.SetActive(value: true);
		Spell spell = RunManager.Instance.GetSpell("TimeMain");
		spell = UnityEngine.Object.Instantiate(spell);
		Spell spell2 = RunManager.Instance.GetSpell("LightningMain2");
		spell2 = UnityEngine.Object.Instantiate(spell2);
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			instance.stances[1].spellSlots[1].SetNewSpell(spell);
			instance.stances[1].spellSlots[1].cooldownTimer = spell.cooldownTime;
			instance.stances[1].spellSlots[2].SetNewSpell(spell2);
			instance.stances[1].spellSlots[2].cooldownTimer = spell2.cooldownTime;
			instance.UpdateExtraSlots();
		}
		TipManager.ShowTip(switchStanceTip.value);
		needsToKillCrystal = true;
	}

	public void DestroyedCrystal()
	{
		TipManager.HideTip();
		killCrystal.GetComponent<Animator>().SetTrigger("End");
		SceneManager.UnloadSceneAsync("Anarchy");
		Tutorial.FinishedTutorial();
	}

	private void Start()
	{
		if (wallColliderObject != null)
		{
			wallCollider = wallColliderObject.GetComponent<EdgeCollider2D>();
			if (wallCollider != null)
			{
				initialColliderPoints = new Vector2[wallCollider.points.Length];
				Array.Copy(wallCollider.points, initialColliderPoints, wallCollider.points.Length);
				initialRoomSize = 0f;
				Vector2[] array = initialColliderPoints;
				foreach (Vector2 vector in array)
				{
					float magnitude = vector.magnitude;
					if (magnitude > initialRoomSize)
					{
						initialRoomSize = magnitude;
					}
				}
				UpdatePlayableSpaceBounds(initialColliderPoints);
			}
		}
		GameObject[] players = PlayerManager.players;
		for (int i = 0; i < players.Length; i++)
		{
			players[i].GetComponent<Movement>().canTeleport = false;
		}
		MusicManager.Instance.PlaySong(musicEvent);
		light.SetActive(value: true);
		EventRoomLever eventRoomLever = UnityEngine.Object.FindObjectOfType<EventRoomLever>();
		RuntimeManager.StudioSystem.setParameterByName("InEvent", 0f, ignoreseekspeed: true);
		if (forceTutorial || SceneManager.GetSceneByName("Tutorial").isLoaded)
		{
			eventRoomLever.gameObject.SetActive(value: false);
			tutorialObjects.SetActive(value: true);
			return;
		}
		if (topFade != null)
		{
			initialTopPos = topFade.transform.position;
		}
		if (bottomFade != null)
		{
			initialBottomPos = bottomFade.transform.position;
		}
		if (leftFade != null)
		{
			initialLeftPos = leftFade.transform.position;
		}
		if (rightFade != null)
		{
			initialRightPos = rightFade.transform.position;
		}
		if (playableSpace != null)
		{
			initialPlayableSpaceSize = playableSpace.size;
		}
		currentPhase = 0;
		relicsToGive = new List<Relic>();
		NewPhase();
		eventRoomLever.OnLeave += Exit;
		DirectionIndicators.Instance?.AddIndicator(eventRoomLever.transform, Color.black);
		Steam.TriggerAchievement("ACH_ANARCHY");
	}

	private void NewPhase()
	{
		timer = 0f;
		if (!BossManager.Instance.inBossFight)
		{
			if (MapGenerator.Instance != null && MapGenerator.Instance.bossRoomInstance != null && DirectionIndicators.Instance != null)
			{
				bossRoomIndicatorWasVisible = DirectionIndicators.Instance.HasIndicator(MapGenerator.Instance.bossRoomInstance.transform);
				if (bossRoomIndicatorWasVisible)
				{
					DirectionIndicators.Instance?.RemoveIndicator(MapGenerator.Instance.bossRoomInstance.transform);
				}
			}
			BossManager.Instance.StartBossFight(phases[currentPhase].title, base.gameObject, musicEvent, 0f, stopWaves: false);
		}
		else
		{
			BossManager.Instance.StartNewBossPhase(base.gameObject, musicEvent, phases[currentPhase].title, 0f);
		}
		BossManager.Instance.UpdateHealthbar(0f, phaseLength);
		float a = initialRoomSize - totalAnarchyTimer * wallCloseSpeed;
		a = Mathf.Max(a, minRoomSize);
		float scaleFactor = ((initialRoomSize > 0f) ? (a / initialRoomSize) : 1f);
		ApplyRoomScale(scaleFactor);
		foreach (GameObject item in phases[currentPhase].objectsToEnable)
		{
			if (item != null && playableSpace != null)
			{
				BoxCollider2D component = item.GetComponent<BoxCollider2D>();
				if (component != null)
				{
					Vector2 vector = (Vector2)component.bounds.size * 0.5f;
					Bounds bounds = playableSpace.bounds;
					Vector2 vector2 = new Vector2(bounds.min.x + vector.x, bounds.min.y + vector.y);
					Vector2 vector3 = new Vector2(bounds.max.x - vector.x, bounds.max.y - vector.y);
					if (vector2.x < vector3.x && vector2.y < vector3.y)
					{
						Vector2 vector4 = new Vector2(UnityEngine.Random.Range(vector2.x, vector3.x), UnityEngine.Random.Range(vector2.y, vector3.y));
						item.transform.position = vector4;
					}
				}
			}
			item.SetActive(value: true);
			DirectionIndicators.Instance?.AddIndicator(item.transform, Color.magenta);
		}
		if (Difficulty.Instance != null && currentPhase != 0)
		{
			Difficulty.Instance.damageMultiplier++;
			TipManager.ShowTip("The enemies grow stronger...", 3f);
		}
		else
		{
			Debug.LogWarning("No difficulty instance found.");
		}
	}

	private void Update()
	{
		if (needsToKillCrystal)
		{
			needsToKillCrystalTimer += Time.deltaTime;
			if (needsToKillCrystalTimer >= 20f)
			{
				needsToKillCrystal = false;
				TipManager.ShowTip(useBothStancesTip.value);
			}
		}
		if (BossManager.Instance.inBossFight)
		{
			timer += Time.deltaTime;
			totalAnarchyTimer += Time.deltaTime;
			checkTimer -= Time.deltaTime;
			if (checkTimer <= 0f)
			{
				checkTimer = 0.5f;
				bool flag = false;
				Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
				foreach (Enemy enemy in array)
				{
					if (enemy != null && enemy.enabled && enemy.gameObject.activeSelf && (enemy.health == null || enemy.health.health > 0f))
					{
						flag = true;
						break;
					}
				}
				bool flag2 = false;
				if (phases != null && currentPhase < phases.Count)
				{
					foreach (GameObject item in phases[currentPhase].objectsToEnable)
					{
						if (item != null && item.activeSelf)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (!flag && !flag2)
				{
					timer = phaseLength;
				}
			}
			BossManager.Instance.UpdateHealthbar(timer, phaseLength);
			if (timer >= phaseLength)
			{
				if (currentPhase + 1 < phases.Count)
				{
					currentPhase++;
					NewPhase();
				}
				else
				{
					BossManager.Instance.EndBossFight(win: false);
				}
				AwardRelic();
			}
		}
		if (BossManager.Instance.inBossFight && !forceTutorial && wallCollider != null)
		{
			float a = initialRoomSize - totalAnarchyTimer * wallCloseSpeed;
			a = Mathf.Max(a, minRoomSize);
			float num = ((initialRoomSize > 0f) ? (a / initialRoomSize) : 1f);
			ApplyRoomScale(num);
			if (topFade != null)
			{
				topFade.transform.position = initialTopPos * num;
			}
			if (bottomFade != null)
			{
				bottomFade.transform.position = initialBottomPos * num;
			}
			if (leftFade != null)
			{
				leftFade.transform.position = initialLeftPos * num;
			}
			if (rightFade != null)
			{
				rightFade.transform.position = initialRightPos * num;
			}
		}
	}

	private void FixedUpdate()
	{
		if (!(playableSpace != null))
		{
			return;
		}
		foreach (AnarchyPhase phase in phases)
		{
			foreach (GameObject item in phase.objectsToEnable)
			{
				if (!(item != null) || !item.activeSelf)
				{
					continue;
				}
				BoxCollider2D component = item.GetComponent<BoxCollider2D>();
				if (component != null)
				{
					Vector2 vector = component.bounds.center;
					if (!playableSpace.OverlapPoint(vector))
					{
						Vector2 vector2 = (Vector2)playableSpace.bounds.ClosestPoint(vector) - vector;
						item.transform.position += (Vector3)vector2;
					}
				}
			}
		}
	}

	private void ApplyRoomScale(float scaleFactor)
	{
		if (initialRoomSize <= 0f)
		{
			if (playableSpace != null)
			{
				playableSpace.size = initialPlayableSpaceSize * scaleFactor;
			}
			return;
		}
		scaleFactor = Mathf.Max(scaleFactor, minRoomSize / initialRoomSize);
		if (wallCollider != null && initialColliderPoints != null && initialColliderPoints.Length != 0)
		{
			Vector2[] array = new Vector2[initialColliderPoints.Length];
			for (int i = 0; i < initialColliderPoints.Length; i++)
			{
				array[i] = initialColliderPoints[i] * scaleFactor;
			}
			wallCollider.points = array;
			UpdatePlayableSpaceBounds(array);
		}
		else if (playableSpace != null)
		{
			playableSpace.size = initialPlayableSpaceSize * scaleFactor;
		}
	}

	private void UpdatePlayableSpaceBounds(Vector2[] colliderPoints)
	{
		if (playableSpace == null || wallCollider == null || colliderPoints == null || colliderPoints.Length == 0)
		{
			return;
		}
		Vector2 vector = playableSpace.transform.InverseTransformPoint(wallCollider.transform.TransformPoint(colliderPoints[0]));
		float minX = vector.x;
		float maxX = vector.x;
		float minY = vector.y;
		float maxY = vector.y;
		for (int i = 1; i < colliderPoints.Length; i++)
		{
			vector = playableSpace.transform.InverseTransformPoint(wallCollider.transform.TransformPoint(colliderPoints[i]));
			if (vector.x < minX)
			{
				minX = vector.x;
			}
			if (vector.x > maxX)
			{
				maxX = vector.x;
			}
			if (vector.y < minY)
			{
				minY = vector.y;
			}
			if (vector.y > maxY)
			{
				maxY = vector.y;
			}
		}
		ApplyEdgeRadiusPadding(ref minX, ref maxX, ref minY, ref maxY);
		if (minX > maxX)
		{
			minX = (maxX = (minX + maxX) * 0.5f);
		}
		if (minY > maxY)
		{
			minY = (maxY = (minY + maxY) * 0.5f);
		}
		Vector2 offset = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
		Vector2 size = new Vector2(maxX - minX, maxY - minY);
		playableSpace.offset = offset;
		playableSpace.size = size;
	}

	private void ApplyEdgeRadiusPadding(ref float minX, ref float maxX, ref float minY, ref float maxY)
	{
		if (!(wallCollider == null) && !(playableSpace == null))
		{
			float edgeRadius = wallCollider.edgeRadius;
			if (!(edgeRadius <= 0f))
			{
				Vector3 vector = wallCollider.transform.TransformVector(Vector3.right * edgeRadius);
				Vector3 vector2 = wallCollider.transform.TransformVector(Vector3.up * edgeRadius);
				Vector2 vector3 = playableSpace.transform.InverseTransformVector(vector);
				Vector2 vector4 = playableSpace.transform.InverseTransformVector(vector2);
				float num = Mathf.Abs(vector3.x) + Mathf.Abs(vector4.x);
				float num2 = Mathf.Abs(vector3.y) + Mathf.Abs(vector4.y);
				minX += num;
				maxX -= num;
				minY += num2;
				maxY -= num2;
			}
		}
	}

	private void AwardRelic()
	{
		Relic relic = RunManager.GetRelic();
		if (relic != null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(collectedRelicPrefab, collectedRelicsParent);
			obj.transform.GetChild(0).GetComponent<Image>().sprite = relic.icon;
			obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = relic.name.value;
			relicsToGive.Add(relic);
		}
	}

	private void Exit()
	{
		SceneManager.UnloadSceneAsync("Anarchy");
		Difficulty.Instance.damageMultiplier -= currentPhase;
		MusicManager.EnterLowIntensity();
		BossManager.Instance.EndBossFight(win: false);
		GameObject[] players = PlayerManager.players;
		for (int i = 0; i < players.Length; i++)
		{
			players[i].GetComponent<Movement>().canTeleport = true;
		}
		foreach (Relic item in relicsToGive)
		{
			RelicCollector.Instance?.GetRelic(item);
		}
		if (bossRoomIndicatorWasVisible && MapGenerator.Instance != null && MapGenerator.Instance.bossRoomInstance != null && DirectionIndicators.Instance != null && !DirectionIndicators.Instance.HasIndicator(MapGenerator.Instance.bossRoomInstance.transform))
		{
			DirectionIndicators.Instance.AddIndicator(MapGenerator.Instance.bossRoomInstance.transform, Color.red);
		}
	}
}
