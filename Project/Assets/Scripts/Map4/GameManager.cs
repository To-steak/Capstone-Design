using TMPro;
using UnityEngine;
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
        private PlayerHealth _playerHealth;

        void Awake()
        {
            _playerHealth = GameObject.Find("PlayerArmature").GetComponent<PlayerHealth>();
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            if (SystemManager.Instance != null)
            {
                _enemyCount += SystemManager.Instance.difficulty;
            }
            Instance = this;
        }

        void Start()
        {
            InitializeEnemy();
            StartCoroutine(RequestLLM());
        }

        private IEnumerator RequestLLM()
        {
            while (true)
            {
                // 살아있는 경우만 llm 응답 요청
                if (WebManager.Instance != null && _playerHealth.currentHP > 0)
                {
                    int temp = (int)_playerHealth.currentHP / 100;
                    // GetResponse 코루틴이 완료될 때까지 기다렸다가,
                    yield return WebManager.Instance.GetResponse("Water", temp, (result) =>
                    {
                        LLMText.text = SystemManager.Instance.OnResponse(result);
                    });
                }

                // 10초 대기
                yield return _interval;
            }
        }

        private void InitializeEnemy()
        {
            Vector3 terrainPos = _terrain.transform.position;
            TerrainData data = _terrain.terrainData;

            for (int i = 0; i < _enemyCount; i++)
            {
                float x = Random.Range(terrainPos.x, terrainPos.x + data.size.x);
                float z = Random.Range(terrainPos.z, terrainPos.z + data.size.z);

                float y = _terrain.SampleHeight(new Vector3(x, 0, z)) + terrainPos.y;

                Vector3 spawnPos = new Vector3(x, y, z);
                Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                EnemyPoolManager.Instance.GetEnemy(spawnPos, rot);
            }
            scoreAndTime.text = $"left enemies\n{_enemyCount}";
        }

        public void GameClear()
        {
            _enemyCount--;
            scoreAndTime.text = $"left enemies\n{_enemyCount}";
            if (SystemManager.Instance != null)
            {
                SystemManager.Instance.Score += 10 * SystemManager.Instance.difficulty;
            }
            else
            {
                print("system manager is null: can't add score");
            }
            if (_enemyCount == 0)
            {
                // gameclear
                SystemManager.Instance.GameClear();
            }
        }


    }
}