using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ForestManager : MonoBehaviour
{
    private float gameDuration = 180f;
    private float timer;

    private int allowTreeLogging = 5;
    private int curAllowTreeLogging;
    private int maxHumanAI = 5;
    private int curHumanAI = 0;
    private float TermOfHumanRespawn = 10; //sec
    private float curTermOfHumanRespawn = 0;
    public float HumanAISpeed = 5; // persec

    private int score = 0;
    private int humanHuntingScore = 10;
    private int loggingTreesScore = 10; // 벌목되면 이만큼 점수깎임
    private int plantingScore = 10;

    private int haveSeedCount = 0;

    private TextMeshProUGUI _textForSeed;
    private TextMeshProUGUI _textForScore;

    public GameObject enemyPrefeb;
    GameObject[] EnemySpawnPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textForSeed = GameObject.Find("TextForSeed").GetComponent<TextMeshProUGUI>();
        _textForScore = GameObject.Find("TextForScore").GetComponent<TextMeshProUGUI>();
        curAllowTreeLogging = allowTreeLogging;
        EnemySpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
    }

    // Update is called once per frame
    void Update()
    {
        _textForScore.text = "Score : " + score;

        if (curHumanAI < maxHumanAI && curTermOfHumanRespawn <= 0)
        { //enemy spawn
            EnemySpawn();
        }
        else if (curHumanAI < maxHumanAI)
        {
            curTermOfHumanRespawn -= Time.deltaTime;
        }
    }

    private void EnemySpawn()
    {
        int random = UnityEngine.Random.Range(0, EnemySpawnPoints.Length);
        Instantiate(enemyPrefeb, EnemySpawnPoints[random].transform.position, EnemySpawnPoints[random].transform.rotation);
        curHumanAI++;
        curTermOfHumanRespawn = TermOfHumanRespawn;
    }

    public int GetHaveSeedCount() { return haveSeedCount; }

    public void HumanHunting()
    {
        score += humanHuntingScore;
        Debug.Log("Human Hunting score changed / score : " + score);
        curHumanAI--;
    }

    public void LoggingTrees()
    {
        score -= loggingTreesScore;
        Debug.Log("Tree Loged score changed / score : " + score);
        curAllowTreeLogging--;
    }
    public void seedPlanting()
    {
        score += plantingScore;
        Debug.Log("Seed Planting, score changed / score : " + score);
    }

    public void addHaveSeedCount(int count) {
        haveSeedCount += count;
        _textForSeed.GetComponent<TextMeshProUGUI>().text = "Seed : " + haveSeedCount;
    }
}
