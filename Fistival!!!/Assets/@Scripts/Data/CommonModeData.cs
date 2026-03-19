using UnityEngine;

namespace Data
{
    public class CommonModeData
    {
        public CommonModeData(CommonModeData original)
        {
            CommonModeDataIdx = original.CommonModeDataIdx;
            Damage = original.Damage;
            //ThrowPower = original.ThrowPower;
            AttackCooldown = original.AttackCooldown;
            AttackableLayers = original.AttackableLayers;
            MoveSpeed = original.MoveSpeed;
            SlownessSensitivity = original.SlownessSensitivity;
            MaxSlowness = original.MaxSlowness;
            JumpPower = original.JumpPower;
            AnimControllerName = original.AnimControllerName;
        }
        public CommonModeData() { }

        public int CommonModeDataIdx;
        public int Damage;
        //public float ThrowPower;
        public float AttackCooldown;
        public int AttackableLayers;
        public float MoveSpeed;
        public float SlownessSensitivity;
        public float MaxSlowness;
        public float JumpPower;
        public string AnimControllerName;
    }
}
