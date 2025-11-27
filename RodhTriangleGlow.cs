using UnityEngine;

public class RodhTriangleGlow : StateMachineBehaviour
{
	private GameObject[] spearGlows;

	private int count;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Entered Glowing");
		count = 0;
		spearGlows = new GameObject[3];
		int num = 0;
		foreach (Transform item in animator.transform)
		{
			if (item.name.Contains("TriangleGlow"))
			{
				spearGlows[num] = item.gameObject;
				num++;
				if (num >= 3)
				{
					break;
				}
			}
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GameObject[] array = spearGlows;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].activeSelf)
			{
				return;
			}
		}
		if (count < 3)
		{
			int num = Random.Range(0, spearGlows.Length);
			spearGlows[num].SetActive(value: true);
			count++;
		}
		else if (count >= 3 && count < 5)
		{
			int num2 = Random.Range(0, spearGlows.Length);
			int num3 = Random.Range(0, spearGlows.Length);
			while (num2 == num3)
			{
				num3 = Random.Range(0, spearGlows.Length);
			}
			spearGlows[num2].SetActive(value: true);
			spearGlows[num3].SetActive(value: true);
			count++;
		}
		else if (count >= 5)
		{
			array = spearGlows;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			animator.SetBool("TriangleGlow", value: false);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		count = 0;
	}
}
