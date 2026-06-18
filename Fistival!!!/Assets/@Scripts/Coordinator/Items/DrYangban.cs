using Coordinator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Items
{
    public class DrYangban : ItemBase
    {
        private const float _accel = 2;
        private float _oldStrongThreshold = 1;
        public override void OnEquip(PlayerCoordinator player)
        {
            _oldStrongThreshold = player.GetPlayerData().StrongAttackThreshold;
            player.GetPlayerData().StrongAttackThreshold /= _accel;
            base.OnEquip(player);
        }

        public override void OnUnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().StrongAttackThreshold = _oldStrongThreshold;
            base.OnUnEquip(player);
        }
    }
}
