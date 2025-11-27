using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpellCasting : MonoBehaviour
{
	public delegate void CastCallback(Spell spell, Vector3 targetPoint);

	public delegate void UpgradeCallback(Spell spell, SpellUpgrade upgrade);

	public static List<SpellCasting> Instances = new List<SpellCasting>();

	private SpellHover spellHover;

	[HideInInspector]
	public bool specialSpellCooldownPaused;

	private DungeonRoom currentSubscribedRoom;

	private bool foundWaveManager;

	private List<SpellSlot> queuedSpells = new List<SpellSlot>();

	private Dictionary<SpellSlot, float> failSoundTimestamps = new Dictionary<SpellSlot, float>();

	private const float failSoundCooldown = 0.5f;

	private Dictionary<SpellSlot, float> lastCastAttemptTimestamps = new Dictionary<SpellSlot, float>();

	private const float castAttemptCooldown = 0.2f;

	private Dictionary<SpellSlot, bool> queuedSpellSuccessfullyCast = new Dictionary<SpellSlot, bool>();

	private Movement movement;

	public List<Stance> stances = new List<Stance>();

	private int stanceIndex;

	private int previousStanceIndex;

	public Transform spellSlotsList;

	public GameObject spellSlotsPrefab;

	public Animator spellSlotsAnimator;

	public GameObject spellsUIWrapper;

	private GameObject extraSlot1;

	private GameObject extraSlot2;

	private Image extraSlot1Icon;

	private Image extraSlot2Icon;

	private Slider extraSlot1Slider;

	private Slider extraSlot2Slider;

	public Interact interact;

	public GameObject tooltip;

	public TextMeshProUGUI tooltipName;

	public TextMeshProUGUI tooltipDescription;

	public TextMeshProUGUI tooltipCooldown;

	public GameObject tutorialHighlight;

	[HideInInspector]
	public bool canCast = true;

	private bool casting;

	public bool choosingSpell;

	private bool choosingUpgrade;

	private bool cancellingUpgrade;

	[HideInInspector]
	public Vector2 cursorPosition;

	public LayerMask enemyHitLayerMask;

	[Header("Spell Upgrading")]
	public GameObject spellUpgradeMenu;

	public GameObject cancelUpgradeModal;

	public Button cancelUpgradeYes;

	public Transform spellUpgradeButtonParent;

	public GameObject spellPickerPrefab;

	private const int upgradeCostMultiplier = 10;

	public int upgradeCost;

	public bool canUpgradeSpells = true;

	[HideInInspector]
	public float spellDamageMultiplier = 1f;

	[HideInInspector]
	public bool randomizeSpellDamage;

	[HideInInspector]
	public float minimumDamageMultiplier = 1f;

	[HideInInspector]
	public float maximumDamageMultiplier = 1f;

	[HideInInspector]
	public bool rechargeSpeedBuff;

	[HideInInspector]
	public bool speedDamage;

	[HideInInspector]
	public float cooldownSpeedModifier = 1f;

	[HideInInspector]
	public float targetSpecialReduction;

	public LocalisedString upgradeString;

	public LocalisedString chooseUpgradeString;

	public LocalisedString knowledgeString;

	public LocalisedString tooFarFromOblomeString;

	public GameObject upgradeParticlesPrefab;

	public bool damageBuffRelic;

	[HideInInspector]
	public bool damageBoostRelic;

	private ControllerAim aim;

	private PlayerInput playerInput;

	[HideInInspector]
	public bool roomKnowledgeRelic;

	public bool lowVisionRelic;

	public GameObject knowledgeCoins;

	private int currentSceneIndex;

	private int previousSceneIndex;

	public Relic knowledgeRoomRelic;

	public Relic visionRelic;

	public Relic healEnemyRelic;

	public Relic buffEnemyRelic;

	public Relic failSpellRelic;

	[HideInInspector]
	public int visionCount;

	public int enemyHealCount;

	public int enemyBuffCount;

	public int spellFailCounter;

	public bool enemyHealRelic;

	public bool enemyBuffRelic;

	public bool spellFailRelic;

	[HideInInspector]
	public float movementSpeedBuff = 1f;

	private Coroutine stanceSwitchCoroutine;

	public Stance currentStance => stances[stanceIndex];

	public event CastCallback OnCastSpell;

	public event UpgradeCallback OnUpgrade;

	private void Awake()
	{
		Instances.Add(this);
		GameObject gameObject = Object.Instantiate(spellSlotsPrefab, spellSlotsList);
		spellHover = gameObject.GetComponent<SpellHover>();
		if (spellHover != null)
		{
			spellHover.owner = this;
		}
		spellSlotsAnimator = gameObject.GetComponent<Animator>();
		foreach (Stance stance in stances)
		{
			for (int i = 0; i < stance.spellSlots.Count; i++)
			{
				SpellSlot spellSlot = stance.spellSlots[i];
				Transform child = gameObject.transform.GetChild(i);
				spellSlot.UIObject = child.gameObject;
				spellSlot.UIBackground = child.GetChild(0).GetComponent<Image>();
				spellSlot.slider = child.GetChild(1).GetComponent<Slider>();
				spellSlot.iconUI = child.GetChild(3).GetComponent<Image>();
				spellSlot.animator = child.GetComponent<Animator>();
			}
		}
		if (gameObject.transform.childCount >= 5)
		{
			extraSlot1 = gameObject.transform.GetChild(3).gameObject;
			extraSlot2 = gameObject.transform.GetChild(4).gameObject;
			extraSlot1Icon = extraSlot1.transform.GetChild(3).GetComponent<Image>();
			extraSlot2Icon = extraSlot2.transform.GetChild(3).GetComponent<Image>();
			extraSlot1Slider = extraSlot1.transform.GetChild(1).GetComponent<Slider>();
			extraSlot2Slider = extraSlot2.transform.GetChild(1).GetComponent<Slider>();
			extraSlot1.SetActive(value: false);
			extraSlot2.SetActive(value: false);
		}
		if (WaveManager.Instance != null)
		{
			Debug.Log("Subscribing to WaveManager");
			WaveManager.Instance.OnEnteredRoom += OnRoomEntered;
			foundWaveManager = true;
		}
	}

	private void OnRoomFinished()
	{
		if (currentSubscribedRoom != null && currentSubscribedRoom.cleared)
		{
			specialSpellCooldownPaused = true;
		}
	}

	private void OnRoomEntered(DungeonRoom room)
	{
		if (currentSubscribedRoom != null)
		{
			currentSubscribedRoom.OnFinishedRoom -= OnRoomFinished;
		}
		currentSubscribedRoom = room;
		if (currentSubscribedRoom != null)
		{
			currentSubscribedRoom.OnFinishedRoom += OnRoomFinished;
		}
		if (room.GetType() != typeof(EventRoom))
		{
			specialSpellCooldownPaused = false;
		}
	}

	public void OnWaveManagerReady(WaveManager waveManager)
	{
		if (currentSubscribedRoom != null)
		{
			currentSubscribedRoom.OnFinishedRoom -= OnRoomFinished;
			currentSubscribedRoom = null;
		}
		if (waveManager != null)
		{
			waveManager.OnEnteredRoom += OnRoomEntered;
			foundWaveManager = true;
		}
	}

	public void StartBossFight()
	{
		specialSpellCooldownPaused = false;
	}

	private void OnDestroy()
	{
		Instances.Remove(this);
		if (currentSubscribedRoom != null)
		{
			currentSubscribedRoom.OnFinishedRoom -= OnRoomFinished;
		}
		if (WaveManager.Instance != null)
		{
			WaveManager.Instance.OnEnteredRoom -= OnRoomEntered;
		}
	}

	public void reduceVision()
	{
		lowVisionRelic = true;
		base.gameObject.transform.Find("Darkness").gameObject.SetActive(value: true);
	}

	public void increaseVision()
	{
		lowVisionRelic = false;
		base.gameObject.transform.Find("Darkness").gameObject.SetActive(value: false);
	}

	public int CalculateSpellUpgradeCost(Spell spell)
	{
		int num = 10;
		for (int i = 0; i < Mathf.Min(spell.upgradeCount, 16); i++)
		{
			num *= 2;
		}
		return Mathf.Clamp(num, 0, 999);
	}

	public void DisplayTooltip(Spell spell)
	{
		Debug.Log("tooltip");
		tooltip.SetActive(value: true);
		tooltipName.text = spell.name.value;
		if (spell.upgradeCount > 0)
		{
			TextMeshProUGUI textMeshProUGUI = tooltipName;
			textMeshProUGUI.text = textMeshProUGUI.text + " +" + spell.upgradeCount;
		}
		tooltipDescription.text = spell.description.value;
		float cooldownTime = spell.cooldownTime;
		cooldownTime = Mathf.Round(cooldownTime * 10f) * 0.1f;
		tooltipCooldown.text = cooldownTime + "s";
	}

	public void ExitHover()
	{
		tooltip.SetActive(value: false);
	}

	private void Start()
	{
		movement = GetComponent<Movement>();
		playerInput = GetComponent<PlayerInput>();
		aim = GetComponentInChildren<ControllerAim>();
		SpecialSpell specialSpell = null;
		foreach (Stance stance in stances)
		{
			for (int i = 0; i < stance.spellSlots.Count; i++)
			{
				SpellSlot spellSlot = stance.spellSlots[i];
				if (!spellSlot.isUsable && stance == currentStance)
				{
					spellSlot.Disable();
				}
				if (!(spellSlot.spell != null))
				{
					continue;
				}
				if (i == 0 && spellSlot.spell is SpecialSpell)
				{
					if (specialSpell == null)
					{
						specialSpell = (SpecialSpell)spellSlot.spell;
					}
					spellSlot.SetNewSpell(specialSpell);
				}
				else
				{
					spellSlot.SetNewSpell(Object.Instantiate(spellSlot.spell));
				}
			}
		}
		SwitchStance(stanceIndex, instant: true);
		UpdateExtraSlots();
	}

	public void SetSpellCooldownTime(Spell spell, float cooldownTime)
	{
		bool flag = false;
		if (currentStance.spellSlots.Count > 0 && currentStance.spellSlots[0].spell == spell)
		{
			flag = true;
		}
		if (flag && spell is SpecialSpell)
		{
			foreach (Stance stance in stances)
			{
				if (stance.spellSlots.Count > 0 && stance.spellSlots[0].spell is SpecialSpell)
				{
					stance.spellSlots[0].cooldownTimer = cooldownTime;
				}
			}
			return;
		}
		foreach (SpellSlot spellSlot in currentStance.spellSlots)
		{
			if (spellSlot.spell == spell)
			{
				spellSlot.cooldownTimer = cooldownTime;
				break;
			}
		}
	}

	public void ResetSpellCooldowns()
	{
		foreach (SpellSlot spellSlot in currentStance.spellSlots)
		{
			if (!(spellSlot.spell != null))
			{
				continue;
			}
			if (typeof(SpecialSpell).IsAssignableFrom(spellSlot.spell.GetType()))
			{
				foreach (Stance stance in stances)
				{
					if (stance.enabled && stance.spellSlots[0].spell == spellSlot.spell)
					{
						stance.spellSlots[0].cooldownTimer = spellSlot.spell.cooldownTime;
					}
				}
			}
			else
			{
				spellSlot.cooldownTimer = spellSlot.spell.cooldownTime;
			}
		}
	}

	public void StartSpellUpgrade()
	{
		Screenshake.Instance.ResetTrauma();
		if ((bool)Object.FindObjectOfType<SettingsMenu>())
		{
			Manager.Instance?.UnpauseGame();
			Object.FindObjectOfType<SettingsMenu>().Exit();
		}
		else
		{
			if (choosingSpell)
			{
				return;
			}
			choosingSpell = false;
			choosingUpgrade = false;
			bool flag = false;
			foreach (SpellSlot spellSlot in currentStance.spellSlots)
			{
				if (spellSlot.spell != null)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				Debug.Log("No spells to upgrade");
				return;
			}
			RuntimeManager.PlayOneShot("event:/SFX/UI/Spell_Menu_Open");
			Manager.Instance?.PauseGame(pauseMusic: false, base.gameObject);
			choosingSpell = true;
			spellUpgradeMenu.SetActive(value: true);
			MusicManager.EnterMenu();
			ClearSpellUpgradePicker();
			SpawnSpellPickers();
		}
	}

	public void AwardKnowledgeForRoom()
	{
		currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		if (roomKnowledgeRelic && previousSceneIndex == 0)
		{
			previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
		}
		if (currentSceneIndex != previousSceneIndex && roomKnowledgeRelic)
		{
			RelicCollector.Instance.DropRelic(knowledgeRoomRelic);
		}
		if (roomKnowledgeRelic)
		{
			(from x in RelicCollector.Instance.Relics()
				where x == knowledgeRoomRelic
				select x).ToList().ForEach(delegate(Relic x)
			{
				x.Use();
			});
			Object.Instantiate(knowledgeCoins, base.gameObject.transform.position, Quaternion.identity);
		}
	}

	private void SpawnSpellPickers()
	{
		if ((bool)GameObject.Find("TeleportVisualEffect(Clone)"))
		{
			Object.Destroy(GameObject.Find("TeleportVisualEffect(Clone)"));
		}
		bool flag = false;
		SpellSlot[] array = currentStance.spellSlots.ToArray();
		SpellSlot spellSlot = array[0];
		array[0] = array[1];
		array[1] = spellSlot;
		GameObject gameObject = GameObject.FindGameObjectWithTag("Core");
		if (gameObject == null || !gameObject.activeSelf)
		{
			gameObject = GameObject.Find("OblomePlaceholder");
		}
		bool flag2 = gameObject != null && (base.transform.position - gameObject.transform.position).magnitude < 5f;
		SpellSlot[] array2 = array;
		foreach (SpellSlot spellSlot2 in array2)
		{
			if (spellSlot2.spell != null && spellSlot2.spell.upgrades.Count > 0 && RelicCollector.Instance.CanAfford(CalculateSpellUpgradeCost(spellSlot2.spell)))
			{
				string actionDescription = upgradeString.value + " - " + CalculateSpellUpgradeCost(spellSlot2.spell) + " " + knowledgeString.value;
				if (!flag2 && canUpgradeSpells)
				{
					actionDescription = tooFarFromOblomeString.value;
				}
				GameObject gameObject2 = InstantiateSpellPicker(spellSlot2.spell, spellSlot2.spell, actionDescription, flag2 && canUpgradeSpells);
				if (!flag)
				{
					gameObject2.GetComponent<UISpellPicker>().first = true;
					flag = true;
				}
			}
			else if (spellSlot2.spell != null)
			{
				string actionDescription2 = upgradeString.value + " - " + CalculateSpellUpgradeCost(spellSlot2.spell) + " " + knowledgeString.value;
				if (!flag2 && canUpgradeSpells)
				{
					actionDescription2 = tooFarFromOblomeString.value;
				}
				InstantiateSpellPicker(spellSlot2.spell, spellSlot2.spell, actionDescription2, canDo: false);
			}
		}
	}

	public void ClearSpellUpgradePicker()
	{
		foreach (Transform item in spellUpgradeButtonParent)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public GameObject InstantiateSpellPicker(Spell originalSpell, Spell spell, string actionDescription, bool canDo, SpellUpgrade upgrade = null)
	{
		GameObject obj = Object.Instantiate(spellPickerPrefab, spellUpgradeButtonParent);
		UISpellPicker component = obj.GetComponent<UISpellPicker>();
		component.button.onClick.AddListener(delegate
		{
			SpellPickerChoose(originalSpell, canDo, upgrade);
		});
		component.spellName.text = spell.name.value;
		if (spell.upgradeCount > 0)
		{
			TextMeshProUGUI spellName = component.spellName;
			spellName.text = spellName.text + " +" + spell.upgradeCount;
		}
		component.spellFlavour.text = spell.flavour.value;
		component.icon.sprite = spell.icon;
		component.description.text = spell.description.value + "\n\n" + spell.lore.value;
		float num = Mathf.Round(spell.cooldownTime * 10f) / 10f;
		component.spellCooldown.text = num.ToString("0.0") + "s";
		component.spellClass.text = spell.school.name.value;
		component.spellClassIcon.sprite = spell.school.schoolIcon;
		if (upgrade != null)
		{
			component.description.text = "";
			component.upgradeDescription.text = upgrade.description.value;
		}
		if (!canDo)
		{
			component.actionDescription.color = new Color(component.upgradeDescription.color.r, component.upgradeDescription.color.g, component.upgradeDescription.color.b, 0.3f);
		}
		component.actionDescription.text = actionDescription;
		return obj;
	}

	public void DisplayUpgrades(Spell spellToUpgrade)
	{
		choosingSpell = false;
		choosingUpgrade = true;
		if (spellToUpgrade == null)
		{
			Debug.LogError("Attempted to display upgrades for a null spell.");
		}
		ClearSpellUpgradePicker();
		List<SpellUpgrade> list = new List<SpellUpgrade>(spellToUpgrade.upgrades);
		List<SpellUpgrade> list2 = new List<SpellUpgrade>();
		SpellUpgrade[] array = list.ToArray();
		foreach (SpellUpgrade spellUpgrade in array)
		{
			if (!spellUpgrade.stackable && spellUpgrade.applied)
			{
				list.Remove(spellUpgrade);
			}
		}
		int num = Mathf.Clamp(list.Count, 0, 3);
		for (int j = 0; j < num; j++)
		{
			int index = Random.Range(0, list.Count);
			list2.Add(list[index]);
			list.Remove(list[index]);
		}
		if (list2.Count > 0)
		{
			RelicCollector.Instance.Buy(CalculateSpellUpgradeCost(spellToUpgrade));
		}
		foreach (SpellSlot spellSlot in currentStance.spellSlots)
		{
			if (!(spellSlot.spell == spellToUpgrade))
			{
				continue;
			}
			bool flag = true;
			{
				foreach (SpellUpgrade item in list2)
				{
					Spell spell = Object.Instantiate(spellToUpgrade);
					spell.Upgrade(spell.upgrades[spellToUpgrade.upgrades.IndexOf(item)]);
					GameObject gameObject = (gameObject = InstantiateSpellPicker(spellSlot.spell, spell, "", canDo: true, item));
					if (flag)
					{
						gameObject.GetComponent<UISpellPicker>().first = true;
						flag = false;
					}
				}
				return;
			}
		}
		Debug.LogError("spellToUpgrade matches none of the spells");
		spellUpgradeMenu.SetActive(value: false);
		choosingSpell = false;
		choosingUpgrade = false;
		if (list2.Count > 0)
		{
			RelicCollector.Instance.GainKnowledge(CalculateSpellUpgradeCost(spellToUpgrade));
		}
		Manager.Instance?.UnpauseGame();
	}

	public void SpellPickerChoose(Spell upgrading, bool canDo, SpellUpgrade chosenUpgrade = null)
	{
		if (!canDo)
		{
			return;
		}
		if (choosingSpell)
		{
			DisplayUpgrades(upgrading);
		}
		else
		{
			if (!choosingUpgrade)
			{
				return;
			}
			choosingUpgrade = false;
			foreach (Transform item in spellUpgradeButtonParent)
			{
				UISpellPicker component = item.GetComponent<UISpellPicker>();
				if (component != null && component.button != null)
				{
					component.button.interactable = false;
				}
			}
			StartCoroutine(CloseSpellMenu());
			if (chosenUpgrade == null)
			{
				Debug.LogError("Chose null upgrade.");
				RelicCollector.Instance.GainKnowledge(CalculateSpellUpgradeCost(upgrading));
				return;
			}
			GameObject gameObject = GameObject.FindGameObjectWithTag("Core");
			GameObject obj = Object.Instantiate(upgradeParticlesPrefab, (gameObject != null) ? gameObject.transform.position : base.transform.position, Quaternion.identity);
			obj.GetComponent<UpgradeParticles>().player = base.transform;
			ParticleSystem.TriggerModule trigger = obj.GetComponent<ParticleSystem>().trigger;
			Screenshake.Instance.AddTrauma(0.8f);
			trigger.AddCollider(GetComponent<Collider2D>());
			foreach (SpellSlot spellSlot in currentStance.spellSlots)
			{
				if (spellSlot.spell == upgrading)
				{
					if (!spellSlot.spell.ToString().Contains("(Clone)"))
					{
						Debug.LogError("Spell is not a clone: " + spellSlot.spell.ToString());
						spellSlot.spell = Object.Instantiate(spellSlot.spell);
					}
					spellSlot.spell.Upgrade(chosenUpgrade);
					if (SceneManager.GetActiveScene().name != "Tutorial")
					{
						Steam.TriggerAchievement("ACH_UPGRADE_SPELL");
					}
					this.OnUpgrade?.Invoke(spellSlot.spell, chosenUpgrade);
					return;
				}
			}
			Debug.LogError("Can't find spell that is being upgraded.");
			RelicCollector.Instance.GainKnowledge(CalculateSpellUpgradeCost(upgrading));
		}
	}

	private IEnumerator CloseSpellMenu()
	{
		spellUpgradeMenu.GetComponent<Animator>().SetTrigger("Close");
		yield return new WaitForSecondsRealtime(0.25f);
		spellUpgradeMenu.SetActive(value: false);
		choosingSpell = false;
		choosingUpgrade = false;
		MusicManager.ExitMenu();
		Manager.Instance?.UnpauseGame();
	}

	public void Cancel(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			CloseMenus(sound: true);
		}
	}

	public static void CancelUpgradeStatic()
	{
		GetActive().CancelUpgrade();
	}

	public void CancelUpgrade()
	{
		if (cancellingUpgrade && cancelUpgradeYes.interactable)
		{
			cancellingUpgrade = false;
			cancelUpgradeModal.SetActive(value: false);
			RuntimeManager.PlayOneShot("event:/SFX/UI/Click", base.transform.position);
			StartSpellUpgrade();
		}
	}

	private static SpellCasting GetActive()
	{
		return PlayerManager.Instance.lastActivePlayerInput.GetComponent<SpellCasting>();
	}

	public static void CloseMenusStatic(bool sound = false)
	{
		GetActive().CloseMenus(sound);
	}

	public void CloseMenus(bool sound = false)
	{
		if (cancellingUpgrade)
		{
			cancelUpgradeModal.SetActive(value: false);
			cancellingUpgrade = false;
			if (spellUpgradeButtonParent.childCount > 0)
			{
				EventSystem.current.SetSelectedGameObject(null);
				UISpellPicker component = spellUpgradeButtonParent.GetChild(0).GetComponent<UISpellPicker>();
				if (component != null && component.button != null)
				{
					EventSystem.current.SetSelectedGameObject(component.button.gameObject);
				}
				else
				{
					EventSystem.current.SetSelectedGameObject(spellUpgradeButtonParent.GetChild(0).gameObject);
				}
			}
		}
		else if (choosingUpgrade)
		{
			cancellingUpgrade = true;
			cancelUpgradeModal.SetActive(value: true);
			if ((bool)Object.FindObjectOfType<Tutorial>())
			{
				cancelUpgradeYes.interactable = false;
			}
		}
		else if (choosingSpell)
		{
			if (sound)
			{
				RuntimeManager.PlayOneShot("event:/SFX/UI/Click", base.transform.position);
			}
			MusicManager.ExitMenu();
			StartCoroutine(CloseSpellMenu());
		}
	}

	private void Update()
	{
		cooldownSpeedModifier = Mathf.Clamp(cooldownSpeedModifier, 0.5f, 2f);
		EffectsVisualiser.Instance?.SetEffectVisual(EffectsVisualiser.EffectType.Cooldown, cooldownSpeedModifier);
		EffectsVisualiser.Instance?.SetEffectVisual(EffectsVisualiser.EffectType.Damage, spellDamageMultiplier);
		movementSpeedBuff = 1f;
		if (!foundWaveManager && WaveManager.Instance != null)
		{
			Debug.Log("Found WaveManager");
			foundWaveManager = true;
			WaveManager.Instance.OnEnteredRoom += OnRoomEntered;
		}
		float cooldownTimer = 0f;
		bool flag = false;
		SpellSlot spellSlot = null;
		foreach (Stance stance in stances)
		{
			foreach (SpellSlot spellSlot4 in stance.spellSlots)
			{
				if (spellSlot4.spell != null && spellSlot4.spell.charge && spellSlot4.spell.chargeTimer > 0f)
				{
					spellSlot = spellSlot4;
					break;
				}
			}
			if (spellSlot != null)
			{
				break;
			}
		}
		foreach (Stance stance2 in stances)
		{
			for (int i = 0; i < stance2.spellSlots.Count; i++)
			{
				SpellSlot spellSlot2 = stance2.spellSlots[i];
				if (!(spellSlot2.spell != null))
				{
					continue;
				}
				spellSlot2.CleanupExpiredPenalties();
				bool flag2 = false;
				if (spellSlot2.spell is SpecialSpell specialSpell && (specialSpell.isCoroutineRunning || specialSpellCooldownPaused))
				{
					flag2 = true;
				}
				if (!flag2)
				{
					float num = cooldownSpeedModifier * spellSlot2.GetCooldownSpeedModifier();
					if (spellSlot != null && spellSlot2 != spellSlot)
					{
						num *= 0.5f;
					}
					spellSlot2.cooldownTimer += Time.deltaTime * num;
				}
				if (i == 0 && spellSlot2.spell is SpecialSpell)
				{
					if (!flag)
					{
						cooldownTimer = spellSlot2.cooldownTimer;
						flag = true;
					}
					else
					{
						spellSlot2.cooldownTimer = cooldownTimer;
					}
				}
			}
		}
		foreach (SpellSlot spellSlot5 in currentStance.spellSlots)
		{
			spellSlot5.UpdateUI(currentStance.uiTint);
			if (spellSlot5.spell != null)
			{
				if (spellSlot5.spell.charge && spellSlot5.spell.chargeTimer > 0f)
				{
					ExecuteSpellSlot(spellSlot5);
				}
				if (spellSlot5.cooldownTimer < spellSlot5.spell.cooldownTime && rechargeSpeedBuff && spellSlot5 != currentStance.spellSlots[0])
				{
					movementSpeedBuff += 0.25f;
				}
			}
		}
		if (targetSpecialReduction > 0f && flag)
		{
			foreach (Stance stance3 in stances)
			{
				for (int j = 0; j < stance3.spellSlots.Count; j++)
				{
					if (j == 0 && stance3.spellSlots[j].spell is SpecialSpell)
					{
						SpellSlot spellSlot3 = stance3.spellSlots[j];
						spellSlot3.cooldownTimer = Mathf.Min(spellSlot3.spell.cooldownTime, spellSlot3.cooldownTimer + targetSpecialReduction);
					}
				}
			}
			targetSpecialReduction = 0f;
		}
		foreach (Stance stance4 in stances)
		{
			if (stance4 == currentStance)
			{
				continue;
			}
			foreach (SpellSlot spellSlot6 in stance4.spellSlots)
			{
				if (spellSlot6.spell != null)
				{
					UpdateExtraSlotCooldowns(stance4, spellSlot6);
				}
			}
		}
		foreach (SpellSlot item in queuedSpells.ToList())
		{
			if (item.spell is SpecialSpell)
			{
				queuedSpells.Remove(item);
				lastCastAttemptTimestamps.Remove(item);
				failSoundTimestamps.Remove(item);
				queuedSpellSuccessfullyCast.Remove(item);
				continue;
			}
			if (item.spell != null && item.isUsable && !interact.isSelecting && canCast && item.cooldownTimer >= item.spell.cooldownTime)
			{
				bool num2 = !item.spell.charge || item.spell.chargeTimer == 0f;
				if (!lastCastAttemptTimestamps.ContainsKey(item))
				{
					lastCastAttemptTimestamps[item] = 0f;
				}
				bool flag3 = Time.time - lastCastAttemptTimestamps[item] >= 0.2f;
				if (num2 && flag3)
				{
					ExecuteSpellSlot(item);
					item.TriggerPressAnimation();
					lastCastAttemptTimestamps[item] = Time.time;
				}
			}
			int num3 = currentStance.spellSlots.IndexOf(item);
			if (num3 != 1 && num3 != 2)
			{
				continue;
			}
			if (playerInput == null)
			{
				playerInput = GetComponent<PlayerInput>();
			}
			string actionNameOrId = ((num3 == 1) ? "Primary" : "Secondary");
			InputAction inputAction = playerInput.actions.FindAction(actionNameOrId);
			if (inputAction == null)
			{
				continue;
			}
			float num4 = 0f;
			if (inputAction.activeControl != null)
			{
				if (inputAction.activeControl is AxisControl axisControl)
				{
					num4 = axisControl.ReadValue();
				}
			}
			else
			{
				Gamepad current8 = Gamepad.current;
				if (current8 != null)
				{
					num4 = ((num3 == 1) ? current8.leftTrigger.ReadValue() : current8.rightTrigger.ReadValue());
				}
			}
			if (num4 == 0f || num4 == -1f)
			{
				HandleSpellRelease(item);
			}
		}
		if (!(movement.upgradeButton != null))
		{
			return;
		}
		bool flag4 = false;
		foreach (SpellSlot spellSlot7 in currentStance.spellSlots)
		{
			if (spellSlot7.spell != null)
			{
				flag4 = true;
			}
		}
		if (flag4 && Time.timeScale != 0f && !choosingSpell && !choosingUpgrade && !Death.Instance.isDead && !DialogueSystem.Instance.inDialogue)
		{
			movement.upgradeButton.color = Color.white;
			movement.upgradeLabel.SetActive(value: true);
		}
		else
		{
			movement.upgradeButton.color = Color.gray;
			movement.upgradeLabel.SetActive(value: false);
		}
	}

	private void PlayFailSound(SpellSlot spellSlot)
	{
		if (spellSlot != null && (!failSoundTimestamps.TryGetValue(spellSlot, out var value) || Time.time - value >= 0.5f))
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spell_Fail");
			failSoundTimestamps[spellSlot] = Time.time;
		}
	}

	private void HandleSpellRelease(SpellSlot spellSlot)
	{
		if (spellSlot != null)
		{
			queuedSpells.Remove(spellSlot);
			lastCastAttemptTimestamps.Remove(spellSlot);
			bool flag = spellSlot.spell != null;
			bool flag2 = flag && spellSlot.spell is SpecialSpell;
			bool value;
			if (flag && spellSlot.spell.charge && spellSlot.spell.chargeTimer > 0f)
			{
				ChargedSpell(spellSlot);
			}
			else if (flag && !flag2 && !(queuedSpellSuccessfullyCast.TryGetValue(spellSlot, out value) && value) && spellSlot.isUsable && !interact.isSelecting && canCast)
			{
				PlayFailSound(spellSlot);
			}
			failSoundTimestamps.Remove(spellSlot);
			queuedSpellSuccessfullyCast.Remove(spellSlot);
		}
	}

	public void DamageBuffRelic()
	{
		spellDamageMultiplier = 1.25f;
	}

	public void SpellTryCast(SpellSlot spellSlot, InputAction.CallbackContext context)
	{
		if (!(spellSlot.spell != null) || !spellSlot.isUsable || interact.isSelecting || !canCast || Time.timeScale == 0f)
		{
			return;
		}
		bool flag = spellSlot.spell is SpecialSpell;
		if (context.started)
		{
			if (spellSlot.cooldownTimer >= spellSlot.spell.cooldownTime)
			{
				ExecuteSpellSlot(spellSlot);
				spellSlot.TriggerPressAnimation();
				if (!flag && !queuedSpells.Contains(spellSlot))
				{
					queuedSpells.Add(spellSlot);
				}
			}
			else if (flag)
			{
				PlayFailSound(spellSlot);
			}
			else if (!queuedSpells.Contains(spellSlot))
			{
				queuedSpells.Add(spellSlot);
			}
		}
		else if (context.canceled)
		{
			HandleSpellRelease(spellSlot);
		}
	}

	public void Special(InputAction.CallbackContext context)
	{
		SpellTryCast(currentStance.spellSlots[0], context);
	}

	public void Primary(InputAction.CallbackContext context)
	{
		SpellTryCast(currentStance.spellSlots[1], context);
	}

	public void Secondary(InputAction.CallbackContext context)
	{
		SpellTryCast(currentStance.spellSlots[2], context);
	}

	public void Tertiary(InputAction.CallbackContext context)
	{
	}

	public void Quaternary(InputAction.CallbackContext context)
	{
	}

	public void SwitchStance(InputAction.CallbackContext context)
	{
		if (!interact.isSelecting && canCast && Time.timeScale != 0f)
		{
			if (context.started)
			{
				if (stanceIndex >= stances.Count - 1)
				{
					SwitchStance(0);
				}
				else
				{
					SwitchStance(stanceIndex + 1);
				}
			}
		}
		else if (choosingSpell && context.started)
		{
			if (stanceIndex >= stances.Count - 1)
			{
				SwitchStance(0, instant: true);
			}
			else
			{
				SwitchStance(stanceIndex + 1, instant: true);
			}
			ClearSpellUpgradePicker();
			SpawnSpellPickers();
		}
	}

	public void SwitchStance(int index, bool instant = false)
	{
		if (!stances[index].enabled)
		{
			return;
		}
		int num = stanceIndex;
		previousStanceIndex = stanceIndex;
		stanceIndex = index;
		if (!instant)
		{
			spellSlotsAnimator.SetTrigger("Switch");
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Switch_Stance", base.transform.position);
			if (stanceSwitchCoroutine != null)
			{
				StopCoroutine(stanceSwitchCoroutine);
			}
			else if (num != index)
			{
				foreach (Stance.StanceEffect effect in stances[num].effects)
				{
					switch (effect)
					{
					case Stance.StanceEffect.SpeedIncrease:
						movement.speedModifier -= 0.2f;
						break;
					case Stance.StanceEffect.SpeedDecrease:
						movement.speedModifier += 0.2f;
						break;
					case Stance.StanceEffect.DamageIncrease:
						spellDamageMultiplier -= 0.2f;
						break;
					case Stance.StanceEffect.DamageDecrease:
						spellDamageMultiplier += 0.2f;
						break;
					}
				}
			}
			stanceSwitchCoroutine = StartCoroutine(SwitchStanceWait());
			Screenshake.Instance.AddTrauma(0.6f);
			return;
		}
		if (num != stanceIndex)
		{
			foreach (Stance.StanceEffect effect2 in stances[num].effects)
			{
				switch (effect2)
				{
				case Stance.StanceEffect.SpeedIncrease:
					movement.speedModifier -= 0.2f;
					break;
				case Stance.StanceEffect.SpeedDecrease:
					movement.speedModifier += 0.2f;
					break;
				case Stance.StanceEffect.DamageIncrease:
					spellDamageMultiplier -= 0.2f;
					break;
				case Stance.StanceEffect.DamageDecrease:
					spellDamageMultiplier += 0.2f;
					break;
				}
			}
		}
		SetStanceActive();
	}

	private IEnumerator SwitchStanceWait()
	{
		yield return new WaitForSecondsRealtime(1f / 6f);
		SetStanceActive();
	}

	private void SetStanceActive()
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (previousStanceIndex >= 0 && previousStanceIndex < stances.Count)
		{
			Stance stance = stances[previousStanceIndex];
			for (int i = 0; i < stance.spellSlots.Count; i++)
			{
				if (queuedSpells.Contains(stance.spellSlots[i]))
				{
					hashSet.Add(i);
				}
			}
		}
		queuedSpells.Clear();
		lastCastAttemptTimestamps.Clear();
		failSoundTimestamps.Clear();
		queuedSpellSuccessfullyCast.Clear();
		foreach (Stance stance2 in stances)
		{
			foreach (SpellSlot spellSlot2 in stance2.spellSlots)
			{
				if (spellSlot2.spell != null && spellSlot2.spell.charge && spellSlot2.spell.chargeTimer > 0f)
				{
					Debug.Log($"Found charging spell: {spellSlot2.spell.name.value} with chargeTimer: {spellSlot2.spell.chargeTimer}");
					ChargedSpell(spellSlot2, fromStanceSwitch: true);
				}
				spellSlot2.SetActive(active: false);
			}
		}
		foreach (SpellSlot spellSlot3 in currentStance.spellSlots)
		{
			spellSlot3.SetActive(active: true);
			spellSlot3.UpdateUI(currentStance.uiTint);
		}
		foreach (int item in hashSet)
		{
			if (item < currentStance.spellSlots.Count)
			{
				SpellSlot spellSlot = currentStance.spellSlots[item];
				if (spellSlot.spell != null && spellSlot.isUsable && !(spellSlot.spell is SpecialSpell))
				{
					queuedSpells.Add(spellSlot);
				}
			}
		}
		foreach (Stance.StanceEffect effect in currentStance.effects)
		{
			switch (effect)
			{
			case Stance.StanceEffect.SpeedIncrease:
				movement.speedModifier += 0.2f;
				break;
			case Stance.StanceEffect.SpeedDecrease:
				movement.speedModifier -= 0.2f;
				break;
			case Stance.StanceEffect.DamageIncrease:
				spellDamageMultiplier += 0.2f;
				break;
			case Stance.StanceEffect.DamageDecrease:
				spellDamageMultiplier -= 0.2f;
				break;
			}
		}
		UpdateExtraSlots();
		spellHover.RefreshTooltip();
		stanceSwitchCoroutine = null;
	}

	public void UpdateExtraSlots()
	{
		if (extraSlot1 == null || extraSlot2 == null)
		{
			return;
		}
		Stance stance = null;
		foreach (Stance stance2 in stances)
		{
			if (stance2 != currentStance && stance2.enabled)
			{
				stance = stance2;
				break;
			}
		}
		if (stance != null && stance.spellSlots.Count >= 3)
		{
			extraSlot1.SetActive(value: true);
			extraSlot2.SetActive(value: true);
			SpellSlot spellSlot = stance.spellSlots[1];
			if (spellSlot.spell != null)
			{
				extraSlot1Icon.sprite = spellSlot.spell.icon;
				extraSlot1Icon.gameObject.SetActive(value: true);
				extraSlot1Slider.maxValue = spellSlot.spell.cooldownTime;
				extraSlot1Slider.value = Mathf.Clamp(spellSlot.cooldownTimer, 0f, spellSlot.spell.cooldownTime);
			}
			else
			{
				extraSlot1Icon.sprite = null;
				extraSlot1Icon.gameObject.SetActive(value: false);
				extraSlot1Slider.value = 0f;
			}
			SpellSlot spellSlot2 = stance.spellSlots[2];
			if (spellSlot2.spell != null)
			{
				extraSlot2Icon.sprite = spellSlot2.spell.icon;
				extraSlot2Icon.gameObject.SetActive(value: true);
				extraSlot2Slider.maxValue = spellSlot2.spell.cooldownTime;
				extraSlot2Slider.value = Mathf.Clamp(spellSlot2.cooldownTimer, 0f, spellSlot2.spell.cooldownTime);
			}
			else
			{
				extraSlot2Icon.sprite = null;
				extraSlot2Icon.gameObject.SetActive(value: false);
				extraSlot2Slider.value = 0f;
			}
		}
		else
		{
			extraSlot1.SetActive(value: false);
			extraSlot2.SetActive(value: false);
		}
	}

	private void UpdateExtraSlotCooldowns(Stance stance, SpellSlot spellSlot)
	{
		if (extraSlot1 == null || extraSlot2 == null || stance == currentStance)
		{
			return;
		}
		int num = stance.spellSlots.IndexOf(spellSlot);
		if (num == 1 && extraSlot1.activeInHierarchy)
		{
			if (spellSlot.spell != null)
			{
				extraSlot1Slider.value = Mathf.Clamp(spellSlot.cooldownTimer, 0f, spellSlot.spell.cooldownTime);
			}
		}
		else if (num == 2 && extraSlot2.activeInHierarchy && spellSlot.spell != null)
		{
			extraSlot2Slider.value = Mathf.Clamp(spellSlot.cooldownTimer, 0f, spellSlot.spell.cooldownTime);
		}
	}

	public void SpellMenuButton(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			if (!choosingSpell && !choosingUpgrade)
			{
				Upgrade();
			}
			else
			{
				CloseMenus(sound: true);
			}
		}
	}

	public void Upgrade()
	{
		if (Time.timeScale != 0f && !choosingSpell && !choosingUpgrade && !Death.Instance.isDead && !DialogueSystem.Instance.inDialogue)
		{
			if (movement != null && movement.upgradePressAnim != null)
			{
				movement.upgradePressAnim.SetTrigger("Press");
			}
			StartSpellUpgrade();
		}
	}

	private void ExecuteSpellSlot(SpellSlot spellSlot)
	{
		Spell spell = null;
		spell = ((!typeof(SpecialSpell).IsAssignableFrom(spellSlot.spell.GetType())) ? spellSlot.spell : ((SpecialSpell)spellSlot.spell));
		if (casting && spell.chargeTimer == 0f)
		{
			PlayFailSound(spellSlot);
		}
		else
		{
			if ((!spell.canCastWhileRolling && movement.rolling) || Death.Instance.isDead || !canCast)
			{
				return;
			}
			if (spell.combo)
			{
				spell.comboCounter++;
			}
			spellSlot.cooldownTimer = 0f;
			if (spell is SpecialSpell)
			{
				foreach (Stance stance in stances)
				{
					if (stance.spellSlots.Count > 0 && stance.spellSlots[0].spell == spell)
					{
						stance.spellSlots[0].cooldownTimer = 0f;
					}
				}
			}
			Vector3 vector = Camera.main.ScreenToWorldPoint(cursorPosition) + new Vector3(0f, 0f, 10f);
			if (aim.usingController)
			{
				Enemy[] array = Object.FindObjectsOfType<Enemy>();
				Boss boss = Object.FindObjectOfType<Boss>();
				Crystal[] array2 = Object.FindObjectsOfType<Crystal>();
				if (aim.isInDeadzone)
				{
					if (boss == null)
					{
						GameObject gameObject = null;
						float num = float.MaxValue;
						Crystal[] array3 = array2;
						foreach (Crystal crystal in array3)
						{
							if (crystal != null && Mathf.Abs(crystal.gameObject.transform.position.y - base.gameObject.transform.position.y) <= 4f)
							{
								float num2 = Vector2.Distance(base.gameObject.transform.position, crystal.gameObject.transform.position);
								if (num2 < num)
								{
									num = num2;
									gameObject = crystal.gameObject;
								}
							}
						}
						Enemy[] array4 = array;
						foreach (Enemy enemy in array4)
						{
							if (enemy != null && Mathf.Abs(enemy.gameObject.transform.position.y - base.gameObject.transform.position.y) <= 4f)
							{
								float num3 = Vector2.Distance(base.gameObject.transform.position, enemy.gameObject.transform.position);
								if (num3 < num)
								{
									num = num3;
									gameObject = enemy.gameObject;
								}
							}
						}
						if (gameObject != null)
						{
							vector = gameObject.transform.position;
						}
					}
					else if (Mathf.Abs(boss.gameObject.transform.position.y - base.gameObject.transform.position.y) <= 4f)
					{
						vector = boss.gameObject.transform.position;
					}
				}
				else if (((Vector2)(base.gameObject.transform.position - vector)).x > 0f)
				{
					if (boss == null)
					{
						GameObject gameObject2 = null;
						float num4 = float.MaxValue;
						Crystal[] array3 = array2;
						foreach (Crystal crystal2 in array3)
						{
							if (crystal2 != null && base.gameObject.transform.position.x - crystal2.gameObject.transform.position.x >= 1f && crystal2.gameObject.transform.position.y - vector.y <= 4f && crystal2.gameObject.transform.position.y - vector.y >= -4f)
							{
								float num5 = Vector2.Distance(base.gameObject.transform.position, crystal2.gameObject.transform.position);
								if (num5 < num4)
								{
									num4 = num5;
									gameObject2 = crystal2.gameObject;
								}
							}
						}
						if (gameObject2 == null)
						{
							Enemy[] array4 = array;
							foreach (Enemy enemy2 in array4)
							{
								if (enemy2 != null && base.gameObject.transform.position.x - enemy2.gameObject.transform.position.x >= 1f && enemy2.gameObject.transform.position.y - vector.y <= 4f && enemy2.gameObject.transform.position.y - vector.y >= -4f)
								{
									float num6 = Vector2.Distance(base.gameObject.transform.position, enemy2.gameObject.transform.position);
									if (num6 < num4)
									{
										num4 = num6;
										gameObject2 = enemy2.gameObject;
									}
								}
							}
						}
						if (gameObject2 != null)
						{
							vector = gameObject2.transform.position;
						}
					}
					else if (base.gameObject.transform.position.x - boss.gameObject.transform.position.x >= 1f && boss.gameObject.transform.position.y - vector.y <= 4f && boss.gameObject.transform.position.y - vector.y >= -4f)
					{
						vector = boss.gameObject.transform.position;
					}
				}
				else if (boss == null)
				{
					GameObject gameObject3 = null;
					float num7 = float.MaxValue;
					Crystal[] array3 = array2;
					foreach (Crystal crystal3 in array3)
					{
						if (base.gameObject.transform.position.x - crystal3.gameObject.transform.position.x <= 1f && crystal3 != null && crystal3.gameObject.transform.position.y - vector.y <= 4f && crystal3.gameObject.transform.position.y - vector.y >= -4f)
						{
							float num8 = Vector2.Distance(base.gameObject.transform.position, crystal3.gameObject.transform.position);
							if (num8 < num7)
							{
								num7 = num8;
								gameObject3 = crystal3.gameObject;
							}
						}
					}
					Enemy[] array4 = array;
					foreach (Enemy enemy3 in array4)
					{
						if (base.gameObject.transform.position.x - enemy3.gameObject.transform.position.x <= 1f && enemy3 != null && enemy3.gameObject.transform.position.y - vector.y <= 4f && enemy3.gameObject.transform.position.y - vector.y >= -4f)
						{
							float num9 = Vector2.Distance(base.gameObject.transform.position, enemy3.gameObject.transform.position);
							if (num9 < num7)
							{
								num7 = num9;
								gameObject3 = enemy3.gameObject;
							}
						}
					}
					if (gameObject3 != null)
					{
						vector = gameObject3.transform.position;
					}
				}
				else if (base.gameObject.transform.position.x - boss.gameObject.transform.position.x <= 1f && boss.gameObject.transform.position.y - vector.y <= 4f && boss.gameObject.transform.position.y - vector.y >= -4f)
				{
					vector = boss.gameObject.transform.position;
				}
			}
			float num10 = spellDamageMultiplier;
			if (speedDamage)
			{
				num10 += (Mathf.Max(movement.speedModifier, 1f) - 1f) / 2f;
			}
			if (randomizeSpellDamage)
			{
				num10 *= Random.Range(minimumDamageMultiplier, maximumDamageMultiplier);
			}
			this.OnCastSpell?.Invoke(spell, vector);
			switch (spell.target)
			{
			case TargetType.None:
				spell.Execute(base.gameObject, num10);
				break;
			case TargetType.Point:
				spell.Execute(base.gameObject, num10, performCombo: false, null, vector);
				break;
			case TargetType.Enemy:
			{
				GameObject[] array5 = GameObject.FindGameObjectsWithTag("Enemy");
				GameObject gameObject4 = null;
				float num11 = float.PositiveInfinity;
				float num12 = 1f;
				if (aim.usingController)
				{
					num12 = 2f;
				}
				GameObject[] array6 = array5;
				foreach (GameObject gameObject5 in array6)
				{
					float num13 = Vector3.Distance(gameObject5.transform.position, vector);
					if (num13 < num11 && num13 < num12)
					{
						gameObject4 = gameObject5;
						num11 = num13;
					}
				}
				if (gameObject4 != null)
				{
					if ((bool)gameObject4.GetComponent<Enemy>() && !gameObject4.GetComponent<Enemy>().enabled)
					{
						SetSpellCooldownTime(spell, spell.cooldownTime);
						break;
					}
					Rigidbody2D component = gameObject4.GetComponent<Rigidbody2D>();
					bool num14 = component != null && !component.simulated;
					RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, (gameObject4.transform.position - base.transform.position).normalized, Vector3.Distance(gameObject4.transform.position, base.transform.position) + 1f, enemyHitLayerMask.value);
					if (num14 || (raycastHit2D.collider != null && raycastHit2D.transform.gameObject == gameObject4))
					{
						spell.Execute(base.gameObject, num10, performCombo: false, gameObject4);
					}
					else
					{
						SetSpellCooldownTime(spell, spell.cooldownTime);
					}
					break;
				}
				SetSpellCooldownTime(spell, spell.cooldownTime);
				PlayFailSound(spellSlot);
				return;
			}
			default:
				spell.Execute(base.gameObject, num10);
				break;
			}
			if (spell.charge)
			{
				if (spell.chargeTimer == 0f)
				{
					if (damageBoostRelic)
					{
						spellSlot.AddCooldownPenalty(0.12f, 6f);
					}
					if (queuedSpells.Contains(spellSlot))
					{
						queuedSpellSuccessfullyCast[spellSlot] = true;
					}
					casting = false;
				}
				else
				{
					casting = true;
				}
			}
			else
			{
				if (damageBoostRelic)
				{
					spellSlot.AddCooldownPenalty(0.12f, 6f);
				}
				if (!(spellSlot.spell is SpecialSpell))
				{
					queuedSpellSuccessfullyCast[spellSlot] = true;
				}
			}
		}
	}

	public void ChargedSpell(SpellSlot spellSlot, bool fromStanceSwitch = false, bool clear = false)
	{
		Spell spell = null;
		spell = ((!typeof(SpecialSpell).IsAssignableFrom(spellSlot.spell.GetType())) ? spellSlot.spell : ((SpecialSpell)spellSlot.spell));
		casting = false;
		spellSlot.cooldownTimer = 0f;
		if (damageBoostRelic)
		{
			spellSlot.AddCooldownPenalty(0.12f, 6f);
		}
		if (spell is SpecialSpell)
		{
			foreach (Stance stance in stances)
			{
				if (stance.spellSlots.Count > 0 && stance.spellSlots[0].spell == spell)
				{
					stance.spellSlots[0].cooldownTimer = 0f;
				}
			}
		}
		float num = spellDamageMultiplier;
		if (speedDamage)
		{
			num += movement.speedModifier / 5f;
		}
		if (randomizeSpellDamage)
		{
			num *= Random.Range(minimumDamageMultiplier, maximumDamageMultiplier);
		}
		if (fromStanceSwitch)
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(cursorPosition) + new Vector3(0f, 0f, 10f);
			Debug.Log($"Executing charged spell from stance switch: {spell.name.value}, targetPoint: {vector}, damageModifier: {num}");
			spell.chargeTimer = 10f;
			spell.Execute(base.gameObject, 0f, performCombo: false, null, vector);
		}
		else
		{
			spell.Charged(num, clear);
		}
	}

	private void LogCooldownDebug(SpellSlot spellSlot)
	{
		if (spellSlot != null && !(spellSlot.spell == null))
		{
			float num = spellSlot.GetCooldownSpeedModifier();
			float num2 = cooldownSpeedModifier;
			float num3 = num * num2;
			float num4 = ((num3 > 0f) ? (spellSlot.spell.cooldownTime / num3) : float.PositiveInfinity);
			Debug.Log("[DamageBoostRelic Debug] Spell: " + spellSlot.spell.name.value + " | " + $"Base Cooldown: {spellSlot.spell.cooldownTime:F2}s | " + $"Penalty Mod: {num:F2} | Global Mod: {num2:F2} | " + $"Total Speed: {num3:F2} | Effective Cooldown: {num4:F2}s");
		}
	}

	public void UpdateCursorPosition(InputAction.CallbackContext context)
	{
		if (aim == null)
		{
			aim = GetComponentInChildren<ControllerAim>();
		}
		if (aim != null && aim.usingController)
		{
			cursorPosition = context.ReadValue<Vector2>();
			cursorPosition.x = (cursorPosition.x + 1f) / 2f * (float)Screen.width;
			cursorPosition.y = (cursorPosition.y + 1f) / 2f * (float)Screen.height;
		}
		else
		{
			cursorPosition = context.ReadValue<Vector2>();
		}
	}

	private void OnDisable()
	{
		if (spellsUIWrapper != null)
		{
			spellsUIWrapper.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		spellsUIWrapper.SetActive(value: true);
	}

	public void ReduceSpecialSpellCooldown(float reductionAmount)
	{
		targetSpecialReduction += reductionAmount;
	}

	public void ResetSpellCooldown(Spell spell)
	{
		if (typeof(SpecialSpell).IsAssignableFrom(spell.GetType()))
		{
			foreach (Stance stance in stances)
			{
				if (stance.enabled && stance.spellSlots[0].spell == spell)
				{
					stance.spellSlots[0].cooldownTimer = 0f;
				}
			}
			return;
		}
		foreach (Stance stance2 in stances)
		{
			foreach (SpellSlot spellSlot in stance2.spellSlots)
			{
				if (spellSlot.spell == spell)
				{
					spellSlot.cooldownTimer = 0f;
					return;
				}
			}
		}
	}
}
