namespace Data
{
    public class ProjectileData
    {
        public ProjectileData(ProjectileData original)
        {
            IDX = original.IDX;
            ExplodableLayerMask= original.ExplodableLayerMask;
            Speed = original.Speed;
            Damage = original.Damage;
            ExplosionKnockBack = original.ExplosionKnockBack;
            Lifetime = original.Lifetime;
            AttackRadius = original.AttackRadius;
            StunTime = original.StunTime;
            Physics2DMaterialName = original.Physics2DMaterialName;
            ProjectilePrefabName = original.ProjectilePrefabName;
        }
        public ProjectileData() { }

        public int IDX;
        public int ExplodableLayerMask;
        public float Speed;
        public int Damage;
        public float ExplosionKnockBack;
        public float Lifetime;
        public float AttackRadius;
        public float StunTime;
        public string Physics2DMaterialName;
        public string ProjectilePrefabName;

        public string LaunchSFX { get; set; }
        public string ExplodeSFX { get; set; }
    }
}
