using UnityEngine;

namespace Waste
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private GameObject _fieldPlane;
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
            InitializeTree();
        }

        void Update()
        {

        }

        private void InitializeTree()
        {
            Vector3 center = _fieldPlane.transform.position;
            Vector3 scale = _fieldPlane.transform.localScale;

            float width = scale.x * 10f;
            float depth = scale.y * 10f;

            for (int i = 0; i < _enemyCount; i++)
            {
                float x = Random.Range(-width / 2f, width / 2f);
                float z = Random.Range(-depth / 2f, depth / 2f);
                Vector3 origin = center + new Vector3(x, 50f, z);

                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 100f, _groundMask))
                {
                    Vector3 spawnPos = hit.point;
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    EnemyPoolManager.Instance.GetEnemy(spawnPos, rotation);
                }
            }
        }
    }
}

