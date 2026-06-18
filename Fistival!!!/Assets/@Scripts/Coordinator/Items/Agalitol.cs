using Coordinator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Items
{
    public class Agalitol : ItemBase
    {
        private const int _throwDmg = 1;
        public override void OnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().ThrowAttackAdditionalDamage += _throwDmg;
            base.OnEquip(player);
        }

        public override void OnUnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().ThrowAttackAdditionalDamage -= _throwDmg;
            base.OnUnEquip(player);
        }
    }
}
