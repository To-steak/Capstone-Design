using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.Rendering.DebugUI;

public class TreePlace_Forest : ObjectInteraction
{
    private ForestManager _forestManager;
    private TextMeshProUGUI _textUI;


    private bool seedPlanted = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _forestManager = GameObject.Find("ForestManager").GetComponent<ForestManager>();
        if (_forestManager == null)
        {
            Debug.LogWarning("This scene has not contain ForestManager");
        }

        _textUI = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void SeedPlanting()
    {
        if(_forestManager.GetHaveSeedCount() >= 1)
        {
            _forestManager.addHaveSeedCount(-1);
            seedPlanted = true;
            this.transform.GetChild(0).gameObject.SetActive(true);
            _forestManager.seedPlanting();
            Debug.Log("Seed Planted");
        }
        else
        {
            ShowTextFaded(_textUI, "doesn't have seed");
        }
    }

    Coroutine coroutine;
    private void ShowTextFaded(TextMeshProUGUI t, string message)
    {
        t.text = message;
        t.enabled = true;
        if(coroutine != null) { StopCoroutine(coroutine); }
        coroutine = StartCoroutine(CoFadeOut(_textUI));
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

    public bool GetSeedPlanted() {  return seedPlanted; }

    public void TreeLogged()
    {
        seedPlanted = false;
        this.transform.GetChild(0).gameObject.SetActive(false);
        _forestManager.LoggingTrees();
    }

}
