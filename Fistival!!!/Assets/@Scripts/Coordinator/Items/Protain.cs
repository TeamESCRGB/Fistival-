using Assets._Scripts.Coordinator;
using Defines;
using UnityEngine;

namespace Coordinator.Items
{
    public class Protain : ItemBase, IBasicInitializer
    {
        [SerializeField]
        private ModeTypes _type;
        
        public void Init()
        {
            gameObject.SetActive(true);
        }

        protected override void InternalCollisionHandler(Collider2D collision)
        {
            if (collision == null || collision.gameObject == null)
            {
                return;
            }

            if ((1 << collision.gameObject.layer) != _activateTargetLayer)
            {
                return;
            }

            var mode = collision.gameObject.GetComponentInChildren<ModeManageCoordinator>();

            if(mode.GetNowMode().ModeType == _type)
            {
                return;
            }
            mode.UnlockMode(_type);
            mode.ChangeMode(_type);
            gameObject.SetActive(false);
        }
    }
}