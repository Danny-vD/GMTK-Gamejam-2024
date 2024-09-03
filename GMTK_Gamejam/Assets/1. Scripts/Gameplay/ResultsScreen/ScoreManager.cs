using TMPro;
using UnityEngine;
using VDFramework;

namespace Gameplay.ResultsScreen
{
	public class ScoreManager : BetterMonoBehaviour
	{
		private const string highScoreKey = "HighScore";

		[SerializeField]
		private TMP_Text currentScoreLabel;

		[SerializeField]
		private TMP_Text highscoreLabel;

		public int HighScore { get; private set; }

		private int score;

		private void Awake()
		{
			LoadHighScore();
			highscoreLabel.text    = HighScore.ToString();
			currentScoreLabel.text = score.ToString();
		}

		public int AddScore(int scoreDelta, out bool newHighScore)
		{
			score += scoreDelta;

			currentScoreLabel.text = score.ToString();

			newHighScore = score > HighScore;

			if (newHighScore)
			{
				HighScore           = score;
				highscoreLabel.text = HighScore.ToString();
				
				SaveHighScore();
			}

			return score;
		}

		public void ResetScore()
		{
			score = 0;

			currentScoreLabel.text = score.ToString();
		}

		private void SaveHighScore()
		{
			PlayerPrefs.GetInt(highScoreKey, HighScore);
		}

		private void LoadHighScore()
		{
			HighScore = PlayerPrefs.GetInt(highScoreKey, 0);
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Data/Reset Data")]
		private static void ResetData()
		{
			PlayerPrefs.DeleteAll();
		}
#endif
	}
}