using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Items
{
    public class Twenty : ItemBase
    {
        private const int _chargeMax = 1;
        public override void OnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().MaxChargeCnt += _chargeMax;
            base.OnEquip(player);
        }
        public override void OnUnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().MaxChargeCnt -= _chargeMax;
            base.OnUnEquip(player);
        }
    }
}
