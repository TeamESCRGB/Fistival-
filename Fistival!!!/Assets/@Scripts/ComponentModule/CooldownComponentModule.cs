using System;

namespace ComponentModule
{
    public class CooldownComponentModule
    {
        private float _cooldownTime = 0;
        private float _accumulatedTime = 0;
        private bool _isCooldownEnded;
        public event Action OnCooldownEnded;

        public int Index { get; set; }

        public void InitCooldown(float cooldownTime, int index)
        {
            SetCooldownTime(cooldownTime);
            _accumulatedTime = 0;
            _isCooldownEnded = true;
            Index = index;
        }

        public void DeinitCooldown()
        {
            Index = -1;
            OnCooldownEnded = null;
        }

        public void SetCooldownTime(float time)
        {
            _cooldownTime = time;
        }

        public void StartCooldown()
        {
            _accumulatedTime = _cooldownTime;
            _isCooldownEnded = false;
        }

        public void StopCooldown()
        {
            _isCooldownEnded = true;
            OnCooldownEnded?.Invoke();
        }

        public bool IsCooldownEnded()
        {
            return _isCooldownEnded;
        }

        public void Tick(float dt)
        {
            if (_isCooldownEnded)
            {
                return;
            }

            _accumulatedTime -= dt;

            if (_accumulatedTime <= 0)
            {
                _isCooldownEnded = true;
                OnCooldownEnded?.Invoke();
            }
        }
    }
    
}