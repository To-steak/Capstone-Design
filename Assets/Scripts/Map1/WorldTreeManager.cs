using UnityEngine;
using UnityEngine.UI;

public class WorldTreeManager : MonoBehaviour 
{
    public float gameDuration = 180f;
    private float timer;

    public int difficulty = 1; // 1 ~ 20
    public int damageOfFireOverflow = 10;
    public int TermOfFireOverflow = 10; //sec
    public int maxHumanAI = 5;
    public int TermOfHumanRespon = 5; //sec
    public float HumanAISpeed = 10; // persec

    private int score = 0;
    public const int FireExtinguishingScore = 10;
    public const int HumanHuntingScore = 10;


    public Image worldTreeHealthBar;
    WorldTree _worldTree;

    void Start()
    {
        timer = gameDuration;
        _worldTree = GameObject.Find("WorldTree").GetComponent<WorldTree>();
        if (_worldTree == null)
        {
            Debug.Log("worldTree NULL");
        }
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 || _worldTree.GetHealth() <= 0)
        {
            GameOver();
        }

        worldTreeHealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(500 * _worldTree.GetHealth() / 100, 20); // UI worldTreeHealthBar
    }

    public void FireExtinguishing()
    {
        score += FireExtinguishingScore;
        Debug.Log("Fire Extinguishing score changed / score : " + score);
    }

    public void HumanHunting()
    {
        score += HumanHuntingScore;
        Debug.Log("Human Hunting score changed / score : " + score);
    }

    public void WorldTreeHitByFireOverflow()
    {
        _worldTree.HealthChange(-damageOfFireOverflow);
    }

    void GameOver()
    {

    }
}
