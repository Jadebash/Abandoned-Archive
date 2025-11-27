using UnityEngine;

public class KnowledgePellet : MonoBehaviour
{
	private GameObject player;

	public int amount = 2;

	public float speed = 3f;

	public float rotationSpeed = 180f;

	public float rotationVariation = 60f;

	public float movementNoiseAmount = 0.3f;

	public float movementNoiseSpeed = 2f;

	public GameObject numberIndicator;

	private float currentRotationSpeed;

	private float noiseOffsetX;

	private float noiseOffsetY;

	private void Start()
	{
		player = PlayerManager.ClosestPlayer(base.transform.position);
		currentRotationSpeed = rotationSpeed + Random.Range(0f - rotationVariation, rotationVariation);
		noiseOffsetX = Random.Range(0f, 1000f);
		noiseOffsetY = Random.Range(0f, 1000f);
	}

	private void Update()
	{
		base.transform.Rotate(0f, 0f, currentRotationSpeed * Time.deltaTime);
		if (player != null)
		{
			Vector3 normalized = (player.transform.position - base.transform.position).normalized;
			float x = (Mathf.PerlinNoise(noiseOffsetX, Time.time * movementNoiseSpeed) - 0.5f) * 2f;
			float y = (Mathf.PerlinNoise(noiseOffsetY, Time.time * movementNoiseSpeed) - 0.5f) * 2f;
			Vector3 vector = new Vector3(x, y, 0f) * movementNoiseAmount;
			Vector3 normalized2 = (normalized + vector).normalized;
			base.transform.position += normalized2 * speed * Time.deltaTime;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player")
		{
			RelicCollector.Instance?.GainKnowledge(amount);
			if (numberIndicator != null)
			{
				Object.Instantiate(numberIndicator, base.transform.position + new Vector3(Random.insideUnitCircle.x * 0.2f, Random.insideUnitCircle.y * 0.2f, 0f), Quaternion.identity).GetComponent<NumberIndicator>().number = amount.ToString();
			}
			Object.Destroy(base.gameObject);
		}
	}
}
