using TMPro;
using UnityEngine;

public class ScoreboardManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int score;

    [SerializeField]
    [Tooltip("This is the text to display the score.")]
    private TMP_Text scoreText;
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = $"Score: {score}";
    }
}
