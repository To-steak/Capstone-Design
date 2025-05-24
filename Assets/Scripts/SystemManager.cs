using System.Collections;

using System;

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    public static SystemManager Instance;
    public int difficulty;
    public GameObject gameOverCanvas;
    public GameObject gameClearCanvas;
    public TMP_Text scoreTextGameOver;
    public TMP_Text scoreTextGameClear;
    public int Score { get; set; }
    public string userName;
    private WebManager _webManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameOverCanvas);
        DontDestroyOnLoad(gameClearCanvas);
        gameOverCanvas.SetActive(false);
        gameClearCanvas.SetActive(false);

        InitUser();
        _webManager = GameObject.FindWithTag("Web").GetComponent<WebManager>();
        if (_webManager == null)
        {
            Debug.LogWarning("This scene has not contain Web Manager");
        }
    }

    void Start()
    {
       
    }

    void Update()
    {

    }

    public void Gameover()
    {
        
        if (scoreTextGameOver != null)
        {
            //scoreText.text = $"Score: {Score:D4}";
            scoreTextGameOver.text = $"Score: {Score:D4}";
        }

        StartCoroutine(_webManager.PostUserIn(userName, Score, 2, (res) =>
        {
            Debug.Log(res);
        }));

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameOverCanvas.SetActive(true);
    }

    public void GameRestart()
    {
        Time.timeScale = 1.0f;
        Score = 0;
        difficulty = 1;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneLoad();
        InitUser();

    }

    public void GameClear()
    {
        if (scoreTextGameClear != null)
        {
            scoreTextGameClear.text = $"Score: {Score:D4}";
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameClearCanvas.SetActive(true);
    }

    public void GameContinue()
    {
        difficulty++;
        Time.timeScale = 1.0f;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneLoad();

    }

    public void GameExit()
    {
        StartCoroutine(_webManager.PostUserIn(userName, Score, 2, (res) =>
        {
            Debug.Log(res);
        }));
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        LoadSceneC("Main");

    }

    public void InitUser()
    {
        Score = 0;
        difficulty = 1;
        userName = $"user@{UnityEngine.Random.Range(0, 10000):D4}";
        gameOverCanvas.SetActive(false);
        Time.timeScale = 1.0f;
    }


    public void SceneLoad()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        //List<String> scenes = new List<String> { "Map1_TreeGuard", "Map3_Forest", "Map4_Waste" };
        List<String> scenes = new List<String> { "Map1_TreeGuard", "Map3_Forest"};

        if (!currentScene.Equals("Main"))
        {
            scenes.Remove(currentScene);
        }


        int randomIndex = UnityEngine.Random.Range(0, scenes.Count);


        StartCoroutine(LoadSceneC(scenes[randomIndex]));

        //SceneManager.LoadScene("Map4_Waste");
    }

    IEnumerator LoadSceneC(string scene)
    {
        SceneManager.LoadScene(scene);
        yield return null;
        AfterSceneLoad();
    }
    public void AfterSceneLoad()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameOverCanvas.SetActive(false);
        gameClearCanvas.SetActive(false);
    }

    public void Ranking()
    {
        string url = "http://localhost:8000/html";
        Application.OpenURL(url);
    }
}
