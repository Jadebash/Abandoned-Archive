using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class OblomeBoss : MonoBehaviour
{
	public GameObject[] spawnableEnemies;

	public GameObject[] hotColdEnemyPrefabs;

	public SpellPickup spellPickup;

	public GameObject spellLight;

	private float speedUpTimer;

	private float invertedTimer;

	[SerializeField]
	public bool spedUp;

	public int numOfAttacks;

	public GameObject orbitePrefab;

	public GameObject laser;

	private int laserCount;

	private bool hotCold;

	private List<GameObject> activeLasers = new List<GameObject>();

	private List<GameObject> activeEnemies = new List<GameObject>();

	public GameObject hotColdHaze;

	public GameObject hotColdHazeInstance;

	public GameObject orangeFlask;

	public GameObject blueFlask;

	public GameObject greenFlask;

	public Vector3 startingPos;

	[Header("Rufus Moves Prefabs")]
	public GameObject rods;

	private GameObject rodDamageArea;

	public GameObject tetheringLine;

	public GameObject jumpAOE;

	[Header("Veltrine Moves Prefabs")]
	public Transform coinSpawn;

	public GameObject coinPrefab;

	private bool flipped;

	public LayerMask teleportLayerMask;

	public LayerMask validTeleportLayerMask;

	public GameObject smokePrefab;

	[Header("Holite Moves Prefabs")]
	public GameObject poisonBallPrefab;

	public GameObject javelinPrefab;

	private List<GameObject> javelinInstances = new List<GameObject>();

	[Header("Lemures Moves Prefabs")]
	public Transform spiritsRed;

	public EventReference sfx;

	public Transform tripleBeam;

	private bool rotateBeam;

	[Header("Crylia Moves Prefabs")]
	public GameObject syringes;

	private bool pouncing;

	private Rigidbody2D rb;

	private bool ended;

	private void Start()
	{
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		startingPos = base.transform.position;
		spedUp = false;
		speedUpTimer = 0f;
		invertedTimer = 0f;
		rb = GetComponent<Rigidbody2D>();
		GetComponent<Health>().OnDeath += Die;
		RuntimeManager.StudioSystem.setParameterByName("Intensity", 100f);
		CameraController.Instance.gameObject.transform.GetChild(0).GetComponent<Camera>().orthographicSize = 6.5f;
	}

	private void Die(GameObject attacker)
	{
		if (hotColdHazeInstance != null)
		{
			Object.Destroy(hotColdHazeInstance);
		}
		ended = true;
		foreach (GameObject activeEnemy in activeEnemies)
		{
			if (activeEnemy != null)
			{
				Object.Destroy(activeEnemy);
			}
		}
		foreach (GameObject activeLaser in activeLasers)
		{
			if (activeLaser != null)
			{
				Object.Destroy(activeLaser);
			}
		}
		GetComponent<Animator>().SetTrigger("Ending");
		BossManager.Instance.EndBossFight();
	}

	private void Update()
	{
		if (spedUp)
		{
			speedUpTimer += Time.deltaTime;
			if (speedUpTimer >= 30f)
			{
				spedUp = false;
				speedUpTimer = 0f;
			}
		}
		if (rotateBeam)
		{
			Vector2 vector = (Vector2)PlayerManager.ClosestPlayer(base.transform.position).transform.position + PlayerManager.ClosestPlayer(base.transform.position).transform.GetComponent<Rigidbody2D>().velocity * 0.15f - (Vector2)base.transform.position;
			float angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			tripleBeam.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	public void SpawnEnemies()
	{
		GameObject[] array = spawnableEnemies;
		foreach (GameObject original in array)
		{
			float num = Random.Range(-5f, 5f);
			float num2 = Random.Range(-5f, 5f);
			Vector2 vector = new Vector2(base.transform.position.x + num, base.transform.position.y + num2);
			GameObject item = Object.Instantiate(original, vector, Quaternion.identity);
			activeEnemies.Add(item);
		}
	}

	public void SpawnRandomEnemy()
	{
		if (hotColdEnemyPrefabs != null && hotColdEnemyPrefabs.Length != 0)
		{
			GameObject original = hotColdEnemyPrefabs[Random.Range(0, hotColdEnemyPrefabs.Length)];
			float num = Random.Range(-5f, 5f);
			float num2 = Random.Range(-5f, 5f);
			Vector2 vector = new Vector2(base.transform.position.x + num, base.transform.position.y + num2);
			GameObject item = Object.Instantiate(original, vector, Quaternion.identity);
			activeEnemies.Add(item);
		}
	}

	public void InvertAttack()
	{
		PlayerManager.ClosestPlayer(base.transform.position).GetComponent<Movement>().inverted = true;
	}

	public void DropSpells()
	{
		SpellCasting component = PlayerManager.ClosestPlayer(base.transform.position).GetComponent<SpellCasting>();
		Stance currentStance = component.currentStance;
		DropSpellSlot(component, currentStance, 1);
		DropSpellSlot(component, currentStance, 2);
	}

	private void DropSpellSlot(SpellCasting spellCasting, Stance currentStance, int slotIndex)
	{
		if (currentStance.spellSlots.Count <= slotIndex || !(currentStance.spellSlots[slotIndex].spell != null))
		{
			return;
		}
		SpellSlot spellSlot = currentStance.spellSlots[slotIndex];
		Spell spell = spellSlot.spell;
		bool flag = false;
		foreach (Stance stance in spellCasting.stances)
		{
			if (stance != currentStance && stance.spellSlots.Count > slotIndex && stance.spellSlots[slotIndex] == spellSlot)
			{
				flag = true;
				break;
			}
		}
		SpellSlot spellSlot2 = spellSlot;
		if (flag)
		{
			spellSlot2 = new SpellSlot();
			spellSlot2.UIObject = spellSlot.UIObject;
			spellSlot2.UIBackground = spellSlot.UIBackground;
			spellSlot2.slider = spellSlot.slider;
			spellSlot2.iconUI = spellSlot.iconUI;
			spellSlot2.animator = spellSlot.animator;
			if (spellSlot.isUsable)
			{
				spellSlot2.Enable();
			}
			else
			{
				spellSlot2.Disable();
			}
			currentStance.spellSlots[slotIndex] = spellSlot2;
		}
		spellSlot2.spell = null;
		if (spellSlot2.iconUI != null)
		{
			spellSlot2.iconUI.GetComponent<Image>().sprite = null;
			spellSlot2.iconUI.gameObject.SetActive(value: false);
		}
		float num = Random.Range(-6f, 6f);
		float num2 = Random.Range(-6f, 6f);
		Vector2 vector = new Vector2(base.transform.position.x + num, base.transform.position.y + num2);
		while (Vector2.Distance(vector, base.transform.position) < 3f)
		{
			num = Random.Range(-6f, 6f);
			num2 = Random.Range(-6f, 6f);
			vector = new Vector2(base.transform.position.x + num, base.transform.position.y + num2);
		}
		GameObject gameObject = Object.Instantiate(spellPickup.gameObject, vector, Quaternion.identity);
		gameObject.GetComponent<SpellPickup>().spell = spell;
		Object.Instantiate(spellLight, vector, Quaternion.identity, gameObject.transform);
	}

	public void Orbite()
	{
		Vector3 position = base.transform.position;
		for (int i = 0; i < 5; i++)
		{
			position = new Vector3(base.transform.position.x + Random.Range(-3f, 3f), base.transform.position.y + Random.Range(-3f, 3f), 0f);
			while (Vector2.Distance(position, PlayerManager.ClosestPlayer(position).transform.position) < 2f)
			{
				position = new Vector3(base.transform.position.x + Random.Range(-3f, 3f), base.transform.position.y + Random.Range(-3f, 3f), 0f);
			}
			Object.Instantiate(orbitePrefab, position, Quaternion.identity).GetComponent<Orbite>().oblome = base.gameObject;
		}
	}

	public IEnumerator LaserAttack()
	{
		laserCount++;
		GameObject item = Object.Instantiate(laser, base.transform.position, Quaternion.identity);
		activeLasers.Add(item);
		if (GetComponent<Health>().health <= GetComponent<Health>().maxHealth / 2f)
		{
			SpawnEnemy();
		}
		yield return new WaitForSeconds(Random.Range(1.3f, 2f));
		GameObject item2 = Object.Instantiate(laser, base.transform.position, Quaternion.identity);
		activeLasers.Add(item2);
		if (laserCount == 2)
		{
			yield return new WaitForSeconds(Random.Range(1.3f, 2f));
			GameObject item3 = Object.Instantiate(laser, base.transform.position, Quaternion.identity);
			activeLasers.Add(item3);
			laserCount = 0;
		}
	}

	public void SpawnEnemy()
	{
		if (spawnableEnemies.Length == 0)
		{
			return;
		}
		GameObject original = spawnableEnemies[Random.Range(0, spawnableEnemies.Length)];
		Vector3 vector = base.transform.position;
		bool flag = false;
		int num = 0;
		while (!flag && num < 10)
		{
			vector = new Vector3(base.transform.position.x + Random.Range(-5f, 5f), base.transform.position.y + Random.Range(-5f, 5f), 0f);
			if (Vector2.Distance(vector, PlayerManager.ClosestPlayer(vector).transform.position) > 3f)
			{
				flag = true;
			}
			num++;
		}
		GameObject item = Object.Instantiate(original, vector, Quaternion.identity);
		activeEnemies.Add(item);
	}

	public bool AllLasersDestroyed()
	{
		foreach (GameObject activeLaser in activeLasers)
		{
			if (activeLaser != null)
			{
				return false;
			}
		}
		activeLasers.Clear();
		return true;
	}

	public void HotCold()
	{
		if (!hotCold)
		{
			hotColdHazeInstance = Object.Instantiate(hotColdHaze, base.transform.position, Quaternion.identity);
			hotCold = true;
		}
		else
		{
			Object.Destroy(hotColdHazeInstance);
			hotColdHazeInstance = null;
			hotCold = false;
		}
	}

	public void ThrowFlask()
	{
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		switch (Random.Range(0, 3))
		{
		case 0:
			gameObject = Object.Instantiate(orangeFlask, base.transform.position, Quaternion.identity);
			break;
		case 1:
			gameObject = Object.Instantiate(blueFlask, base.transform.position, Quaternion.identity);
			break;
		case 2:
			gameObject = Object.Instantiate(greenFlask, base.transform.position, Quaternion.identity);
			break;
		}
		if (GetComponent<Health>().health < GetComponent<Health>().maxHealth / 2f)
		{
			switch (Random.Range(0, 3))
			{
			case 0:
				gameObject2 = Object.Instantiate(orangeFlask, base.transform.position, Quaternion.identity);
				break;
			case 1:
				gameObject2 = Object.Instantiate(blueFlask, base.transform.position, Quaternion.identity);
				break;
			case 2:
				gameObject2 = Object.Instantiate(greenFlask, base.transform.position, Quaternion.identity);
				break;
			}
			gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 10f, ForceMode2D.Impulse);
			gameObject2.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 10f, ForceMode2D.Impulse);
		}
		else
		{
			Vector3 normalized = (PlayerManager.ClosestPlayer(base.transform.position).transform.position - base.transform.position).normalized;
			gameObject.GetComponent<Rigidbody2D>().AddForce(normalized * 7.5f, ForceMode2D.Impulse);
		}
	}

	private void ThrowRods()
	{
		GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
		if (base.gameObject.transform.position.x > gameObject.transform.position.x && !base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
		Vector3 vector = gameObject.transform.position - base.transform.position;
		GameObject gameObject2 = Object.Instantiate(rods, base.transform.position, Quaternion.identity);
		gameObject2.transform.up = -vector;
		rodDamageArea = gameObject2.transform.Find("DamageArea").gameObject;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/ArmWoosh", base.transform.position);
	}

	private IEnumerator StrikeRods()
	{
		RufusLightningRod[] array = Object.FindObjectsOfType<RufusLightningRod>();
		foreach (RufusLightningRod rufusLightningRod in array)
		{
			GameObject obj = Object.Instantiate(tetheringLine);
			obj.GetComponent<TetheringLine>().from = base.transform;
			obj.GetComponent<TetheringLine>().to = rufusLightningRod.transform;
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusLightning", base.transform.position);
			Screenshake.Instance.AddTrauma(0.5f);
		}
		rodDamageArea.SetActive(value: true);
		yield return new WaitForSeconds(0.1f);
		rodDamageArea.SetActive(value: false);
		if (base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	private IEnumerator JumpAttack()
	{
		Random.Range(0, 2);
		StartCoroutine(Jump());
		yield return new WaitForSeconds(1.5f);
		StartCoroutine(Jump());
		GetComponent<Animator>().SetBool("RufusWalking", value: true);
	}

	private IEnumerator Jump()
	{
		GameObject player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		if (base.gameObject.transform.position.x > player.transform.position.x && !base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.AddForce(base.transform.up * 2f, ForceMode2D.Impulse);
		yield return new WaitForSeconds(0.75f);
		Vector3 vector = player.transform.position + new Vector3(0f, 0.5f, 0f) - base.gameObject.transform.position;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusWoosh", base.transform.position);
		rb.AddForce(-rb.velocity, ForceMode2D.Impulse);
		rb.AddForce(vector * 20f, ForceMode2D.Impulse);
		yield return new WaitForSeconds(0.25f);
		jumpAOE.SetActive(value: true);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusHittingFloor", base.transform.position);
		rb.AddForce(-rb.velocity, ForceMode2D.Impulse);
		rb.bodyType = RigidbodyType2D.Kinematic;
		Screenshake.Instance.AddTrauma(0.5f);
		yield return new WaitForSeconds(0.1f);
		jumpAOE.SetActive(value: false);
		yield return new WaitForSeconds(0.5f);
		if (base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	public void ThrowCoin(float angleModifier)
	{
		Vector3 position = base.transform.position;
		Vector2 vector = PlayerManager.ClosestPlayer(base.transform.position).transform.position - position;
		if (vector.x < 0f && !base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			flipped = true;
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
			coinSpawn.localPosition = new Vector3(-0.44f, 1.1f, 0f);
		}
		if (vector.x > 0f && base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			flipped = false;
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
			coinSpawn.localPosition = new Vector3(0.44f, 1.1f, 0f);
		}
		Vector2 vector2 = new Vector2(vector.normalized.y, 0f - vector.normalized.x);
		GameObject obj = Object.Instantiate(coinPrefab, coinSpawn.position, Quaternion.identity);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Shoot", position);
		obj.GetComponent<Rigidbody2D>().AddForce(vector.normalized * 5f * GetComponent<Animator>().speed + vector2 * angleModifier * 2f, ForceMode2D.Impulse);
		Screenshake.Instance.AddTrauma(0.3f);
	}

	public void Teleport()
	{
		GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
		float num = 7f;
		Vector2 vector = (Vector2)gameObject.transform.position + Random.insideUnitCircle * num;
		int num2 = 0;
		while (Physics2D.OverlapCircleAll(vector, 0.25f, teleportLayerMask).Length != 0 || Physics2D.OverlapCircleAll(vector, 0.1f, validTeleportLayerMask).Length == 0 || Vector2.Distance(vector, gameObject.transform.position) < 3.5f)
		{
			vector = (Vector2)gameObject.transform.position + Random.insideUnitCircle * num;
			num2++;
			if (num2 >= 100)
			{
				Debug.LogWarning("Veltrine could not find a teleport position.");
				break;
			}
		}
		Object.Instantiate(smokePrefab, base.transform.position, Quaternion.identity);
		rb.position = vector;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/poof", base.transform.position);
		Screenshake.Instance.AddTrauma(0.4f);
	}

	public void TeleportBack()
	{
		Object.Instantiate(smokePrefab, base.transform.position, Quaternion.identity);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/poof", base.transform.position);
		rb.position = startingPos;
	}

	private void RollBall()
	{
		GameObject obj = PlayerManager.ClosestPlayer(base.transform.position);
		Vector3 position = base.transform.position;
		Vector2 vector = obj.transform.position - position;
		Vector2 force = vector.normalized * 5f * GetComponent<Animator>().speed;
		Object.Instantiate(poisonBallPrefab, position + (Vector3)(vector.normalized * 0.8f), Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
		Screenshake.Instance.AddTrauma(0.9f);
	}

	private IEnumerator FireJavelins()
	{
		ThrowJavelin(-2f);
		yield return new WaitForSeconds(0.1f);
		ThrowJavelin(-1f);
		yield return new WaitForSeconds(0.1f);
		ThrowJavelin(0f);
		yield return new WaitForSeconds(0.1f);
		ThrowJavelin(1f);
		yield return new WaitForSeconds(0.1f);
		ThrowJavelin(2f);
	}

	private void ThrowJavelin(float angleModifier)
	{
		GameObject obj = PlayerManager.ClosestPlayer(base.transform.position);
		Vector3 position = base.transform.position;
		Vector2 vector = obj.transform.position - position;
		Vector2 vector2 = new Vector2(vector.normalized.y, 0f - vector.normalized.x);
		Vector2 vector3 = vector.normalized * 9f * GetComponent<Animator>().speed + vector2 * angleModifier;
		GameObject gameObject = Object.Instantiate(javelinPrefab, position + (Vector3)(vector.normalized * 1.5f), Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, vector3), Vector3.forward));
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Shoot", position);
		gameObject.GetComponent<Rigidbody2D>().AddForce(vector3, ForceMode2D.Impulse);
		javelinInstances.Add(gameObject);
		Screenshake.Instance.AddTrauma(0.6f);
	}

	private IEnumerator ReturnJavelins()
	{
		GameObject[] array = javelinInstances.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject == null))
			{
				ReturnJavelin(gameObject);
				Screenshake.Instance.AddTrauma(0.8f);
				yield return new WaitForSeconds(0.2f);
			}
		}
		javelinInstances.Clear();
	}

	private void ReturnJavelin(GameObject javelin)
	{
		Vector2 vector = PlayerManager.ClosestPlayer(base.transform.position).transform.position - javelin.transform.position;
		javelin.GetComponent<Javelin>().Recall();
		javelin.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		Vector2 vector2 = vector.normalized * 20f * GetComponent<Animator>().speed;
		javelin.GetComponent<Rigidbody2D>().AddForce(vector2, ForceMode2D.Impulse);
		javelin.transform.rotation = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, vector2), Vector3.forward);
	}

	public void SpawnSpiritsRed()
	{
		StartCoroutine(SpiritSpawningRed());
	}

	private IEnumerator SpiritSpawningRed()
	{
		RuntimeManager.PlayOneShot(sfx);
		Transform player = PlayerManager.ClosestPlayer(base.transform.position).transform;
		foreach (Transform item in spiritsRed)
		{
			item.gameObject.SetActive(value: false);
		}
		foreach (Transform item2 in spiritsRed)
		{
			item2.position = player.position + (Vector3)Random.insideUnitCircle * 5f;
			int num = 0;
			while (Vector2.Distance(item2.position, player.position) < 3f)
			{
				item2.position = player.position + (Vector3)Random.insideUnitCircle * 5f;
				num++;
				if (num > 100)
				{
					Debug.LogWarning("Couldnt find valid position for spirit.");
					break;
				}
			}
			item2.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(2f / (float)spiritsRed.childCount);
		}
	}

	public void StartTripleBeam()
	{
		Transform transform = PlayerManager.ClosestPlayer(base.transform.position).transform;
		Vector2 vector = (Vector2)transform.transform.position + transform.GetComponent<Rigidbody2D>().velocity * 0.33f - (Vector2)base.transform.position;
		float angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		tripleBeam.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public void StopBeam()
	{
		rotateBeam = false;
	}

	private void FlipSprite()
	{
		if (PlayerManager.ClosestPlayer(base.transform.position).transform.position.x < base.gameObject.transform.position.x && !base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
		else if (base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	private void ThrowRedSyringe()
	{
		GameObject obj = PlayerManager.ClosestPlayer(base.transform.position);
		GameObject gameObject = Object.Instantiate(syringes, base.transform.position, Quaternion.identity);
		Vector2 vector = obj.transform.position - gameObject.transform.position;
		gameObject.transform.up = vector.normalized;
		gameObject.SetActive(value: true);
	}

	private IEnumerator Pounce()
	{
		GameObject obj = PlayerManager.ClosestPlayer(base.transform.position);
		if (obj.transform.position.x < base.gameObject.transform.position.x && !base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
		pouncing = true;
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Dynamic;
		Vector3 vector = obj.transform.position + new Vector3(-1.25f, 0f, 0f) - base.gameObject.transform.position;
		rb.AddForce(vector * 15f, ForceMode2D.Impulse);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Woosh", base.transform.position);
		yield return new WaitForSeconds(0.2f);
		rb.velocity = new Vector2(0f, 0f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/SmallThud", base.transform.position);
		Screenshake.Instance.AddTrauma(0.5f);
		if (base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	private IEnumerator FinalPounce()
	{
		GameObject obj = PlayerManager.ClosestPlayer(base.transform.position);
		if (obj.transform.position.x < base.gameObject.transform.position.x && !base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		Vector3 vector = obj.transform.position - base.gameObject.transform.position;
		rb.AddForce(vector * 15f, ForceMode2D.Impulse);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Woosh", base.transform.position);
		yield return new WaitForSeconds(0.2f);
		rb.velocity = new Vector2(0f, 0f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/SmallThud", base.transform.position);
		Screenshake.Instance.AddTrauma(0.5f);
		GetComponent<Animator>().SetBool("CryliaWalking", value: true);
		rb.bodyType = RigidbodyType2D.Kinematic;
		pouncing = false;
		if (base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	public void StartEnding()
	{
		Ending.Instance.KillOblomeEnding();
	}
}
