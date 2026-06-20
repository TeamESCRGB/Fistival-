using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Equipments
{
    public class NitrogenCider : Equipment
    {
        private const float _accel = 2;
        private float _oldCharge = 1;
        public override void OnEquip(PlayerCoordinator player)
        {
            _oldCharge = player.GetPlayerData().ChargeTimeInterval;
            player.GetPlayerData().ChargeTimeInterval /= _accel;
            base.OnEquip(player);
        }
        public override void OnUnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().ChargeTimeInterval = _oldCharge;
            base.OnUnEquip(player);
        }
    }
}
