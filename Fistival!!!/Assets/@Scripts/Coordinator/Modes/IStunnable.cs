namespace Coordinator.Movements
{
    public interface IStunnable
    {
        public void StunFor(float time);
        public void ReleaseStun();
    }
}