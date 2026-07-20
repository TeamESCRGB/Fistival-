using UnityEngine;

namespace Coordinator.Trigger
{
    public class DisableObjectTrigger : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _disableTargets;
        private void Start()
        {
            var trigger = GetComponent<TouchTrigger>();
            trigger.OnTriggerActivated -= DisableObjects;
            trigger.OnTriggerActivated += DisableObjects;
        }
        private void DisableObjects(GameObject _)
        {
            for(int i = 0;  i < _disableTargets.Length; i++)
            {
                _disableTargets[i].SetActive(false);
            }
        }
    }
}