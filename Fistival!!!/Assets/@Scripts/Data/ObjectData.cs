namespace Data
{
    public class ObjectData
    {
        public ObjectData() { }

        public int ObjectIndex { get; set; }
        public string PrefabKey { get; set; }
        public int Damage { get; set; }
        public int Durability { get; set; }
        public int AbrasableLayerMask { get; set; }
        public int PlatformLayerMask { get; set; }
        public float Weight { get; set; }//날아가는 속도에 영향.
        public float PlatformSpeedThreshold { get; set; }
        public float StunTime { get; set; }
        public string SpriteName { get; set; }
        public string PhysicsMaterialName { get; set; }
    }
}
