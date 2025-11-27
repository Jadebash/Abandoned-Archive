using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CoopButton : MonoBehaviour
{
	private void OnEnable()
	{
		if (PlayerInputManager.instance != null && PlayerInputManager.instance.joiningEnabled)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void EnableCoop()
	{
		PlayerInputManager.instance.EnableJoining();
		base.gameObject.SetActive(value: false);
		EventSystem.current.SetSelectedGameObject(GameObject.Find("BackButton"));
		TipManager.ShowTip("Press A to join");
	}
}
