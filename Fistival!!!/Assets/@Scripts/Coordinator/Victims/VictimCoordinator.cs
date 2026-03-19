using ComponentModule;
using Manager;
using UnityEngine;
using Utils;

namespace Coordinator.Victims
{
    public class VictimCoordinator : MonoBehaviour, IAttackable
    {
        //방어도 없다ㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏ
        private HPCoordinator _hpCoord;
        private CooldownComponentModule _cooldownModule = null;
        private int _maskedLayer = 0;
        private void Awake()
        {
            _hpCoord = gameObject.GetOrAddComponent<HPCoordinator>();
        }


        public void Init(int hp, int maxHP, float invincibilityTime)
        {
            _maskedLayer = 1 << gameObject.layer;
            _hpCoord.Init(hp, maxHP);
            _cooldownModule = Managers.Instance.CooldownManager.GetCooldownModule(invincibilityTime);
        }

        private void OnDisable()
        {
            if(_cooldownModule is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_cooldownModule);
                _cooldownModule = null;
            }
        }

        public bool CanAttack()
        {
            if(_cooldownModule is null)
            {
                return false;
            }
            return (_hpCoord.IsDead() == false) && _cooldownModule.IsCooldownEnded();//무적시간도 고려할 것
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
            if(_cooldownModule is null)
            {
                return;
            }
            _cooldownModule.StartCooldown();
        }

        public int GetMaskedLayer()
        {
            return _maskedLayer;
        }
    }
}
