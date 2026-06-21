using DG.Tweening;
using Manager;
using System;
using System.Collections;
using UI.Transition;
using Unity.Mathematics;
using UnityEngine;

namespace UI.Popup
{
    public abstract class CutsceneControllerBase : UIPopupBase
    {
        protected Action _stepFunctions;
        [SerializeField]
        protected BookFlip[] _flippers;
        protected int _index = -1;
        protected GameObject _blocker;
        protected Coroutine _blockerRoutine;
        [SerializeField]
        protected float _timePerPage = 1;
        protected bool _isFlipping = false;
        protected float _closedPos = 0;
        protected event Action _onBlockRoutineEnd;


        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }
            _blocker = transform.Find("@BookFlipControllerClickBlocker").gameObject;
            _blocker.SetActive(false);
            _closedPos = GetComponent<RectTransform>().rect.height;
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -_closedPos);
            return true;
        }


        protected void Open()
        {
            GetComponent<RectTransform>().DOAnchorPosY(0, 1f).SetEase(Ease.OutBack);
            if (_blockerRoutine != null)
            {
                StopCoroutine(_blockerRoutine);
            }
            _blockerRoutine = StartCoroutine(BlockerRoutine(1));
        }

        protected void Close()
        {
            Managers.Instance.GameManager.DisablePause();
            GetComponent<RectTransform>().DOAnchorPosY(-_closedPos, 1f).SetEase(Ease.InBack);
            if (_blockerRoutine != null)
            {
                StopCoroutine(_blockerRoutine);
            }
            _blockerRoutine = StartCoroutine(BlockerRoutine(1));
            _onBlockRoutineEnd += () =>
            {
                Managers.Instance.UIManager.ClosePopupUI();
                Managers.Instance.GameManager.EnablePause();
            };
        }

        protected bool FlipTo(int idx)
        {
            if (idx == _index || _isFlipping)
            {
                return false;
            }

            if (_blockerRoutine != null)
            {
                StopCoroutine(_blockerRoutine);
            }

            int mul = math.abs(idx - _index) - 1;

            _isFlipping = true;
            StartCoroutine(FlipRoutine(idx));


            _blockerRoutine = StartCoroutine(BlockerRoutine(_timePerPage * 0.5f + _timePerPage * 0.25f * mul));
            return true;
        }

        protected IEnumerator FlipRoutine(int idx)
        {
            if (idx < _index)
            {
                for (int i = _index; i > idx; i--)
                {
                    _flippers[i].Close(_timePerPage);
                    yield return new WaitForSeconds(_timePerPage * 0.25f);
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

        protected IEnumerator BlockerRoutine(float sec)
        {
            _blocker.SetActive(true);
            yield return new WaitForSeconds(sec);
            _blocker.SetActive(false);
            _onBlockRoutineEnd?.Invoke();
        }
    }
}