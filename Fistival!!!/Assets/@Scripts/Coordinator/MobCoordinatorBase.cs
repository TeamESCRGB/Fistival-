using ComponentModule;
using Coordinator.Movements;
using Coordinator.Objects;
using Coordinator.Victims;
using Data;
using Manager;
using UnityEngine;

namespace Coordinator
{
    public abstract class MobCoordinatorBase : MonoBehaviour, IStunnable, IPushable
    {
        [SerializeField]
        protected LayerMask _playerLayer;
        [SerializeField]
        protected LayerMask _groundLayer;

        protected float _skillDelay = 5;

        protected int _dropObjectIdx;
        protected string _dropObjectPrefab;

        protected Rigidbody2D _rb2d;

        protected IPushable _internalTarget;

        protected CooldownComponentModule _stunCounter;
        protected IMovementLockable _movLock;

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
            _internalTarget = GetComponentInChildren<IPushable>();
            _movLock = GetComponentInChildren<IMovementLockable>();
        }

        protected virtual void OnStart()
        {
            GetComponentInChildren<AggroCoordinator>().Init(0.1f, OnAggroStateChanged, _playerLayer, _groundLayer);
        }

        public virtual void Init(CommonMobData data)
        {
            GetComponentInChildren<VictimCoordinator>().Init(data.HP, data.HP, data.InvincibilityTime);

            var hp = GetComponentInChildren<HPCoordinator>();
            hp.UnSubscribeOnDead(OnDead);
            hp.UnSubscribeOnHPChanged(OnHPChanged);
            hp.SubscribeOnHPChanged(OnHPChanged);
            hp.SubscribeOnDead(OnDead);

            var detector = transform.Find("@DetectRange");
            detector.localScale = data.AggroRange;
            _skillDelay = data.SkillDelay;
            _dropObjectIdx = data.DropObjectIdx;
            _dropObjectPrefab = data.DropObjectPrefabName;
            if (_stunCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_stunCounter);
            }
            _stunCounter = Managers.Instance.CooldownManager.GetCooldownModule(0);
            _stunCounter.OnCooldownEnded += OnStunEnd;
            //애니메이터 달기
        }

        protected abstract void OnAggroStateChanged(bool isAggroOn, Collider2D player);

        protected virtual void OnDead()
        {
            var go = Managers.Instance.ResourceManager.Instantiate(_dropObjectPrefab);
            if(go != null)
            {
                if(go.TryGetComponent<ObjectCoordinator>(out var comp) == false || Managers.Instance.DataManager.ObjectDataDict.ContainsKey(_dropObjectIdx) == false)
                {
                    Managers.Instance.ResourceManager.Destroy(go);
                }
                else
                {
                    comp.Init(Managers.Instance.DataManager.ObjectDataDict[_dropObjectIdx]);
                }
            }
            Managers.Instance.ResourceManager.Destroy(gameObject);
        }

        protected virtual void OnHPChanged(int old, int now, int delta)
        {

        }

        public abstract void StunFor(float time);

        public abstract void ReleaseStun();

        public virtual void OnStunEnd()
        {

        }

        public void PushTo(Vector2 force)
        {
            _internalTarget?.PushTo(force);
        }

    }
}