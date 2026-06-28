using Coordinator.Movements;
using UnityEngine;

namespace Coordinator.TriggerMovement
{
    public class PlatfoermerTriggerMovementCoordinator : MonoBehaviour
    {
        private PlatformerMovementCoordinator _platformerMov;
        private void Awake()
        {
            _platformerMov = GetComponent<PlatformerMovementCoordinator>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == null)
            {
                return;
            }

            if(collision.CompareTag("RightMovementTag"))
            {
                _platformerMov.OnLeftMovementInputEvent(false);
                _platformerMov.OnRightMovementInputEvent(true);
            }
            else if(collision.CompareTag("LeftMovementTag"))
            {
                _platformerMov.OnRightMovementInputEvent(false);
                _platformerMov.OnLeftMovementInputEvent(true);
            }
            else if(collision.CompareTag("JumpTag"))
            {
                _platformerMov.OnJumpMovementInputEvent(true);
            }
            else if(collision.CompareTag("DownJumpTag"))
            {
                _platformerMov.OnDownJumpMovementInputEvent(true);
            }
            else if(collision.CompareTag("StopTag"))
            {
                _platformerMov.OnLeftMovementInputEvent(false);
                _platformerMov.OnRightMovementInputEvent(false);
            }
        }
    }
}