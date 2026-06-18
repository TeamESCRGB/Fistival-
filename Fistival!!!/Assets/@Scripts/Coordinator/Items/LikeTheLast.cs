using Coordinator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Items
{
    public class LikeTheLast : ItemBase
    {
        //감소
        private int _oldMaxHP = 5;
        private int _oldLife = 5;

        //효과
        private const int _damageAdd = 2;
        private const int _strongDmgAdd = 2;
        private const int _throwDmgAdd = 2;

        private const int _chargeAdd = 2;

        private const float _accel = 2;
        private float _oldStrongThreshold = 1;
        private float _oldChargeInterval = 1;


        public override void OnEquip(PlayerCoordinator player)
        {
            var data = player.GetPlayerData();
            //backup
            _oldMaxHP = data.MaxHP;
            _oldLife = data.MaxLife;

            _oldStrongThreshold = data.StrongAttackThreshold;
            _oldChargeInterval = data.ChargeTimeInterval;

            //apply
            data.MaxHP = 1;
            data.MaxLife = 1;
            data.Damage += _damageAdd;
            data.StrongAttackDamage += _strongDmgAdd;
            data.ThrowAttackAdditionalDamage += _throwDmgAdd;
            data.MaxChargeCnt += _chargeAdd;
            data.StrongAttackThreshold /= _accel;
            data.ChargeTimeInterval /= _accel;

            base.OnEquip(player);
        }

        public override void OnUnEquip(PlayerCoordinator player)
        {
            var data = player.GetPlayerData();
            data.MaxHP = _oldMaxHP;
            data.MaxLife = _oldLife;
            data.Damage -= _damageAdd;
            data.StrongAttackDamage -= _strongDmgAdd;
            data.ThrowAttackAdditionalDamage -= _throwDmgAdd;
            data.MaxChargeCnt -= _chargeAdd;
            data.StrongAttackThreshold = _oldStrongThreshold;
            data.ChargeTimeInterval = _oldChargeInterval;

            base.OnUnEquip(player);
        }
    }
}
