using ComponentModule;
using Manager;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Coordinator.Victims
{
    public class VictimCoordinator : MonoBehaviour, IAttackable
    {
        //방어도 없다ㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏ
        private HPCoordinator _hpCoord;
        private CooldownComponentModule _cooldownModule = null;
        private void Awake()
        {
            _hpCoord = gameObject.GetOrAddComponent<HPCoordinator>();
        }


        public void Init(int hp, int maxHP, float invincibilityTime)
        {
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
    }
}
