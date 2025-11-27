using UnityEngine;

public class RodhWispSpear : MonoBehaviour
{
	public float speed;

	private void Start()
	{
		base.transform.localScale = new Vector3(2f, 2f, 1f);
	}

	private void FixedUpdate()
	{
		base.transform.position += base.transform.right * speed * Time.fixedDeltaTime;
	}
}
