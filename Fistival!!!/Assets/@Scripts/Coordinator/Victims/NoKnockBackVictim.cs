using UnityEngine;

namespace Coordinator.Victims
{
    public class NoKnockBackVictim : VictimCoordinator
    {
        public override void TakeKnockBack(Vector2 force)
        {
            //no
        }
    }
}