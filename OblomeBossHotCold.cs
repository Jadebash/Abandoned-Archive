using FMODUnity;
using TMPro;
using UnityEngine;

public class OblomeBossHotCold : StateMachineBehaviour
{
	private Vector3 location;

	private SpriteRenderer spriteRenderer;

	private GameObject player;

	private OblomeBoss boss;

	private bool foundLocation;

	private float fadeTimer;

	private GameObject speech;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		location = animator.gameObject.transform.position + new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-7.5f, 7.5f), 0f);
		while (Vector2.Distance(location, PlayerManager.ClosestPlayer(location).transform.position) < 2.5f)
		{
			location = animator.gameObject.transform.position + new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-7.5f, 7.5f), 0f);
		}
		spriteRenderer = animator.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		boss = animator.gameObject.GetComponent<OblomeBoss>();
		foundLocation = false;
		Color color = spriteRenderer.color;
		color.a = 0f;
		spriteRenderer.color = color;
		fadeTimer = 0f;
		speech = boss.hotColdHazeInstance.GetComponent<HotCold>().speech;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		float num = Vector3.Distance(player.transform.position, location);
		float num2 = 7.5f;
		float t = Mathf.Clamp01(num / num2);
		Color red = Color.red;
		Color green = Color.green;
		if (stateInfo.normalizedTime <= 0.25f)
		{
			float a = stateInfo.normalizedTime * 4f;
			if (boss.hotColdHazeInstance != null && !foundLocation)
			{
				Color color = Color.Lerp(green, red, t);
				color.a = a;
				boss.hotColdHazeInstance.GetComponent<SpriteRenderer>().color = color;
			}
			foreach (Transform item in speech.transform)
			{
				Color color2 = item.GetComponent<TextMeshProUGUI>().color;
				color2.a = a;
				item.GetComponent<TextMeshProUGUI>().color = color2;
			}
		}
		else if (stateInfo.normalizedTime >= 0.75f && !foundLocation)
		{
			if (boss.hotColdHazeInstance != null)
			{
				float a2 = 1f - (stateInfo.normalizedTime - 0.75f) * 4f;
				Color color3 = Color.Lerp(green, red, t);
				color3.a = a2;
				boss.hotColdHazeInstance.GetComponent<SpriteRenderer>().color = color3;
				foreach (Transform item2 in speech.transform)
				{
					if (item2.GetComponent<TextMeshProUGUI>().color != Color.red)
					{
						item2.GetComponent<TextMeshProUGUI>().color = Color.red;
					}
					Color color4 = item2.GetComponent<TextMeshProUGUI>().color;
					color4.a = a2;
					item2.GetComponent<TextMeshProUGUI>().color = color4;
				}
			}
		}
		else if (boss.hotColdHazeInstance != null && !foundLocation)
		{
			Color color5 = Color.Lerp(green, red, t);
			color5.a = 1f;
			boss.hotColdHazeInstance.GetComponent<SpriteRenderer>().color = color5;
		}
		if (num <= 2f && !foundLocation)
		{
			foundLocation = true;
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/HotColdDing");
			foreach (Transform item3 in speech.transform)
			{
				if (item3.GetComponent<TextMeshProUGUI>().color != Color.green)
				{
					item3.GetComponent<TextMeshProUGUI>().color = Color.green;
				}
			}
		}
		if (foundLocation && boss.hotColdHazeInstance != null && fadeTimer <= 1f)
		{
			fadeTimer += Time.deltaTime;
			Color color6 = boss.hotColdHazeInstance.GetComponent<SpriteRenderer>().color;
			Debug.Log(fadeTimer);
			color6.a = 1f - fadeTimer;
			boss.hotColdHazeInstance.GetComponent<SpriteRenderer>().color = color6;
			foreach (Transform item4 in speech.transform)
			{
				Color color7 = item4.GetComponent<TextMeshProUGUI>().color;
				color7.a = 1f - fadeTimer;
				item4.GetComponent<TextMeshProUGUI>().color = color7;
			}
		}
		if (boss.hotColdHazeInstance != null && boss.hotColdHazeInstance.GetComponent<SpriteRenderer>().color.a <= 0f)
		{
			fadeTimer = 0f;
			boss.HotCold();
			animator.SetTrigger("ForceIdle");
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!foundLocation && boss.gameObject.GetComponent<Health>().health > 0f)
		{
			boss.SpawnRandomEnemy();
			boss.SpawnRandomEnemy();
		}
		fadeTimer = 0f;
	}
}
