using StarterAssets;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldTreeManager : MonoBehaviour 
{
    private float gameDuration = 60f;
    private float timer;

    private int damageOfFireOverflow;
    private float TermOfFireOverflow; //sec
    private int maxHumanAI;
    private int curHumanAI = 0;
    private float TermOfHumanRespawn; //sec
    private float curTermOfHumanRespawn = 0;
    private float HumanAISpeed; // persec
    private float humanAIHP;

    private int FireExtinguishingScore;
    private int HumanHuntingScore;

    public GameObject enemyPrefeb;
    public Image worldTreeHealthBar;
    private float healthBarLen;
    private TextMeshProUGUI _textForScore;
    private TextMeshProUGUI _text;
    private TextMeshProUGUI _notify;
    private TextMeshProUGUI _time;
    //private TextMeshProUGUI _gameover;
    WorldTree _worldTree;
    private WebManager _webManager;
    private SystemManager _systemManager;
    GameObject[] EnemySpawnPoints;

    private void Awake()
    {
        if(GameObject.FindWithTag("Web")) { _webManager = GameObject.FindWithTag("Web").GetComponent<WebManager>(); }
        if (_webManager == null)
        {
            Debug.LogWarning("This scene has not contain Web Manager");
        }

        _worldTree = GameObject.Find("WorldTree").GetComponent<WorldTree>();
        if (_worldTree == null)
        {
            Debug.Log("worldTree NULL");
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
        
        
        _text = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        _textForScore = GameObject.Find("TextForScore").GetComponent<TextMeshProUGUI>();
        EnemySpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        _notify = GameObject.Find("Notify").GetComponent<TextMeshProUGUI>();
        _time = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
        //_gameover = GameObject.Find("GameOver").GetComponent<TextMeshProUGUI>();
        healthBarLen = worldTreeHealthBar.rectTransform.rect.width;
    }

    private void LevelOfDifficulty(float difficulty)
    {
        maxHumanAI = (int)(3 + (0.5 * difficulty));
        TermOfHumanRespawn = (float)(10 - (0.15 * difficulty));
        damageOfFireOverflow = (int)(10 + (0.5 * difficulty));
        TermOfFireOverflow = (float)(20 - (0.25 * difficulty));
        HumanAISpeed = (float)(4 * (1 + (0.1 * difficulty)));
        humanAIHP = 80 + (10 * difficulty);

        ThirdPersonController tpc = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        tpc.MoveSpeed = (float)(4 * (1 + (0.1 * difficulty)));
        tpc.SprintSpeed = (float)(8 * (1 + (0.1 * difficulty)));

        FireExtinguishingScore = (int)(10 * (difficulty));
        HumanHuntingScore = (int)(15 * (difficulty));
    }

    private void Update()
    {
        _textForScore.text = "Score : " + _systemManager.Score;

        timer -= Time.deltaTime;
        _time.text = "Time : " + timer;
        if (_worldTree.GetHealth() <= 0)
        {
            GameOver();
        }
        else if(timer <= 0)
        {
            GameClear();
        }
        
        worldTreeHealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBarLen * _worldTree.GetHealth() / 100, 20); // UI worldTreeHealthBar
        
        if(curHumanAI < maxHumanAI && curTermOfHumanRespawn <= 0) { //enemy spawn
            EnemySpawn();
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
        _systemManager.Score += FireExtinguishingScore;
        ShowTextFaded(_text, "Fire Extinguished");
        Debug.Log("Fire Extinguishing score changed / score : " + _systemManager.Score);
    }

    public void HumanHunting()
    {
        _systemManager.Score += HumanHuntingScore;
        ShowTextFaded(_text, "Human Hunted");
        Debug.Log("Human Hunting score changed / score : " + _systemManager.Score);
        curHumanAI--;
    }

    public void WorldTreeHitByFireOverflow()
    {
        _worldTree.HealthChange(-damageOfFireOverflow);
        StartCoroutine(_webManager.GetResponse("World Tree", 10 - (int)(_worldTree.GetHealth() / 10), (res) =>
        {
            ShowTextFaded(_notify, Regex.Match(res, @"'([^']*)'").Groups[1].Value, 10f);
        }));
    }

    
    void GameOver()
    {
        //_gameover.enabled = true;
        _systemManager.Gameover();
    }

    void GameClear()
    {
        _systemManager.GameClear();
    }
    
    public float GetHumanAISpeed() { return HumanAISpeed; }
    public float GetTermOfFireOverflow() { return TermOfFireOverflow; }
    public float GetHumanAIHP() { return humanAIHP; }

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
}
