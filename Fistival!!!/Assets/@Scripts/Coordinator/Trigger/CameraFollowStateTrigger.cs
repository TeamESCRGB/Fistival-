using UnityEngine;

namespace Coordinator.Trigger
{
    public class CameraFollowStateTrigger : MonoBehaviour
    {
        private SmoothFollowCoordinator _camCoord;
        [SerializeField]
        private bool _isCameraFollowPlayer;
        private void Start()
        {
            _camCoord = Camera.main.GetComponent<SmoothFollowCoordinator>();
            var trigger = GetComponent<TouchTrigger>();
            trigger.OnTriggerActivated -= SetCameraOpt;
            trigger.OnTriggerActivated += SetCameraOpt;
        }

        private void SetCameraOpt(GameObject _)
        {
            _camCoord.SetFollowState(_isCameraFollowPlayer);
        }
    }
}
