using UnityEngine;

public class AnimationEventHelper : MonoBehaviour
{
	public GameObject sendMessageReceiver;

	public void DisableSelf()
	{
		base.gameObject.SetActive(value: false);
	}

	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}

	public void DestroyParent()
	{
		Object.Destroy(base.transform.parent.gameObject);
	}

	public void DetachSelfAndDestroyParent()
	{
		Transform parent = base.transform.parent;
		base.transform.parent = null;
		if (parent != null)
		{
			Object.Destroy(parent.gameObject);
		}
	}

	public void SendMessageToReceiver(string message)
	{
		if (sendMessageReceiver != null)
		{
			sendMessageReceiver.SendMessage(message);
		}
	}
}
