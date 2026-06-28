using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.Movements.TriggerMovement
{
    public class FlightTriggerMovementCoordinator : MonoBehaviour
    {
        private FlightMovementCoordinator _flightMov;
        private void Awake()
        {
            _flightMov = GetComponent<FlightMovementCoordinator>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == null)
            {
                return;
            }

            if (collision.CompareTag("RightMovementTag"))
            {
                _flightMov.OnLeftMovementInputEvent(false);
                _flightMov.OnRightMovementInputEvent(true);
            }
            else if (collision.CompareTag("LeftMovementTag"))
            {
                _flightMov.OnRightMovementInputEvent(false);
                _flightMov.OnLeftMovementInputEvent(true);
            }
            else if (collision.CompareTag("UpMovementTag"))
            {
                _flightMov.OnDownMovementInputEvent(false);
                _flightMov.OnUpMovementInputEvent(true);
            }
            else if (collision.CompareTag("DownMovementTag"))
            {
                _flightMov.OnUpMovementInputEvent(false);
                _flightMov.OnDownMovementInputEvent(true);
            }
            else if (collision.CompareTag("StopTag"))
            {
                _flightMov.OnLeftMovementInputEvent(false);
                _flightMov.OnRightMovementInputEvent(false);
                _flightMov.OnUpMovementInputEvent(false);
                _flightMov.OnDownMovementInputEvent(false);
            }
        }
    }
}
