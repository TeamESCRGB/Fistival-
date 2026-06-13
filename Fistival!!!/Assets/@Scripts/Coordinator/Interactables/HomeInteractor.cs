using Coordinator;
using UnityEngine;

namespace Coordinator.Interactables
{
    public class HomeInteractor : InteractableObjectCoordinator
    {
        public override void Interact()
        {
            Debug.Log("Home Interact");
        }
    }
}
