using Data;
using Manager;
using UnityEngine;

namespace Coordinator
{
    public abstract class ProjectileCoordinator : MonoBehaviour
    {
        protected Camera _mainCam;
        protected SkillCoordinatorBase _skill;
        protected Rigidbody2D _rb2d;
        protected LayerMask _explodableLayer;
        protected float _baseSpeed;

        private void Awake()
        {
            _mainCam = Camera.main;
            _rb2d = GetComponent<Rigidbody2D>();
            _skill = GetComponent<SkillCoordinatorBase>();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        public virtual void Init(LayerMask attackableLayerMask, ProjectileData data)
        {
            _explodableLayer = data.ExplodableLayerMask;
            _baseSpeed = data.Speed;
            _rb2d.sharedMaterial = Managers.Instance.ResourceManager.Load<PhysicsMaterial2D>(data.Physics2DMaterialName);
            _skill.Init(attackableLayerMask, data.Damage);
        }

        protected virtual void OnExplode()
        {

        }

        protected virtual void OnFixedUpdate()
        {

        }
    }
}