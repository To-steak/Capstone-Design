using StarterAssets;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldTreeManager : MonoBehaviour 
{
    private float gameDuration = 180f;
    private float timer;

    private int damageOfFireOverflow;
    private float TermOfFireOverflow; //sec
    private int maxHumanAI;
    private int curHumanAI = 0;
    private float TermOfHumanRespawn; //sec
    private float curTermOfHumanRespawn = 0;
    private float HumanAISpeed; // persec

    private int score = 0;
    private int FireExtinguishingScore;
    private int HumanHuntingScore;

    public GameObject enemyPrefeb;
    public Image worldTreeHealthBar;
    private float healthBarLen;
    private TextMeshProUGUI _textForScore;
    private TextMeshProUGUI _text;
    private TextMeshProUGUI _notify;
    private TextMeshProUGUI _gameover;
    WorldTree _worldTree;
    private WebManager _webManager;
    GameObject[] EnemySpawnPoints;

    private void Awake()
    {
        _webManager = GameObject.FindWithTag("Web").GetComponent<WebManager>();
        if (_webManager == null)
        {
            Debug.LogWarning("This scene has not contain Web Manager");
        }

        _worldTree = GameObject.Find("WorldTree").GetComponent<WorldTree>();
        if (_worldTree == null)
        {
            Debug.Log("worldTree NULL");
        }
        LevelOfDifficulty(20);
    }
    void Start()
    {
        timer = gameDuration;
        
        
        _text = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        _textForScore = GameObject.Find("TextForScore").GetComponent<TextMeshProUGUI>();
        EnemySpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        _notify = GameObject.Find("Notify").GetComponent<TextMeshProUGUI>();
        _gameover = GameObject.Find("GameOver").GetComponent<TextMeshProUGUI>();
        healthBarLen = worldTreeHealthBar.rectTransform.rect.width;
    }

    private void LevelOfDifficulty(float difficulty)
    {
        maxHumanAI = (int)(3 + (0.5 * difficulty));
        TermOfHumanRespawn = (float)(10 - (0.15 * difficulty));
        damageOfFireOverflow = (int)(10 + (0.5 * difficulty));
        TermOfFireOverflow = (float)(20 - (0.25 * difficulty));
        HumanAISpeed = (float)(3 * (1 + (0.1 * difficulty)));

        ThirdPersonController tpc = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        tpc.MoveSpeed = (float)(4 * (1 + (0.1 * difficulty)));
        tpc.SprintSpeed = (float)(8 * (1 + (0.1 * difficulty)));

        FireExtinguishingScore = (int)(10 * (difficulty + 1));
        HumanHuntingScore = (int)(10 * (difficulty + 1));
    }

    private void Update()
    {
        _textForScore.text = "Score : " + score;

        timer -= Time.deltaTime;
        if (timer <= 0 || _worldTree.GetHealth() <= 0)
        {
            GameOver();
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
        score += FireExtinguishingScore;
        ShowTextFaded(_text, "Fire Extinguished");
        Debug.Log("Fire Extinguishing score changed / score : " + score);
    }

    public void HumanHunting()
    {
        score += HumanHuntingScore;
        ShowTextFaded(_text, "Human Hunted");
        Debug.Log("Human Hunting score changed / score : " + score);
        curHumanAI--;
    }

    public void WorldTreeHitByFireOverflow()
    {
        _worldTree.HealthChange(-damageOfFireOverflow);
        StartCoroutine(_webManager.GetResponse("World Tree", 10 - (int)(_worldTree.GetHealth() / 10), (res) =>
        {
            ShowTextFaded(_notify, res, 10f);
        }));
    }

    void GameOver()
    {
        _gameover.enabled = true;
    }

    Coroutine coroutine;
    public float GetHumanAISpeed() { return HumanAISpeed; }
    public float GetTermOfFireOverflow() { return TermOfFireOverflow; }

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
