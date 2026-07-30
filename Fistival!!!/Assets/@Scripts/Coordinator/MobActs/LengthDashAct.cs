using Coordinator.Movements;
using Defines;
using Manager;
using System;
using System.Collections;
using UnityEngine;

namespace Coordinator.MobActs
{
    public class LengthDashAct : MobActBase
    {
        private Transform _myTransform;
        private PlatformerMovementCoordinator _mov;
        private Transform _player;
        private float _rightRot = 0;
        private float _leftRot = 180;
        private float _speed;
        private Vector2 _force;
        private bool _isActing = false;
        private Vector3 _targetPos;
        private float _dashStopTime;
        private float _dashTime;
        private MovementKeyStatus _dir;

        [SerializeField]
        private string _dashSFX;

        public void Init(Action onEnd, Animator animator, PlatformerMovementCoordinator movCoord, Rigidbody2D rb2d, float dashSpeed, float dashStopTime)
        {
            Init(onEnd,animator);
            _isActing = false;
            _speed = dashSpeed;
            _myTransform = rb2d.transform;
            _mov= movCoord;
            _dashStopTime= dashStopTime;
            _dashTime = 0;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;
            _dir = MovementKeyStatus.OFF;
            if (rb2d.gravityScale < 0)
            {
                float tmp = _rightRot;
                _rightRot = _leftRot;
                _leftRot = tmp;
            }
        }

        private void OnDisable()
        {
            Vector3 eularAngle = _myTransform.eulerAngles;
            eularAngle.y = _rightRot;
            _myTransform.eulerAngles = eularAngle;
        }

        public void LengthDashReady()
        {
            _targetPos = _player.position;
            if (_targetPos.x < transform.position.x)
            {
                Vector3 eularAngle = _myTransform.eulerAngles;
                eularAngle.y = _leftRot;
                _myTransform.eulerAngles = eularAngle;
                _dir = MovementKeyStatus.LEFT;
            }
            else
            {
                Vector3 eularAngle = _myTransform.eulerAngles;
                eularAngle.y = _rightRot;
                _myTransform.eulerAngles = eularAngle;
                _dir = MovementKeyStatus.RIGHT;
            }
            var dis = _targetPos.x - transform.position.x;
            _force = new Vector2(dis * _speed,0);
        }

        private void FixedUpdate()
        {
            if(_isActing == false)
            {
                return;
            }
            _dashTime += Time.fixedDeltaTime;

            if(_dashTime >= _dashStopTime || (_dir == MovementKeyStatus.RIGHT && transform.position.x >= _targetPos.x) || (_dir == MovementKeyStatus.LEFT && transform.position.x <= _targetPos.x))
            {
                _mov.SetNowSpeed(Vector2.zero);
                _animator.SetTrigger("DashEnd");
            }
        }

        public void DoLengthDash()
        {
            _mov.PushTo(_force);
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, _dashSFX, false);
        }

        public void LengthDashEnd()
        {
            if(_isActing == false)
            {
                return;
            }
            _isActing = false;
            _onActEnd?.Invoke();
        }

        public override void Act()
        {
            _isActing = true;
            _dashTime = 0;
            _animator.SetTrigger(Animator.StringToHash("Dash"));
        }

        public override void StopAct()
        {
            LengthDashEnd();
        }
    }
}
