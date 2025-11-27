using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
	[Header("Reference Scripts")]
	public GameObject player;

	private SpellCasting spellCasting;

	private Movement movement;

	private Health playerHealth;

	[Header("Spells")]
	public GameObject spellPickupPrefab;

	public GameObject spellSelectionScreen;

	public Image spellSelectionIcon;

	private SpellPickup cache;

	private List<Interactable> interactables = new List<Interactable>();

	private Interactable currentInteract;

	[HideInInspector]
	public bool isSelecting;

	[Header("Tooltip")]
	public GameObject tooltip;

	public TextMeshProUGUI tooltipName;

	public TextMeshProUGUI tooltipDescription;

	public TextMeshProUGUI tooltipCooldown;

	[Header("Relic UI")]
	public GameObject buyRelicModalUI;

	private bool inBuyRelicScreen;

	public Button cancelButtonBuyRelicUI;

	public Button searchButtonBuyRelicUI;

	public GameObject obtainedRelicModalUI;

	private bool inObtainedRelicScreen;

	public Button keepButtonObtainedRelicUI;

	public Button loseButtonObtainedRelicUI;

	public Button searchDeeperButtonObtainedRelicUI;

	public TextMeshProUGUI relicBuyTitleUI;

	public TextMeshProUGUI relicBuyDescriptionUI;

	public TextMeshProUGUI relicNameUI;

	public TextMeshProUGUI relicEffectUI;

	public TextMeshProUGUI relicLoreUI;

	public Image relicIconUI;

	private RelicPickup relicCache;

	private float relicInteractTimer = 0.25f;

	private bool exitedRelicModal;

	private bool relicInteractable = true;

	[Header("Effects")]
	public GameObject knowledgeNumberIndicator;

	private AudioSource audioSource;

	[Header("Interaction Cooldowns")]
	public float dialogueInteractCooldown = 0.35f;

	private float dialogueCooldownTimer;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		spellCasting = player.GetComponent<SpellCasting>();
		movement = player.GetComponent<Movement>();
		playerHealth = player.GetComponent<Health>();
		if (DialogueSystem.Instance != null)
		{
			DialogueSystem.Instance.OnFinishDialogue += OnDialogueFinished;
		}
	}

	private void OnDialogueFinished(Dialogue dialogue)
	{
		dialogueCooldownTimer = dialogueInteractCooldown;
	}

	private void EndSelection()
	{
		isSelecting = false;
		Manager.Instance?.UnpauseGame();
		spellSelectionScreen.SetActive(value: false);
		cache = null;
		tooltip.SetActive(value: false);
		movement.enabled = true;
	}

	private void Update()
	{
		if (dialogueCooldownTimer > 0f)
		{
			dialogueCooldownTimer -= Time.deltaTime;
		}
		if ((!inBuyRelicScreen || !inObtainedRelicScreen) && !isSelecting)
		{
			SetCurrentPickup();
		}
		if (exitedRelicModal)
		{
			relicInteractTimer -= Time.deltaTime;
			if (relicInteractTimer <= 0f && !relicInteractable)
			{
				relicInteractable = true;
				relicInteractTimer = 0.25f;
				exitedRelicModal = false;
			}
		}
	}

	public void TryInteract(InputAction.CallbackContext context)
	{
		if (context.canceled && Time.timeScale != 0f && !DialogueSystem.Instance.inDialogue)
		{
			StartCoroutine(InteractWait());
		}
	}

	public void DoInteract()
	{
		if (!(currentInteract != null) || !(dialogueCooldownTimer <= 0f))
		{
			return;
		}
		currentInteract.Interact(playerHealth.gameObject);
		if (typeof(SpellPickup).IsAssignableFrom(currentInteract.GetType()))
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spell_Pickup", base.transform.position);
			if (typeof(SpecialSpell).IsAssignableFrom(((SpellPickup)currentInteract).spell.GetType()))
			{
				SpecialSpell newSpell = (SpecialSpell)((SpellPickup)currentInteract).spell;
				MakePickup(spellCasting.currentStance.spellSlots[0].spell);
				foreach (Stance stance in spellCasting.stances)
				{
					if (stance.enabled)
					{
						SpellSlot spellSlot = stance.spellSlots[0];
						if (spellSlot.spell != null && spellSlot.spell.charge && spellSlot.spell.chargeTimer > 0f)
						{
							spellCasting.ChargedSpell(spellSlot);
						}
						spellSlot.SetNewSpell(newSpell);
					}
				}
				Object.Destroy(currentInteract.gameObject);
				return;
			}
			for (int i = 1; i < spellCasting.currentStance.spellSlots.Count; i++)
			{
				if (spellCasting.currentStance.spellSlots[i].spell == null && spellCasting.currentStance.spellSlots[i].isUsable)
				{
					spellCasting.currentStance.spellSlots[i].SetNewSpell(((SpellPickup)currentInteract).spell);
					Object.Destroy(currentInteract.gameObject);
					return;
				}
			}
			if (!Death.Instance.isDead && !isSelecting)
			{
				spellSelectionScreen.SetActive(value: true);
				isSelecting = true;
				spellSelectionIcon.sprite = ((SpellPickup)currentInteract).spell.icon;
				cache = (SpellPickup)currentInteract;
				if (BossManager.Instance != null && BossManager.Instance.inBossFight && BossManager.Instance.GetCurrentBoss() == "oblome_boss_name")
				{
					Manager.Instance?.PauseGame(pauseMusic: false, player);
				}
				movement.enabled = false;
				BeginSpellPreview();
				Object.Destroy(currentInteract.gameObject);
			}
		}
		else if (typeof(HealthPickup).IsAssignableFrom(currentInteract.GetType()))
		{
			HealthPickup healthPickup = (HealthPickup)currentInteract;
			bool flag = playerHealth.Heal(healthPickup.healthRegain, healthPack: true);
			if (flag && !playerHealth.negateHealthPack)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Main Character/Health_Pickup", base.transform.position);
				Object.Destroy(currentInteract.gameObject);
			}
			else if (flag && playerHealth.negateHealthPack)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spell_Fail", base.transform.position);
				Object.Destroy(currentInteract.gameObject);
			}
			else
			{
				RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spell_Fail", base.transform.position);
			}
		}
		else if (typeof(BossRelicPickup).IsAssignableFrom(currentInteract.GetType()))
		{
			if (inBuyRelicScreen || inObtainedRelicScreen || !relicInteractable)
			{
				return;
			}
			BossRelicPickup bossRelicPickup = (BossRelicPickup)currentInteract;
			if (bossRelicPickup.randomRelic)
			{
				List<Relic> list = new List<Relic>(RunManager.Instance.availableRelics);
				List<Relic> negativeRelics = bossRelicPickup.negativeRelics;
				List<Relic> list2 = new List<Relic>();
				foreach (Relic item in negativeRelics)
				{
					if (!RelicCollector.Instance.Relics().Contains(item))
					{
						list2.Add(item);
					}
				}
				if (list.Count > 0)
				{
					if (list2.Count > 0)
					{
						if (Random.Range(0, 3) == 0)
						{
							Relic relic = negativeRelics[Random.Range(0, negativeRelics.Count)];
							while (RelicCollector.Instance.Relics().Contains(relic))
							{
								relic = negativeRelics[Random.Range(0, negativeRelics.Count)];
							}
							bossRelicPickup.relic = relic;
						}
						else
						{
							bossRelicPickup.relic = list[Random.Range(0, list.Count)];
						}
					}
					else
					{
						bossRelicPickup.relic = list[Random.Range(0, list.Count)];
					}
				}
				else
				{
					bossRelicPickup.relic = null;
				}
			}
			if (bossRelicPickup.relic == null)
			{
				return;
			}
			relicCache = new RelicPickup
			{
				relic = bossRelicPickup.relic,
				hasSearchedDeeper = true
			};
			relicInteractable = false;
			Manager.Instance?.PauseGame(pauseMusic: false, player);
			relicNameUI.text = bossRelicPickup.relic.name.value;
			relicEffectUI.text = bossRelicPickup.relic.effect.value;
			relicLoreUI.text = bossRelicPickup.relic.loreDescription.value;
			relicIconUI.sprite = bossRelicPickup.relic.icon;
			searchDeeperButtonObtainedRelicUI.interactable = false;
			Navigation navigation = keepButtonObtainedRelicUI.navigation;
			Navigation navigation2 = loseButtonObtainedRelicUI.navigation;
			navigation.selectOnRight = loseButtonObtainedRelicUI;
			navigation.selectOnLeft = loseButtonObtainedRelicUI;
			navigation2.selectOnLeft = keepButtonObtainedRelicUI;
			navigation2.selectOnRight = keepButtonObtainedRelicUI;
			if (bossRelicPickup.forceRelicPickup)
			{
				loseButtonObtainedRelicUI.interactable = false;
				navigation.selectOnLeft = null;
				navigation.selectOnRight = null;
			}
			else
			{
				loseButtonObtainedRelicUI.interactable = true;
			}
			keepButtonObtainedRelicUI.navigation = navigation;
			loseButtonObtainedRelicUI.navigation = navigation2;
			keepButtonObtainedRelicUI.Select();
			DungeonRoom dungeonRoom = WaveManager.Instance?.currentRoom;
			if (dungeonRoom != null && dungeonRoom.GetType() == typeof(EventRoom))
			{
				Volume component = GameObject.Find("EventRoomVolume").GetComponent<Volume>();
				if (component != null && component.profile != null && component.profile.TryGet<PaniniProjection>(out var component2))
				{
					component2.active = false;
				}
			}
			obtainedRelicModalUI.SetActive(value: true);
			inObtainedRelicScreen = true;
			MusicManager.EnterMenu();
		}
		else if (typeof(RelicPickup).IsAssignableFrom(currentInteract.GetType()))
		{
			if (inBuyRelicScreen || inObtainedRelicScreen || !relicInteractable)
			{
				return;
			}
			if (((RelicPickup)currentInteract).relic == null)
			{
				((RelicPickup)currentInteract).relic = RunManager.GetRelic();
			}
			if (((RelicPickup)currentInteract).relic == null)
			{
				Debug.LogWarning("No more available relics.");
				Object.Destroy(currentInteract.gameObject);
				return;
			}
			relicCache = (RelicPickup)currentInteract;
			int cost = ((RelicPickup)currentInteract).cost;
			Manager.Instance?.PauseGame(pauseMusic: false, player);
			if (!RelicCollector.Instance.CanAfford(cost))
			{
				searchButtonBuyRelicUI.interactable = false;
				cancelButtonBuyRelicUI.Select();
			}
			else
			{
				searchButtonBuyRelicUI.interactable = true;
				searchButtonBuyRelicUI.Select();
			}
			relicInteractable = false;
			FloorManager instance = FloorManager.Instance;
			if (instance != null && instance.currentFloor == 3)
			{
				relicBuyTitleUI.text = relicCache.relicBuyTitleFloor3.value;
				relicBuyDescriptionUI.text = relicCache.relicBuyDescriptionFloor3.value.Replace("[COST]", relicCache.cost.ToString());
			}
			else
			{
				relicBuyTitleUI.text = relicCache.relicBuyTitle.value;
				relicBuyDescriptionUI.text = relicCache.relicBuyDescription.value.Replace("[COST]", relicCache.cost.ToString());
			}
			buyRelicModalUI.SetActive(value: true);
			inBuyRelicScreen = true;
			MusicManager.EnterMenu();
		}
		else if (typeof(KnowledgePickup).IsAssignableFrom(currentInteract.GetType()))
		{
			RelicCollector.Instance.GainKnowledge(((KnowledgePickup)currentInteract).amount);
			if (knowledgeNumberIndicator != null)
			{
				Object.Instantiate(knowledgeNumberIndicator, base.transform.position + new Vector3(Random.insideUnitCircle.x * 0.2f, Random.insideUnitCircle.y * 0.2f, 0f), Quaternion.identity).GetComponent<NumberIndicator>().number = ((KnowledgePickup)currentInteract).amount.ToString();
			}
			Object.Destroy(currentInteract.gameObject);
		}
		else if (typeof(EventRoomLever).IsAssignableFrom(currentInteract.GetType()))
		{
			if (typeof(EventRoom).IsAssignableFrom(WaveManager.Instance.currentRoom.GetType()))
			{
				RuntimeManager.PlayOneShot("event:/SFX/World/Lever", currentInteract.gameObject.transform.position);
				EventRoom eventRoom = (EventRoom)WaveManager.Instance.currentRoom;
				if (((EventRoomLever)currentInteract).doesLeave)
				{
					((EventRoomLever)currentInteract).Leave();
					eventRoom.EndEvent();
				}
				else
				{
					eventRoom.StartEvent();
				}
				Object.Destroy(currentInteract.gameObject);
			}
			else
			{
				Debug.LogError("Current room is not an event room.");
			}
		}
		else if (typeof(SpellUpgradeInteract).IsAssignableFrom(currentInteract.GetType()))
		{
			spellCasting.Upgrade();
		}
		else if (typeof(MessageInteract).IsAssignableFrom(currentInteract.GetType()))
		{
			((MessageInteract)currentInteract).target.SendMessage(((MessageInteract)currentInteract).message);
			Object.Destroy(currentInteract.gameObject);
		}
		else if (typeof(OblomeHealthInteract).IsAssignableFrom(currentInteract.GetType()))
		{
			((OblomeHealthInteract)currentInteract).Pay();
		}
		else if (typeof(OblomeFinalFloorInteract).IsAssignableFrom(currentInteract.GetType()))
		{
			((OblomeFinalFloorInteract)currentInteract).Pay();
		}
		else if (typeof(EndlessPortalInteract).IsAssignableFrom(currentInteract.GetType()))
		{
			((EndlessPortalInteract)currentInteract).Loop();
		}
	}

	public IEnumerator InteractWait()
	{
		yield return new WaitForEndOfFrame();
		DoInteract();
	}

	private void SetCurrentPickup()
	{
		if (interactables.Count <= 0)
		{
			return;
		}
		if (interactables.Count == 1)
		{
			if (currentInteract != interactables[0] && interactables[0].isInteractable)
			{
				if (currentInteract != null)
				{
					currentInteract.OutOfRange();
				}
				currentInteract = interactables[0];
				currentInteract.InRange();
			}
			return;
		}
		Interactable interactable = null;
		float num = float.PositiveInfinity;
		Interactable[] array = interactables.ToArray();
		foreach (Interactable interactable2 in array)
		{
			if (interactable2 == null || interactable2.transform == null)
			{
				interactables.Remove(interactable2);
			}
			else if (interactable2.isInteractable)
			{
				float num2 = Vector3.Distance(base.transform.position, interactable2.transform.position);
				if (num2 < num)
				{
					num = num2;
					interactable = interactable2;
				}
			}
		}
		if (interactable == null)
		{
			if (currentInteract != null)
			{
				currentInteract.OutOfRange();
			}
			currentInteract = null;
		}
		else if (currentInteract != interactable)
		{
			if (currentInteract != null)
			{
				currentInteract.OutOfRange();
			}
			currentInteract = interactable;
			currentInteract.InRange();
		}
	}

	public void ChooseSpell(SpellSlot spellSlot, InputAction.CallbackContext context)
	{
		if (context.canceled && isSelecting && spellSlot.isUsable)
		{
			MakePickup(spellSlot.spell);
			if (spellSlot.spell.charge && spellSlot.spell.chargeTimer > 0f)
			{
				spellCasting.ChargedSpell(spellSlot, fromStanceSwitch: false, clear: true);
			}
			spellSlot.SetNewSpell(cache.spell);
			EndSelection();
		}
	}

	public void ChoosePrimary(InputAction.CallbackContext context)
	{
		ChooseSpell(spellCasting.currentStance.spellSlots[1], context);
	}

	public void ChooseSecondary(InputAction.CallbackContext context)
	{
		ChooseSpell(spellCasting.currentStance.spellSlots[2], context);
	}

	public void ChooseTertiary(InputAction.CallbackContext context)
	{
		ChooseSpell(spellCasting.currentStance.spellSlots[3], context);
	}

	public void ChooseQuaternary(InputAction.CallbackContext context)
	{
		ChooseSpell(spellCasting.currentStance.spellSlots[4], context);
	}

	public static void MenuKeepRelicButtonStatic()
	{
		GetActivePlayer().MenuKeepRelicButton();
	}

	public void MenuKeepRelicButton()
	{
		Manager.Instance?.UnpauseGame();
		obtainedRelicModalUI.SetActive(value: false);
		inObtainedRelicScreen = false;
		MusicManager.ExitMenu();
		RelicCollector.Instance.GetRelic(relicCache.relic);
		Object.Destroy(currentInteract.gameObject);
		currentInteract = null;
		inBuyRelicScreen = false;
		relicInteractable = true;
	}

	private void ReturnRelicToPool(Relic relic)
	{
		if (relic != null && !RunManager.Instance.availableRelics.Contains(relic))
		{
			RunManager.Instance.availableRelics.Add(relic);
		}
	}

	public static Interact GetActivePlayer()
	{
		return PlayerManager.Instance.lastActivePlayerInput.GetComponentInChildren<Interact>();
	}

	public static void MenuSearchButtonStatic()
	{
		GetActivePlayer().MenuSearchButton();
	}

	public void MenuSearchButton()
	{
		if (!inBuyRelicScreen)
		{
			return;
		}
		if (relicCache.relic == null)
		{
			Debug.LogWarning("No more available relics.");
			buyRelicModalUI.SetActive(value: false);
			obtainedRelicModalUI.SetActive(value: false);
			inBuyRelicScreen = false;
			inObtainedRelicScreen = false;
			Manager.Instance?.UnpauseGame();
			return;
		}
		RelicCollector.Instance.Buy(relicCache.cost);
		buyRelicModalUI.SetActive(value: false);
		inBuyRelicScreen = false;
		relicNameUI.text = relicCache.relic.name.value;
		relicEffectUI.text = relicCache.relic.effect.value;
		relicLoreUI.text = relicCache.relic.loreDescription.value;
		relicIconUI.sprite = relicCache.relic.icon;
		if ((bool)Object.FindObjectOfType<Tutorial>())
		{
			loseButtonObtainedRelicUI.interactable = false;
			searchDeeperButtonObtainedRelicUI.interactable = false;
			Navigation navigation = keepButtonObtainedRelicUI.navigation;
			navigation.selectOnRight = keepButtonObtainedRelicUI;
			navigation.selectOnLeft = keepButtonObtainedRelicUI;
			keepButtonObtainedRelicUI.navigation = navigation;
		}
		else
		{
			bool flag = !relicCache.hasSearchedDeeper && RelicCollector.Instance.CanAfford(5) && RunManager.GetAvailableRelicCount() + 1 > 1;
			searchDeeperButtonObtainedRelicUI.interactable = flag;
			Navigation navigation2 = keepButtonObtainedRelicUI.navigation;
			Navigation navigation3 = loseButtonObtainedRelicUI.navigation;
			if (flag)
			{
				navigation3.selectOnRight = keepButtonObtainedRelicUI;
				navigation3.selectOnLeft = searchDeeperButtonObtainedRelicUI;
				navigation2.selectOnLeft = loseButtonObtainedRelicUI;
				navigation2.selectOnRight = searchDeeperButtonObtainedRelicUI;
				Navigation navigation4 = searchDeeperButtonObtainedRelicUI.navigation;
				navigation4.selectOnLeft = keepButtonObtainedRelicUI;
				navigation4.selectOnRight = loseButtonObtainedRelicUI;
			}
			else
			{
				navigation2.selectOnRight = loseButtonObtainedRelicUI;
				navigation2.selectOnLeft = loseButtonObtainedRelicUI;
				navigation3.selectOnLeft = keepButtonObtainedRelicUI;
				navigation3.selectOnRight = keepButtonObtainedRelicUI;
			}
			keepButtonObtainedRelicUI.navigation = navigation2;
			loseButtonObtainedRelicUI.navigation = navigation3;
			loseButtonObtainedRelicUI.interactable = true;
		}
		keepButtonObtainedRelicUI.Select();
		obtainedRelicModalUI.SetActive(value: true);
		inObtainedRelicScreen = true;
		MusicManager.EnterMenu();
	}

	public static void MenuSearchDeeperButtonStatic()
	{
		GetActivePlayer().MenuSearchDeeperButton();
	}

	public void MenuSearchDeeperButton()
	{
		if (!inObtainedRelicScreen || !RelicCollector.Instance.CanAfford(5))
		{
			return;
		}
		RelicCollector.Instance.Buy(5);
		Relic relic = relicCache.relic;
		Relic relic2 = RunManager.GetRelic();
		if (relic2 != null)
		{
			if (relic != null && !RunManager.Instance.availableRelics.Contains(relic))
			{
				RunManager.Instance.availableRelics.Add(relic);
			}
			relicCache.relic = relic2;
			relicNameUI.text = relicCache.relic.name.value;
			relicEffectUI.text = relicCache.relic.effect.value;
			relicLoreUI.text = relicCache.relic.loreDescription.value;
			relicIconUI.sprite = relicCache.relic.icon;
			relicCache.hasSearchedDeeper = true;
			searchDeeperButtonObtainedRelicUI.interactable = false;
			Navigation navigation = keepButtonObtainedRelicUI.navigation;
			Navigation navigation2 = loseButtonObtainedRelicUI.navigation;
			navigation.selectOnRight = loseButtonObtainedRelicUI;
			navigation2.selectOnLeft = keepButtonObtainedRelicUI;
			navigation2.selectOnRight = keepButtonObtainedRelicUI;
			keepButtonObtainedRelicUI.navigation = navigation;
			loseButtonObtainedRelicUI.navigation = navigation2;
			keepButtonObtainedRelicUI.Select();
		}
		else
		{
			RelicCollector.Instance.GainKnowledge(5);
			if (relic != null && !RunManager.Instance.availableRelics.Contains(relic))
			{
				RunManager.Instance.availableRelics.Add(relic);
			}
			Debug.LogWarning("No more available relics.");
			obtainedRelicModalUI.SetActive(value: false);
			inObtainedRelicScreen = false;
			Manager.Instance?.UnpauseGame();
		}
	}

	public static void MenuCancelButtonStatic()
	{
		GetActivePlayer().MenuCancelButton();
	}

	public void MenuCancelButton()
	{
		if (inBuyRelicScreen)
		{
			if (relicCache != null && relicCache.relic != null)
			{
				ReturnRelicToPool(relicCache.relic);
			}
			buyRelicModalUI.SetActive(value: false);
			Manager.Instance?.UnpauseGame();
			inBuyRelicScreen = false;
			exitedRelicModal = true;
			MusicManager.ExitMenu();
		}
		if (inObtainedRelicScreen && loseButtonObtainedRelicUI.interactable)
		{
			if (relicCache != null && relicCache.relic != null)
			{
				ReturnRelicToPool(relicCache.relic);
			}
			exitedRelicModal = true;
			obtainedRelicModalUI.SetActive(value: false);
			Manager.Instance?.UnpauseGame();
			inObtainedRelicScreen = false;
			MusicManager.ExitMenu();
			Object.Destroy(currentInteract.gameObject);
			currentInteract = null;
		}
	}

	public void Cancel(InputAction.CallbackContext context)
	{
		if (context.canceled)
		{
			if (isSelecting)
			{
				MakePickup(cache.spell);
				EndSelection();
			}
			if (inBuyRelicScreen || inObtainedRelicScreen)
			{
				MenuCancelButton();
				RuntimeManager.PlayOneShot("event:/SFX/UI/Click", base.transform.position);
			}
		}
	}

	public void MakePickup(Spell spell)
	{
		if (spell != null)
		{
			Object.Instantiate(spellPickupPrefab, base.transform.position, Quaternion.identity).GetComponent<SpellPickup>().spell = spell;
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if ((bool)col.GetComponent<Interactable>())
		{
			interactables.Add(col.GetComponent<Interactable>());
		}
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		if ((bool)col.GetComponent<Interactable>())
		{
			if (currentInteract == col.GetComponent<Interactable>())
			{
				currentInteract = null;
				col.GetComponent<Interactable>().OutOfRange();
			}
			interactables.Remove(col.GetComponent<Interactable>());
		}
	}

	public void BeginSpellPreview()
	{
		if (cache != null)
		{
			tooltip.SetActive(value: true);
			tooltipName.text = cache.spell.name.value;
			tooltipDescription.text = cache.spell.description.value;
			tooltipCooldown.text = cache.spell.cooldownTime + "s";
		}
	}

	public void EndSpellPreview()
	{
		tooltip.SetActive(value: false);
	}

	private void OnDestroy()
	{
		if (DialogueSystem.Instance != null)
		{
			DialogueSystem.Instance.OnFinishDialogue -= OnDialogueFinished;
		}
	}
}
