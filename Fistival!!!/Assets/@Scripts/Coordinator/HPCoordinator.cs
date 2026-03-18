using ComponentModule;
using System;
using UnityEngine;

namespace Coordinator
{
    public class HPCoordinator : MonoBehaviour
    {
        private event Action<int, int, int> OnHPChanged;//old,now,delta
        private event Action OnDead;
        private bool _isDead=false;
        private HPComponentModule _hpModule;// <= 이거 나중에 object pooling 가능할거같기도 한데, 일단 이렇게 둔다.
        private void Awake()
        {
            _hpModule = new HPComponentModule();

            if(_hpModule is null)
            {
#if UNITY_EDITOR
                Debug.LogError($"{gameObject.name} 이 HPComponentModule 생성에 실패함");
#endif
            }
        }

        public void Init(int hp, int maxHP, bool resetEvent =false)//일단 이렇게 해두는데, 이벤트를 리셋하는 경우는 아마 없을듯
        {

            if(resetEvent)
            {
                OnHPChanged = null;
                OnDead = null;
            }

            _isDead = false;
            _hpModule.Init(hp, maxHP);
        }

        public int GetHP()
        {
            return _hpModule.GetHP();
        }

        public void AddHP(int hp)
        {
            int old = _hpModule.GetHP();
            _hpModule.AddHP(hp);

            OnHPChanged?.Invoke(old,_hpModule.GetHP(),hp);
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public void SubtractHP(int hp)
        {
            int old = _hpModule.GetHP();
            bool ret = _hpModule.SubHP(hp);

            OnHPChanged?.Invoke(old, _hpModule.GetHP(), hp);
            if(ret)
            {
                OnDead?.Invoke();
                _isDead = true;
            }
        }

        public void SubscribeOnHPChanged(Action<int,int,int> callback)
        {
            OnHPChanged -= callback;
            OnHPChanged += callback;
        }

        public void UnSubscribeOnHPChanged(Action<int, int, int> callback)
        {
            OnHPChanged -= callback;
        }

        public void SubscribeOnDead(Action callback)
        {
            OnDead -= callback;
            OnDead += callback;
        }

        public void UnSubscribeOnDead(Action callback)
        {
            OnDead -= callback;
        }

    }
}