using Coordinator.Movements;
using System;
using System.Collections;
using UnityEngine;

namespace Coordinator.MobActs
{
    public class DashAct : MobActBase
    {
        private Transform _myTransform;
        private PlatformerMovementCoordinator _mov;
        private Transform _player;
        private float _rightRot = 0;
        private float _leftRot = 180;
        private float _speed;
        private Vector2 _force;

        public void Init(Action onEnd, PlatformerMovementCoordinator movCoord, Rigidbody2D rb2d, float dashSpeed)
        {
            Init(onEnd);
            _speed = dashSpeed;
            _myTransform = rb2d.transform;
            _mov= movCoord;
            _player = FindAnyObjectByType<PlayerCoordinator>().transform;

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

        public void Ready()
        {
            if (_player.position.x < transform.position.x)
            {
                Vector3 eularAngle = _myTransform.eulerAngles;
                eularAngle.y = _leftRot;
                _myTransform.eulerAngles = eularAngle;
                _force = Vector2.left * _speed;
            }
            else
            {
                Vector3 eularAngle = _myTransform.eulerAngles;
                eularAngle.y = _rightRot;
                _myTransform.eulerAngles = eularAngle;
                _force = Vector2.right * _speed;
            }
        }

        public void Dash()
        {
            _mov.PushTo(_force);
        }

        private IEnumerator ActRoutine()
        {
            Ready();
            yield return new WaitForSeconds(0.5f);
            Dash();
            yield return new WaitForSeconds(1);
            End();
        }

        private void End()
        {
            _onActEnd?.Invoke();
        }

        public override void Act()
        {
            _routine = StartCoroutine(ActRoutine());
            
        }

        public override void StopAct()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }
            End();
        }
    }
}
