using ComponentModule;
using Data.NonLodable;
using Manager;
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
        private HPSoundKeys _hpSoundKeys;
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

        public void Init(int hp, int maxHP, HPSoundKeys hpSoundKeys)//일단 이렇게 해두는데, 이벤트를 리셋하는 경우는 아마 없을듯
        {
            _hpSoundKeys = hpSoundKeys;
            _isDead = false;
            _hpModule.Init(hp, maxHP);
        }

        private void OnDisable()
        {
            OnHPChanged = null;
            OnDead = null;
        }


        public void Respawn()
        {
            _isDead = false;
            _hpModule.Respawn();
            OnHPChanged?.Invoke(0, _hpModule.GetHP(), _hpModule.GetHP());
        }

        public void SetMaxHP(int maxHP)
        {
            int old = _hpModule.GetHP();
            if(old > maxHP)
            {
                OnHPChanged?.Invoke(old, maxHP, maxHP - old);
            }

            _hpModule.Init(maxHP, maxHP);//문제
            bool isDeadOld = _isDead;
            _isDead = _hpModule.IsDead();

            if(isDeadOld == false && _isDead)
            {
                OnDead?.Invoke();
            }
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
            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, _hpSoundKeys.Hit, false, Managers.Instance.GameManager.SFXVolume);
            OnHPChanged?.Invoke(old, _hpModule.GetHP(), hp);
            if(ret)
            {
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, _hpSoundKeys.Dead, false, Managers.Instance.GameManager.SFXVolume);
                _isDead = true;
                OnDead?.Invoke();
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