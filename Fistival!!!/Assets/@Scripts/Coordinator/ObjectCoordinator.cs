using Data;
using Manager;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class ObjectCoordinator : MonoBehaviour
    {
        //추가 예정인 것: 소리(날아가는거, 충돌, 파괴), 파티클(날아가는거, 충돌, 파괴), 애니메이션
        private ObjectData _data;
        private Rigidbody2D _rb2d;
        private Collider2D _col2d;
        private int _durability = 1;
        private int _abrasableLayerMask = 1;
        private float _platformSpeedThreshold=1;
        private bool _isThrown = false;
        
        private void Awake()
        {
            _rb2d = gameObject.GetOrAddComponent<Rigidbody2D>();
            _col2d = gameObject.GetOrAddComponent<Collider2D>();
        }

        public virtual void Init(ObjectData data)
        {
            if(data == null)
            {
#if UNITY_EDITOR
                Debug.LogError("data null");
#endif
                return;
            }
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
        }

        private void FixedUpdate()
        {
            if(_isThrown == false)
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

        public virtual bool Throw(in Vector2 dir,in Vector2 parentLinVelocity ,float force)
        {
            if(Drop(parentLinVelocity) == false)
            {
                return false;
            }
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
            if (col == null || col.gameObject == null)
            {
                return;
            }
            
            if(((1<<col.gameObject.layer) & _abrasableLayerMask) != 0)
            {
                _durability--;
            }

            if(_durability <= 0)
            {
                Managers.Instance.ResourceManager.Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            InternalCollisionHandler(collision);
        }
    }
}
