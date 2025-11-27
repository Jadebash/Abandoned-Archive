using UnityEngine;

public class Tween : MonoBehaviour
{
	public bool animateOnEnable;

	public float value;

	public float time = 0.5f;

	private void OnEnable()
	{
		if (animateOnEnable)
		{
			Animate();
		}
	}

	public void Animate()
	{
		LeanTween.moveY(base.gameObject.GetComponent<RectTransform>(), value, time);
	}
}
