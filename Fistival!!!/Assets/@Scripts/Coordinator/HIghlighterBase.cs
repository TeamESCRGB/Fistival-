using UnityEngine;

namespace Coordinator
{
    public abstract class HIghlighterBase : MonoBehaviour
    {
        [SerializeField]
        protected GameObject _highlightObj;

        private void Start()
        {
            _highlightObj.SetActive(false);
        }
    }
}