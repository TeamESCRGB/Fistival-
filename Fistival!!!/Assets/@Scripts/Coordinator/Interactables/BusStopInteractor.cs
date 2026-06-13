using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.Interactables
{
    public class BusStopInteractor : InteractableObjectCoordinator
    {
        public override void Interact()
        {
            Debug.Log("busstop interact");
        }
    }
}
