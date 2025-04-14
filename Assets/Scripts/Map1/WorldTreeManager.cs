using UnityEngine;

public class WorldTreeManager : MonoBehaviour 
{
    public float gameDuration = 180f;
    private float timer;
    public int difficulty = 0;

    private int score;
    public const int FireExtinguishingScore = 10;
    public const int HumanHuntingScore = 10;

    WorldTree worldTreeSC;

    void Start()
    {
        timer = gameDuration;
        object worldTreeSC = GameObject.Find("WorldTree").GetComponent<WorldTree>();
        if (worldTreeSC == null)
        {
            Debug.Log("worldTree NULL");
        }
    }

    public void FireExtinguishing()
    {
        score += FireExtinguishingScore;
        Debug.Log("score changed / score : " + score);
    }

    public void HumanHunting()
    {
        score += HumanHuntingScore;
        Debug.Log("score changed / score : " + score);
    }

    void GameOver()
    {

    }
}
