using UnityEngine;
using UnityEngine.UI;

public class Trash_GameManager : MonoBehaviour
{
    public static Trash_GameManager Instance;

    private int score = 0;
    public Text scoreText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}