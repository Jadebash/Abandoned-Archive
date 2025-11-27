using UnityEngine;

public class RodhSpear : MonoBehaviour
{
	private Rigidbody2D rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		base.transform.localScale = new Vector3(3f, 3f, 1f);
	}

	private void Update()
	{
		rb.velocity = base.transform.right * 50f;
	}
}
