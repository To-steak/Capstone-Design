using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Waste
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour
    {
        private enum State { Wander, Chase }

        [SerializeField] private float maxHP;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackInterval = 1.5f;
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float minWanderInterval = 3f;
        [SerializeField] private float maxWanderInterval = 5f;
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private float attackDamage = 10f;

        private NavMeshAgent _agent;
        private Animator _animator;
        private State _state = State.Wander;
        private Transform _playerTransform;
        private Coroutine _wanderCoroutine;
        private float _currentHp;
        private bool _isDead;
        private float _nextAttackTime = 0f;
        private bool _isAttacking = false;
        private PlayerHealth _playerHealth;

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = attackRange;
            _animator = GetComponent<Animator>();
            _isDead = false;
            _nextAttackTime = 0f;
            if (SystemManager.Instance != null)
            {
                attackDamage = 10 + 1 * SystemManager.Instance.difficulty;
            }

            if (SystemManager.Instance == null)
            {
                _currentHp = maxHP;
                Debug.Log("System Manager is null");
            }
            else
            {
                _currentHp = maxHP * SystemManager.Instance.difficulty;
            }
        }

        void Start()
        {
            EnterWander();
        }

        void Update()
        {
            if (!_isDead && _currentHp <= 0f)
            {
                _isDead = true;
                Die();
            }

            if (_isDead)
            {
                return;
            }

            // 애니메이터에 speed 전달
            _animator.SetFloat("Speed", _agent.velocity.magnitude);

            // 공격 딜레이 계산
            _nextAttackTime += Time.deltaTime;

            // 매 프레임마다 반경 내 플레이어 검색
            var detected = DetectPlayerInRadius();
            if (detected != null)
            {
                if (_state != State.Chase)
                {
                    EnterChase(detected);
                }

                float distance = Vector3.Distance(transform.position, detected.position);
                if (distance <= attackRange && _nextAttackTime >= attackInterval && !_isAttacking)
                {
                    _isAttacking = true;
                    _agent.isStopped = true;
                    _animator.SetTrigger("Attack");
                    _nextAttackTime = 0f;
                    var ph = _playerTransform.GetComponent<PlayerHealth>();

                    if (ph != null)
                    {
                        ph.TakeDamage(attackDamage);
                    }

                    StartCoroutine(AttackRoutine());
                }
                else
                {
                    _agent.isStopped = false;
                    _agent.SetDestination(detected.position);
                }
            }
            else
            {
                if (_state != State.Wander)
                {
                    EnterWander();
                }
            }

            // Chase 상태에서는 매 프레임 목적지 갱신
            if (_state == State.Chase && _playerTransform != null)
            {
                _agent.SetDestination(_playerTransform.position);
            }
        }

        private IEnumerator AttackRoutine()
        {
            yield return new WaitForSeconds(2.3f);

            _agent.isStopped = false;
            _isAttacking = false;
        }

        public void TakeDamage(float amount)
        {
            _currentHp -= amount;
            Debug.Log($"Enemy took {amount} dmg, remaining HP: {_currentHp}");
        }

        private void Die()
        {
            _animator.SetTrigger("Dead");

            if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
                _agent.enabled = false;
            }

            StopCoroutine(WanderRoutine());

            StartCoroutine(DeathDelayRoutine());
        }

        private IEnumerator DeathDelayRoutine()
        {
            yield return new WaitForSeconds(3f);
            EnemyPoolManager.Instance.ReturnEnemy(gameObject);
        }

        private void EnterWander()
        {
            _state = State.Wander;
            _playerTransform = null;
            _animator.ResetTrigger("Attack");

            if (_wanderCoroutine != null)
            {
                StopCoroutine(_wanderCoroutine);
            }
            _wanderCoroutine = StartCoroutine(WanderRoutine());
        }

        private void EnterChase(Transform player)
        {
            _state = State.Chase;
            _playerTransform = player;
            _animator.SetTrigger("Attack");

            if (_wanderCoroutine != null)
            {
                StopCoroutine(_wanderCoroutine);
            }
        }

        private IEnumerator WanderRoutine()
        {
            while (_state == State.Wander)
            {
                if (RandomNavMeshPosition(transform.position, wanderRadius, out Vector3 randomPos))
                {
                    if (_agent.enabled)
                    {
                        _agent.SetDestination(randomPos);
                    }
                }
                float wait = Random.Range(minWanderInterval, maxWanderInterval);
                yield return new WaitForSeconds(wait);
            }
        }

        private Transform DetectPlayerInRadius()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, wanderRadius);
            foreach (var col in hits)
            {
                if (col.CompareTag(playerTag))
                {
                    return col.transform;
                }
            }

            return null;
        }

        private bool RandomNavMeshPosition(Vector3 origin, float radius, out Vector3 result)
        {
            Vector3 randomPoint = origin + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
            result = origin;
            return false;
        }
    }
}
