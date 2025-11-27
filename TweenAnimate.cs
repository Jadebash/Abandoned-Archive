using System;
using TMPro;
using UnityEngine;

public class TweenAnimate : MonoBehaviour
{
	public float duration = 0.5f;

	public bool animateScale;

	public bool animateTextAlpha;

	[HideInInspector]
	public bool animatingIn;

	[HideInInspector]
	public bool animatingOut;

	private TextMeshProUGUI _text;

	private void Awake()
	{
		if (animateTextAlpha)
		{
			_text = GetComponent<TextMeshProUGUI>();
			if (_text == null)
			{
				Debug.LogWarning("TweenAnimate on " + base.gameObject.name + ": animateTextAlpha is enabled but no TextMeshProUGUI component found!");
			}
		}
	}

	public void AnimateIn()
	{
		if (!animatingIn)
		{
			base.gameObject.SetActive(value: true);
			LeanTween.cancel(base.gameObject);
			animatingIn = true;
			animatingOut = false;
			if (animateScale)
			{
				base.transform.localScale = Vector3.zero;
				LeanTween.scale(base.gameObject, Vector3.one, duration).setEase(LeanTweenType.easeInQuad);
			}
			if (animateTextAlpha)
			{
				LeanTween.value(base.gameObject, TextAlpha, 0f, 1f, duration).setEase(LeanTweenType.easeInQuad);
			}
		}
	}

	public void AnimateOut()
	{
		if (animatingOut)
		{
			return;
		}
		LeanTween.cancel(base.gameObject);
		animatingOut = true;
		animatingIn = false;
		if (animateScale)
		{
			base.transform.localScale = Vector3.one;
			LeanTween.scale(base.gameObject, Vector3.zero, duration).setEase(LeanTweenType.easeInQuad).setOnComplete((Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
		if (animateTextAlpha)
		{
			LeanTween.value(base.gameObject, TextAlpha, 1f, 0f, duration).setEase(LeanTweenType.easeInQuad).setOnComplete((Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
	}

	public void TextAlpha(float alpha)
	{
		if (_text != null)
		{
			_text.alpha = alpha;
		}
	}
}
