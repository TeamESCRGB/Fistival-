namespace Coordinator.Movements
{
    public interface IInputLockable
    {
        public void LockInputFor(float time);
        public void UnlockInput();
    }
}
