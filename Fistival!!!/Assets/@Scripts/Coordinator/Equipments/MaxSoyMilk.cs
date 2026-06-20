using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Equipments
{
    public class MaxSoyMilk : Equipment
    {
        private const int _life = 2;
        public override void OnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().MaxLife += _life;
            base.OnEquip(player);
        }

        public override void OnUnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().MaxLife -= _life;
            base.OnUnEquip(player);
        }
    }
}
