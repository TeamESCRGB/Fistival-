using Coordinator;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Assets._Scripts.TestScripts
{
    public class TestItem : Equipment
    {
        public override void OnEquip(PlayerCoordinator player)
        {
            Debug.Log("eq");
            player.GetPlayerData().MaxChargeCnt+=2;
            base.OnEquip(player);
        }

        public override void OnUnEquip(PlayerCoordinator player)
        {
            Debug.Log("ueq");
            player.GetPlayerData().MaxChargeCnt -= 2;
            base.OnUnEquip(player);
        }
    }
}
