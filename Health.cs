using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
	public delegate void DeathCallBack(GameObject attacker = null);

	public delegate void DamageCallback(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null);

	public delegate void HealCallback(float amount, bool healthPack);

	[HideInInspector]
	public float health;

	[Header("Health")]
	public float maxHealth;

	public Sprite[] heartUI;

	private Image statusPanel;

	[HideInInspector]
	public List<Health> forceSyncHealths = new List<Health>();

	[HideInInspector]
	public bool godmode;

	public bool invincible;

	private bool hasDied;

	[HideInInspector]
	public float invincibilityTimer;

	[HideInInspector]
	public bool invincibilityFrames;

	[Header("Damage Effects")]
	public bool invincibilityOnDamage;

	public float invincibilityTime = 1f;

	public float invincibilityTimeBetweenFlashes = 0.25f;

	[Space(5f)]
	public bool doScreenshake;

	[Space(5f)]
	public bool flashOnDamage;

	public float timeToFlashFor = 0.05f;

	public bool flashWhite = true;

	public Material whiteMaterial;

	public SpriteRenderer spriteToFlash;

	[Space(5f)]
	public bool freezeTimeOnDamage;

	[Range(0f, 0.2f)]
	public float freezeTimeAmount = 0.1f;

	[Space(5f)]
	public bool playHitAnimation;

	public Animator hitAnimator;

	[Space(5f)]
	public bool spawnHitEffectPrefab;

	public GameObject hitEffectPrefab;

	private float damageFlashTimer;

	private float damageTimer;

	[Space(5f)]
	public bool spawnDamageNumberIndicators;

	public GameObject damageIndicatorPrefab;

	public float indicatorSpawnRadius = 0.2f;

	[Space(5f)]
	public bool doAnimationOnLow;

	public Animator animateOnLow;

	public float lowThreshold = 20f;

	[Header("Health Bar")]
	public bool useHealthBar;

	public bool hideHealthBar = true;

	private bool hidingHealthBar = true;

	public Slider healthSlider;

	[Header("Health Regen")]
	[Range(0f, 50f)]
	public float regenRate;

	[Range(0f, 5f)]
	public float regenDelay;

	private float regenTimer;

	[HideInInspector]
	public float damageModifier = 1f;

	[HideInInspector]
	public float evasionChance;

	private int totalPoison;

	private float poisonTimer;

	public bool pushBack;

	[HideInInspector]
	public bool negateHealthPack;

	private int healthPackCounter;

	public Relic negateHealthPackRelic;

	private bool bleeding;

	private float bleedAmount;

	private float bleedTick = 1f;

	private float bleedTimer = 4f;

	public GameObject lowHealthIndicator;

	public event DeathCallBack OnDeath;

	public event DamageCallback OnDamage;

	public event HealCallback OnHeal;

	private void Awake()
	{
		health = maxHealth;
	}

	private void Start()
	{
		if (hideHealthBar && useHealthBar)
		{
			hidingHealthBar = true;
			healthSlider.gameObject.SetActive(value: false);
		}
		statusPanel = GameObject.Find("StatusPanel").GetComponent<Image>();
		if (base.gameObject.tag == "Player" && PlayerManager.players.Length > 1)
		{
			health = PlayerManager.players[0].GetComponent<Health>().health;
		}
	}

	private void Update()
	{
		if (this == null || base.gameObject == null)
		{
			return;
		}
		if (bleeding)
		{
			bleedTick -= Time.deltaTime;
			bleedTimer -= Time.deltaTime;
			if (bleedTick <= 0f)
			{
				Damage(bleedAmount);
				bleedTick = 1f;
			}
			if (bleedTimer <= 0f)
			{
				bleeding = false;
				bleedTick = 1f;
				bleedTimer = 4f;
				bleedAmount = 0f;
			}
		}
		if (base.transform.tag == "Player")
		{
			EffectsVisualiser.Instance?.SetEffectVisual(EffectsVisualiser.EffectType.IncomingDamage, damageModifier);
		}
		damageFlashTimer += Time.deltaTime;
		damageTimer += Time.deltaTime;
		if (damageTimer > 3f && useHealthBar && !hidingHealthBar)
		{
			hidingHealthBar = true;
			healthSlider?.gameObject?.SetActive(value: false);
		}
		if (regenRate > 0f)
		{
			Regen();
		}
		if (doAnimationOnLow)
		{
			animateOnLow.SetBool("low", health <= lowThreshold);
		}
		if (invincibilityFrames)
		{
			invincibilityTimer += Time.deltaTime;
			if (invincibilityTimer % invincibilityTimeBetweenFlashes > invincibilityTimeBetweenFlashes / 2f)
			{
				spriteToFlash.enabled = false;
			}
			else
			{
				spriteToFlash.enabled = true;
			}
			if (invincibilityTimer >= invincibilityTime)
			{
				invincibilityFrames = false;
				spriteToFlash.enabled = true;
			}
		}
		if (totalPoison > 0)
		{
			poisonTimer += Time.deltaTime;
			if (poisonTimer >= 1f)
			{
				poisonTimer = 0f;
				Damage(totalPoison);
				totalPoison /= 2;
			}
		}
	}

	public void inflictBleed(float damagePerTick)
	{
		if (!bleeding)
		{
			bleedAmount = damagePerTick;
			bleeding = true;
			bleedTick = 1f;
			bleedTimer = 4f;
		}
	}

	public void ChangeHealthUI()
	{
		if (maxHealth <= 80f && maxHealth > 60f)
		{
			statusPanel.sprite = heartUI[1];
			statusPanel.SetNativeSize();
		}
		else if (maxHealth <= 60f)
		{
			statusPanel.sprite = heartUI[2];
			statusPanel.SetNativeSize();
		}
		else
		{
			statusPanel.sprite = heartUI[0];
			statusPanel.SetNativeSize();
		}
	}

	public void ForceSyncHealth(float newHealth)
	{
		health = newHealth;
	}

	public void ResetHasDied()
	{
		hasDied = false;
	}

	public bool Damage(float damage, GameObject attacker = null, bool ignoreInvincibility = false)
	{
		bool result = false;
		if (UnityEngine.Random.Range(0f, 1f) < evasionChance)
		{
			UnityEngine.Object.Instantiate(damageIndicatorPrefab, base.transform.position + new Vector3(UnityEngine.Random.insideUnitCircle.x * indicatorSpawnRadius, UnityEngine.Random.insideUnitCircle.y * indicatorSpawnRadius, 0f), Quaternion.identity).GetComponent<NumberIndicator>().number = "MISS";
			return false;
		}
		if ((!ignoreInvincibility && invincible) || (!ignoreInvincibility && invincibilityFrames) || health == 0f || damage == 0f || godmode)
		{
			return false;
		}
		damage *= damageModifier;
		if (base.gameObject.GetComponent<Enemy>() != null)
		{
			GameObject gameObject = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
			if (gameObject != null)
			{
				SpellCasting component = gameObject.GetComponent<SpellCasting>();
				if (component != null)
				{
					if (component.enemyBuffRelic)
					{
						damage /= 1.75f;
					}
					if (component.spellFailRelic && UnityEngine.Random.Range(0, 4) == 1)
					{
						damage = 0f;
					}
					if (component.damageBuffRelic && Vector2.Distance(gameObject.transform.position, base.transform.position) <= 1.5f)
					{
						damage *= 1.25f;
					}
				}
			}
		}
		if (spawnDamageNumberIndicators)
		{
			UnityEngine.Object.Instantiate(damageIndicatorPrefab, base.transform.position + new Vector3(UnityEngine.Random.insideUnitCircle.x * indicatorSpawnRadius, UnityEngine.Random.insideUnitCircle.y * indicatorSpawnRadius, 0f), Quaternion.identity).GetComponent<NumberIndicator>().number = damage.ToString();
		}
		damageTimer = 0f;
		if (damageFlashTimer > 0.2f && flashOnDamage)
		{
			damageFlashTimer = 0f;
			StartCoroutine(FlashSprite());
		}
		health -= damage;
		foreach (Health forceSyncHealth in forceSyncHealths)
		{
			forceSyncHealth.ForceSyncHealth(health);
		}
		this.OnDamage?.Invoke(damage, health <= 0f && !hasDied, base.gameObject, attacker);
		if (doScreenshake)
		{
			Screenshake.Instance?.AddTrauma(damage * 40f / 100f);
		}
		if (health <= 0f)
		{
			health = 0f;
			result = true;
			if (!hasDied)
			{
				hasDied = true;
				try
				{
					this.OnDeath?.Invoke(attacker);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error in OnDeath callback: " + ex.Message);
					hasDied = false;
				}
			}
		}
		else
		{
			if (invincibilityOnDamage)
			{
				invincibilityFrames = true;
				invincibilityTimer = 0f;
			}
			if (freezeTimeOnDamage)
			{
				Screenshake.Instance?.TimeImpact(freezeTimeAmount);
			}
			if (playHitAnimation)
			{
				hitAnimator?.SetTrigger("Hit");
			}
			if (spawnHitEffectPrefab)
			{
				UnityEngine.Object.Instantiate(hitEffectPrefab, base.transform.position, Quaternion.identity);
			}
		}
		if (useHealthBar)
		{
			if (healthSlider == null)
			{
				Debug.LogWarning("Health slider is null for " + base.gameObject.name);
			}
			else
			{
				healthSlider.value = health / maxHealth;
				if (hideHealthBar && hidingHealthBar)
				{
					healthSlider.gameObject.SetActive(value: true);
					hidingHealthBar = false;
				}
			}
		}
		if (base.gameObject.tag == "Player" && health <= 20f && lowHealthIndicator != null)
		{
			GameObject[] players = PlayerManager.players;
			for (int i = 0; i < players.Length; i++)
			{
				players[i].GetComponent<Health>()?.lowHealthIndicator?.SetActive(value: true);
			}
		}
		return result;
	}

	public bool Heal(float heal, bool healthPack = false)
	{
		if (healthPack && health + heal > maxHealth)
		{
			return false;
		}
		if (!negateHealthPack)
		{
			if (PlayerManager.players.Length > 1)
			{
				GameObject[] players = PlayerManager.players;
				foreach (GameObject gameObject in players)
				{
					if (gameObject.GetComponent<Health>().health != maxHealth)
					{
						Debug.Log("health synced");
						gameObject.GetComponent<Health>().health += heal;
					}
				}
			}
			else
			{
				if (health == maxHealth)
				{
					return false;
				}
				health += heal;
				hasDied = false;
			}
		}
		else
		{
			healthPackCounter++;
			if (healthPackCounter >= 3)
			{
				health += heal;
				RelicCollector.Instance.DropRelic(negateHealthPackRelic);
				healthPackCounter = 0;
			}
		}
		foreach (Health forceSyncHealth in forceSyncHealths)
		{
			forceSyncHealth.ForceSyncHealth(forceSyncHealth.health);
		}
		if (health > maxHealth)
		{
			health = maxHealth;
		}
		if (base.gameObject.tag == "Player" && health > 20f && lowHealthIndicator != null)
		{
			GameObject[] players = PlayerManager.players;
			for (int i = 0; i < players.Length; i++)
			{
				players[i].GetComponent<Health>().lowHealthIndicator.SetActive(value: false);
			}
			if (lowHealthIndicator != null)
			{
				lowHealthIndicator.SetActive(value: false);
			}
		}
		this.OnHeal?.Invoke(heal, healthPack);
		if (spawnDamageNumberIndicators)
		{
			UnityEngine.Object.Instantiate(damageIndicatorPrefab, base.transform.position + new Vector3(UnityEngine.Random.insideUnitCircle.x * indicatorSpawnRadius, UnityEngine.Random.insideUnitCircle.y * indicatorSpawnRadius, 0f), Quaternion.identity).GetComponent<NumberIndicator>().number = "+" + heal;
		}
		if (useHealthBar)
		{
			healthSlider.value = health / maxHealth;
			if (hideHealthBar && hidingHealthBar)
			{
				healthSlider.gameObject.SetActive(value: true);
				hidingHealthBar = false;
			}
		}
		return true;
	}

	public void Poison(int amounts)
	{
		totalPoison += amounts;
	}

	private IEnumerator FlashSprite()
	{
		Material previousMaterial = spriteToFlash.material;
		Color previousColor = spriteToFlash.color;
		if (spriteToFlash != null && flashOnDamage)
		{
			if (flashWhite)
			{
				spriteToFlash.material = whiteMaterial;
				spriteToFlash.color = Color.white;
			}
			else
			{
				spriteToFlash.enabled = false;
			}
		}
		yield return new WaitForSeconds(timeToFlashFor);
		if (spriteToFlash != null && flashOnDamage)
		{
			if (flashWhite)
			{
				spriteToFlash.material = previousMaterial;
				spriteToFlash.color = previousColor;
			}
			else
			{
				spriteToFlash.enabled = true;
			}
		}
	}

	public void Regen()
	{
		regenTimer += Time.deltaTime;
		if (regenTimer >= 0.5f && damageTimer > regenDelay)
		{
			Heal(Mathf.Round(regenRate * 0.5f));
			regenTimer = 0f;
		}
	}

	private void OnParticleCollision(GameObject particle)
	{
		if (particle.gameObject.name == "FlameParticles")
		{
			Damage(20f);
		}
	}
}
