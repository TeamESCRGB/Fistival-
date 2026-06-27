using ComponentModule;
using Coordinator.Movements;
using Coordinator.Victims;
using Data;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator.Objects
{
    public class ObjectCoordinator : SkillCoordinatorBase, IChainPullable
    {
        //추가 예정인 것: 소리(날아가는거, 충돌, 파괴), 파티클(날아가는거, 충돌, 파괴), 애니메이션
        protected ObjectData _data;
        protected Rigidbody2D _rb2d;
        protected Collider2D _col2d;
        protected int _durability = 1;
        protected int _abrasableLayerMask = 0;
        protected float _platformSpeedThreshold=1;
        protected bool _isThrown = false;

        protected int _chargeRate;
        protected int _attackCnt;

        private FixedCooldownComponentModule _pullGroundDisableCounter;
        private Action _pullGroundDisableEndCallback;

        private float _gravityConstant;
        [SerializeField]
        protected LayerMask _groundLayermask;

        protected int _additionalDamage = 0;

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            _pullGroundDisableEndCallback = OnGroundDisableEnd;
            _rb2d = gameObject.GetOrAddComponent<Rigidbody2D>();
            _col2d = gameObject.GetOrAddComponent<Collider2D>();
        }
        private void OnDisable()
        {
            OnDisabled();
        }
        protected virtual void OnDisabled()
        {
            ResetOnAttack();
            _rb2d.excludeLayers &= ~_groundLayermask;
            if (Managers.Instance != null && _pullGroundDisableCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnFixedModule(_pullGroundDisableCounter);
                _pullGroundDisableCounter = null;
            }
        }

        public virtual void Init(ObjectData data)
        {
            if (data == null)
            {
#if UNITY_EDITOR
                Debug.LogError("data null");
#endif
                return;
            }

            _gravityConstant = Mathf.Abs(Physics2D.gravity.y);

            _chargeRate = 0;
            _attackCnt = 0;

            _data = data;
            _platformSpeedThreshold = data.PlatformSpeedThreshold;
            transform.SetParent(null, false);
            _rb2d.bodyType = RigidbodyType2D.Dynamic;
            _col2d.enabled = true;
            _rb2d.mass = data.Weight;
            _rb2d.linearVelocity = Vector2.zero;
            _rb2d.angularVelocity = 0;
            _durability = data.Durability;
            _abrasableLayerMask = data.AbrasableLayerMask;
            _isThrown = false;
            _additionalDamage = 0;
            base.Init(0, data.Damage, data.StunTime);
        }

        public void SetAdditionalDamage(int damage)
        {
            _additionalDamage = damage;
        }

        private void FixedUpdate()
        {
            if(_isThrown == false)
            {
                return;
            }

            if(Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }

            if(_rb2d.linearVelocity.magnitude < _platformSpeedThreshold)
            {
                _isThrown = false;
                _rb2d.excludeLayers &= ~_data.PlatformLayerMask;
            }
            else
            {
                _rb2d.excludeLayers |= _data.PlatformLayerMask;
            }
        }

        public ObjectData GetSharedData()
        {
            return _data;
        }

        public virtual bool Throw(in Vector2 dir,in Vector2 parentLinVelocity ,float force, int chargeRate)
        {
            if(Drop(parentLinVelocity) == false)
            {
                return false;
            }

            _attackCnt = 0;
            _chargeRate=chargeRate;
            _isThrown = true;
            _rb2d.AddForce(dir*force,ForceMode2D.Impulse);
            return true;
        }

        public virtual bool Smash()
        {
            _durability--;
            if(_durability <= 0)
            {
                Managers.Instance.ResourceManager.Destroy(gameObject);
                return false;
            }
            return true;
        }

        public virtual bool Drop(in Vector2 parentLinVelocity)
        {
            _rb2d.bodyType = RigidbodyType2D.Dynamic;
            _col2d.enabled = true;
            transform.SetParent(null, true);
            _rb2d.linearVelocity = parentLinVelocity;
            return true;
        }

        public virtual bool PickUp(Transform anchor)
        {
            if(anchor == null)
            {
#if UNITY_EDITOR
                Debug.LogError("anchor is null");
#endif
                return false;
            }
            else
            {
                _isThrown = false;
                _rb2d.linearVelocity = Vector2.zero;
                _rb2d.angularVelocity = 0;
                transform.SetParent(anchor,false);
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _rb2d.bodyType = RigidbodyType2D.Kinematic;
                _col2d.enabled = false;
                return true;
            }
        }

        protected virtual void InternalCollisionHandler(Collision2D col)
        {
            if(_isThrown == false)
            {
                return;
            }

            if (col == null || col.gameObject == null)
            {
                return;
            }
            
            if(col.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            if(((1<<col.gameObject.layer) & _abrasableLayerMask) != 0)
            {
                _durability--;
            }
            else if (((1 << col.gameObject.layer) & _attackableLayers) != 0)
            {
                Managers.Instance.AttackManager.RequestAttack(comp, this, (int)(_baseDamage * _rb2d.linearVelocity.magnitude) + _additionalDamage, _rb2d.linearVelocity, _baseStunTime * _chargeRate);
                _durability--;
            }

            if(_durability <= 0)
            {
                Managers.Instance.ResourceManager.Destroy(gameObject,true);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            InternalCollisionHandler(collision);
        }

        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback, float stun)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage);
                target.TakeKnockBack(knockback);
                target.StunFor(stun);
                target.StartInvincibleTime();
                _attackCnt++;
                CallOnAttack(_attackCnt, _chargeRate);
            }
            return true;
        }



        private void OnGroundDisableEnd()
        {
            _rb2d.excludeLayers &= ~_groundLayermask;
            if (Managers.Instance != null && _pullGroundDisableCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnFixedModule(_pullGroundDisableCounter);
                _pullGroundDisableCounter = null;
            }
        }

        public void Pull(Vector2 distance, float totalMoveTime, float dampingThreshold)
        {
            _isThrown = false;
            if(_pullGroundDisableCounter is not null)
            {
                _pullGroundDisableCounter.StopCooldown();
            }

            _rb2d.excludeLayers |= _groundLayermask;
            
            Vector2 impulseForce = MovementUtils.CaculateThrowPower(distance, totalMoveTime, _rb2d.linearDamping, dampingThreshold, _gravityConstant, _rb2d.linearVelocity) ;

            _rb2d.AddForce(impulseForce, ForceMode2D.Impulse);

            _pullGroundDisableCounter = Managers.Instance.CooldownManager.GetFixedCooldownModule(1 / _rb2d.linearVelocity.magnitude);

            _pullGroundDisableCounter.OnCooldownEnded += _pullGroundDisableEndCallback;
            _pullGroundDisableCounter.StartCooldown();
        }
    }
}
