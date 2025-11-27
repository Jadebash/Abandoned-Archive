using UnityEngine;
using UnityEngine.UI;

public class InheritSpriteFromParent : MonoBehaviour
{
	[ExecuteInEditMode]
	private void Start()
	{
		Inherit();
	}

	[ExecuteInEditMode]
	private void OnEnable()
	{
		Inherit();
	}

	public void Inherit()
	{
		GetComponent<Image>().sprite = base.transform.parent.GetComponent<Image>().sprite;
	}
}
