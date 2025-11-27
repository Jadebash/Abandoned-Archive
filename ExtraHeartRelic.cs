using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Extra Heart Relic")]
public class ExtraHeartRelic : Relic
{
	public Sprite heartUIExtraHeart;

	public Sprite heartUI;

	public Sprite heartUIFour;

	public override void GainedRelic(GameObject player)
	{
		player.GetComponent<Health>().maxHealth += 20f;
		GameObject gameObject = GameObject.Find("Hearts");
		GameObject gameObject2 = gameObject.transform.parent.gameObject;
		if (player.GetComponent<Health>().maxHealth == 120f)
		{
			gameObject2.GetComponent<Image>().sprite = heartUIExtraHeart;
			gameObject2.GetComponent<Image>().SetNativeSize();
			gameObject.transform.GetChild(5).gameObject.SetActive(value: true);
			Transform transform = GameObject.Find("PlayerManager").GetComponent<RelicCollector>().relicUIParent.transform;
			transform.localPosition = new Vector3(transform.localPosition.x + 50f, transform.localPosition.y, transform.localPosition.z);
		}
		else if (player.GetComponent<Health>().maxHealth == 100f)
		{
			gameObject2.GetComponent<Image>().sprite = heartUI;
			gameObject2.GetComponent<Image>().SetNativeSize();
			gameObject.transform.GetChild(5).gameObject.SetActive(value: false);
			gameObject.transform.GetChild(4).gameObject.SetActive(value: true);
		}
		else if (player.GetComponent<Health>().maxHealth == 80f)
		{
			gameObject2.GetComponent<Image>().sprite = heartUIFour;
			gameObject2.GetComponent<Image>().SetNativeSize();
			gameObject.transform.GetChild(5).gameObject.SetActive(value: false);
			gameObject.transform.GetChild(4).gameObject.SetActive(value: false);
			gameObject.transform.GetChild(3).gameObject.SetActive(value: true);
		}
		player.GetComponent<Health>().Heal(20f);
		Use();
	}

	public override void LostRelic(GameObject player)
	{
		GameObject gameObject = GameObject.Find("Hearts");
		GameObject gameObject2 = gameObject.transform.parent.gameObject;
		gameObject2.GetComponent<Image>().sprite = heartUI;
		gameObject2.GetComponent<Image>().SetNativeSize();
		gameObject.transform.GetChild(5).gameObject.SetActive(value: false);
		Transform transform = player.GetComponent<RelicCollector>().relicUIParent.transform;
		transform.localPosition = new Vector3(transform.localPosition.x - 50f, transform.localPosition.y, transform.localPosition.z);
		player.GetComponent<Health>().health = Mathf.Min(100f, player.GetComponent<Health>().health);
		player.GetComponent<Health>().maxHealth = 100f;
	}
}
