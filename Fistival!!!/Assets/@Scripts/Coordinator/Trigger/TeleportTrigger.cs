using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.Trigger
{
    public class TeleportTrigger : MonoBehaviour
    {
        [SerializeField]
        protected Transform _endPosBundle;
        protected Camera _main;
        protected Transform _playerTPPos;
        protected Transform _cameraTPPos;

        private void Start()
        {
            _main = Camera.main;
            var trigger = GetComponent<TouchTrigger>();
            trigger.OnTriggerActivated -= Teleport;
            trigger.OnTriggerActivated += Teleport;
            _playerTPPos = _endPosBundle.Find("@PlayerTPPos");
            _cameraTPPos = _endPosBundle.Find("@CameraTPPos");
        }

        protected virtual void Teleport(GameObject player)
        {
            var playerTPPos = _playerTPPos.position;
            var camTPPos = _cameraTPPos.position;

            var playerPos = player.transform.position;
            var camPos = _main.transform.position;
            playerPos.x = playerTPPos.x;
            playerPos.y = playerTPPos.y;
            camPos.x = camTPPos.x;
            camPos.y = camTPPos.y;

            player.transform.position = playerPos;
            _main.transform.position = camPos;
        }
    }
}
