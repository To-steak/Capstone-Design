using System.Collections.Generic;
using UnityEngine;

namespace Waste
{
    public class EnemyPoolManager : MonoBehaviour
    {
        public static EnemyPoolManager Instance { get; private set;}

        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private int _poolSize;

        private Queue<GameObject> _pool = new Queue<GameObject>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            for (int i = 0; i < _poolSize; i++)
            {
                GameObject enemy = Instantiate(_enemyPrefab);
                enemy.SetActive(false);
                _pool.Enqueue(enemy);
            }
        }

        public GameObject GetEnemy(Vector3 position, Quaternion rotation)
        {
            GameObject enemy = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(_enemyPrefab);
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.SetActive(true);
            return enemy;
        }

        public void ReturnEnemy(GameObject enemy)
        {
            enemy.SetActive(false);
            _pool.Enqueue(enemy);
        }
    }
}

