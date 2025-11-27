using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TipManager : MonoBehaviour
{
	public static TipManager Instance;

	[HideInInspector]
	public TextMeshProUGUI text;

	[HideInInspector]
	public TweenAnimate animate;

	[HideInInspector]
	public PlayerInput playerInput;

	private void Awake()
	{
		Instance = this;
		text = base.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		animate = base.transform.GetChild(0).GetComponent<TweenAnimate>();
		playerInput = UnityEngine.Object.FindObjectOfType<PlayerInput>();
	}

	public static void ShowTip(string tip, float time = -1f)
	{
		if (Instance == null)
		{
			return;
		}
		Instance.animate.AnimateIn();
		string text = "";
		string[] array = tip.Split('[');
		if (array.Length > 1)
		{
			text += array[0];
			for (int i = 1; i < array.Length; i++)
			{
				string text2 = array[i].Split(']')[0];
				try
				{
					text += Instance.playerInput.actions[text2].GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
				}
				catch (Exception)
				{
					text += text2;
				}
				text += array[i].Split(']')[1];
			}
		}
		else
		{
			text = tip;
		}
		Instance.text.text = text;
		if (time > 0f)
		{
			LeanTween.delayedCall(Instance.gameObject, time, (Action)delegate
			{
				HideTip();
			});
		}
	}

	public static void HideTip()
	{
		if (!(Instance == null))
		{
			Instance.animate.AnimateOut();
		}
	}
}
