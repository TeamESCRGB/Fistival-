using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace UI.Transition
{
    public class BookFlipController : MonoBehaviour
    {
        [SerializeField]
        private BookFlip[] _flippers;
        private int _index = -1;
        private GameObject _blocker;
        private Coroutine _blockerRoutine;
        [SerializeField]
        private float _timePerPage = 1;

        private bool _isFlipping = false;

        private void Awake()
        {
            _blocker = transform.Find("@BookFlipControllerClickBlocker").gameObject;
        }

        private void Start()
        {
            _blocker.SetActive(false);
        }

        public bool FlipToFirst()
        {
            return FlipTo(-1);
        }

        public bool FlipTo(int idx)
        {
            if (idx == _index || _isFlipping)
            {
                return false;
            }

            if(_blockerRoutine != null)
            {
                StopCoroutine(_blockerRoutine);
            }

            int mul = math.abs(idx - _index) - 1;

            _isFlipping = true;
            StartCoroutine(FlipRoutine(idx));

            
            _blockerRoutine = StartCoroutine(BlockerRoutine(_timePerPage*0.5f + _timePerPage*0.25f*mul));
            return true;
        }

        private IEnumerator FlipRoutine(int idx)
        {
            if (idx < _index)
            {
                for (int i = _index; i > idx; i--)
                {
                    _flippers[i].Close(_timePerPage);
                    yield return new WaitForSeconds(_timePerPage*0.25f);
                }
            }
            else
            {
                for (int i = _index + 1; i <= idx; i++)
                {
                    _flippers[i].Open(_timePerPage);
                    yield return new WaitForSeconds(_timePerPage * 0.25f);
                }
            }

            _index = idx;
            _isFlipping = false;
        }

        private IEnumerator BlockerRoutine(float sec)
        {
            _blocker.SetActive(true);
            yield return new WaitForSeconds(sec);
            _blocker.SetActive(false);
        }

        public int tar;
        [ContextMenu("flip")]
        void f()
        {
            FlipTo(tar);
        }
    }
}