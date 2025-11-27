using UnityEngine;

public class NailArm : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (base.transform.parent.transform.parent.GetComponent<SpriteRenderer>().flipX && !GetComponent<SpriteRenderer>().flipX)
		{
			GetComponent<SpriteRenderer>().flipX = true;
		}
		else if (!base.transform.parent.transform.parent.GetComponent<SpriteRenderer>().flipX && GetComponent<SpriteRenderer>().flipX)
		{
			GetComponent<SpriteRenderer>().flipX = false;
		}
	}
}
