using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class WebManager : MonoBehaviour
{
    private static string url = "http://localhost:8000/response";

    public IEnumerator GetResponse(Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
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
    }
}
