using Coordinator.Movements;
using Coordinator.Objects;
using Data;
using Defines;
using Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.MobActs.CameleonBossPattern
{
    public class CameleonPattern3 : MobActBase
    {
        private ObjectData _objData;
        private Transform _objSpawnPoint;
        private IReadOnlyList<Transform> _points;
        private Vector2 _fieldCenterPos;
        private bool _isActing;
        private MovementKeyStatus _dir;
        private float _moveTime;
        private Rigidbody2D _rb2d;
        private PointMovement _mov;

        public void Init(Action onActionEnd, Animator animator, Rigidbody2D rb2d, PointMovement mov, float moveTime, IReadOnlyList<Transform> points, Vector2 fieldCenterPos, ObjectData objData, Transform objSpawnPoint)
        {
            Init(onActionEnd, animator);

            _points = points;
            _objSpawnPoint = objSpawnPoint;
            _objData = objData;
            _fieldCenterPos = fieldCenterPos;
            _dir = MovementKeyStatus.LEFT;
            _isActing = false;
            _moveTime = moveTime;
            _rb2d = rb2d;
            _mov = mov;
        }
        public void EndCameleonPattern3()
        {
            _isActing = false;
            if (_dir == MovementKeyStatus.RIGHT)
            {
                _rb2d.MovePosition(_points[_points.Count-1].position);
            }
            else
            {
                _rb2d.MovePosition(_points[0].position);
            }
            _onActEnd?.Invoke();
        }

        private void OnMovEndCb(bool result)
        {
            if (result == false)
            {
                return;
            }

            if (_dir == MovementKeyStatus.RIGHT)
            {
                var rot = _rb2d.transform.eulerAngles;
                rot.z = 0;
                rot.y = 0;
                _rb2d.transform.eulerAngles = rot;
            }
            else
            {
                var rot = _rb2d.transform.eulerAngles;
                rot.z = 0;
                rot.y = 180;
                _rb2d.transform.eulerAngles = rot;
            }
            var go = Managers.Instance.ResourceManager.Instantiate(_objData.PrefabKey, null, true, true);
            if(go != null)
            {
                if(go.TryGetComponent<ObjectCoordinator>(out var comp))
                {
                    comp.Init(_objData);
                    go.transform.position = _objSpawnPoint.position;
                }
                else
                {
                    Managers.Instance.ResourceManager.Destroy(go);
                }
            }
            _animator.SetTrigger("CameleonPattern3End");
        }

        public void StartCameleonPattern3()
        {
            var pos = _rb2d.position;
            if (pos.x < _fieldCenterPos.x)
            {
                _dir = MovementKeyStatus.RIGHT;
                var rot = _rb2d.transform.eulerAngles;
                rot.z = 90;
                _rb2d.transform.eulerAngles = rot;
                _mov.ReqStartMove(_points[_points.Count - 1], _moveTime, _rb2d, OnMovEndCb);
            }
            else
            {
                _dir = MovementKeyStatus.LEFT;
                var rot = _rb2d.transform.eulerAngles;
                rot.z = 90;
                _rb2d.transform.eulerAngles = rot;
                _mov.ReqStartMove(_points[0], _moveTime, _rb2d, OnMovEndCb);
            }
        }

        public override void Act()
        {
            _isActing = true;
            _animator.SetTrigger("CameleonPattern3");Debug.Log("act");
        }

        public override void StopAct()
        {
            if (_isActing == false)
            {
                return;
            }
            _isActing = false;
            _mov.StopMove();
        }
    }
}
