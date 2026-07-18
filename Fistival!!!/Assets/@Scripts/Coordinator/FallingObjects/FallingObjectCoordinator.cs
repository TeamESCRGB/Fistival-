using Coordinator.Objects;
using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;
using Utils;

namespace Coordinator.FallingObjectCoordinator
{
    public class FallingObjectCoordinator : SkillCoordinatorBase
    {
        protected FallingObjectData _data;
        protected float _knockBackForce;
        protected float _objSpawnForce;
        protected Rigidbody2D _rb2d;
        protected Animator _animator;
        protected Vector2 _velocity;

        private void Awake()
        {
            OnAwake();   
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        protected virtual void OnFixedUpdate()
        {
            _velocity = _rb2d.linearVelocity;
        }

        protected virtual void OnAwake()
        {
            _animator = GetComponent<Animator>();
            _rb2d = GetComponent<Rigidbody2D>();
        }

        public virtual void Init(FallingObjectData data)
        {
            _objSpawnForce = data.ObjSpawnForce;
            _data = data;
            _knockBackForce = data.KnockBackForce;
            _animator.runtimeAnimatorController = Managers.Instance.ResourceManager.Load<RuntimeAnimatorController>(data.AnimControllerName);
            Init(data.BreakableLayerMask, data.Damage, data.StunTime);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            var go = col.collider.gameObject;
            var velocity = _velocity;

            if (((1 << go.layer) & _attackableLayers) != 0 && go.TryGetComponent<IAttackable>(out var comp))
            {
                Managers.Instance.AttackManager.RequestAttack(comp, this, _baseDamage, velocity * _knockBackForce, _baseStunTime);
            }

            for(int i = 0; i < _data.SpawnableObjects.Length; i++)
            {
                if(Managers.Instance.DataManager.ObjectDataDict.TryGetValue(_data.SpawnableObjects[i],out var data) == false)
                {
                    continue;
                }

                var obj = Managers.Instance.ResourceManager.Instantiate(data.PrefabKey,null,true,true);
                var objKnockBack = velocity + new Vector2(Random.Range(-_objSpawnForce,_objSpawnForce), Random.Range(-_objSpawnForce, _objSpawnForce));

                if(obj == null)
                {
                    continue;
                }
                if(obj.TryGetComponent<ObjectCoordinator>(out var objCoord))
                {

                    objCoord.Init(data);
                    objCoord.transform.position = transform.position;
                    objCoord.GetComponent<Rigidbody2D>().AddForce(objKnockBack,ForceMode2D.Impulse);
                }
                else
                {
                    Managers.Instance.ResourceManager.Destroy(obj);
                }
            }

            Managers.Instance.ResourceManager.Destroy(gameObject,true);
        }

        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback, float stun)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage,transform.position);
                target.TakeKnockBack(knockback);
                target.StunFor(stun);
                target.StartInvincibleTime();
            }
            return true;
        }
    }
}