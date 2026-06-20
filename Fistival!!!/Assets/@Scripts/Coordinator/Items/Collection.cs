using Manager;
using UnityEngine;

namespace Coordinator.Item
{
    public class Collection : ItemBase
    {
        [SerializeField]
        private int _collectionIdx;

        private void Start()
        {
            gameObject.SetActive(Managers.Instance.StageManager.CanCollect(_collectionIdx));
        }

        public int GetIdx()
        {
            return _collectionIdx;
        }

        protected override void InternalCollisionHandler(Collider2D collision)
        {
            if (collision == null || collision.gameObject == null)
            {
                return;
            }

            if((1<< collision.gameObject.layer) != _activateTargetLayer)
            {
                return;
            }

            Managers.Instance.StageManager.CollectCollection(_collectionIdx);
            gameObject.SetActive(false);
        }
    }
}