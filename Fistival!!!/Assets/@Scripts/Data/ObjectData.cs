namespace Data
{
    public class ObjectData
    {
        public ObjectData(ObjectData original)
        {
            ObjectIndex = original.ObjectIndex;
            Damage = original.Damage;
            Durability = original.Durability;
            AbrasableLayerMask = original.AbrasableLayerMask;
            Weight = original.Weight;
            PlatformSpeedThreshold = original.PlatformSpeedThreshold;
            SpriteName = original.SpriteName;
            AnimControllerName = original.AnimControllerName;
            PhysicsMaterialName = original.PhysicsMaterialName;
        }
        public ObjectData() { }

        public int ObjectIndex;
        public int Damage;
        public int Durability;
        public int AbrasableLayerMask;
        public int PlatformLayerMask;
        public float Weight;//날아가는 속도에 영향.
        public float PlatformSpeedThreshold;
        public string SpriteName;
        public string AnimControllerName;
        public string PhysicsMaterialName;
    }
}
