using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class WebManager : MonoBehaviour
{
    public static WebManager Instance;
    private static string url = "http://localhost:8000/";

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public IEnumerator GetResponse(string role, int damaged, Action<string> callback)
    {
        string json = JsonUtility.ToJson(new RequestData { role = role, damaged = damaged });

        UnityWebRequest request = new UnityWebRequest(url + "response", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
            callback?.Invoke(null);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            callback?.Invoke(responseText);
        }
    }

    public IEnumerator PostUserIn(string name, int score, int badge, Action<string> callback)
    {
        UserData payload = new UserData
        {
            name = name,
            score = score,
            badge = badge
        };

        string json = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(url + "users", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error {request.responseCode}: {request.downloadHandler.text}");
            callback?.Invoke(null);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            callback?.Invoke(responseText);
        }
    }

    [Serializable]
    private class RequestData
    {
        public string role;
        public int damaged;
    }

    [Serializable]
    private class UserData
    {
        public string name;
        public int score;
        public int badge;
    }
}
