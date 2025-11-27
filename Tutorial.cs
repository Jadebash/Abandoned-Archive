using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
	public static Tutorial Instance;

	public bool skipIntroAnimation;

	[HideInInspector]
	public bool inIntroAnimation;

	public GameObject spawnRoomBarrier;

	public GameObject crystalsSpellOne;

	public GameObject crystalsSpellTwo;

	public DungeonRoom firstRoom;

	public GameObject loadAnarchyScene;

	public GameObject oblome;

	private Animator anim;

	[Header("Dialogue")]
	public Dialogue meetOblome;

	public LocalisedString dodgeTip;

	public List<Dialogue> attackDodgeFail;

	public Dialogue oblomeAttackDodge;

	public Dialogue oblomeSecondSpell;

	public LocalisedString targetTip;

	public LocalisedString chargeSpellsTip;

	public Dialogue oblomeSpellUpgrade;

	public LocalisedString upgradeTip;

	public Dialogue oblomeKillEnemies;

	public Dialogue oblomeNoRelic;

	public Dialogue oblomeTeleport;

	public LocalisedString teleportTip;

	public LocalisedString pickupRelicTip;

	public LocalisedString returnToOblomeTip;

	private float pickupRelicTimer;

	private float returnToOblomeTimer;

	private bool needsToPickupRelic;

	private bool needsToReturnToOblome;

	private float targetTimer;

	private bool waitingOnTarget;

	private bool didTargetTip;

	public Dialogue oblomeFinal;

	private SpellCasting spellCasting;

	private Movement movement;

	private Health playerHealth;

	private bool failedAttack;

	private int failedAttackCounter;

	private int destroyedCrystals;

	private Vector3 respawnLocation = new Vector3(25.25f, -10.4f, 0f);

	private bool waitingForPlayer;

	public List<GameObject> enemyPrefabs;

	public List<Vector3> enemySpawnPoints;

	private int enemies;

	private void Awake()
	{
		Instance = this;
		Debug.Log(Instance);
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void Start()
	{
		inIntroAnimation = true;
		Manager.Instance.PauseGame(pauseMusic: true);
		Debug.Log("Tutorial Start");
		spellCasting = Object.FindObjectOfType<SpellCasting>();
		spellCasting.gameObject.transform.position = new Vector3(0f, 0f, 0f);
		movement = Object.FindObjectOfType<Movement>();
		movement.canTeleport = false;
		movement.SetTargetVelocity(Vector2.zero);
		movement.desiredVelocity = Vector2.zero;
		movement.enabled = false;
		anim = GetComponent<Animator>();
		if (skipIntroAnimation)
		{
			anim.enabled = false;
			WakeUpAnimationComplete();
		}
		RelicCollector.Instance.OnRelicPickup += RelicPickup;
	}

	private void RelicPickup(Relic relic)
	{
		needsToPickupRelic = false;
		TipManager.HideTip();
		needsToReturnToOblome = true;
	}

	private void Update()
	{
		if (needsToPickupRelic)
		{
			pickupRelicTimer += Time.deltaTime;
			if (pickupRelicTimer >= 25f)
			{
				needsToPickupRelic = false;
				TipManager.ShowTip(pickupRelicTip.value);
			}
		}
		if (needsToReturnToOblome)
		{
			returnToOblomeTimer += Time.deltaTime;
			if (returnToOblomeTimer >= 30f)
			{
				needsToReturnToOblome = false;
				TipManager.ShowTip(returnToOblomeTip.value);
			}
		}
		if (waitingOnTarget)
		{
			targetTimer += Time.deltaTime;
			if (targetTimer >= 5f)
			{
				waitingOnTarget = false;
				TipManager.ShowTip(targetTip.value);
				didTargetTip = true;
			}
		}
	}

	public void WakeUpAnimationComplete()
	{
		inIntroAnimation = false;
		if ((bool)Object.FindObjectOfType<SettingsMenu>())
		{
			Object.FindObjectOfType<SettingsMenu>().Exit();
		}
		Manager.Instance.UnpauseGame(unpauseMusic: true);
		spellCasting = Object.FindObjectOfType<SpellCasting>();
		spellCasting.enabled = false;
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			instance.canUpgradeSpells = false;
		}
		playerHealth = spellCasting.GetComponent<Health>();
		movement = Object.FindObjectOfType<Movement>();
		movement.canTeleport = false;
		movement.enabled = true;
		if (spellCasting.stances.Count > 1)
		{
			for (int i = 1; i < spellCasting.stances.Count; i++)
			{
				spellCasting.stances[i].enabled = false;
			}
		}
		spellCasting.SwitchStance(0, instant: true);
		GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
		playerHealth.OnDamage += PlayerTakesFirstHit;
	}

	public void PlayerTakesFirstHit(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null)
	{
		playerHealth.OnDamage -= PlayerTakesFirstHit;
		playerHealth.Heal(20f);
		movement.enabled = false;
		movement.ForceTeleport(new Vector3(27f, 0.3f, 0f));
		MusicManager.Instance.StopMusic();
		StartCoroutine(MeetOblome());
	}

	private IEnumerator MeetOblome()
	{
		yield return new WaitForSeconds(1f);
		Enemy[] array = Object.FindObjectsOfType<Enemy>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
		DialogueSystem.Instance.StartDialogue(meetOblome);
		DialogueSystem.Instance.OnFinishDialogue += MetOblome;
		MusicManager.EnterLowIntensity();
	}

	private void MetOblome(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= MetOblome;
		spellCasting.enabled = true;
		anim.enabled = true;
		DoAttack(dialogue);
		TipManager.ShowTip(dodgeTip.value);
		playerHealth.OnDamage += PlayerTakesDamage;
	}

	public void DoAttack(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= DoAttack;
		failedAttack = false;
		anim.SetTrigger("OblomeAttack");
		if (playerHealth.health < playerHealth.maxHealth)
		{
			playerHealth.Heal(playerHealth.maxHealth - playerHealth.health);
		}
	}

	public void PlayerTakesDamage(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null)
	{
		failedAttack = true;
		failedAttackCounter++;
		if (failedAttackCounter > 10)
		{
			failedAttackCounter = 10;
		}
	}

	public void OblomeAttackAnimationFinish()
	{
		if (destroyedCrystals <= 0)
		{
			if (failedAttack)
			{
				DialogueSystem.Instance.StartDialogue(attackDodgeFail[failedAttackCounter]);
				DialogueSystem.Instance.OnFinishDialogue += DoAttack;
			}
			else
			{
				DialogueSystem.Instance.StartDialogue(oblomeAttackDodge);
				DialogueSystem.Instance.OnFinishDialogue += SpawnCrystals;
				TipManager.HideTip();
			}
		}
	}

	private void SpawnCrystals(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= SpawnCrystals;
		RunManager.Instance.SpawnSpell("LightningMain");
		waitingOnTarget = true;
		crystalsSpellOne.SetActive(value: true);
		firstRoom.OnFinishedRoom += FinishedRoom;
	}

	public void FinishedRoom()
	{
		MusicManager.EnterLowIntensity();
		EnableBarrier();
	}

	public void DestroyedCrystal()
	{
		destroyedCrystals++;
		waitingOnTarget = false;
		if (didTargetTip && destroyedCrystals < 3)
		{
			TipManager.HideTip();
			didTargetTip = false;
		}
		if (destroyedCrystals == 3)
		{
			DialogueSystem.Instance.StartDialogue(oblomeSecondSpell);
			DialogueSystem.Instance.OnFinishDialogue += SpawnSecondSpell;
		}
		if (destroyedCrystals == 7)
		{
			DialogueSystem.Instance.StartDialogue(oblomeSpellUpgrade);
			DialogueSystem.Instance.OnFinishDialogue += PromptUpgrade;
		}
	}

	public void SpawnSecondSpell(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= SpawnSecondSpell;
		RunManager.Instance.SpawnSpell("WaterMain");
		crystalsSpellTwo.SetActive(value: true);
		TipManager.ShowTip(chargeSpellsTip.value, 8f);
	}

	public void PromptUpgrade(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= PromptUpgrade;
		TipManager.ShowTip(upgradeTip.value);
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			instance.canUpgradeSpells = true;
			instance.OnUpgrade += UpgradedSpell;
		}
	}

	public void UpgradedSpell(Spell spell, SpellUpgrade upgrade)
	{
		DialogueSystem.Instance.StartDialogue(oblomeKillEnemies);
		DialogueSystem.Instance.OnFinishDialogue += EnableRoom;
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			instance.canUpgradeSpells = false;
		}
		TipManager.HideTip();
	}

	public void EnableRoom(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= EnableRoom;
		DisableBarrier();
		waitingForPlayer = true;
		Death.Instance.CustomRespawn += PlayerRespawn;
	}

	public void PlayerRespawn()
	{
		PlayerManager.Instance.ui.SetActive(value: true);
		playerHealth.Heal(playerHealth.maxHealth - playerHealth.health);
		spellCasting.ResetSpellCooldowns();
		movement.ForceTeleport(respawnLocation);
		playerHealth.invincibilityFrames = true;
		playerHealth.invincibilityTimer = playerHealth.invincibilityTime - 5f;
		Screenshake.Instance.TimeImpact(0.2f);
		movement.enabled = true;
		PlayerInput[] array = Object.FindObjectsOfType<PlayerInput>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		Death.Instance.isDead = false;
		Bus bus = RuntimeManager.GetBus("bus:/SoundEffects");
		if (SaveManager.Instance != null)
		{
			bus.setVolume(SaveManager.Instance.currentSave.volumeSoundEffects);
		}
		else
		{
			bus.setVolume(1f);
		}
		MusicManager.EnterHighIntensity();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player" && waitingForPlayer)
		{
			if (RelicCollector.Instance.Relics().Count == 0)
			{
				DialogueSystem.Instance.StartDialogue(oblomeNoRelic);
				needsToPickupRelic = true;
				return;
			}
			needsToReturnToOblome = false;
			TipManager.HideTip();
			DialogueSystem.Instance.StartDialogue(oblomeTeleport);
			DialogueSystem.Instance.OnFinishDialogue += OblomeTeleport;
			waitingForPlayer = false;
		}
	}

	public void OblomeTeleport(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= OblomeTeleport;
		movement.canTeleport = true;
		movement.teleportBasePosition = new Vector3(27f, 0.3f, 0f);
		movement.teleportThreshold = 6.6f;
		movement.OnTeleport += EnterHighIntensity;
		TipManager.ShowTip(teleportTip.value);
		for (int i = 0; i < enemyPrefabs.Count; i++)
		{
			Object.Instantiate(enemyPrefabs[i], base.transform.position + enemySpawnPoints[i] + new Vector3(Random.insideUnitCircle.x * 0.4f, Random.insideUnitCircle.y * 0.4f), Quaternion.identity).GetComponent<Health>().OnDeath += EnemyDeath;
			enemies++;
		}
		respawnLocation = new Vector3(27f, 0.3f, 0f);
	}

	private void EnterHighIntensity()
	{
		movement.OnTeleport -= EnterHighIntensity;
		TipManager.HideTip();
		MusicManager.EnterHighIntensity();
	}

	private void EnemyDeath(GameObject attacker = null)
	{
		enemies--;
		if (enemies <= 0)
		{
			enemies = 0;
			DialogueSystem.Instance.StartDialogue(oblomeFinal);
			DialogueSystem.Instance.OnFinishDialogue += OblomeDisappear;
			MusicManager.EnterLowIntensity();
		}
	}

	public void OblomeDisappear(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= OblomeDisappear;
		loadAnarchyScene.SetActive(value: true);
	}

	public static void FinishedTutorial()
	{
		SaveManager.Instance?.SetTutorialComplete();
		FloorManager.Instance.UpdateRunStartTimestamp();
		FloorManager.Instance.QueueTutorialExitDialogue();
		FloorManager.Instance.LoadFloor(1, loadPlayer: true, forceShowIntro: true);
		Debug.Log("Tutorial Complete!");
		Steam.TriggerAchievement("ACH_FINISH_TUTORIAL");
	}

	protected void DisableBarrier()
	{
		LeanTween.value(base.gameObject, DissolveBarrier, 0f, 1f, 0.6f);
	}

	private void EnableBarrier()
	{
		spawnRoomBarrier.SetActive(value: true);
		spawnRoomBarrier.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", 0f);
	}

	public void DissolveBarrier(float dissolve)
	{
		spawnRoomBarrier.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", dissolve);
		if (dissolve == 1f)
		{
			spawnRoomBarrier.SetActive(value: false);
		}
	}
}
