using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuyHealthModal : MonoBehaviour
{
	private int cost;

	private GameObject oblome;

	public Button buyButton;

	public GameObject upgradeParticlesPrefab;

	public Dialogue firstDialogueAccept;

	public Dialogue firstDialogueDecline;

	public TextMeshProUGUI costText;

	private void Start()
	{
		oblome = GameObject.FindGameObjectWithTag("Core");
		if (oblome == null)
		{
			oblome = GameObject.Find("OblomePlaceholder");
			Debug.Log(oblome.gameObject.name);
		}
	}

	public void BuyHealth()
	{
		Debug.Log("Buying health");
		RelicCollector.Instance.BuyHealth(cost);
		PlayerManager.ClosestPlayer(oblome.transform.position).GetComponent<Health>().Heal(20f);
		GameObject gameObject = GameObject.FindGameObjectWithTag("Core");
		GameObject obj = Object.Instantiate(upgradeParticlesPrefab, (gameObject != null) ? gameObject.transform.position : base.transform.position, Quaternion.identity);
		obj.GetComponent<UpgradeParticles>().player = PlayerManager.ClosestPlayer(oblome.transform.position).transform;
		ParticleSystem.TriggerModule trigger = obj.GetComponent<ParticleSystem>().trigger;
		Screenshake.Instance.AddTrauma(0.8f);
		trigger.AddCollider(PlayerManager.ClosestPlayer(oblome.transform.position).GetComponent<Collider2D>());
		Manager.Instance?.UnpauseGame();
		MusicManager.ExitMenu();
		if (!SceneManager.GetActiveScene().name.ToLower().Contains("floor 4"))
		{
			DialogueSystem.Instance.StartDialogue(firstDialogueAccept);
			DialogueSystem.Instance.OnFinishDialogue += OnAcceptDialogueFinished;
			oblome.GetComponent<OblomeHealthInteract>().thinking = false;
			oblome.GetComponent<OblomeHealthInteract>().sameFloor = true;
		}
		else
		{
			OblomeFinalFloorInteract component = oblome.GetComponent<OblomeFinalFloorInteract>();
			if (component != null)
			{
				component.floor4PurchaseCount++;
				component.exitedHealthModal = true;
				component.healthInteractable = false;
			}
			base.gameObject.SetActive(value: false);
		}
		Save currentSave = SaveManager.Instance.currentSave;
		currentSave.oblomeHealthInteract = true;
		SaveManager.Instance.currentSave = currentSave;
	}

	private void OnAcceptDialogueFinished(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= OnAcceptDialogueFinished;
		if (oblome != null)
		{
			OblomeHealthInteract component = oblome.GetComponent<OblomeHealthInteract>();
			if (component != null)
			{
				component.exitedHealthModal = true;
				component.healthInteractable = false;
			}
			OblomeFinalFloorInteract component2 = oblome.GetComponent<OblomeFinalFloorInteract>();
			if (component2 != null)
			{
				component2.exitedHealthModal = true;
				component2.healthInteractable = false;
			}
		}
	}

	public void CloseButton()
	{
		Manager.Instance?.UnpauseGame();
		MusicManager.ExitMenu();
		if (!SceneManager.GetActiveScene().name.ToLower().Contains("floor 4"))
		{
			DialogueSystem.Instance.StartDialogue(firstDialogueDecline);
			DialogueSystem.Instance.OnFinishDialogue += OnDeclineDialogueFinished;
			return;
		}
		if (oblome != null)
		{
			OblomeFinalFloorInteract component = oblome.GetComponent<OblomeFinalFloorInteract>();
			if (component != null)
			{
				component.exitedHealthModal = true;
				component.healthInteractable = false;
			}
		}
		base.gameObject.SetActive(value: false);
	}

	private void OnDeclineDialogueFinished(Dialogue dialogue)
	{
		DialogueSystem.Instance.OnFinishDialogue -= OnDeclineDialogueFinished;
		if (oblome != null)
		{
			OblomeHealthInteract component = oblome.GetComponent<OblomeHealthInteract>();
			if (component != null)
			{
				component.exitedHealthModal = true;
				component.healthInteractable = false;
			}
			OblomeFinalFloorInteract component2 = oblome.GetComponent<OblomeFinalFloorInteract>();
			if (component2 != null)
			{
				component2.exitedHealthModal = true;
				component2.healthInteractable = false;
			}
		}
	}

	private void OnEnable()
	{
		Debug.Log("OnEnable");
		if (oblome == null)
		{
			oblome = GameObject.FindGameObjectWithTag("Core");
			if (oblome == null)
			{
				oblome = GameObject.Find("OblomePlaceholder");
			}
		}
		Debug.Log(FloorManager.Instance.currentLoop);
		switch (SceneManager.GetActiveScene().name.ToLower())
		{
		case "floor 1":
			cost = 10 * FloorManager.Instance.currentLoop;
			break;
		case "floor 2":
			cost = 20 * FloorManager.Instance.currentLoop;
			break;
		case "floor 3":
			cost = 30 * FloorManager.Instance.currentLoop;
			break;
		case "floor 4":
			cost = 30 * FloorManager.Instance.currentLoop;
			break;
		}
		Health component = PlayerManager.ClosestPlayer(oblome.transform.position).GetComponent<Health>();
		float health = component.health;
		if (RelicCollector.Instance.knowledge < cost || health >= component.maxHealth)
		{
			buyButton.interactable = false;
		}
		else
		{
			buyButton.interactable = true;
		}
		costText.text = cost.ToString();
	}
}
