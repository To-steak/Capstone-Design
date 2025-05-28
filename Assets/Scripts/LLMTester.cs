using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LLMTester : MonoBehaviour
{
    [SerializeField] private string serverUrl = "http://localhost:8000/response";
    [SerializeField] private int damaged = 1;
    [SerializeField] private string role = "Water";

    [SerializeField] private float maxAllowedResponseTime = 0.5f;
    [SerializeField] private float minExpectedFPS = 55f;
    [SerializeField] private int testRepeatCount = 10;

    // ✅ 버튼 클릭 시 호출
    public void RunLLMTests()
    {
        StartCoroutine(RunTestsMultipleTimes());
    }

    // ✅ 전체 반복 루프
    private IEnumerator RunTestsMultipleTimes()
    {
        for (int i = 1; i <= testRepeatCount; i++)
        {
            Debug.Log($"==== 테스트 {i}회 시작 ====");
            yield return StartCoroutine(RunTests());
            Debug.Log($"==== 테스트 {i}회 종료 ====\n");
        }
    }

    private IEnumerator RunTests()
    {
        yield return StartCoroutine(TestLLMResponseTime());
        yield return StartCoroutine(TestFPSStability());
    }

    private IEnumerator TestLLMResponseTime()
    {
        string json = $"{{\"role\": \"{role}\", \"damaged\": {damaged}}}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        float start = Time.realtimeSinceStartup;
        yield return request.SendWebRequest();
        float elapsed = Time.realtimeSinceStartup - start;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[LLM] 요청 실패: {request.error}");
        }
        else
        {
            Debug.Log($"[LLM] 응답 시간: {elapsed * 1000f:F2} ms");

            if (elapsed > maxAllowedResponseTime)
                Debug.LogWarning($"[LLM] 응답 시간이 기준을 초과했습니다 ({elapsed:F2}s)");
        }
    }

    private IEnumerator TestFPSStability()
    {
        Application.targetFrameRate = 60;
        yield return null; // 한 프레임 대기

        float fpsBefore = 1f / Time.smoothDeltaTime;
        yield return new WaitForSeconds(0.1f); // 안정화 시간

        string json = $"{{\"role\": \"{role}\", \"damaged\": {damaged}}}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        float fpsAfter = 1f / Time.smoothDeltaTime;

        Debug.Log($"[FPS] Before: {fpsBefore:F2}, After: {fpsAfter:F2}");

        if (fpsAfter < minExpectedFPS)
        {
            Debug.LogWarning("[FPS] 프레임 저하 감지됨!");
        }
    }
}
