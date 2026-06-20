using UnityEngine;

namespace Coordinator
{
    public abstract class ItemBase : MonoBehaviour
    {
        [SerializeField]
        protected LayerMask _activateTargetLayer;
        protected abstract void InternalCollisionHandler(Collider2D collision);

        private void OnTriggerEnter2D(Collider2D collision)
        {
            InternalCollisionHandler(collision);
        }
    }
}