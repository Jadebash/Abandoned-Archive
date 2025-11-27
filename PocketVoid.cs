using UnityEngine;

public class PocketVoid : MonoBehaviour
{
	public delegate void DispenseCallback();

	private Movement movement;

	private SpellCasting spellCasting;

	public GameObject pocketVoidPrefab;

	public GameObject rechargeEffectPrefab;

	private float cooldownTimer = 2.5f;

	private const float cooldownDuration = 2.5f;

	private bool wasOnCooldown;

	public event DispenseCallback OnDispense;

	private void Start()
	{
		movement = base.transform.parent.GetComponent<Movement>();
		spellCasting = base.transform.parent.GetComponent<SpellCasting>();
		movement.OnRoll += FireProjectile;
	}

	private void Update()
	{
		if (cooldownTimer < 2.5f)
		{
			cooldownTimer += Time.deltaTime;
			wasOnCooldown = true;
			if (cooldownTimer >= 2.5f && wasOnCooldown && rechargeEffectPrefab != null)
			{
				Object.Instantiate(rechargeEffectPrefab, base.transform.parent.position, Quaternion.identity, base.transform.parent);
				wasOnCooldown = false;
			}
		}
	}

	private void FireProjectile()
	{
		if (!(cooldownTimer < 2.5f))
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(spellCasting.cursorPosition) + new Vector3(0f, 0f, 10f) - base.transform.position;
			GameObject obj = Object.Instantiate(pocketVoidPrefab, base.transform.position + vector.normalized * 1f, Quaternion.identity);
			obj.layer = 15;
			Rigidbody2D component = obj.GetComponent<Rigidbody2D>();
			if (component != null)
			{
				component.AddForce(vector.normalized * 6.5f, ForceMode2D.Impulse);
			}
			cooldownTimer = 0f;
			this.OnDispense?.Invoke();
		}
	}

	private void OnDestroy()
	{
		if (movement != null)
		{
			movement.OnRoll -= FireProjectile;
		}
	}
}
