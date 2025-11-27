using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Ending : MonoBehaviour
{
	public static Ending Instance;

	[Header("Ending Videos")]
	public VideoClip oblomeEndingVideo;

	public VideoClip rodhEndingVideo;

	[Header("Ending Music (FMOD)")]
	[Tooltip("FMOD event path for Oblome ending music")]
	public EventReference oblomeEndingMusic;

	[Tooltip("FMOD event path for Rodh ending music")]
	public EventReference rodhEndingMusic;

	[Header("Credits Animation")]
	[Tooltip("Animator trigger name to start credits scrolling (e.g., 'StartCredits')")]
	public string creditsAnimatorTrigger = "StartCredits";

	[Header("Oblome Ending Timing (in seconds of video time)")]
	[Tooltip("Video timestamp to start music for Oblome ending")]
	public float oblomeMusicStartTime = 30f;

	[Tooltip("Video timestamp to trigger credits animation for Oblome ending")]
	public float oblomeCreditsStartTime = 35f;

	[Header("Rodh Ending Timing (in seconds of video time)")]
	[Tooltip("Video timestamp to start music for Rodh ending")]
	public float rodhMusicStartTime = 45f;

	[Tooltip("Video timestamp to trigger credits animation for Rodh ending")]
	public float rodhCreditsStartTime = 50f;

	private bool isLoadingEnding;

	private EventInstance currentMusicInstance;

	private void Awake()
	{
		Instance = this;
	}

	public void KillOblomeEnding()
	{
		if (!isLoadingEnding)
		{
			StartCoroutine(LoadEndingScene("Oblome", oblomeEndingVideo, oblomeEndingMusic, oblomeMusicStartTime, oblomeCreditsStartTime));
		}
	}

	public void KillRodhEnding()
	{
		if (!isLoadingEnding)
		{
			StartCoroutine(LoadEndingScene("Rodh", rodhEndingVideo, rodhEndingMusic, rodhMusicStartTime, rodhCreditsStartTime));
		}
	}

	public void StopEndingMusic()
	{
		if (currentMusicInstance.isValid())
		{
			currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			currentMusicInstance.release();
		}
	}

	private void OnDestroy()
	{
		StopEndingMusic();
	}

	private IEnumerator LoadEndingScene(string endingType, VideoClip videoClip, EventReference musicEventReference, float musicStartTime, float creditsStartTime)
	{
		isLoadingEnding = true;
		Debug.Log("[Ending] Loading " + endingType + " ending sequence...");
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeOutAllMusic();
			MusicManager.Calm();
			MusicManager.Instance.StopSFX();
		}
		if (FloorManager.Instance != null)
		{
			FloorManager.Instance.UnloadPlayer();
			if (!string.IsNullOrEmpty(FloorManager.Instance.currentScene))
			{
				yield return SceneManager.UnloadSceneAsync(FloorManager.Instance.currentScene);
				FloorManager.Instance.currentScene = "";
			}
		}
		yield return SceneManager.LoadSceneAsync("Credits", LoadSceneMode.Additive);
		Scene sceneByName = SceneManager.GetSceneByName("Credits");
		if (sceneByName.IsValid())
		{
			SceneManager.SetActiveScene(sceneByName);
			Debug.Log("[Ending] Credits scene loaded and set as active");
		}
		if (Manager.Instance.isPaused)
		{
			Debug.Log("[Ending] Unpausing game before ending playback");
			Manager.Instance.UnpauseGame();
		}
		Time.timeScale = 1f;
		Debug.Log($"[Ending] Time.timeScale set to {Time.timeScale}");
		yield return new WaitForSecondsRealtime(0.5f);
		yield return StartCoroutine(PlayEndingContent(videoClip, musicEventReference, musicStartTime, creditsStartTime));
		isLoadingEnding = false;
	}

	private IEnumerator PlayEndingContent(VideoClip videoClip, EventReference musicEventReference, float musicStartTime, float creditsStartTime)
	{
		GameObject gameObject = GameObject.FindWithTag("EndingVideoPlayer");
		GameObject gameObject2 = GameObject.FindWithTag("CreditsAnimator");
		VideoPlayer videoPlayer = ((gameObject != null) ? gameObject.GetComponent<VideoPlayer>() : null);
		Animator creditsAnimator = ((gameObject2 != null) ? gameObject2.GetComponent<Animator>() : null);
		Debug.Log($"[Ending] Found VideoPlayer: {videoPlayer != null}, Found Animator: {creditsAnimator != null}");
		if (videoPlayer == null || videoClip == null)
		{
			Debug.LogError("[Ending] VideoPlayer or VideoClip is null! Make sure EndingVideoPlayer GameObject is tagged as 'EndingVideoPlayer'");
			yield break;
		}
		if (creditsAnimator == null)
		{
			Debug.LogWarning("[Ending] Credits Animator not found! Make sure Credits GameObject is tagged as 'CreditsAnimator'");
		}
		Debug.Log($"[Ending] Starting video preparation. Music at {musicStartTime}s, Credits at {creditsStartTime}s");
		videoPlayer.clip = videoClip;
		videoPlayer.playOnAwake = false;
		videoPlayer.waitForFirstFrame = true;
		bool videoPrepared = false;
		bool videoStarted = false;
		VideoPlayer.EventHandler onPrepareCompleted = delegate
		{
			videoPrepared = true;
			Debug.Log("[Ending] Video preparation completed");
		};
		VideoPlayer.EventHandler onStarted = delegate
		{
			videoStarted = true;
			Debug.Log("[Ending] Video playback started");
		};
		videoPlayer.prepareCompleted += onPrepareCompleted;
		videoPlayer.started += onStarted;
		Debug.Log("[Ending] Calling videoPlayer.Prepare()...");
		videoPlayer.Prepare();
		float prepareTimeout = 10f;
		float prepareElapsed = 0f;
		while (!videoPrepared && prepareElapsed < prepareTimeout)
		{
			prepareElapsed += Time.unscaledDeltaTime;
			yield return null;
		}
		if (!videoPrepared)
		{
			Debug.LogError("[Ending] Video failed to prepare after 10 seconds! Falling back to credits only.");
			videoPlayer.prepareCompleted -= onPrepareCompleted;
			videoPlayer.started -= onStarted;
			if (!musicEventReference.IsNull)
			{
				currentMusicInstance = RuntimeManager.CreateInstance(musicEventReference);
				currentMusicInstance.start();
			}
			if (creditsAnimator != null && !string.IsNullOrEmpty(creditsAnimatorTrigger))
			{
				creditsAnimator.SetTrigger(creditsAnimatorTrigger);
			}
			yield break;
		}
		Debug.Log("[Ending] Video prepared successfully, starting playback...");
		videoPlayer.Play();
		float startTimeout = 5f;
		float startElapsed = 0f;
		while (!videoStarted && startElapsed < startTimeout)
		{
			startElapsed += Time.unscaledDeltaTime;
			yield return null;
		}
		if (!videoStarted || !videoPlayer.isPlaying)
		{
			Debug.LogError("[Ending] Video failed to start playing after 5 seconds! Falling back to credits only.");
			videoPlayer.prepareCompleted -= onPrepareCompleted;
			videoPlayer.started -= onStarted;
			videoPlayer.Stop();
			if (!musicEventReference.IsNull)
			{
				currentMusicInstance = RuntimeManager.CreateInstance(musicEventReference);
				currentMusicInstance.start();
			}
			if (creditsAnimator != null && !string.IsNullOrEmpty(creditsAnimatorTrigger))
			{
				creditsAnimator.SetTrigger(creditsAnimatorTrigger);
			}
			yield break;
		}
		Debug.Log("[Ending] Video is now playing successfully, monitoring timestamps...");
		bool musicStarted = false;
		bool creditsStarted = false;
		while (videoPlayer != null && videoPlayer.isPlaying && (!musicStarted || !creditsStarted))
		{
			double time = videoPlayer.time;
			if (!musicStarted && time >= (double)musicStartTime)
			{
				if (!musicEventReference.IsNull)
				{
					try
					{
						currentMusicInstance = RuntimeManager.CreateInstance(musicEventReference);
						currentMusicInstance.start();
						Debug.Log($"[Ending] Started music at video time: {time}s");
					}
					catch (Exception ex)
					{
						Debug.LogWarning("[Ending] Failed to play ending music: " + musicEventReference.ToString() + ". Error: " + ex.Message);
					}
				}
				musicStarted = true;
			}
			if (!creditsStarted && time >= (double)creditsStartTime)
			{
				if (creditsAnimator != null && !string.IsNullOrEmpty(creditsAnimatorTrigger))
				{
					creditsAnimator.SetTrigger(creditsAnimatorTrigger);
					Debug.Log($"[Ending] Triggered credits at video time: {time}s");
				}
				else
				{
					Debug.LogWarning("[Ending] Credits Animator not found or trigger name is empty!");
				}
				creditsStarted = true;
			}
			yield return null;
		}
		videoPlayer.prepareCompleted -= onPrepareCompleted;
		videoPlayer.started -= onStarted;
		Debug.Log("[Ending] Ending sequence monitoring complete.");
	}
}
