using UnityEngine;

public class DestroyOnRodhFightEnd : MonoBehaviour
{
	private Rodh rodh;

	private void Start()
	{
		rodh = Object.FindObjectOfType<Rodh>();
	}

	private void Update()
	{
		if (rodh.ended)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
