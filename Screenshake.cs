using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Screenshake : MonoBehaviour
{
	public static Screenshake Instance;

	[Header("Variables")]
	public float speed = 16f;

	public float curve = 0.3f;

	public float polynomial = 2f;

	public float amount = 0.04f;

	public float reductionSpeed = 1f;

	[Space(10f)]
	public bool rumbleController = true;

	private float trauma;

	private Vector3 offset;

	private float timeCounter;

	private bool doingTimeImpact;

	private void Start()
	{
		Instance = this;
		offset = base.transform.position;
	}

	private void Update()
	{
		timeCounter += Time.deltaTime * Mathf.Pow(trauma, curve) * speed;
		trauma -= Time.deltaTime * reductionSpeed;
		trauma = Mathf.Clamp01(trauma);
		bool flag = false;
		foreach (ControllerAim instance in ControllerAim.Instances)
		{
			if (instance != null && instance.usingController)
			{
				flag = true;
			}
		}
		if (trauma == 0f && rumbleController && flag && base.transform != null)
		{
			base.transform.localPosition = offset;
			if (Gamepad.current != null)
			{
				Gamepad.current.SetMotorSpeeds(0f, 0f);
			}
		}
		else
		{
			Shake();
		}
	}

	private void Shake()
	{
		float num = 1f;
		if (SaveManager.Instance != null)
		{
			num = SaveManager.Instance.currentSave.screenshakeIntensity;
		}
		float num2 = amount * Mathf.Pow(trauma, polynomial) * PerlinNoise(10f) * num;
		float num3 = amount * Mathf.Pow(trauma, polynomial) * PerlinNoise(20f) * num;
		if (float.IsNaN(num2) || float.IsNaN(num3))
		{
			return;
		}
		base.transform.localPosition = new Vector3(num2, num3, 0f) + offset;
		bool flag = false;
		foreach (ControllerAim instance in ControllerAim.Instances)
		{
			if (instance.usingController)
			{
				flag = true;
			}
		}
		if (rumbleController && flag)
		{
			Gamepad.current.SetMotorSpeeds(trauma / 2f * Time.timeScale * num, trauma / 2f * Time.timeScale * num);
		}
	}

	private float PerlinNoise(float seed)
	{
		return Mathf.PerlinNoise(seed, timeCounter) * 2f - 1f;
	}

	public void AddTrauma(float amount)
	{
		trauma += amount;
	}

	public void ResetTrauma()
	{
		trauma = 0f;
	}

	public void TimeImpact(float freezeTime = 0.1f)
	{
		if (!doingTimeImpact)
		{
			doingTimeImpact = true;
			StartCoroutine(FreezeTime(freezeTime));
		}
	}

	private IEnumerator FreezeTime(float freezeTime)
	{
		float previousTimeScale = Time.timeScale;
		Time.timeScale = 0.02f;
		yield return new WaitForSeconds(freezeTime * Time.timeScale);
		while (Time.timeScale == 0f)
		{
			yield return null;
		}
		if (Mathf.Abs(Time.timeScale - 0.02f) < 0.001f)
		{
			Time.timeScale = previousTimeScale;
		}
		doingTimeImpact = false;
	}

	private void OnDestroy()
	{
		Gamepad.current?.SetMotorSpeeds(0f, 0f);
	}
}
