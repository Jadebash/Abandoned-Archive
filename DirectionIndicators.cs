using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DirectionIndicators : MonoBehaviour
{
	public static DirectionIndicators Instance;

	[SerializeField]
	private GameObject indicatorUIPrefab;

	private Dictionary<Transform, RectTransform> indicators = new Dictionary<Transform, RectTransform>();

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		Transform[] array = indicators.Keys.ToArray();
		foreach (Transform objectTransform in array)
		{
			PositionIndicator(objectTransform);
		}
	}

	private void PositionIndicator(Transform objectTransform)
	{
		if (objectTransform == null)
		{
			RemoveIndicator(objectTransform);
			return;
		}
		Vector3 to = objectTransform.position - base.transform.position;
		to.z = 0f;
		if (to.magnitude > 5f)
		{
			indicators[objectTransform].gameObject.SetActive(value: true);
			indicators[objectTransform].localPosition = to.normalized * 300f;
			Vector3 localEulerAngles = indicators[objectTransform].localEulerAngles;
			localEulerAngles.z = Vector3.SignedAngle(Vector3.up, to, Vector3.forward);
			indicators[objectTransform].localEulerAngles = localEulerAngles;
		}
		else
		{
			indicators[objectTransform].gameObject.SetActive(value: false);
		}
	}

	public void AddIndicator(Transform objectTransform, Color color)
	{
		if (!indicators.ContainsKey(objectTransform))
		{
			GameObject gameObject = Object.Instantiate(indicatorUIPrefab, base.transform);
			gameObject.GetComponent<Image>().color = color;
			indicators.Add(objectTransform, gameObject.GetComponent<RectTransform>());
			PositionIndicator(objectTransform);
		}
	}

	public void RemoveIndicator(Transform objectTransform)
	{
		if (indicators.ContainsKey(objectTransform))
		{
			Object.Destroy(indicators[objectTransform].gameObject);
			indicators.Remove(objectTransform);
		}
	}

	public bool HasIndicator(Transform objectTransform)
	{
		return indicators.ContainsKey(objectTransform);
	}
}
