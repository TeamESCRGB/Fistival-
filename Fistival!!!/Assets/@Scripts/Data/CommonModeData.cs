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
            MoveSpeed = original.MoveSpeed;
            AnimControllerName = original.AnimControllerName;
        }
        public CommonModeData() { }

        public int CommonModeDataIdx;
        public int Damage;
        //public float ThrowPower;
        public float AttackCooldown;
        public float MoveSpeed;
        public string AnimControllerName;
    }
}
