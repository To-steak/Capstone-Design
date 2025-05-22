using UnityEngine;

namespace Waste
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private Terrain _terrain;
        [SerializeField] private int _enemyCount;
        [SerializeField] private LayerMask _groundMask;

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
        }

        void Update()
        {

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

