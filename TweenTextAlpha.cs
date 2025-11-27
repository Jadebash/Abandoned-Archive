using TMPro;
using UnityEngine;

public class TweenTextAlpha : MonoBehaviour
{
	public bool onEnable = true;

	public TextMeshProUGUI text;

	public float duration = 1f;

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void OnEnable()
	{
		text = GetComponent<TextMeshProUGUI>();
		if (onEnable)
		{
			text.alpha = 0f;
			LeanTween.value(base.gameObject, FadeText, 0f, 1f, duration);
		}
	}

	private void OnDisable()
	{
		LeanTween.cancel(base.gameObject);
	}

	public void FadeText(float alpha)
	{
		text.alpha = alpha;
	}
}
