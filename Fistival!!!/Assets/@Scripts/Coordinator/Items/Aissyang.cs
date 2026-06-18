using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Items
{
    public class Aissyang : ItemBase
    {
        private const int _strongDmg = 1;
        public override void OnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().StrongAttackDamage += _strongDmg;
            base.OnEquip(player);
        }

        public override void OnUnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().StrongAttackDamage -= _strongDmg;
            base.OnUnEquip(player);
        }
    }
}
