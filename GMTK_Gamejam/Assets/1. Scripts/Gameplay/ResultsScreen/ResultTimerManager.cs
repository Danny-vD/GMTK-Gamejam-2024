using ECS.Components.Scoring.HeightMeasurer.Tags;
using TMPro;
using Unity.Entities;
using UnityEngine;
using VDFramework;

namespace Gameplay.ResultsScreen
{
	public class ResultTimerManager : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text timerLabel;

		[SerializeField]
		private ResultsManager resultsManager;
		
		private EntityManager entityManager;
		private EntityQuery timerSingletonQuery;

		private void Awake()
		{
			entityManager     = World.DefaultGameObjectInjectionWorld.EntityManager;
			timerSingletonQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ShouldMeasureHeightComponent>());
		}

		private void Start()
		{
			resultsManager.OnShowingResults += HideTimer;
		}

		private void LateUpdate()
		{
			if (timerSingletonQuery.IsEmpty)
			{
				return;
			}
			
			ShouldMeasureHeightComponent shouldMeasureHeightComponent = timerSingletonQuery.GetSingleton<ShouldMeasureHeightComponent>();

			float timeRemaining = shouldMeasureHeightComponent.WaitTimeBeforeMeasure;

			if (timeRemaining < 0)
			{
				timeRemaining = 0;
			}

			timerLabel.text = timeRemaining.ToString("N2");
		}

		private void HideTimer()
		{
			gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			resultsManager.OnShowingResults -= HideTimer;
		}
	}
}