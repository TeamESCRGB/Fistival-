using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Equipments
{
    public class Makta500 :Equipment
    {
        private const int _hpAdd = 1;
        public override void OnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().MaxHP += _hpAdd;
            base.OnEquip(player);
        }

        public override void OnUnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().MaxHP -= _hpAdd;
            base.OnUnEquip(player);
        }
    }
}
