using Coordinator;
using Manager;
using UI.Popup;
using UnityEngine;

namespace Coordinator.Interactables
{
    public class ShopInteractor : InteractableObjectCoordinator
    {
        public override void Interact()
        {
            Managers.Instance.UIManager.ShowPopupUI<ShopUI>("ShopUI");
        }
    }
}
