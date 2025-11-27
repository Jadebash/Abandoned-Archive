using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectionFixer : MonoBehaviour
{
	public bool selectOnEnable;

	private void Start()
	{
		if (base.gameObject.activeInHierarchy && selectOnEnable)
		{
			GetComponent<Button>().Select();
			GetComponent<Button>().OnSelect(null);
		}
	}

	private void OnEnable()
	{
		if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == base.gameObject)
		{
			GetComponent<Button>().OnSelect(null);
		}
		if (selectOnEnable)
		{
			GetComponent<Button>().Select();
			GetComponent<Button>().OnSelect(null);
		}
	}
}
