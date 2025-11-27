using FMOD.Studio;
using FMODUnity;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Death : MonoBehaviour
{
	public delegate DeathOverride DeathCallback();

	public delegate void CustomRespawnCallback();

	public static Death Instance;

	[HideInInspector]
	public bool isDead;

	public Animator fadeOutAnim;

	private Bus sfxBus;

	private float timeAlive;

	public event DeathCallback OnDeath;

	public event CustomRespawnCallback CustomRespawn;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		timeAlive += Time.deltaTime;
	}

	public bool PlayerDeath(GameObject attacker, Health health)
	{
		if (isDead)
		{
			return false;
		}
		Debug.Log("Player Death");
		if (this.OnDeath != null)
		{
			DeathOverride deathOverride = this.OnDeath();
			Debug.Log(deathOverride);
			switch (deathOverride)
			{
			case DeathOverride.ExtraLife:
				health.health = 20f;
				health.invincibilityFrames = true;
				health.invincibilityTimer = health.invincibilityTime - 5f;
				Screenshake.Instance.TimeImpact(0.2f);
				return false;
			case DeathOverride.Custom:
				return false;
			}
		}
		if (AnalyticsManager.Instance != null)
		{
			DeathEvent e = new DeathEvent
			{
				Attacker = ((attacker == null) ? "null" : attacker.name),
				Time = Mathf.RoundToInt(timeAlive)
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
		Movement[] array = Object.FindObjectsOfType<Movement>();
		foreach (Movement obj in array)
		{
			obj.enabled = false;
			obj.SetTargetVelocity(Vector2.zero);
		}
		PlayerInput[] array2 = Object.FindObjectsOfType<PlayerInput>();
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
		isDead = true;
		Steam.TriggerAchievement("ACH_DIE");
		StartDeathSequence();
		PlayerManager.Instance.ui.SetActive(value: false);
		return true;
	}

	private void StartDeathSequence()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Death");
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeOutAllMusic();
		}
		sfxBus = RuntimeManager.GetBus("bus:/SoundEffects");
		sfxBus.setVolume(0f);
		if (BossManager.Instance != null)
		{
			BossManager.Instance.EndBossFight(win: false);
		}
		Time.timeScale = 0.2f;
		Enemy[] array = Object.FindObjectsOfType<Enemy>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		if (this.CustomRespawn == null)
		{
			Object.Destroy(Object.FindObjectOfType<Settings>());
		}
	}

	public void FinishDeath()
	{
		if (this.CustomRespawn != null)
		{
			Time.timeScale = 1f;
			Enemy[] array = Object.FindObjectsOfType<Enemy>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			this.CustomRespawn();
		}
		else if (Manager.Instance != null)
		{
			Manager.Instance.Death();
		}
	}
}
