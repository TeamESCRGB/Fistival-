using Actor;
using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator
{
    public abstract class ProjectileCoordinator : MonoBehaviour
    {
        protected ProjectileActor _projActor;
        protected Camera _mainCam;
        protected SkillCoordinatorBase _skill;
        protected Rigidbody2D _rb2d;
        protected LayerMask _explodableLayer;
        protected LayerMask _attackableLayer;
        protected LayerMask _targetLayer;
        protected float _baseSpeed;

        private void Awake()
        {
            _mainCam = Camera.main;
            _rb2d = GetComponent<Rigidbody2D>();
            _skill = GetComponent<SkillCoordinatorBase>();
            _projActor = new ProjectileActor(_rb2d);
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        public virtual void Init(LayerMask attackableLayerMask, ProjectileData data)
        {
            _explodableLayer = data.ExplodableLayerMask;
            _attackableLayer = attackableLayerMask;
            _targetLayer = _explodableLayer | attackableLayerMask;
            _baseSpeed = data.Speed;
            _rb2d.sharedMaterial = Managers.Instance.ResourceManager.Load<PhysicsMaterial2D>(data.Physics2DMaterialName);
            _skill.Init(attackableLayerMask, data.Damage);
        }

        public virtual void Destruct()
        {
            OnExplode();
        }

        public virtual void Launch(Vector3 initialPos, Vector2 dir)
        {
            _rb2d.AddForce(dir * _baseSpeed);
            _projActor.LookDir(dir);
        }

        protected virtual void OnExplode()
        {
            Managers.Instance.ResourceManager.Destroy(gameObject, true);
        }

        protected virtual void OnFixedUpdate()
        {
            var enemies = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, _targetLayer);

            if (enemies is null)
            {
                return;
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                var enemy = enemies[i];
                if (enemy.TryGetComponent<IAttackable>(out var target) && _skill.CanAttackTarget(target))
                {
                    Managers.Instance.AttackManager.RequestAttack(target, _skill, _skill.GetBaseDamage);
                }
            }

            if(enemies.Length > 0)
            {
                OnExplode();
            }
        }
    }
}