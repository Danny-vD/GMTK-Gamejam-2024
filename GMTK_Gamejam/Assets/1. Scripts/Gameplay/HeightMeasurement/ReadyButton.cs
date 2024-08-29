using System.Collections;
using ECS.Components.Scoring.HeightMeasurer.Tags;
using Gameplay.Events;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;

namespace Gameplay.HeightMeasurement
{
	public class ReadyButton : BetterMonoBehaviour
	{
		[SerializeField]
		private float waitTimeBeforeScore;

		private EntityManager entityManager;

		private Button button;

		private void Awake()
		{
			button = GetComponent<Button>();

			AddListenerToButton();
		}

		private void OnEnable()
		{
			HeightReachedEvent.AddListener(AddListenerToButton);
		}

		private void OnDisable()
		{
			HeightReachedEvent.RemoveListener(AddListenerToButton);
		}

		private void Start()
		{
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		}

		private void AddListenerToButton()
		{
			button.onClick.AddListener(StartMeasuringHeight);
		}

		private void StartMeasuringHeight()
		{
			button.onClick.RemoveListener(StartMeasuringHeight);
			
			Entity entity = entityManager.CreateEntity();

			entityManager.AddComponentData(entity, new ShouldMeasureHeightComponent() { WaitTimeBeforeMeasure = waitTimeBeforeScore });
		}
	}
}