namespace Data
{
    public class ProjectileData
    {
        public ProjectileData(ProjectileData original)
        {
            IDX = original.IDX;
            BouncableLayerMask= original.BouncableLayerMask;
            ExplodableLayerMask= original.ExplodableLayerMask;
            Speed = original.Speed;
            Damage = original.Damage;
            Physics2DMaterialName = original.Physics2DMaterialName;
            ProjectilePrefabName = original.ProjectilePrefabName;
        }
        public ProjectileData() { }

        public int IDX;
        public int BouncableLayerMask;
        public int ExplodableLayerMask;
        public float Speed;
        public int Damage;
        public string Physics2DMaterialName;
        public string ProjectilePrefabName;
    }
}
