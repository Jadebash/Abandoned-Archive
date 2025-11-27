using FMODUnity;
using UnityEngine;

public class FinalFloorDetector : MonoBehaviour
{
	public enum side
	{
		Left = 0,
		Right = 1
	}

	public FinalFloor floorManager;

	public side roomDirection;

	private bool bossSpawned;

	public GameObject oblomePlaceholder;

	private float startingHealth;

	[Header("Oblome Dialogues")]
	public Dialogue dialogue1;

	public Dialogue dialogue2;

	private Health oblomeHealth;

	private float cumulativeDamage;

	private bool dialogueRunning;

	private bool phase1Triggered;

	private bool phase2Triggered;

	private readonly float phase1Threshold = 10f;

	private readonly float phase2Threshold = 40f;

	private readonly float bossThreshold = 75f;

	private void Start()
	{
		bossSpawned = false;
		if (oblomePlaceholder != null)
		{
			oblomeHealth = oblomePlaceholder.GetComponent<Health>();
			if (oblomeHealth != null)
			{
				startingHealth = oblomeHealth.health;
				oblomeHealth.OnDamage += OnOblomeDamage;
			}
		}
		RuntimeManager.StudioSystem.setParameterByName("Intensity", 100f);
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		if (oblomeHealth != null)
		{
			oblomeHealth.OnDamage -= OnOblomeDamage;
		}
	}

	private void OnOblomeDamage(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker)
	{
		GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
		if (gameObject != null && !gameObject.GetComponent<Collider2D>().IsTouching(GetComponent<Collider2D>()))
		{
			oblomeHealth.health = startingHealth;
		}
		else if (!bossSpawned && !dialogueRunning && !(damage <= 0f))
		{
			cumulativeDamage += damage;
			if (!phase1Triggered && cumulativeDamage >= phase1Threshold)
			{
				phase1Triggered = true;
				TriggerDialogue(dialogue1, phase1Threshold);
			}
			else if (phase1Triggered && !phase2Triggered && cumulativeDamage >= phase2Threshold)
			{
				phase2Triggered = true;
				TriggerDialogue(dialogue2, phase2Threshold);
			}
			else if (phase2Triggered && cumulativeDamage >= bossThreshold)
			{
				StartBossFight();
			}
		}
	}

	private void TriggerDialogue(Dialogue dialogue, float clampDamageAt)
	{
		dialogueRunning = true;
		cumulativeDamage = clampDamageAt;
		if (oblomeHealth != null)
		{
			oblomeHealth.invincible = true;
			float num = startingHealth - clampDamageAt;
			if (oblomeHealth.health < num)
			{
				oblomeHealth.health = num;
			}
		}
		DialogueSystem instance = DialogueSystem.Instance;
		if (dialogue != null && instance != null)
		{
			instance.StartDialogue(dialogue);
			instance.OnFinishDialogue += OnDialogueFinished;
		}
		else
		{
			OnDialogueFinished(null);
		}
	}

	private void OnDialogueFinished(Dialogue dialogue)
	{
		DialogueSystem instance = DialogueSystem.Instance;
		if (instance != null)
		{
			instance.OnFinishDialogue -= OnDialogueFinished;
		}
		dialogueRunning = false;
		if (oblomeHealth != null)
		{
			oblomeHealth.invincible = false;
		}
	}

	private void StartBossFight()
	{
		if (!bossSpawned)
		{
			if (oblomeHealth != null)
			{
				oblomeHealth.OnDamage -= OnOblomeDamage;
			}
			if (oblomePlaceholder != null)
			{
				Object.Destroy(oblomePlaceholder);
			}
			base.transform.parent.GetComponent<RoomInfo>().downBarrier.SetActive(value: true);
			base.transform.parent.GetComponent<RoomInfo>().upBarrier.SetActive(value: true);
			Object.Instantiate(floorManager.OblomeBoss, base.transform.parent.gameObject.transform);
			bossSpawned = true;
			Object.Destroy(base.gameObject);
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player" && !bossSpawned && roomDirection == side.Right)
		{
			base.transform.parent.GetComponent<RoomInfo>().leftBlock.SetActive(value: true);
			base.transform.parent.GetComponent<RoomInfo>().leftBarrier.SetActive(value: true);
			Object.Instantiate(floorManager.MysteriousVoiceBoss, base.transform.parent.gameObject.transform);
			bossSpawned = true;
			Object.Destroy(base.gameObject);
		}
	}
}
