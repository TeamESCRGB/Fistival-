using UnityEngine;

namespace Coordinator.Movements
{
    public interface IChainPullable
    {
        public void Pull(Vector2 distance,float totalMoveTime,float dampingThreshold);
    }
}
