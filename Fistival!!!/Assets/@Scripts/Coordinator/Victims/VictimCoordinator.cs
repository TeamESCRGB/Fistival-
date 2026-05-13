using ComponentModule;
using Coordinator.Movements;
using Manager;
using UnityEngine;
using Utils;

namespace Coordinator.Victims
{
    public class VictimCoordinator : MonoBehaviour, IAttackable, IStunnable
    {
        //방어도 없다ㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏ
        private IStunnable _internalTarget;
        private HPCoordinator _hpCoord;
        private CooldownComponentModule _invincibilityTimeCounter = null;
        private int _maskedLayer = 0;
        private void Awake()
        {
            _hpCoord = gameObject.GetOrAddComponent<HPCoordinator>();
            _internalTarget = transform.parent.GetComponentInParent<IStunnable>();
#if UNITY_EDITOR
            Debug.Assert(_internalTarget != null, $"{gameObject.name} 이 붙어있는 상위 오브젝트 중 IStunnable이 없다.");
#endif
        }


        public void Init(int hp, int maxHP, float invincibilityTime)
        {
            _maskedLayer = 1 << gameObject.layer;
            _hpCoord.Init(hp, maxHP);
            _invincibilityTimeCounter = Managers.Instance.CooldownManager.GetCooldownModule(invincibilityTime);
        }

        private void OnDisable()
        {
            if(_invincibilityTimeCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_invincibilityTimeCounter);
                _invincibilityTimeCounter = null;
            }
        }

        public bool CanAttack()
        {
            if(_invincibilityTimeCounter is null)
            {
                return false;
            }
            return (_hpCoord.IsDead() == false) && _invincibilityTimeCounter.IsCooldownEnded();
        }

        public T RequestComponent<T>() where T : class
        {
            //나중에 기획 더 나오면 자주 쓰이는 컴포넌트들은 미리 내부에 저장해두고, 그게 아닌것들은 다른 경로에서 가져오도록 코드 짜둘 것
            //솔직히 이거 자체가 solid위반이긴 한데, 그렇게 하기에는 성능이 너무 떨어질 가능성이 높음
            return GetComponent<T>();
        }

        public void TakeDamage(int damage)
        {
            if(damage < 0)
            {
                return;
            }

            //스턴 시스템은 나중에

            _hpCoord.SubtractHP(damage);
        }

        public void StartInvincibleTime()
        {
            if(_invincibilityTimeCounter is null)
            {
                return;
            }
            _invincibilityTimeCounter.StartCooldown();
        }

        public int GetMaskedLayer()
        {
            return _maskedLayer;
        }

        public void StunFor(float time)
        {
            _internalTarget?.StunFor(time);
        }

        public void ReleaseStun()
        {
            _internalTarget?.ReleaseStun();
        }
    }
}