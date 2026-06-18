namespace Data
{
    public class PlayerData
    {
        public PlayerData(PlayerData original)
        {
            MaxHP = original.MaxHP;
            MaxChargeCnt = original.MaxChargeCnt;
            MaxLife = original.MaxLife;
            Damage = original.Damage;
            StrongAttackDamage = original.StrongAttackDamage;
            ThrowAttackAdditionalDamage = original.ThrowAttackAdditionalDamage;
            AttackCooldown = original.AttackCooldown;
            AttackableLayers = original.AttackableLayers;

            PickableLayers = original.PickableLayers;

            ForcePerCharge = original.ForcePerCharge;
            ChargeTimeInterval = original.ChargeTimeInterval;
            StrongAttackThreshold = original.StrongAttackThreshold;

            MoveSpeed = original.MoveSpeed;
            SlownessSensitivity = original.SlownessSensitivity;
            MaxSlowness = original.MaxSlowness;
            JumpPower = original.JumpPower;
            InvincibilityTime = original.InvincibilityTime;
        }

        public PlayerData() { }

        public int MaxHP { get; set; }
        public int MaxChargeCnt { get; set; }
        public int MaxLife { get; set; }
        public int Damage { get; set; }
        public int StrongAttackDamage { get; set; }
        public int ThrowAttackAdditionalDamage { get; set; }
        public float AttackCooldown { get; set; }
        public int AttackableLayers { get; set; }

        public int PickableLayers { get; set; }

        public float ForcePerCharge { get; set; }
        public float ChargeTimeInterval { get; set; }

        public float StrongAttackThreshold { get; set; }

        public float MoveSpeed { get; set; }
        public float SlownessSensitivity { get; set; }
        public float MaxSlowness { get; set; }
        public float JumpPower { get; set; }
        public float InvincibilityTime { get; set; }
    }
}
