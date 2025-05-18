using TMPro;
using UnityEngine;

public class ScoreboardManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int score;

    [SerializeField]
    [Tooltip("This is the maximum number of hints that can be used.")]
    private int maxHints;
    private int currentHints;

    [SerializeField]
    [Tooltip("This is the maximum number of task skips that can be used.")]
    private int maxNumberOfTaskSkips;
    private int currentTaskSkipsUsed;

    [SerializeField]
    [Tooltip("This is the text to display the score.")]
    private TMP_Text scoreText;
    [SerializeField]
    [Tooltip("This is the text that will indicate the number of hints used and the number of maximum hints that the detective can provide.")]
    private TMP_Text hints;
    [SerializeField]
    [Tooltip("This is the text that will indicate the number of task skips used and the number of maximum task skips that the character can provide.")]
    private TMP_Text taskSkipsText;

    void Start()
    {
        score = 0;
        currentHints = maxHints;
        currentTaskSkipsUsed = maxNumberOfTaskSkips;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = $"Score: {score}";
        hints.text = $"Hints: {currentHints}/{maxHints}";
        taskSkipsText.text = $"Task Skips: {currentTaskSkipsUsed}/{maxNumberOfTaskSkips}";
    }

    public void AddScore(int score)
    {
        this.score += score;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void SetScore(int score)
    {
        this.score = score;
    }

    public void DecrementHintsUsed()
    {
        if (currentHints > 0)
        {
            currentHints--;
        }
    }

    public void DecrementTaskSkips()
    {
        if (currentTaskSkipsUsed > 0)
        {
            currentTaskSkipsUsed--;
        }
    }

    public int GetCurrentNumberOfTaskSkipsUsed()
    {
        return currentTaskSkipsUsed;
    }

    public int GetMaxNUmberOfTaskSkips()
    {
        return maxNumberOfTaskSkips;
    }
}
