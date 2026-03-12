using InputHandler;
using System.Collections;
using UnityEngine;

namespace Coordinator
{
    public class PlatformerMovementCoordinator : MonoBehaviour ,IMovementInputHandler
    {
        private Collider2D _parentCol;
        [SerializeField]
        private LayerMask _platformMask;
        [SerializeField]
        private LayerMask _groundLayer;
        [SerializeField]
        private Transform _checkBox;
        private Vector2 _vel;
        private Rigidbody2D _rb2d;
        private Transform _parentTransform;
        private float _speed;
        private float _jumpPow;

        [SerializeField]
        private float _platformIgnoreThreshold;

        public void Init(float speed,float jumpPow ,Rigidbody2D rb2d)
        {
            _rb2d = rb2d;
            _speed = speed;
            _jumpPow = jumpPow;
            _parentCol = _rb2d.gameObject.GetComponent<Collider2D>();
            _parentTransform = _rb2d.transform;
        }

        private void FixedUpdate()
        {
            if(_rb2d == null)
            {
                return;
            }
            var vel = _rb2d.linearVelocity;
            vel.x = _vel.x;
            _rb2d.linearVelocity = vel;
        }

        public void OnDownMovementInputEvent(bool pressed)
        {
            if(_rb2d == null || _parentCol == null)
            {
                return;
            }
            if(pressed)
            {
                var col = Physics2D.OverlapBox(_checkBox.position, _checkBox.localScale, 0,_platformMask);
                if(col != null)
                {
                    StartCoroutine(DisablePlatform(col));
                }
            }
        }

        private IEnumerator DisablePlatform(Collider2D col)
        {
            Physics2D.IgnoreCollision(col,_parentCol);
            yield return new WaitForSeconds(_platformIgnoreThreshold);
            Physics2D.IgnoreCollision(col, _parentCol, false);
        }

        public void OnJumpMovementInputEvent(bool pressed)
        {
            if (pressed && IsGrounded())
            {
                _rb2d.AddForce(Vector2.up * _jumpPow,ForceMode2D.Impulse);
            }
        }

        public void OnLeftMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                _vel.x = -_speed;
                _parentTransform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if(_vel.x == -_speed)
            {
                _vel.x = 0;
            }
        }

        public void OnRightMovementInputEvent(bool pressed)
        {
            if (pressed)
            {
                _vel.x = _speed;
                _parentTransform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if(_vel.x == _speed)
            {
                _vel.x = 0;
            }
        }

        public bool IsGrounded()
        {
            if(_checkBox == null)
            {
                return false;
            }
            return (Physics2D.OverlapBox(_checkBox.position, _checkBox.localScale, 0, _groundLayer) != null);
        }
    }
}
