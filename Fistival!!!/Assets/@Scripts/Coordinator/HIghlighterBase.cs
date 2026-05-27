using UnityEngine;

namespace Coordinator
{
    public abstract class HIghlighterBase : MonoBehaviour
    {
        [SerializeField]
        protected Material _on;
        [SerializeField]
        protected Material _off;
        private Renderer _renderer;
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
#if UNITY_EDITOR
            Debug.Assert(_renderer != null, $"{name}에 Renderer가 없음");
#endif
        }

        private void Start()
        {
            _renderer.sharedMaterial = _off;
        }

        public void SetOnMaterial(Material mat)
        {
            _on = mat;
        }

        public void SetOffMaterial(Material mat)
        {
            _off = mat;
        }
    }
}