using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class OblomeLaser : MonoBehaviour
{
	public enum Directions
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3
	}

	public Directions direction;

	private OblomeBoss oblome;

	private EventInstance fireNoiseInstance;

	private void Start()
	{
		int num = Random.Range(0, 4);
		direction = (Directions)num;
		GetComponent<Animator>().speed = Random.Range(0.1f, 0.25f);
		switch (direction)
		{
		case Directions.Up:
			base.transform.position += new Vector3(-15f, -15f, 0f);
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			GetComponent<Animator>().SetTrigger("Up");
			break;
		case Directions.Down:
			base.transform.position += new Vector3(-15f, 15f, 0f);
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			GetComponent<Animator>().SetTrigger("Down");
			break;
		case Directions.Left:
			base.transform.position += new Vector3(15f, 15f, 0f);
			GetComponent<Animator>().SetTrigger("Left");
			break;
		case Directions.Right:
			base.transform.position += new Vector3(-15f, 15f, 0f);
			GetComponent<Animator>().SetTrigger("Right");
			break;
		default:
			Debug.LogError("Invalid laser direction");
			break;
		}
		oblome = Object.FindObjectOfType<OblomeBoss>();
		fireNoiseInstance = RuntimeManager.CreateInstance("event:/SFX/Enemies/FireNoise");
		fireNoiseInstance.set3DAttributes(oblome.gameObject.transform.position.To3DAttributes());
		fireNoiseInstance.start();
		fireNoiseInstance.release();
	}

	private void Update()
	{
		if (oblome == null || oblome.GetComponent<Health>().health <= 0f)
		{
			DestroySelf();
		}
	}

	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (fireNoiseInstance.isValid())
		{
			fireNoiseInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}
}
