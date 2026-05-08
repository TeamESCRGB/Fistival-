namespace Coordinator.Movements
{
    public interface IStunable
    {
        public void StunFor(float time);
        public void ReleaseStun();
    }
}