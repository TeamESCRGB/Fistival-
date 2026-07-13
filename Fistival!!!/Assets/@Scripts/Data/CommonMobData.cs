using UnityEngine;

namespace Data
{
    public class CommonMobData
    {
        public int Idx { get; set; }
        public string PrefabKey { get; set; }
        public int HP { get; set; }
        public int TouchDamage { get; set; }
        public float TouchStunTime { get; set; }
        public float KnockBackForce { get; set; }
        public float InvincibilityTime { get; set; }
        public float Speed { get; set; }
        public float SkillDelay { get; set; }
        public Vector2 AggroRange { get; set; }
        public int DropObjectIdx { get; set; }
        public int PlayerLayer { get; set; }
        public int PlayerHitboxLayer { get; set; }
        public int InitialDir { get; set; }
        public string DropObjectPrefabName { get; set; }
        public string AnimationController { get; set; }
    }
}