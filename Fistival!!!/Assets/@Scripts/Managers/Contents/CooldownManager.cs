using ComponentModule;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.Contents
{
    public class CooldownManager : MonoBehaviour
    {
        private List<CooldownComponentModule> _cooldownObjects = new List<CooldownComponentModule>(128);
        private int _idx = 0;

        private void Awake()
        {
            for (int i = 0; i < 64; i++)
            {
                _cooldownObjects.Add(new CooldownComponentModule());
            }
        }

        public CooldownComponentModule GetCooldownModule(float cooldownTime)
        {
            CooldownComponentModule tmp = null;

            if (_idx >= _cooldownObjects.Count)
            {
                _cooldownObjects.Add(new CooldownComponentModule());
            }

            tmp = _cooldownObjects[_idx];

            tmp.InitCooldown(cooldownTime, _idx);
            _idx++;
            return tmp;
        }

        public void ReturnModule(CooldownComponentModule module)
        {
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
        }

        public void Compact()
        {
            _cooldownObjects.RemoveRange(_idx, _cooldownObjects.Count);
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
    }
}