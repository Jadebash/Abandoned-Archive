using System;
using System.Collections;
using FMODUnity;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
	public static BossManager Instance;

	[HideInInspector]
	public bool inBossFight;

	public GameObject bossUIWrapper;

	private string currentBoss;

	public TextMeshProUGUI bossTitleText;

	public Slider bossHealthbar;

	public GameObject bossDeathPrefab;

	private bool nohit = true;

	private GameObject _bossGameObject;

	public event Action OnBossDefeated;

	private void Awake()
	{
		Instance = this;
		if ((bool)UnityEngine.Object.FindObjectOfType<Boss>())
		{
			UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<Boss>().gameObject);
		}
		bossUIWrapper.SetActive(value: false);
		inBossFight = false;
	}

	public string GetCurrentBoss()
	{
		return currentBoss;
	}

	public void StartBossFight(LocalisedString name, GameObject bossGameObject, EventReference bossMusic, float introLength = 1f, bool stopWaves = true)
	{
		if (stopWaves && WaveManager.Instance != null)
		{
			WaveManager.Instance.StopAllCoroutines();
		}
		if (AnalyticsManager.Instance != null)
		{
			BossStartEvent e = new BossStartEvent
			{
				Boss = name.key
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		if (MapGenerator.Instance != null && MapGenerator.Instance.bossRoomInstance != null && DirectionIndicators.Instance != null && stopWaves)
		{
			DirectionIndicators.Instance?.RemoveIndicator(MapGenerator.Instance.bossRoomInstance.transform);
		}
		bossUIWrapper.SetActive(value: true);
		currentBoss = name.key;
		inBossFight = true;
		DestroyPickups();
		StartNewBossPhase(bossGameObject, bossMusic, name, introLength);
		nohit = true;
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			instance.GetComponent<Health>().OnDamage += PlayerTakesDamage;
			instance.StartBossFight();
		}
		_bossGameObject = bossGameObject;
	}

	private void PlayerTakesDamage(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null)
	{
		nohit = false;
	}

	public void StartNewBossPhase(GameObject bossGameObject, EventReference newBossMusic, LocalisedString newName, float introLength = 1f)
	{
		if (newName.key != "")
		{
			bossTitleText.text = newName.value;
		}
		if (MusicManager.Instance != null && !newBossMusic.IsNull)
		{
			MusicManager.Instance.PlaySong(newBossMusic);
		}
		if (introLength > 0f)
		{
			StartCoroutine(BossIntroCutscene(bossGameObject.transform, introLength));
		}
	}

	private IEnumerator BossIntroCutscene(Transform bossTransform, float waitLength)
	{
		CameraController.Instance.lookTarget = bossTransform;
		yield return new WaitForSeconds(waitLength);
		CameraController.Instance.lookTarget = null;
		CameraController.Instance.biasTarget = bossTransform;
	}

	private void DestroyPickups()
	{
		HealthPickup[] array = UnityEngine.Object.FindObjectsOfType<HealthPickup>();
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i].gameObject);
		}
		SpellPickup[] array2 = UnityEngine.Object.FindObjectsOfType<SpellPickup>();
		for (int i = 0; i < array2.Length; i++)
		{
			UnityEngine.Object.Destroy(array2[i].gameObject);
		}
		RelicPickup[] array3 = UnityEngine.Object.FindObjectsOfType<RelicPickup>();
		for (int i = 0; i < array3.Length; i++)
		{
			UnityEngine.Object.Destroy(array3[i].gameObject);
		}
	}

	public void EndBossFight(bool win = true)
	{
		if (_bossGameObject != null && win)
		{
			UnityEngine.Object.Instantiate(bossDeathPrefab, _bossGameObject.transform.position, Quaternion.identity);
		}
		bossUIWrapper.SetActive(value: false);
		inBossFight = false;
		CameraController.Instance.biasTarget = null;
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			instance.GetComponent<Health>().OnDamage -= PlayerTakesDamage;
		}
		if (win)
		{
			this.OnBossDefeated?.Invoke();
			if (Death.Instance != null && Death.Instance.isDead)
			{
				return;
			}
			if (AnalyticsManager.Instance != null)
			{
				BossEndEvent e = new BossEndEvent
				{
					Boss = currentBoss
				};
				AnalyticsService.Instance.RecordEvent(e);
			}
			Steam.TriggerAchievement("ACH_BEAT_BOSS");
			if (currentBoss == "oblome_boss_name")
			{
				Steam.TriggerAchievement("ACH_BEAT_OBLOME");
			}
			if (currentBoss == "rodh_boss_name")
			{
				Steam.TriggerAchievement("ACH_BEAT_RODH");
			}
			if (nohit)
			{
				if (currentBoss == "sir_veltrine_name")
				{
					Steam.TriggerAchievement("ACH_NO_HIT_VELTRINE");
				}
				if (currentBoss == "holite_name")
				{
					Steam.TriggerAchievement("ACH_NO_HIT_HOLITE");
				}
				if (currentBoss == "crylia_name")
				{
					Steam.TriggerAchievement("ACH_NO_HIT_CRYLIA");
				}
				if (currentBoss == "lost_apparition_name")
				{
					Steam.TriggerAchievement("ACH_NO_HIT_LOST_APPARITION");
				}
				if (currentBoss == "lemures_name")
				{
					Steam.TriggerAchievement("ACH_NO_HIT_LEMURES");
				}
				if (currentBoss == "rufus_name")
				{
					Steam.TriggerAchievement("ACH_NO_HIT_RUFUS");
				}
			}
			FloorManager.Instance?.CompletedCurrentFloor();
			if (FloorManager.Instance != null && FloorManager.Instance.currentFloor == 3 && Version.Instance.phase == "BETA")
			{
				Debug.Log("Showing beta ending");
				ShowBetaEnding();
				return;
			}
			if (FloorManager.Instance != null && FloorManager.Instance.GetFloorNameLower() == "floor 4")
			{
				MusicManager.Instance.FadeOutAllMusic();
				return;
			}
			GameObject obj = GameObject.FindGameObjectWithTag("Player");
			obj.GetComponent<Movement>().canTeleportToNextFloor = true;
			obj.GetComponent<SpellCasting>();
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.FadeOutAllMusic();
			}
		}
		else
		{
			inBossFight = false;
		}
		if (_bossGameObject != null && FloorManager.Instance.GetFloorNameLower() != "floor 4")
		{
			FadeOutAndDestroy fadeOutAndDestroy = _bossGameObject.AddComponent<FadeOutAndDestroy>();
			fadeOutAndDestroy.fadeTime = 1f;
			fadeOutAndDestroy.offsetTime = 10f;
		}
	}

	public void UpdateHealthbar(float health, float maxHealth)
	{
		bossHealthbar.maxValue = maxHealth;
		bossHealthbar.value = health;
	}

	private void ShowBetaEnding()
	{
		BetaEnding betaEnding = UnityEngine.Object.FindObjectOfType<BetaEnding>();
		if (betaEnding != null)
		{
			betaEnding.ShowEnding();
		}
	}
}
