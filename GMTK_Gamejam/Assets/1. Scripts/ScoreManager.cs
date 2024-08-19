using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
	public TMP_Text CScoreText;
	public TMP_Text BScoreText;

	int CScore = 0;
	int BScore = 0;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		CScoreText.text = CScore.ToString();
		BScoreText.text = BScore.ToString();
	}
}