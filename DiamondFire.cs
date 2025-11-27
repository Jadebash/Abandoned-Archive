using System.Collections.Generic;
using UnityEngine;

public class DiamondFire : StateMachineBehaviour
{
	public GameObject firePrefab;

	private List<GameObject> fires = new List<GameObject>();

	private float timer;

	private float timeBetweenFires = 0.8f;

	private float playerStartingHealth;

	private bool IsPositionValidDistance(Vector3 position, float minDistance)
	{
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject in players)
		{
			if (Vector2.Distance(position, gameObject.transform.position) < minDistance)
			{
				return false;
			}
		}
		return true;
	}

	private Vector3 GetRandomPositionInBounds(Animator animator)
	{
		float num = 1f;
		Vector3 vector2;
		while (true)
		{
			Vector2 vector = Random.insideUnitCircle * 9f;
			Vector3 position = new Vector3(vector.x, vector.y, 0f);
			vector2 = animator.transform.TransformPoint(position);
			Rodh component = animator.GetComponent<Rodh>();
			if (component != null)
			{
				vector2 = component.ClampPositionToBounds(vector2);
			}
			if (IsPositionValidDistance(vector2, num))
			{
				break;
			}
			num *= 0.95f;
		}
		return vector2;
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GreenFire[] array = Object.FindObjectsOfType<GreenFire>();
		foreach (GreenFire greenFire in array)
		{
			if (greenFire != null)
			{
				Object.Destroy(greenFire.gameObject);
			}
		}
		fires.Clear();
		timeBetweenFires = 0.8f;
		Rodh component = animator.GetComponent<Rodh>();
		if (component != null && component.attackHelper != null)
		{
			GameObject gameObject = PlayerManager.ClosestPlayer(animator.transform.position);
			if (gameObject != null)
			{
				component.attackHelper.FindPlayableSpaceBounds(gameObject.transform.position);
			}
		}
		for (int j = 0; j < 2; j++)
		{
			Vector3 randomPositionInBounds = GetRandomPositionInBounds(animator);
			GameObject item = Object.Instantiate(firePrefab, randomPositionInBounds, Quaternion.identity);
			fires.Add(item);
		}
		timer = 0f;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (timer > timeBetweenFires)
		{
			Vector3 randomPositionInBounds = GetRandomPositionInBounds(animator);
			GameObject item = Object.Instantiate(firePrefab, randomPositionInBounds, Quaternion.identity);
			fires.Add(item);
			timer = 0f;
			timeBetweenFires *= 0.9f;
			if (timeBetweenFires < 0.1f)
			{
				timeBetweenFires = 0.1f;
			}
			if (fires.Count >= 20)
			{
				animator.SetBool("OnFire", value: false);
			}
		}
		bool flag = true;
		foreach (GameObject fire in fires)
		{
			if (fire != null)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			animator.SetBool("OnFire", value: false);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		foreach (GameObject fire in fires)
		{
			if (fire != null)
			{
				fire.GetComponent<Animator>().SetTrigger("FadeOut");
			}
		}
	}
}
