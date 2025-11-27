using TMPro;
using UnityEngine;

public class OrbHealthText : MonoBehaviour
{
	public TextMeshProUGUI orbHealthText;

	public float orbHealth = 500f;

	public float roundedOrbHealth;

	private Health healthScript;

	private void Start()
	{
		healthScript = GetComponent<Health>();
	}

	private void Update()
	{
		orbHealth = healthScript.health;
		roundedOrbHealth = Mathf.RoundToInt(orbHealth);
		orbHealthText.text = roundedOrbHealth + "/500";
	}
}
