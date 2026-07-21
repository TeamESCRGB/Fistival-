using System;
using UnityEngine;

namespace Coordinator.Trigger
{
    public class TouchTrigger : MonoBehaviour
    {
        [SerializeField]
        protected LayerMask _targetLayer;

        public event Action<GameObject> OnTriggerActivated;

        protected virtual void InternalCollisionHandler(GameObject go)
        {
            if (((1 << go.layer) & _targetLayer) != 0)
            {
                OnTriggerActivated?.Invoke(go);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            InternalCollisionHandler(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            InternalCollisionHandler(collision.gameObject);
        }
    }
}