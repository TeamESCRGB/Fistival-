using UnityEngine;
using Utils;

namespace Coordinator.Victims
{
    public class VictimCoordinator : MonoBehaviour, IAttackable
    {
        //방어도 없다ㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏ
        private HPCoordinator _hpCoord;
        private void Awake()
        {
            _hpCoord = gameObject.GetOrAddComponent<HPCoordinator>();
        }


        public void Init(int hp, int maxHP, float invincibilityTime)
        {
            _hpCoord.Init(hp, maxHP);
        }

        public bool CanAttack()
        {
            return !_hpCoord.IsDead();//무적시간도 고려할 것
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
            //not implemented yet
            //to be implemented when CooldownSystem
        }
    }
}
