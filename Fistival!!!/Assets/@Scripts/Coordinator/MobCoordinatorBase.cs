using ComponentModule;
using Coordinator.Movements;
using Coordinator.Objects;
using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator
{
    public abstract class MobCoordinatorBase : MonoBehaviour, IStunnable
    {
        protected int _damage;
        protected float _stunTime;
        protected float _knockbackForce;
        protected LayerMask _playerLayer;
        [SerializeField]
        protected LayerMask _groundLayer;

        protected float _skillDelay = 5;

        protected int _dropObjectIdx;
        protected string _dropObjectPrefab;

        protected Rigidbody2D _rb2d;

        protected CooldownComponentModule _stunCounter;
        protected IMovementLockable _movLock;

        protected Animator _animator;
        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        private void OnDisable()
        {
            OnDisabled();
        }

        protected virtual void OnDisabled()
        {
            if(_stunCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_stunCounter);
                _stunCounter = null;
            }
        }

        protected virtual void OnAwake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _movLock = GetComponentInChildren<IMovementLockable>();
            _animator= GetComponent<Animator>();
        }

        protected virtual void OnStart()
        {
            GetComponentInChildren<AggroCoordinator>().Init(0.1f, OnAggroStateChanged, _playerLayer, _groundLayer);
        }

        public virtual void Init(CommonMobData data)
        {
            GetComponentInChildren<VictimCoordinator>().Init(data.HP, data.HP, data.InvincibilityTime);
            _animator.runtimeAnimatorController = Managers.Instance.ResourceManager.Load<RuntimeAnimatorController>(data.AnimationController);
            _animator.Rebind();
            var hp = GetComponentInChildren<HPCoordinator>();
            hp.UnSubscribeOnDead(OnDead);
            hp.UnSubscribeOnHPChanged(OnHPChanged);
            hp.SubscribeOnHPChanged(OnHPChanged);
            hp.SubscribeOnDead(OnDead);
            _playerLayer = data.PlayerLayer;
            var detector = transform.Find("@DetectRange");
            detector.localScale = data.AggroRange;
            _skillDelay = data.SkillDelay;
            _dropObjectIdx = data.DropObjectIdx;
            _dropObjectPrefab = data.DropObjectPrefabName;
            _damage = data.TouchDamage;
            _stunTime = data.TouchStunTime;
            _knockbackForce = data.KnockBackForce;
            if (_stunCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_stunCounter);
            }
            _stunCounter = Managers.Instance.CooldownManager.GetCooldownModule(0);
            _stunCounter.OnCooldownEnded += OnStunEnd;
        }

        protected abstract void OnAggroStateChanged(bool isAggroOn, Collider2D player);


        public void AnimatorOnDead()
        {
            var go = Managers.Instance.ResourceManager.Instantiate(_dropObjectPrefab);
            if (go != null)
            {
                if (go.TryGetComponent<ObjectCoordinator>(out var comp) == false || Managers.Instance.DataManager.ObjectDataDict.ContainsKey(_dropObjectIdx) == false)
                {
                    Managers.Instance.ResourceManager.Destroy(go);
                }
                else
                {
                    comp.Init(Managers.Instance.DataManager.ObjectDataDict[_dropObjectIdx]);
                }
            }
            Managers.Instance.ResourceManager.Destroy(gameObject, true);
        }

        public void AnimatorOnHit()
        {

        }

        protected virtual void OnDead()
        {
            _animator.SetTrigger("Dead");
        }

        protected virtual void OnHPChanged(int old, int now, int delta)
        {
            _animator.SetTrigger("Hit");
        }

        public abstract void StunFor(float time);

        public abstract void ReleaseStun();

        public virtual void OnStunEnd()
        {

        }
    }
}