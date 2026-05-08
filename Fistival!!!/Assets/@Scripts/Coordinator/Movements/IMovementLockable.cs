namespace Coordinator.Movements
{
    public interface IMovementLockable
    {
        public void LockMovement();
        public void UnlockMovement();
    }
}
