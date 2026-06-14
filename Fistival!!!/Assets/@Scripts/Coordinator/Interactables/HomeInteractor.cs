using Coordinator;
using Manager;
using UI.Popup;
using UnityEngine;

namespace Coordinator.Interactables
{
    public class HomeInteractor : InteractableObjectCoordinator
    {
        public override void Interact()
        {
            Managers.Instance.UIManager.ShowPopupUI<HomeUI>("HomeUI");
        }
    }
}
