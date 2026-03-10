namespace Data
{
    public class ObjectData
    {
        public ObjectData(ObjectData original)
        {
            ObjectIndex = original.ObjectIndex;
            Damage = original.Damage;
            Durability = original.Durability;
            Weight = original.Weight;
            SpriteName = original.SpriteName;
            AnimControllerName = original.AnimControllerName;
            PhysicsMaterialName = original.PhysicsMaterialName;
        }
        public ObjectData() { }

        public int ObjectIndex;
        public int Damage;
        public int Durability;
        public float Weight;//날아가는 속도에 영향.
        public string SpriteName;
        public string AnimControllerName;
        public string PhysicsMaterialName;
    }
}
