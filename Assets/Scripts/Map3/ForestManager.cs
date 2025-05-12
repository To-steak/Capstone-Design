using StarterAssets;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
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
    private float HumanAISpeed = 5; // persec
    private float HumanAiLogDamage = 20;

    private int maxRegenSeeds = 20;
    private int curRegenSeeds = 0;

    private int score = 0;
    private int humanHuntingScore = 10;
    private int loggingTreesScore = 10; // 벌목되면 이만큼 점수깎임
    private int plantingScore = 10;

    private int haveSeedCount = 0;

    private TextMeshProUGUI _textForSeed;
    private TextMeshProUGUI _textForScore;
    private TextMeshProUGUI _text;

    public GameObject enemyPrefeb;
    GameObject[] EnemySpawnPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = gameDuration;
        _textForSeed = GameObject.Find("TextForSeed").GetComponent<TextMeshProUGUI>();
        _textForScore = GameObject.Find("TextForScore").GetComponent<TextMeshProUGUI>();
        _text = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        curAllowTreeLogging = allowTreeLogging;
        EnemySpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        LevelOfDifficulty(20);

    }

    private void LevelOfDifficulty(float difficulty)
    {
        maxHumanAI = (int)(5 + (0.4 * difficulty));
        TermOfHumanRespawn = (float)(10 - (0.15 * difficulty));
        HumanAiLogDamage = (float)(30 + (difficulty));
        HumanAISpeed = (float)(3 * (1 + (0.1 * difficulty)));

        ThirdPersonController tpc = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        tpc.MoveSpeed = (float)(4 * (1 + (0.05 * difficulty)));
        tpc.SprintSpeed = (float)(8 * (1 + (0.05 * difficulty)));

        loggingTreesScore = (int)(10 + difficulty);
        humanHuntingScore = (int)(10 + difficulty);
        plantingScore = (int)(10 + difficulty);
    }

    //private void PreSetting()
    //{
    //    GameObject[] frontTrees = GameObject.Find("Trees2").transform.GetComponentsInChildren<GameObject>();
    //    for (int i = 0; i < frontTrees.Length; i++)
    //    {
    //        frontTrees[i].transform.GetChild(0).gameObject.SetActive(false);
    //        Debug.Log("setactive");
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
        _textForScore.text = "Score : " + score;

        timer -= Time.deltaTime;
        if(timer <= 0 || curAllowTreeLogging <= 0)
        {
            GameOver();
        }
        if (curHumanAI < maxHumanAI && curTermOfHumanRespawn <= 0)
        { //enemy spawn
            EnemySpawn();
            EnemySpawn();
            EnemySpawn();
        }
        else if (curHumanAI < maxHumanAI)
        {
            curTermOfHumanRespawn -= Time.deltaTime;
        }
    }

    private void GameOver()
    {
        
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
        ShowTextFaded(_text, "Human hunted");
        Debug.Log("Human Hunting score changed / score : " + score);
        curHumanAI--;
    }

    public void LoggingTrees()
    {
        score -= loggingTreesScore;
        ShowTextFaded(_text, "Tree logged");
        Debug.Log("Tree Loged score changed / score : " + score);
        curAllowTreeLogging--;
    }
    public void seedPlanting()
    {
        score += plantingScore;
        ShowTextFaded(_text, "Seed planted");
        Debug.Log("Seed Planting, score changed / score : " + score);
    }

    public void addHaveSeedCount(int count) {
        haveSeedCount += count;
        ShowTextFaded(_text, "Get the seeds");
        _textForSeed.GetComponent<TextMeshProUGUI>().text = "Seed : " + haveSeedCount;
    }

    
    Coroutine coroutine;
    private void ShowTextFaded(TextMeshProUGUI t, string message)
    {
        t.text = message;
        t.enabled = true;
        if (coroutine != null) { StopCoroutine(coroutine); }
        coroutine = StartCoroutine(CoFadeOut(_text));
    }
    IEnumerator CoFadeOut(TextMeshProUGUI t)
    {
        float elapsedTime = 0f; // 누적 경과 시간
        float fadedTime = 2f; // 총 소요 시간

        t.GetComponent<CanvasRenderer>().SetAlpha(1f);
        while (elapsedTime <= fadedTime)
        {
            t.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadedTime));

            elapsedTime += Time.deltaTime;
            Debug.Log("Fade Out 중...");
            yield return null;
        }

        t.GetComponent<TextMeshProUGUI>().enabled = false;
        coroutine = null;
        Debug.Log("Fade Out 끝");
        yield break;
    }

    public float GetHumanAISpeed() { return HumanAISpeed; }
    public float GetHumanAILogDamage() { return HumanAiLogDamage; }
    public int GetCurRegenSeeds() { return curRegenSeeds; }
    public void AddCurRegenSeeds(int i) { curRegenSeeds += i; }
    public bool IsSeedRegenPossible() { return curRegenSeeds < maxRegenSeeds; }
}
