using Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Popup;
using UnityEngine;

namespace Coordinator.Interactables
{
    public class BusStopInteractor : InteractableObjectCoordinator
    {
        public override void Interact()
        {
            Managers.Instance.UIManager.ShowPopupUI<BusstopUI>();
        }
    }
}
