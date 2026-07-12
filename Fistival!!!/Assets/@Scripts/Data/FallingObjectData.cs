using UnityEngine;

namespace Data
{
    public class FallingObjectData
    {
        public int IDX { get; set; }
        public string PrefabKey {  get; set; }
        public int BreakableLayerMask { get; set; }
        public int[] SpawnableObjects { get; set; }
        public int Damage {  get; set; }
        public float StunTime { get; set; }
        public Vector2 Scale { get; set; }
        public float ObjSpawnForce { get; set; }
        public float KnockBackForce { get; set; }
        public string AnimControllerName { get; set; }
    }
}