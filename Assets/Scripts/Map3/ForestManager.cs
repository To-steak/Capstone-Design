using StarterAssets;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Windows;

public class ForestManager : MonoBehaviour
{
    private float gameDuration = 60f;
    private float timer;

    private int allowTreeLogging = 10;
    private int curAllowTreeLogging;
    private int maxHumanAI;
    private int curHumanAI = 0;
    private float TermOfHumanRespawn; //sec
    private float curTermOfHumanRespawn = 0;
    private float HumanAISpeed; // persec
    private float HumanAiLogDamage;
    private float humanAIHP;
    private int humanAIRegenOnceOfTime;

    private int maxRegenSeeds = 20;
    private int curRegenSeeds = 0;

    private int humanHuntingScore;
    private int loggingTreesScore; // 벌목되면 이만큼 점수깎임
    private int plantingScore;

    private int haveSeedCount = 0;

    private TextMeshProUGUI _textForSeed;
    private TextMeshProUGUI _textForScore;
    private TextMeshProUGUI _text;
    private TextMeshProUGUI _notify;
    private TextMeshProUGUI _time;
    private TextMeshProUGUI _treeLife;
    //private TextMeshProUGUI _gameOver;
    private WebManager _webManager;
    private SystemManager _systemManager;

    public GameObject enemyPrefeb;
    GameObject[] EnemySpawnPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (GameObject.FindWithTag("Web")) { _webManager = GameObject.FindWithTag("Web").GetComponent<WebManager>(); }
        if (_webManager == null)
        {
            Debug.LogWarning("This scene has not contain Web Manager");
        }
        if (GameObject.Find("SystemManager")) { _systemManager = GameObject.Find("SystemManager").GetComponent<SystemManager>(); }
        if (_systemManager == null)
        {
            Debug.LogWarning("This scene has not contain System Manager");
            LevelOfDifficulty(5);
        }
        else
        {
            LevelOfDifficulty(_systemManager.difficulty);
        }

    }
    void Start()
    {
        timer = gameDuration;
        _textForSeed = GameObject.Find("TextForSeed").GetComponent<TextMeshProUGUI>();
        _textForScore = GameObject.Find("TextForScore").GetComponent<TextMeshProUGUI>();
        _text = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        _notify = GameObject.Find("Notify").GetComponent<TextMeshProUGUI>();
        _time = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
        _treeLife = GameObject.Find("TreeLife").GetComponent<TextMeshProUGUI>();
        //_gameOver = GameObject.Find("GameOver").GetComponent<TextMeshProUGUI>();
        EnemySpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        curAllowTreeLogging = allowTreeLogging;
    }

    private void LevelOfDifficulty(float difficulty)
    {
        maxHumanAI = (int)(5 + (0.4 * difficulty));
        TermOfHumanRespawn = (float)(10 - (0.15 * difficulty));
        HumanAiLogDamage = (float)(10 + (difficulty));
        HumanAISpeed = (float)(4 * (1 + (0.1 * difficulty)));
        humanAIHP = 80 + (10 * difficulty);
        humanAIRegenOnceOfTime = (int)(3 + System.Math.Truncate(difficulty / 5));

        ThirdPersonController tpc = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        tpc.MoveSpeed = (float)(4 * (1 + (0.05 * difficulty)));
        tpc.SprintSpeed = (float)(8 * (1 + (0.05 * difficulty)));

        loggingTreesScore = (int)(10 * (difficulty));
        humanHuntingScore = (int)(15 * (difficulty));
        plantingScore = (int)(10 * (difficulty));
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
        _textForScore.text = "Score : " + _systemManager.Score;
        _treeLife.text = "Tree Life : " + curAllowTreeLogging;
        timer -= Time.deltaTime;
        _time.text = "Time : " + timer;
        if(curAllowTreeLogging <= 0)
        {
            GameOver();
        }
        else if(timer <= 0)
        {
            GameClear();
        }

        if (curHumanAI < maxHumanAI && curTermOfHumanRespawn <= 0)
        { //enemy spawn
            for(int i = humanAIRegenOnceOfTime; i > 0; i--)
            {
                EnemySpawn();
            }
        }
        else if (curHumanAI < maxHumanAI)
        {
            curTermOfHumanRespawn -= Time.deltaTime;
        }
    }

    private void GameOver()
    {
        //_gameOver.enabled = true;
        _systemManager.Gameover();
    }

    private void GameClear()
    {
        _systemManager.GameClear();
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
        _systemManager.Score += humanHuntingScore;
        ShowTextFaded(_text, "Human hunted");
        Debug.Log("Human Hunting score changed / score : " + _systemManager.Score);
        curHumanAI--;
    }

    public void LoggingTrees()
    {
        _systemManager.Score -= loggingTreesScore;
        ShowTextFaded(_text, "Tree logged");
        Debug.Log("Tree Loged score changed / score : " + _systemManager.Score);
        curAllowTreeLogging--;

        int i = 10 - curAllowTreeLogging;
        if (i <= 0) { i = 0; }
        StartCoroutine(_webManager.GetResponse("Forest", i, (res) => {
            ShowTextFaded(_notify, Regex.Match(res, @"'([^']*)'").Groups[1].Value, 10f);
        }));
        
    }
    public void seedPlanting()
    {
        _systemManager.Score += plantingScore;
        ShowTextFaded(_text, "Seed planted");
        Debug.Log("Seed Planting, score changed / score : " + _systemManager.Score);
    }

    public void addHaveSeedCount(int count) {
        haveSeedCount += count;
        ShowTextFaded(_text, "Get the seeds");
        _textForSeed.GetComponent<TextMeshProUGUI>().text = "Seed : " + haveSeedCount;
    }

    
    Coroutine coroutine;
    private void ShowTextFaded(TextMeshProUGUI t, string message, float time = 2f)
    {
        t.text = message;
        t.enabled = true;
        if (coroutine != null) { StopCoroutine(coroutine); }
        coroutine = StartCoroutine(CoFadeOut(_text, time));
    }
    IEnumerator CoFadeOut(TextMeshProUGUI t, float time)
    {
        float elapsedTime = 0f; // 누적 경과 시간
        float fadedTime = time; // 총 소요 시간

        t.GetComponent<CanvasRenderer>().SetAlpha(1f);
        while (elapsedTime <= fadedTime)
        {
            t.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadedTime));

            elapsedTime += Time.deltaTime;
            //Debug.Log("Fade Out 중...");
            yield return null;
        }

        t.GetComponent<TextMeshProUGUI>().enabled = false;
        coroutine = null;
        //Debug.Log("Fade Out 끝");
        yield break;
    }

    public float GetHumanAISpeed() { return HumanAISpeed; }
    public float GetHumanAILogDamage() { return HumanAiLogDamage; }
    public int GetCurRegenSeeds() { return curRegenSeeds; }
    public void AddCurRegenSeeds(int i) { curRegenSeeds += i; }
    public bool IsSeedRegenPossible() { return curRegenSeeds < maxRegenSeeds; }
    public float GetHumanAIHP() {  return humanAIHP; }
}
