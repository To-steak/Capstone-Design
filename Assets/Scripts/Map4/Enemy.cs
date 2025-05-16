using UnityEngine;

namespace Waste
{
    public class Enemy : MonoBehaviour
    {
        // public

        // SerializeField
        [SerializeField] private float maxHP;

        // private
        private float _currentHp;

        void Awake()
        {
            // _currentHp = maxHP * Waste.GameManager.Instance.difficulty;
        }

        void Start()
        {
            
        }

        void Update()
        {

        }
    }
}

