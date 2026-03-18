namespace Coordinator.Victims
{
    public interface IAttackable
    {
        public bool CanAttack();
        public T RequestComponent<T>() where T : class;
        public void TakeDamage(int damage);
        public void StartInvincibleTime();
    }
}