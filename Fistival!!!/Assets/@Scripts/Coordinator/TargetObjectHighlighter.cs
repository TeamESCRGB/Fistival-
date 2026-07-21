using UnityEngine;

namespace Coordinator
{
    public class TargetObjectHighlighter : MonoBehaviour
    {
        [SerializeField]
        protected GameObject _highlightObj;
        protected bool _isStateFixed;

        private void Start()
        {
            _isStateFixed = false;
            _highlightObj.SetActive(false);
        }
        public void ActivateShader()
        {
            if(_isStateFixed)
            {
                return;
            }
            _highlightObj.SetActive(true);
        }

        public void DeActivateShader()
        {
            if (_isStateFixed)
            {
                return;
            }
            _highlightObj.SetActive(false);
        }

        public void FixHighlightState(bool fix)
        {
            _isStateFixed = fix;
        }
    }
}