using Coordinator.Movements;
using Coordinator.Victims;
using Manager;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinator.Chain
{
    public class ChainAnchor : MonoBehaviour
    {
        private Rigidbody2D _rb2d;
        private Transform _rope;
        private Collider2D _chain;
        private Collider2D _anchor;
        private List<Collider2D> _detectedColliders = new List<Collider2D>(8);
        private float _ropeScaleY;
        private float _maxLen;
        private bool _isMoving;
        private Vector3 _initialPos;
        private Vector3 _initialScale;
        private LayerMask _attackableMask;
        [SerializeField]
        private LayerMask _objectMask;
        [SerializeField]
        private LayerMask _chainPullPadMask;
        [SerializeField]
        private LayerMask _groundMask;
        private SkillCoordinatorBase _baseSkill;
        private int _damage;
        private float _totalMoveTime;
        [SerializeField]
        private float _dampingThreshold = 0.001f;
        private Transform _parentTransform;
        private Vector2 _dir;
        private IChainPullable _player;
        private ContactFilter2D _filter;

        private void Awake()
        {
            _parentTransform = transform.parent;
            _baseSkill = GetComponent<SkillCoordinatorBase>();
            _rope = transform.parent.Find("@ChainParent");
            _chain = _rope.Find("@Chain").GetComponent<Collider2D>();
            _anchor = GetComponent<Collider2D>();
            _rb2d = GetComponent<Rigidbody2D>();
            _initialPos = transform.localPosition;
            _initialScale = _rope.localScale;
            _ropeScaleY = _rope.localScale.y;

            _filter.useLayerMask = true;
            _filter.useTriggers = true;
        }

        public bool IsMoving()
        {
            return _isMoving;
        }

        public void Init(LayerMask attackableMask,float totalMoveTime, IChainPullable player)
        {
            _player = player;
            _totalMoveTime = totalMoveTime;
            _isMoving = false;
            _attackableMask= attackableMask;
            _rb2d.linearVelocity = Vector2.zero;
            _rb2d.includeLayers = _attackableMask | _objectMask | _groundMask | _chainPullPadMask;
            _filter.layerMask   = _attackableMask | _objectMask | _groundMask | _chainPullPadMask;
            _baseSkill.Init(attackableMask,0);
            Retrive();
        }

        public void Launch(Vector2 dir, float len, float totalMovTime, int damage)
        {
            _dir= dir;
            _damage = damage;
            _rb2d.simulated = true;
            _rb2d.WakeUp();
            _isMoving = true;
            Vector2 targetSpd = dir;
            _maxLen = len;
            targetSpd.x = targetSpd.x / totalMovTime;

            targetSpd.y = targetSpd.y / totalMovTime;

            _rb2d.linearVelocity = targetSpd;
        }

        public void Retrive()
        {
            _isMoving = false;
            transform.localPosition = _initialPos;
            _rope.localScale = _initialScale;
            _maxLen = 0;
            _rb2d.linearVelocity = Vector2.zero;
            _rb2d.simulated = false;
        }
        private void FixedUpdate()
        {
            if(_isMoving == false)
            {
                return;
            }

            if (Managers.Instance.GameManager.IsGamePaused())
            {
                return;
            }

            Vector2 my = transform.position;
            Vector2 tart = _rope.position;
            float len = (my - tart).magnitude;
            _rope.localScale = new Vector2(len, _ropeScaleY);

            int cnt = 0;
            cnt+=CheckCollision(_anchor);
            cnt+=CheckCollision(_chain);

            if(len >= _maxLen || cnt > 0)
            {
                Retrive();
            }
        }

        private int CheckCollision(Collider2D collider)
        {
            int ret = Physics2D.OverlapCollider(collider,_filter , _detectedColliders);

            for(int i = 0; i < ret; i++)
            {
                GameObject go = _detectedColliders[i].gameObject;
                int layer = 1 << go.layer;

                if ((layer & _objectMask) != 0)
                {
                    if(go.TryGetComponent<IChainPullable>(out var pullComp) == false)
                    {
                        continue;
                    }
                    Vector2 start = go.transform.position;
                    Vector2 end = _parentTransform.position;
                    Vector2 distance = end - start;
                    pullComp.Pull(distance, _totalMoveTime, _dampingThreshold);

                }
                else if ((layer & _attackableMask) != 0)
                {
                    if(go.TryGetComponent<IAttackable>(out var attackTarget) == false)
                    {
                        continue;
                    }
                    Managers.Instance.AttackManager.RequestAttack(attackTarget, _baseSkill, _damage, _dir * _damage);
                }
                else if ((layer & _chainPullPadMask) != 0)
                {
                    Vector2 start = _parentTransform.position;
                    Vector2 end = transform.position;
                    Vector2 distance = end - start;
                    _player.Pull(distance, _totalMoveTime, _dampingThreshold);
                }
            }

            return ret;
        }
    }
}