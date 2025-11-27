using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicCollector : MonoBehaviour
{
	public delegate void KnowledgeGain(int knowledge);

	public delegate void KnowledgeSpend(int knowledge);

	public delegate void RelicPickup(Relic relic);

	public static RelicCollector Instance;

	public TextMeshProUGUI[] knowledgeText;

	public LocalisedString knowledgeString;

	[HideInInspector]
	public float knowledgeGainModifier = 1f;

	public int startingKnowledge;

	public Transform relicUIParent;

	public GameObject relicUIPrefab;

	public GameObject relicTooltip;

	public Image relicTooltipIcon;

	public TextMeshProUGUI relicTooltipName;

	public TextMeshProUGUI relicTooltipEffect;

	private Dictionary<Relic, GameObject> relics = new Dictionary<Relic, GameObject>();

	[HideInInspector]
	public int knowledge { get; private set; }

	public event KnowledgeGain OnGainKnowledge;

	public event KnowledgeSpend OnSpendKnowledge;

	public event RelicPickup OnRelicPickup;

	public bool CanAfford(int price)
	{
		return price <= knowledge;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		GainKnowledge(startingKnowledge);
		UpdateKnowledgeUI();
	}

	private void Update()
	{
		EffectsVisualiser.Instance?.SetEffectVisual(EffectsVisualiser.EffectType.Knowledge, knowledgeGainModifier);
	}

	public void GetRelic(Relic relic)
	{
		if (relics.ContainsKey(relic))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate(relicUIPrefab, relicUIParent);
		gameObject.transform.GetChild(0).GetComponent<Image>().sprite = relic.icon;
		relics.Add(relic, gameObject);
		relic.SetCollector(this);
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject2 in players)
		{
			if (gameObject2 != null)
			{
				relic.GainedRelic(gameObject2);
			}
		}
		this.OnRelicPickup?.Invoke(relic);
	}

	public void ShowRelicTooltip(GameObject uiInstance)
	{
		Relic key = relics.FirstOrDefault((KeyValuePair<Relic, GameObject> keyValuePair) => keyValuePair.Value == uiInstance).Key;
		if (!(key != null))
		{
			return;
		}
		relicTooltip.SetActive(value: true);
		relicTooltipName.text = key.name.value;
		relicTooltipIcon.sprite = key.icon;
		relicTooltipEffect.text = key.effect.value;
		relicTooltip.transform.position = uiInstance.transform.position;
		Canvas.ForceUpdateCanvases();
		Transform transform = relicTooltip.transform.Find("Pivot");
		RectTransform rectTransform = ((transform != null) ? transform.GetComponent<RectTransform>() : null);
		Canvas componentInParent = relicTooltip.GetComponentInParent<Canvas>();
		if (rectTransform != null && componentInParent != null)
		{
			RectTransform component = componentInParent.GetComponent<RectTransform>();
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			Vector3[] array2 = new Vector3[4];
			component.GetWorldCorners(array2);
			for (int num = 0; num < 4; num++)
			{
				array[num] = component.InverseTransformPoint(array[num]);
				array2[num] = component.InverseTransformPoint(array2[num]);
			}
			float x = array[2].x;
			float x2 = array2[2].x;
			if (x > x2)
			{
				float num2 = x - x2;
				Vector3 localPosition = relicTooltip.transform.localPosition;
				localPosition.x -= num2;
				relicTooltip.transform.localPosition = localPosition;
			}
		}
		else
		{
			relicTooltip.transform.position = uiInstance.transform.position;
		}
	}

	public void HideRelicTooltip()
	{
		relicTooltip.SetActive(value: false);
	}

	public List<Relic> Relics()
	{
		List<Relic> list = new List<Relic>();
		foreach (KeyValuePair<Relic, GameObject> relic in relics)
		{
			list.Add(relic.Key);
		}
		return list;
	}

	public void DropRelic(Relic relic)
	{
		Object.Destroy(relics[relic]);
		GameObject[] players = PlayerManager.players;
		foreach (GameObject player in players)
		{
			relic.LostRelic(player);
		}
		relics.Remove(relic);
	}

	public void RemoveFloorBoundRelics()
	{
		List<Relic> list = new List<Relic>();
		foreach (KeyValuePair<Relic, GameObject> relic in relics)
		{
			if (relic.Key.boundToFloor)
			{
				list.Add(relic.Key);
			}
		}
		foreach (Relic item in list)
		{
			DropRelic(item);
		}
	}

	public void AnimateRelic(Relic relic)
	{
		relics[relic].GetComponent<Animator>().SetTrigger("Use");
	}

	public void GainKnowledge(int knowledgeToGain)
	{
		int num = knowledgeToGain;
		knowledge += knowledgeToGain;
		if ((float)Random.Range(0, 100) < (knowledgeGainModifier - 1f) * 100f)
		{
			knowledge += knowledgeToGain;
			num += knowledgeToGain;
		}
		if (knowledge > 999)
		{
			int num2 = knowledge - 999;
			num -= num2;
			knowledge = 999;
		}
		if (num > 0)
		{
			this.OnGainKnowledge?.Invoke(num);
		}
		UpdateKnowledgeUI();
	}

	public void Buy(int price)
	{
		knowledge -= price;
		if (knowledge < 0)
		{
			Debug.LogWarning("Knowledge is less than 0.");
			knowledge = 0;
		}
		this.OnSpendKnowledge?.Invoke(price);
		UpdateKnowledgeUI();
	}

	public void BuyHealth(int price)
	{
		knowledge -= price;
		if (knowledge < 0)
		{
			Debug.LogWarning("Knowledge is less than 0.");
			knowledge = 0;
		}
		UpdateKnowledgeUI();
	}

	public void UpdateKnowledgeUI()
	{
		string text = knowledge.ToString();
		if (text.Length == 1)
		{
			text = "00" + text;
		}
		else if (text.Length == 2)
		{
			text = "0" + text;
		}
		TextMeshProUGUI[] array = knowledgeText;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = text;
		}
	}
}
