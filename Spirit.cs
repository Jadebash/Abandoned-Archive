using System;
using UnityEngine;

public class Spirit : MonoBehaviour
{
	public enum SpiritColor
	{
		Red = 0,
		Blue = 1,
		Green = 2
	}

	public SpiritColor color;

	private Transform player;

	private float timer;

	private Health health;

	private SpriteRenderer sr;

	private bool enraged;

	private AttackHelper attackHelper;

	private bool disabling;

	private void Start()
	{
		attackHelper = GetComponent<AttackHelper>();
		if (attackHelper == null)
		{
			attackHelper = base.gameObject.AddComponent<AttackHelper>();
		}
		attackHelper.FindPlayableSpaceBounds();
		player = PlayerManager.ClosestPlayer(base.transform.position).transform;
		health = GetComponent<Health>();
		health.OnDeath += OnDeath;
	}

	private void OnEnable()
	{
		LeanTween.cancel(base.gameObject);
		sr = GetComponent<SpriteRenderer>();
		timer = 0f;
		Color color = sr.color;
		color.a = 0f;
		sr.color = color;
		LeanTween.value(base.gameObject, 0f, 1f, 0.25f).setOnUpdate(delegate(float val)
		{
			Color color2 = sr.color;
			color2.a = val;
			sr.color = color2;
		}).setEase(LeanTweenType.easeInOutSine);
		disabling = false;
	}

	private void OnDisable()
	{
		if (health != null)
		{
			health.health = health.maxHealth;
			health.ResetHasDied();
		}
	}

	public void OnDeath(GameObject attacker)
	{
		LeanTween.cancel(base.gameObject);
		disabling = true;
		LeanTween.value(base.gameObject, 1f, 0f, 0.1f).setOnUpdate(delegate(float val)
		{
			Color color = sr.color;
			color.a = val;
			sr.color = color;
		}).setEase(LeanTweenType.easeInOutSine)
			.setOnComplete((Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
	}

	private void Update()
	{
		switch (color)
		{
		case SpiritColor.Red:
			base.transform.Translate((player.transform.position - base.transform.position).normalized * 2f * Time.deltaTime);
			base.transform.position = attackHelper.ClampPositionToBounds(base.transform.position);
			timer += Time.deltaTime;
			if (timer > 3.5f && !disabling)
			{
				disabling = true;
				LeanTween.value(base.gameObject, 1f, 0f, 0.25f).setOnUpdate(delegate(float val)
				{
					Color color = sr.color;
					color.a = val;
					sr.color = color;
				}).setEase(LeanTweenType.easeInOutSine)
					.setOnComplete((Action)delegate
					{
						base.gameObject.SetActive(value: false);
					});
			}
			break;
		case SpiritColor.Blue:
			if (health.health == 999f)
			{
				base.gameObject.GetComponent<DamageOnEnter>().damage = 0f;
				sr.color = Color.blue;
				base.transform.Translate((player.transform.position - base.transform.position).normalized * 0.25f * Time.deltaTime);
				base.transform.position = attackHelper.ClampPositionToBounds(base.transform.position);
			}
			else
			{
				base.gameObject.GetComponent<DamageOnEnter>().damage = 20f;
				health.health = 1f;
				sr.color = Color.red;
				base.transform.Translate((player.transform.position - base.transform.position).normalized * 3.5f * Time.deltaTime);
				base.transform.position = attackHelper.ClampPositionToBounds(base.transform.position);
			}
			timer += Time.deltaTime;
			if (timer > 20f && !disabling)
			{
				disabling = true;
				LeanTween.value(base.gameObject, 1f, 0f, 0.25f).setOnUpdate(delegate(float val)
				{
					Color color = sr.color;
					color.a = val;
					sr.color = color;
				}).setEase(LeanTweenType.easeInOutSine)
					.setOnComplete((Action)delegate
					{
						health.health = 999f;
						base.gameObject.transform.parent.gameObject.SetActive(value: false);
					});
			}
			break;
		case SpiritColor.Green:
		{
			Transform transform = base.transform.parent.transform.parent.gameObject.transform;
			base.transform.Translate(-(player.transform.position - base.transform.position).normalized * 0.25f * Time.deltaTime);
			base.transform.position = attackHelper.ClampPositionToBounds(base.transform.position);
			timer += Time.deltaTime;
			if (timer > 10f)
			{
				base.transform.Translate((transform.position - base.transform.position).normalized * 10f * Time.deltaTime);
				base.transform.position = attackHelper.ClampPositionToBounds(base.transform.position);
			}
			if (timer > 10.5f && !disabling)
			{
				if (base.gameObject.activeSelf)
				{
					transform.gameObject.GetComponent<Health>().Heal(8f);
				}
				disabling = true;
				LeanTween.value(base.gameObject, 1f, 0f, 0.25f).setOnUpdate(delegate(float val)
				{
					Color color = sr.color;
					color.a = val;
					sr.color = color;
				}).setEase(LeanTweenType.easeInOutSine)
					.setOnComplete((Action)delegate
					{
						base.gameObject.SetActive(value: false);
					});
			}
			break;
		}
		default:
			Debug.Log("Invalid Spirit Color");
			break;
		}
	}
}
