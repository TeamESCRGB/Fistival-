using Manager;
using UnityEngine;

namespace Coordinator.Items
{
    public class HealItem : ItemBase
    {
        private int _heal = 0;
        private string _sfx = "";
        
        public void Init(int idx)
        {
            var data = Managers.Instance.DataManager.HealItemDataDict[idx];
            GetComponent<SpriteRenderer>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.Image);
            _sfx = data.HealSFX;
            _heal = data.HealRate;
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

            collision.GetComponentInChildren<HPCoordinator>().AddHP(_heal);
            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, _sfx, false);
            Managers.Instance.ResourceManager.Destroy(gameObject);
        }
    }
}
