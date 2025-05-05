using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldTreeManager : MonoBehaviour 
{
    private float gameDuration = 180f;
    private float timer;

    public int damageOfFireOverflow = 10;
    public int TermOfFireOverflow = 10; //sec
    public int maxHumanAI = 5;
    private int curHumanAI = 0;
    public float TermOfHumanRespawn = 5; //sec
    private float curTermOfHumanRespawn = 0;
    public float HumanAISpeed = 10; // persec

    private int score = 0;
    public int FireExtinguishingScore = 10;
    public int HumanHuntingScore = 10;

    public GameObject enemyPrefeb;
    public Image worldTreeHealthBar;
    WorldTree _worldTree;
    GameObject[] EnemySpawnPoints;

    void Start()
    {
        timer = gameDuration;
        _worldTree = GameObject.Find("WorldTree").GetComponent<WorldTree>();
        if (_worldTree == null)
        {
            Debug.Log("worldTree NULL");
        }

        EnemySpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        LevelOfDifficulty(20);
    }

    private void LevelOfDifficulty(float difficulty)
    {
        maxHumanAI = (int)(5 + (0.5 * difficulty));
        TermOfHumanRespawn = (int)(8 - (0.2 * difficulty));
        damageOfFireOverflow = (int)(10 + (0.5 * difficulty));
        TermOfFireOverflow = (int)(15 - (0.25 * difficulty));
        HumanAISpeed = (float)(10 + (0.5 * difficulty));

        ThirdPersonController tpc = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        tpc.MoveSpeed = (float)(2 + (0.5 * difficulty));
        tpc.SprintSpeed = (float)(5 + (0.7 * difficulty));

        FireExtinguishingScore = 10;
        HumanHuntingScore = 10;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 || _worldTree.GetHealth() <= 0)
        {
            GameOver();
        }

        worldTreeHealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(500 * _worldTree.GetHealth() / 100, 20); // UI worldTreeHealthBar
        
        if(curHumanAI < maxHumanAI && curTermOfHumanRespawn <= 0) { //enemy spawn
            EnemySpawn();       
        }
        else if(curHumanAI < maxHumanAI)
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

    public void FireExtinguishing()
    {
        score += FireExtinguishingScore;
        Debug.Log("Fire Extinguishing score changed / score : " + score);
    }

    public void HumanHunting()
    {
        score += HumanHuntingScore;
        Debug.Log("Human Hunting score changed / score : " + score);
        curHumanAI--;
    }

    public void WorldTreeHitByFireOverflow()
    {
        _worldTree.HealthChange(-damageOfFireOverflow);
    }

    void GameOver()
    {

    }
}
