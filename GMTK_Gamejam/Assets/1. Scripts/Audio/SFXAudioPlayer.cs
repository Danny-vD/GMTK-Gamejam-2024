using Gameplay.Events;
using UnityEngine;

namespace Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class SFXAudioPlayer : MonoBehaviour
	{
		[SerializeField]
		private AudioClip startDraggingClip;

		[SerializeField]
		private AudioClip stopDraggingClip;

		private AudioSource audioSource;

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void OnEnable()
		{
			StartDraggingEvent.AddListener(PlayStartDragging);
			StopDraggingEvent.AddListener(PlayStopDragging);
		}

		private void OnDisable()
		{
			StartDraggingEvent.RemoveListener(PlayStartDragging);
			StopDraggingEvent.RemoveListener(PlayStopDragging);
		}

		private void PlayStartDragging()
		{
			audioSource.clip = startDraggingClip;
			
			audioSource.Play();
		}
		
		private void PlayStopDragging()
		{
			audioSource.clip = stopDraggingClip;
			
			audioSource.Play();
		}
	}
}