using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class WebManager : MonoBehaviour
{
    private static string url = "http://localhost:8000/response";

    // public IEnumerator GetResponse(Action<string> callback)
    // {
    //     using (UnityWebRequest request = UnityWebRequest.Get(url))
    //     {
    //         yield return request.SendWebRequest();

    //         if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //         {
    //             Debug.LogError($"Error: {request.error}");
    //             callback?.Invoke(null);
    //         }
    //         else
    //         {
    //             string responseText = request.downloadHandler.text;
    //             callback?.Invoke(responseText);
    //         }
    //     }
    // }

    public IEnumerator GetResponse(string role, int damaged, Action<string> callback)
    {
        string json = JsonUtility.ToJson(new RequestData { role = role, damaged = damaged });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
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
}
