using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Items
{
    public class Malpollo : ItemBase
    {
        private const int _damage = 2;
        private const int _hp = 2;
        public override void OnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().Damage += _damage;
            player.GetPlayerData().MaxHP -= _hp;
            base.OnEquip(player);
        }

        public override void OnUnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().Damage -= _damage;
            player.GetPlayerData().MaxHP += _hp;
            base.OnUnEquip(player);
        }
    }
}
