using StarterAssets;
using UnityEngine;
using UnityEngine.UI;    // UI가 필요하다면

namespace Waste
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("HP Settings")]
        [SerializeField] private float maxHP = 100f;
        private float _currentHP;

        [Header("UI")]
        [SerializeField] private Slider hpSlider;

        void Awake()
        {
            _currentHP = maxHP;
            if (hpSlider != null)
            {
                hpSlider.maxValue = maxHP;
                hpSlider.value = _currentHP;
            }
        }

        /// <summary>
        /// 외부에서 호출해서 플레이어가 데미지를 입도록
        /// </summary>
        public void TakeDamage(float amount)
        {
            _currentHP -= amount;
            _currentHP = Mathf.Max(_currentHP, 0f);

            if (hpSlider != null)
            {
                hpSlider.value = _currentHP;
            }

            if (_currentHP <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            // 1) 플레이어 컨트롤러 비활성화
            var controller = GetComponent<ThirdPersonController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            // 2) 애니메이션이나 이펙트 재생 (있다면)
            var anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Dead");
            }

            // 3) SystemManager 에 GameOver 알림
            if (SystemManager.Instance != null)
            {
                SystemManager.Instance.Gameover();              
            }
        }
    }
}

