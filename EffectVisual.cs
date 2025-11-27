using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EffectVisual
{
	public string name;

	public TweenAnimate increase;

	public TweenAnimate decrease;

	[HideInInspector]
	public Image increaseImage;

	[HideInInspector]
	public Image decreaseImage;

	[HideInInspector]
	public float currentValue = 1f;
}
