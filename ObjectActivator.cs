using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
	[SerializeField]
	private string activatorTag;

	[SerializeField]
	private bool deactivateOnExit;

	[SerializeField]
	private GameObject[] objects;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(activatorTag))
		{
			GameObject[] array = objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (deactivateOnExit && collision.CompareTag(activatorTag))
		{
			GameObject[] array = objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}
}
