using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;

namespace Waste
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public TMP_Text scoreAndTime;
        public TMP_Text LLMText;
        [SerializeField] private Terrain _terrain;
        [SerializeField] private int _enemyCount;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private int _interval;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            InitializeEnemy();
            StartCoroutine(RequestLLM());
        }

        void Update()
        {

        }

        private IEnumerator RequestLLM()
        {
            while (true)
            {
                if (WebManager.Instance != null)
                {
                    // GetResponse 코루틴이 완료될 때까지 기다렸다가,
                    yield return WebManager.Instance.GetResponse("Water", 1, OnResponse);
                }

                // 10초 대기
                yield return _interval;
            }
        }

        private void OnResponse(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                string pattern = @"<think>[\s\S]*?</think>";
                LLMText.text = Regex.Replace(result, pattern, string.Empty);
            }
        }

        private void InitializeEnemy()
        {
            TerrainData terrainData = _terrain.terrainData;
            Vector3 terrainPos = _terrain.transform.position;

            float width = terrainData.size.x;
            float depth = terrainData.size.y;

            for (int i = 0; i < _enemyCount; i++)
            {
                float x = Random.Range(0f, width);
                float z = Random.Range(0f, depth);
                Vector3 origin = terrainPos + new Vector3(x, 100f, z);

                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 200f, _groundMask))
                {
                    Vector3 spawnPos = hit.point;
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    EnemyPoolManager.Instance.GetEnemy(spawnPos, rotation);
                }
            }
        }
    }
}

