using System.Collections;

using System;
using System.Text.RegularExpressions;

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

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

    public void Gameover()
    {

        if (scoreTextGameOver != null)
        {
            //scoreText.text = $"Score: {Score:D4}";
            scoreTextGameOver.text = $"Score: {Score:D4}";
        }

        if (Score < 0)
            Score = 0;

        int badge;
        if (Score < 1000)
            badge = 0;
        else if (Score < 2000)
            badge = 1;
        else if (Score < 3000)
            badge = 2;
        else
            badge = 4;

        StartCoroutine(_webManager.PostUserIn(userName, Score, badge, (res) =>
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
            scoreTextGameClear.text = $"{userName}\nScore: {Score:D4}";
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
        gameClearCanvas.SetActive(false);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneLoad();
    }

    public void GameExit()
    {
        if (Score < 0)
            Score = 0;

        int badge;
        if (Score < 1000)
            badge = 0;
        else if (Score < 2000)
            badge = 1;
        else if (Score < 3000)
            badge = 2;
        else
            badge = 4;

        StartCoroutine(_webManager.PostUserIn(userName, Score, badge, (res) =>
        {
            Debug.Log(res);
        }));
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        LoadSceneC("Main");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
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

        List<String> scenes = new List<String> { "Map1_TreeGuard", "Map3_Forest", "Map4_Waste" };

        if (!currentScene.Equals("Main"))
        {
            scenes.Remove(currentScene);
        }

        int randomIndex = UnityEngine.Random.Range(0, scenes.Count);

        StartCoroutine(LoadSceneC(scenes[randomIndex]));
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

    public String OnResponse(string result)
    {
        if (string.IsNullOrEmpty(result))
            return "";

        LLMResp resp = JsonUtility.FromJson<LLMResp>(result);
        string text = resp.message;

        string pattern = @"<think>[\s\S]*?</think>";
        text = Regex.Replace(text, pattern, string.Empty);

        text = text.TrimStart('\r', '\n');
        return text;
    }
    private class LLMResp
    {
        public string message;
    }
}
