using ComponentModule;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.Contents
{
    public class CooldownManager : MonoBehaviour
    {
        private List<FixedCooldownComponentModule> _fixedCooldownObjects = new List<FixedCooldownComponentModule>(128);
        private List<CooldownComponentModule> _cooldownObjects = new List<CooldownComponentModule>(128);
        private int _idx = 0;
        private int _fixedIdx = 0;

        private void Awake()
        {
            for (int i = 0; i < 64; i++)
            {
                _cooldownObjects.Add(new CooldownComponentModule());
            }

            for (int i = 0; i < 64; i++)
            {
                _fixedCooldownObjects.Add(new FixedCooldownComponentModule());
            }
        }

        private T GetCooldownModuleInternal<T>(float cooldownTime,ref int idx, List<T> cooldownPool, float timeChangedCallInterval) where T : ICooldownComponentModuleBase, new()
        {
            T tmp = default(T);

            if (idx >= cooldownPool.Count)
            {
                cooldownPool.Add(new T());
            }

            tmp = cooldownPool[idx];

            tmp.InitCooldown(cooldownTime, idx, timeChangedCallInterval);
            idx++;
            return tmp;
        }

        private void ReturnCooldownModuleInternal<T>(T module, List<T> cooldownPool, ref int idx) where T : ICooldownComponentModuleBase
        {
            if (module is null || idx <= 0 || module.Index < 0)
            {
                return;
            }

            idx--;

            var last = cooldownPool[idx];
            cooldownPool[idx] = module;
            cooldownPool[module.Index] = last;

            last.Index = module.Index;
            module.Index = -666775;
            module.DeinitCooldown();
        }

        #region Impl
        public CooldownComponentModule GetCooldownModule(float cooldownTime, float timeChangedCallInterval = 1)
        {
            return GetCooldownModuleInternal<CooldownComponentModule>(cooldownTime, ref _idx, _cooldownObjects, timeChangedCallInterval);
        }

        public FixedCooldownComponentModule GetFixedCooldownModule(float cooldownTime, float timeChangedCallInterval = 1)
        {
            return GetCooldownModuleInternal<FixedCooldownComponentModule>(cooldownTime, ref _fixedIdx, _fixedCooldownObjects, timeChangedCallInterval);
        }

        public void ReturnModule(CooldownComponentModule module)
        {
            ReturnCooldownModuleInternal<CooldownComponentModule>(module, _cooldownObjects, ref _idx);
        }

        public void ReturnFixedModule(FixedCooldownComponentModule module)
        {
            ReturnCooldownModuleInternal<FixedCooldownComponentModule>(module, _fixedCooldownObjects, ref _fixedIdx);
        }

        #endregion





        public void Compact()
        {
            _cooldownObjects.RemoveRange(_idx, _cooldownObjects.Count);
            _fixedCooldownObjects.RemoveRange(_fixedIdx, _fixedCooldownObjects.Count);
        }

        private void Update()
        {
            //나중에 GameContext라던지, GameManager라던지 게임 상태 관리해줄거 하나 만들면, 거기서 IsGamePaused하나 때와서 검사한다 이건
            float dt = Time.deltaTime;
            for (int i = 0; i < _idx; i++)
            {
                _cooldownObjects[i].Tick(dt);
            }
        }

        private void FixedUpdate()
        {
            //나중에 GameContext라던지, GameManager라던지 게임 상태 관리해줄거 하나 만들면, 거기서 IsGamePaused하나 때와서 검사한다 이건
            float dt = Time.fixedDeltaTime;
            for (int i = 0; i < _fixedIdx; i++)
            {
                _fixedCooldownObjects[i].Tick(dt);
            }
        }
    }
}
#if false
CooldownComponentModule tmp = null;

if (_idx >= _cooldownObjects.Count)
{
    _cooldownObjects.Add(new CooldownComponentModule());
}

tmp = _cooldownObjects[_idx];

tmp.InitCooldown(cooldownTime, _idx, timeChangedCallInterval);
_idx++;
return tmp;

FixedCooldownComponentModule tmp = null;

if (_fixedIdx >= _fixedCooldownObjects.Count)
{
    _fixedCooldownObjects.Add(new FixedCooldownComponentModule());
}

tmp = _fixedCooldownObjects[_fixedIdx];

tmp.InitCooldown(cooldownTime, _fixedIdx, timeChangedCallInterval);
_fixedIdx++;
return tmp;



if (module is null || _idx <= 0 || module.Index < 0)
            {
                return;
            }

            _idx--;

            var last = _cooldownObjects[_idx];
            _cooldownObjects[_idx] = module;
            _cooldownObjects[module.Index] = last;

            last.Index = module.Index;

            module.DeinitCooldown();

            if (module is null || _fixedIdx <= 0 || module.Index < 0)
            {
                return;
            }

            _fixedIdx--;

            var last = _fixedCooldownObjects[_fixedIdx];
            _fixedCooldownObjects[_fixedIdx] = module;
            _fixedCooldownObjects[module.Index] = last;

            last.Index = module.Index;

            module.DeinitCooldown();
#endif