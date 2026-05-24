using Defines;
using UnityEngine;

namespace Coordinator.Chain
{
    public class ChainMorningStar : MonoBehaviour
    {
        private ChainAnchor _anchor;
        [SerializeField]private Rigidbody2D _parentRb2d;
        [SerializeField]private Rigidbody2D _rb2d;

        private Vector2 _lastPos;

        private void Awake()
        {
            _anchor = GetComponentInChildren<ChainAnchor>();
            _rb2d = GetComponentInChildren<Rigidbody2D>();
        }
        public void Init(LayerMask attackableMask, Rigidbody2D parentRb2d)
        {
            _parentRb2d = parentRb2d;
            _anchor.Init(attackableMask);
        }

        private void FixedUpdate()
        {
            transform.position = _parentRb2d.transform.position;
        }

        public void SetRotation(Vector2 dir)
        {
            if(_anchor.GetStatus() != ChainStatus.OFF)
            {
                return;
            }
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

        public void Launch(Vector2 dir, float len, float totalMoveTime)
        {
            if(_anchor.GetStatus() != ChainStatus.OFF)
            {
                return;
            }
            
            SetRotation(dir);
            _anchor.Launch(dir,len,totalMoveTime);
            _lastPos = _parentRb2d.position;
        }

        public void Retrive()
        {
            _anchor.Retrive();
        }
    }
}