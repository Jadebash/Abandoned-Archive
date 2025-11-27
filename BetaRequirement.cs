using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class BetaRequirement : MonoBehaviour
{
	public delegate void Verify();

	public event Verify OnVerify;

	private void Start()
	{
		if (SceneManager.GetActiveScene().name == "Manager")
		{
			StartCoroutine(VerifyBeta());
		}
	}

	private IEnumerator VerifyBeta()
	{
		UnityWebRequest www = UnityWebRequest.Get("https://vedal.xyz/abandoned-archive/beta");
		yield return www.SendWebRequest();
		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Failed to verify Beta");
			Application.Quit();
		}
		else if (www.downloadHandler.text == "BETA")
		{
			Debug.Log("Verified Beta");
			this.OnVerify?.Invoke();
		}
		else
		{
			Debug.Log("Failed to verify Beta");
			Application.Quit();
		}
	}
}
