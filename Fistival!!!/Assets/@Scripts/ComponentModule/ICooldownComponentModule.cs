namespace ComponentModule
{
    public interface ICooldownComponentModuleBase
    {
        public int Index { get; set; }
        public void InitCooldown(float cooldownTime, int index, float timeChangedCallInterval);
        public void DeinitCooldown();
        public void SetCooldownTime(float time);
        public void StartCooldown();
        public void StopCooldown();
        public bool IsCooldownEnded();
        public float GetRemainedTime();
        public void Tick(float dt);
    }
}