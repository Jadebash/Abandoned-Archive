using System.Collections;
using UnityEngine;

public class LemuresFlameWall : MonoBehaviour
{
	public GameObject[] wallPieces;

	public GameObject[] particles;

	private GameObject flameCentre;

	private GameObject player;

	private float timer;

	private Vector3 startingPos;

	private void Start()
	{
		timer = 0f;
		flameCentre = base.transform.Find("FlameCentre").gameObject;
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		StartCoroutine(closeInWall());
		flameCentre.transform.position = player.transform.position;
		startingPos = player.transform.position;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		GameObject[] array = wallPieces;
		foreach (GameObject gameObject in array)
		{
			if (Vector2.Distance(gameObject.transform.position, startingPos) > 1.75f && timer <= 3f)
			{
				gameObject.transform.position += new Vector3(startingPos.x - gameObject.transform.position.x, startingPos.y - gameObject.transform.position.y, 0f) * Time.deltaTime;
			}
		}
	}

	private IEnumerator closeInWall()
	{
		yield return new WaitForSeconds(1.75f);
		flameCentre.SetActive(value: true);
		yield return new WaitForSeconds(3f);
		if (flameCentre != null && flameCentre.activeSelf)
		{
			flameCentre.SetActive(value: false);
		}
	}
}
