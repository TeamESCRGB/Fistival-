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
            AttackRadius = original.AttackRadius;
            Physics2DMaterialName = original.Physics2DMaterialName;
            ProjectilePrefabName = original.ProjectilePrefabName;
        }
        public ProjectileData() { }

        public int IDX;
        public int ExplodableLayerMask;
        public float Speed;
        public int Damage;
        public float AttackRadius;
        public string Physics2DMaterialName;
        public string ProjectilePrefabName;
    }
}
