using UnityEngine;

public class EnableAfterFrames : MonoBehaviour
{
	public GameObject target;

	public int framesToWait = 1;

	private int frames;

	private void Update()
	{
		if (frames != -1)
		{
			frames++;
			if (frames >= framesToWait)
			{
				target.SetActive(value: true);
				frames = -1;
			}
		}
	}
}
