using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;    // UI가 필요하다면

namespace Waste
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("HP Settings")]
        [SerializeField] private float maxHP = 100f;
        public float currentHP;

        [Header("UI")]
        [SerializeField] private Slider hpSlider;
        private Animator _animator;
        private ThirdPersonController _controller;

        void Awake()
        {
            currentHP = maxHP;
            _animator = GetComponent<Animator>();
            _controller = GetComponent<ThirdPersonController>();
            _controller.enabled = true;
            _animator.enabled = true;

            if (hpSlider != null)
            {
                hpSlider.maxValue = maxHP;
                hpSlider.value = currentHP;
            }
        }

        /// <summary>
        /// 외부에서 호출해서 플레이어가 데미지를 입도록
        /// </summary>
        public void TakeDamage(float amount)
        {
            currentHP -= amount;
            currentHP = Mathf.Max(currentHP, 0f);

            if (hpSlider != null)
            {
                hpSlider.value = currentHP;
            }

            if (currentHP <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            // 1) 플레이어 컨트롤러 비활성화
            if (_controller != null)
            {
                _controller.enabled = false;
            }

            // 2) 애니메이션이나 이펙트 재생 (있다면)
            if (_animator != null)
            {
                _animator.SetTrigger("Dead");

                StartCoroutine(DeathDelay());
            }

            // 3) SystemManager 에 GameOver 알림
            if (SystemManager.Instance != null)
            {
                SystemManager.Instance.Gameover();
            }
        }

        private IEnumerator DeathDelay()
        {
            yield return new WaitForSeconds(3.16f);

            _animator.enabled = false;
        }
    }
}

