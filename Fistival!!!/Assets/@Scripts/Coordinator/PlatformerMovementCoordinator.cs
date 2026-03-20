using InputHandler;
using System.Collections;
using UnityEngine;

namespace Coordinator
{
    public class PlatformerMovementCoordinator : MonoBehaviour ,IMovementInputHandler
    {
        [SerializeField]
        private float _platformIgnoreTime = 0.5f;
        
        [SerializeField]
        private LayerMask _platformMask;
        [SerializeField]
        private LayerMask _groundLayer;

        private Transform _groundedCheckBox;
        private Vector2 _vel;

        private Collider2D _parentCol;
        private Rigidbody2D _parentRb2d;
        private Transform _parentTransform;
        private float _speed;
        private float _jumpPow;
        private float _slowness;

        private float _slownessSensitivity=1;
        private float _maxSlowness=0;

        private void Awake()
        {
            _groundedCheckBox = transform.Find("@GroundedCheckBox");
        }

        public void Init(float speed,float jumpPow ,float slownessSensitivity,float maxSlowness,Rigidbody2D parentRb2d)
        {
            _parentRb2d = parentRb2d;
            _speed = speed;
            _jumpPow = jumpPow;
            _parentCol = _parentRb2d.gameObject.GetComponent<Collider2D>();
            _parentTransform = _parentRb2d.transform;
            _slowness = 1;
            _maxSlowness = maxSlowness;
            _slownessSensitivity = slownessSensitivity;
        }

        private void FixedUpdate()
        {
            if(_parentRb2d == null)
            {
                return;
            }

            var vel = _parentRb2d.linearVelocity;
            vel.x = _vel.x * _slowness;
            _parentRb2d.linearVelocity = vel;
        }

        public void SetSlowness(float slowness)
        {
            if(_slownessSensitivity == 0)
            {
                _slowness = 1;
                return;
            }
            _slowness = slowness/_slownessSensitivity;

            if(_slowness >= 1)
            {
                _slowness = 1;
            }
            else if(_slowness <= _maxSlowness)
            {
                _slowness = _maxSlowness;
            }
            
        }

        public void OnDownMovementInputEvent(bool pressed)
        {
            if(_parentRb2d == null || _parentCol == null)
            {
                return;
            }
            if(pressed)
            {
                var col = Physics2D.OverlapBox(_groundedCheckBox.position, _groundedCheckBox.localScale, 0,_platformMask);
                if(col != null)
                {
                    StartCoroutine(DisablePlatform(col));
                }
            }
        }

        private IEnumerator DisablePlatform(Collider2D col)
        {
            Physics2D.IgnoreCollision(col,_parentCol);
            yield return new WaitForSeconds(_platformIgnoreTime);
            Physics2D.IgnoreCollision(col, _parentCol, false);
        }

        public void OnJumpMovementInputEvent(bool pressed)
        {
            if (pressed && IsGrounded())
            {
                _parentRb2d.AddForce(Vector2.up * _jumpPow,ForceMode2D.Impulse);
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
            if(_groundedCheckBox == null)
            {
                return false;
            }
            return (Physics2D.OverlapBox(_groundedCheckBox.position, _groundedCheckBox.localScale, 0, _groundLayer) != null);
        }
    }
}
