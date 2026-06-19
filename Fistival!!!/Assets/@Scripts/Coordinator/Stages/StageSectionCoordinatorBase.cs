using UnityEngine;

namespace Coordinator.Stages
{
    public abstract class StageSectionCoordinatorBase : MonoBehaviour
    {
        public virtual void InitChunk()
        {
            Debug.Log($"InitChunk of {gameObject.name}");
        }

        public virtual void DeInitChunk()
        {
            Debug.Log($"DeinitChunk of {gameObject.name}");
        }

    }
}