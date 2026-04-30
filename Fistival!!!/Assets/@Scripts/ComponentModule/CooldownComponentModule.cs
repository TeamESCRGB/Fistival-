using System;

namespace ComponentModule
{
    public class CooldownComponentModule
    {
        private float _cooldownTime = 0;
        private float _accumulatedTime = 0;
        private bool _isCooldownEnded;

        private float _timeChangedCallInterval = 0;
        private float _lastTimeChanged = 0;

        public event Action OnCooldownEnded;
        public event Action<float, float> OnTimeChanged;

        public int Index { get; set; }

        public void InitCooldown(float cooldownTime, int index, float timeChangedCallInterval)
        {
            SetCooldownTime(cooldownTime);
            _accumulatedTime = 0;
            _isCooldownEnded = true;
            Index = index;
            _timeChangedCallInterval = timeChangedCallInterval;
        }

        public void DeinitCooldown()
        {
            Index = -1;
            OnCooldownEnded = null;
            OnTimeChanged = null;
        }

        public void SetCooldownTime(float time)
        {
            _cooldownTime = time;
        }

        public void StartCooldown()
        {
            _accumulatedTime = _cooldownTime;
            _isCooldownEnded = false;
            _lastTimeChanged = _cooldownTime + _timeChangedCallInterval;
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

            if(_lastTimeChanged - _accumulatedTime >= _timeChangedCallInterval)
            {
                _lastTimeChanged -= _timeChangedCallInterval;
                OnTimeChanged?.Invoke(_cooldownTime, _cooldownTime - _accumulatedTime);
            }

            if (_accumulatedTime <= 0)
            {
                _isCooldownEnded = true;
                OnCooldownEnded?.Invoke();
            }
        }
    }
    
}