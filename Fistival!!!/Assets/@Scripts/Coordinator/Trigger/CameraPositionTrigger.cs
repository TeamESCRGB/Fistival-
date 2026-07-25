using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.Trigger
{
    public class CameraPositionTrigger : MonoBehaviour
    {
        private Transform _cam;
        [SerializeField]
        private Transform _tarPosTransform;
        private void Start()
        {
            _cam = Camera.main.transform;
            var trigger = GetComponent<TouchTrigger>();
            trigger.OnTriggerActivated -= SetCameraPos;
            trigger.OnTriggerActivated += SetCameraPos;
        }

        private void SetCameraPos(GameObject _)
        {
            var pos = _cam.position;
            var tarPos = _tarPosTransform.position;
            pos.x = tarPos.x;
            pos.y = tarPos.y;
            _cam.position = pos;
        }
    }
}
