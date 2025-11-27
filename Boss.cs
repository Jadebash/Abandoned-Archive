using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Boss : MonoBehaviour
{
	public new LocalisedString name;

	public EventReference bossMusicEvent;

	public bool phaseTwo;

	public bool onPhaseTwo;

	public float phaseTwoHealth = 100f;

	public float phaseTwoIntroLength = 1f;

	public EventReference bossMusicPhaseTwoEvent;

	public float scaleDifficultyWithTime;

	public GameObject knowledgeDrop;

	public GameObject bossRelicPickupPrefab;

	public Relic[] bossRelics;

	private Health health;

	private Animator animator;

	private Vector3 startingPos;

	private bool hasDied;

	public void Death(GameObject attacker)
	{
		if (hasDied)
		{
			return;
		}
		hasDied = true;
		if ((bool)base.gameObject.GetComponent<OblomeBoss>())
		{
			return;
		}
		BossManager.Instance.EndBossFight();
		StopAllCoroutines();
		base.enabled = false;
		MonoBehaviour[] components = GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in components)
		{
			if (monoBehaviour != this && monoBehaviour != health)
			{
				monoBehaviour.StopAllCoroutines();
				monoBehaviour.enabled = false;
			}
		}
		animator?.SetBool("Dead", value: true);
		if (knowledgeDrop != null)
		{
			Object.Instantiate(knowledgeDrop, base.transform.position, Quaternion.identity);
		}
		List<Relic> list = new List<Relic>();
		List<Relic> list2 = RelicCollector.Instance.Relics();
		Relic[] array = bossRelics;
		foreach (Relic item in array)
		{
			if (!list2.Contains(item) && RunManager.Instance.availableRelics.Contains(item))
			{
				list.Add(item);
			}
		}
		if (list.Count <= 0 || !(bossRelicPickupPrefab != null))
		{
			return;
		}
		Relic relic = list[Random.Range(0, list.Count)];
		BossRelicPickup component = Object.Instantiate(bossRelicPickupPrefab, base.transform.position, Quaternion.identity).GetComponent<BossRelicPickup>();
		if (component != null)
		{
			component.relic = relic;
			if (component.spriteIcon != null)
			{
				component.spriteIcon.sprite = relic.icon;
			}
		}
		RunManager.Instance.availableRelics.Remove(relic);
	}

	public void Start()
	{
		startingPos = base.transform.position;
		health = GetComponent<Health>();
		health.maxHealth *= FloorManager.Instance.currentLoop;
		health.health *= FloorManager.Instance.currentLoop;
		phaseTwoHealth *= FloorManager.Instance.currentLoop;
		health.OnDamage += TookDamage;
		health.OnHeal += delegate
		{
			BossManager.Instance.UpdateHealthbar(health.health, health.maxHealth);
		};
		health.OnDeath += Death;
		animator = GetComponent<Animator>();
		if (BossManager.Instance != null)
		{
			BossManager.Instance.StartBossFight(name, base.gameObject, bossMusicEvent);
			BossManager.Instance.UpdateHealthbar(health.health, health.maxHealth);
		}
		health.health = health.maxHealth;
	}

	public void TookDamage(float damage, bool causedDeath, GameObject beingAttacked, GameObject attacker = null)
	{
		BossManager.Instance.UpdateHealthbar(health.health, health.maxHealth);
		if (health.health < phaseTwoHealth && !onPhaseTwo && phaseTwo && !causedDeath)
		{
			animator?.SetFloat("TimeMultiplier", 1.2f);
			onPhaseTwo = true;
			if (animator != null && AnimatorHasTrigger(animator, "StartPhaseTwo"))
			{
				animator.SetTrigger("StartPhaseTwo");
				return;
			}
			BossManager.Instance.StartNewBossPhase(base.gameObject, bossMusicPhaseTwoEvent, name, phaseTwoIntroLength);
			if (phaseTwoIntroLength > 0f)
			{
				base.transform.position = startingPos;
			}
		}
		if (scaleDifficultyWithTime > 0f)
		{
			float num = animator?.GetFloat("TimeMultiplier") ?? 1f;
			float num2 = (health.maxHealth - health.health) / health.maxHealth;
			float num3 = 1f + scaleDifficultyWithTime * num2 - num;
			animator?.SetFloat("TimeMultiplier", num + num3);
		}
	}

	private bool AnimatorHasTrigger(Animator animator, string v)
	{
		AnimatorControllerParameter[] parameters = animator.parameters;
		foreach (AnimatorControllerParameter animatorControllerParameter in parameters)
		{
			if (animatorControllerParameter.type == AnimatorControllerParameterType.Trigger && animatorControllerParameter.name == v)
			{
				return true;
			}
		}
		return false;
	}

	public void StartPhaseTwo()
	{
		BossManager.Instance.StartNewBossPhase(base.gameObject, bossMusicPhaseTwoEvent, name, phaseTwoIntroLength);
		base.transform.position = startingPos;
		onPhaseTwo = true;
	}

	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}
