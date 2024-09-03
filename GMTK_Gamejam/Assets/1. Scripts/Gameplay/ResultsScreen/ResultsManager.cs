using System;
using ECS.Authoring.Scoring;
using ECS.Components.Scoring;
using ECS.Components.Scoring.HeightMeasurer;
using Gameplay.Enums;
using Gameplay.HeightMeasurement;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;
using VDFramework.Utility;

namespace Gameplay.ResultsScreen
{
	public class ResultsManager : BetterMonoBehaviour
	{
		public event Action OnShowingResults = delegate { };
		public event Action OnHidingResults = delegate { };

		[Header("Result Screen")]
		[SerializeField]
		private GameObject resultsScreen;

		[SerializeField]
		private TMP_Text resultLabel;

		[SerializeField]
		private GameObject highscoreReachedObject;

		[SerializeField]
		private TMP_Text scoreText;

		[SerializeField]
		private TMP_Text continueButtonLabel;

		[SerializeField]
		private Button continueButton;

		[SerializeField]
		private string failButtonText = "Retry";

		[SerializeField]
		private string continueButtonText = "Continue";

		[Header("Gameplay logic")]
		[SerializeField]
		private GraphPositionManager graphPositionManager;

		private ScoreManager scoreManager;

		private EntityManager entityManager;
		private EntityQuery resultsQuery;

		private StringVariableWriter resultWriter;
		private StringVariableWriter scoreWriter;

		private void Start()
		{
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			resultsQuery  = entityManager.CreateEntityQuery(ComponentType.ReadOnly<HeightReachedComponent>(), ComponentType.ReadOnly<ScoreCalculatedComponent>());

			resultWriter = new StringVariableWriter(resultLabel.text);
			scoreWriter  = new StringVariableWriter(scoreText.text);

			scoreManager = GetComponent<ScoreManager>();

			continueButton.onClick.AddListener(HideScreen);
			continueButton.onClick.AddListener(MoveGraph);
			continueButton.onClick.AddListener(DestroyEntity);

			enabled = false;
		}

		private void LateUpdate() // Update because not sure when exactly the component will be added
		{
			if (resultsQuery.IsEmpty)
			{
				return;
			}

			ShowScreen();

			foreach (Entity entity in resultsQuery.ToEntityArray(Allocator.Temp))
			{
				HeightReachedComponent heightReachedComponent = entityManager.GetComponentData<HeightReachedComponent>(entity);
				ScoreCalculatedComponent scoreCalculatedComponent = entityManager.GetComponentData<ScoreCalculatedComponent>(entity);

				int finalScore = scoreCalculatedComponent.FinalScore;

				int currentScore = scoreManager.AddScore(finalScore, out bool newHighscore);

				highscoreReachedObject.SetActive(newHighscore);

				resultLabel.text = resultWriter.UpdateText(heightReachedComponent.HighestMultiplierReached.ToString());
				scoreText.text   = scoreWriter.UpdateText(scoreCalculatedComponent.Score, scoreCalculatedComponent.Multiplier, finalScore, currentScore, scoreManager.HighScore);

				bool success = heightReachedComponent.HighestMultiplierReached > MultiplierName.Fail;

				continueButtonLabel.text = success ? continueButtonText : failButtonText;

				if (!success)
				{
					continueButton.onClick.AddListener(ResetScore);
				}
			}

			enabled = false;
		}

		private void ShowScreen()
		{
			resultsScreen.SetActive(true);
			OnShowingResults.Invoke();
		}

		private void HideScreen()
		{
			resultsScreen.SetActive(false);
			OnHidingResults.Invoke();
		}

		private void ResetScore()
		{
			continueButton.onClick.RemoveListener(HideScreen);
			scoreManager.ResetScore();
		}

		private void MoveGraph()
		{
			graphPositionManager.SetToRandomPosition();
		}

		private void DestroyEntity()
		{
			entityManager.DestroyEntity(resultsQuery.GetSingletonEntity());
		}
	}
}