using Actor;
using ComponentModule;
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
        protected Transform _attackRange;
        protected Transform _activateRange;
        protected CooldownComponentModule _explodeTimer;

        private void Awake()
        {
            _mainCam = Camera.main;
            _rb2d = GetComponent<Rigidbody2D>();
            _skill = GetComponent<SkillCoordinatorBase>();
            _projActor = new ProjectileActor(_rb2d);
            _attackRange = transform.Find("@AttackBox");
            _activateRange = transform.Find("@ActivationRange");
        }

        private void OnDisable()
        {
            OnDisabled();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        public virtual void Init(LayerMask attackableLayerMask, ProjectileData data)
        {
            _attackRange.localScale = new Vector3(data.AttackRadius*2, data.AttackRadius * 2, 1);
            _explodableLayer = data.ExplodableLayerMask;
            _attackableLayer = attackableLayerMask;
            _targetLayer = _explodableLayer | attackableLayerMask;
            _baseSpeed = data.Speed;
            _rb2d.sharedMaterial = Managers.Instance.ResourceManager.Load<PhysicsMaterial2D>(data.Physics2DMaterialName);
            _skill.Init(attackableLayerMask, data.Damage);
            
            if(data.Lifetime > 0)
            {
                _explodeTimer = Managers.Instance.CooldownManager.GetCooldownModule(data.Lifetime);
                _explodeTimer.OnCooldownEnded += Destruct;
                _explodeTimer.StartCooldown();
            }

        }

        public virtual void Destruct()
        {
            OnExplode();
        }

        public virtual void Launch(Vector3 initialPos, Vector2 dir)
        {
            _rb2d.AddForce(dir * _baseSpeed, ForceMode2D.Impulse);
            _projActor.LookDir(dir);
        }

        protected virtual void OnExplode()
        {
            Managers.Instance.ResourceManager.Destroy(gameObject, true);
        }

        protected virtual bool CanExplode()
        {
            var obj = Physics2D.OverlapCircle(_activateRange.position, _activateRange.localScale.x / 2, _targetLayer);
            return obj != null && obj.gameObject != null;
        }

        protected virtual void OnDisabled()
        {
            if(_explodeTimer != null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_explodeTimer);
                _explodeTimer = null;
            }
        }

        protected virtual void OnFixedUpdate()
        {

            if(CanExplode() == false)
            {
                return;
            }

            var enemies = Physics2D.OverlapCircleAll(_attackRange.position, _attackRange.localScale.x/2, _targetLayer);

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