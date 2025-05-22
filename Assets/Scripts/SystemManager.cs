<<<<<<< Updated upstream
using System.Collections;
=======
using System;
>>>>>>> Stashed changes
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    public static SystemManager Instance;
    public int difficulty;
    public GameObject gameOverCanvas;
    public TMP_Text scoreText;
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
        gameOverCanvas.SetActive(false);

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
        if (scoreText != null)
        {
            scoreText.text = $"Score: {Score:D4}";
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

    public void InitUser()
    {
        Score = 0;
        difficulty = 0;
        userName = $"user@{UnityEngine.Random.Range(0, 10000):D4}";
    }


    public void SceneLoad()
    {
        string currentScene = SceneManager.GetActiveScene().name;
<<<<<<< Updated upstream
        var scenes = new List<string> { "Map1_TreeGuard", "Map3_Forest", "Map4_Waste"};
=======
        List<String> scenes = new List<String> { "Map1_TreeGuard", "Map3_Forest", "Map4_Waste" };
>>>>>>> Stashed changes

        if (!currentScene.Equals("Main"))
        {
            scenes.Remove(currentScene);
        }

<<<<<<< Updated upstream
        int randomIndex = Random.Range(0, scenes.Count);
        
        difficulty++;

        StartCoroutine(LoadSceneC(scenes[randomIndex]));
        //SceneManager.LoadScene(scenes[randomIndex]);

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
=======
        int randomIndex = UnityEngine.Random.Range(0, scenes.Count);

        SceneManager.LoadScene(scenes[randomIndex]);
        difficulty++;
>>>>>>> Stashed changes
    }

    public void Ranking()
    {
        string url = "http://localhost:8000/html";
        Application.OpenURL(url);
    }
}
