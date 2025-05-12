using System.Collections.Generic;
using UnityEngine;

namespace Grove
{
    public class TreePoolManager : MonoBehaviour
    {
        public static TreePoolManager Instance { get; private set;}

        [SerializeField] private GameObject _treePrefab;
        [SerializeField] private int _poolSize = 10;

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
                GameObject tree = Instantiate(_treePrefab);
                tree.SetActive(false);
                _pool.Enqueue(tree);
            }
        }

        /// <summary>
        /// Get tree from the pool
        /// Create a tree if there is no tree in the pool
        /// </summary>
        /// <param name="position">tree's position</param>
        /// <param name="rotation">tree's rotation</param>
        /// <returns></returns>
        public GameObject GetTree(Vector3 position, Quaternion rotation)
        {
            GameObject tree = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(_treePrefab);
            tree.transform.position = position;
            tree.transform.rotation = rotation;
            tree.SetActive(true);
            return tree;
        }

        /// <summary>
        /// tree return
        /// </summary>
        /// <param name="tree">target object will be returned</param>
        public void ReturnTree(GameObject tree)
        {
            tree.SetActive(false);
            _pool.Enqueue(tree);
        }
    }
}

