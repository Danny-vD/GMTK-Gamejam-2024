using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    public Text CScoreText;
    public Text BScoreText;

    int CScore = 0;
    int BScore = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CScoreText.text = CScore.ToString();
        BScoreText.text = BScore.ToString();
  }

    // Update is called once per frame
    void Update()
    {
        
    }
}
