using Coordinator.Victims;
using Defines;
using Manager;
using UnityEngine;

namespace Coordinator.Chain
{
    public class ChainAnchor : MonoBehaviour
    {
        private Rigidbody2D _rb2d;
        private Transform _rope;
        private float _ropeScaleY;
        private float _maxLen;
        private ChainStatus _status;
        private Vector3 _initialPos;
        private Vector3 _initialScale;
        private LayerMask _attackableMask;
        [SerializeField]
        private LayerMask _objectMask;
        [SerializeField]
        private LayerMask _chainPullPadMask;
        [SerializeField]
        private LayerMask _groundMask;
        private LayerMask _interactableFilter;
        private SkillCoordinatorBase _baseSkill;
        private int _damage;

        private Vector2 _dir;


        private void Awake()
        {
            _baseSkill = GetComponent<SkillCoordinatorBase>();
            _rope = transform.parent.Find("@Chain");
            _rb2d = GetComponent<Rigidbody2D>();
            _initialPos = transform.localPosition;
            _initialScale = _rope.localScale;
            _ropeScaleY = _rope.localScale.y;
        }

        public ChainStatus GetStatus()
        {
            return _status;
        }

        public void Init(LayerMask attackableMask)
        {
            _status = ChainStatus.OFF;
            _attackableMask= attackableMask;
            _rb2d.linearVelocity = Vector2.zero;
            _interactableFilter = _attackableMask | _objectMask | _groundMask |_chainPullPadMask;
            _baseSkill.Init(attackableMask,0);
            Retrive();
        }

        public void Launch(Vector2 dir, float len, float totalMovTime, int damage)
        {
            _dir= dir;
            _damage = damage;
            _rb2d.simulated = true;
            _rb2d.WakeUp();
            _status = ChainStatus.MOVING;
            Vector2 targetSpd = dir;
            _maxLen = len;
            targetSpd.x = targetSpd.x / totalMovTime;

            targetSpd.y = targetSpd.y / totalMovTime;

            _rb2d.linearVelocity = targetSpd;
        }

        public void Retrive()
        {
            _status = ChainStatus.OFF;
            transform.localPosition = _initialPos;
            _rope.localScale = _initialScale;
            _maxLen = 0;
            _rb2d.simulated = false;
        }
        private void FixedUpdate()
        {
            if(_status == ChainStatus.OFF)
            {
                return;
            }
            else if(_status == ChainStatus.RETURN)
            {
                Retrive();
                return;
            }

            Vector2 my = transform.position;
            Vector2 tart = _rope.position;
            float len = (my - tart).magnitude;
            _rope.localScale = new Vector2(len, _ropeScaleY);

            if(len >= _maxLen)
            {
                _status = ChainStatus.RETURN;
                _rb2d.linearVelocity = Vector2.zero;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject != null && (1 << collision.gameObject.layer & _interactableFilter) == 0)
            {
                return;
            }

            GameObject go = collision.gameObject;
            int layer = 1 << go.layer;
            
            if((layer & _objectMask) != 0)
            {
                Debug.Log("obj");
            }
            else if((layer & _attackableMask) != 0)
            {
                Managers.Instance.AttackManager.RequestAttack(go.GetComponent<IAttackable>(), _baseSkill, _damage, _dir*_damage);
            }
            else if((layer & _chainPullPadMask) != 0)
            {
                Debug.Log("pull");
            }

            _rb2d.linearVelocity = Vector2.zero;
            Retrive();
        }
    }
}