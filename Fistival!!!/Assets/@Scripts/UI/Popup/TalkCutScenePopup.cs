using Data;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using DG.Tweening;
using System.Collections;
using System;

namespace UI.Popup
{
    public class TalkCutScenePopup : UIPopupBase
    {
        [SerializeField]
        private float _typeInterval;
        private WaitForSeconds _waiter;
        private TalkScriptData _data;
        private int _idx;
        private Coroutine _talkRoutine;
        private bool _isTalking;
        private Action _onEnd;

        enum Images
        {
            CharacterSprite
        }

        enum Texts
        {
            Name,
            Script
        }

        enum Buttons
        {
            Next
        }

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            GetButton((int)Buttons.Next).gameObject.BindUIEvent(OnNextButton);
            _waiter = new WaitForSeconds(_typeInterval);
            _isTalking = false;
            return true;
        }

        public TalkCutScenePopup SetData(string dataKey)
        {
            Managers.Instance.DataManager.TalkScriptDataDict.TryGetValue(dataKey, out _data);
            return this;
        }

        public TalkCutScenePopup SetOnEnd(Action onEnd)
        {
            _onEnd = onEnd;
            return this;
        }


        private void OnEnd()
        {
            _onEnd?.Invoke();
        }

        private void OnTextTypeEnd()
        {
            GetText((int)Texts.Script).text = _data.TalkData[_idx].script;
            _isTalking = false;
            _idx++;
        }

        private IEnumerator ContinueScript()
        {
            _isTalking = true;
            yield return _waiter;
            _isTalking = false;
            OnTextTypeEnd();
        }

        private void OnNextButton(PointerEventData _)
        {
            if (_idx >= _data.TalkData.Count)
            {
                OnEnd();
            }
            else if(_isTalking)
            {
                StopCoroutine(_talkRoutine);
                OnTextTypeEnd();
            }
            else
            {
                _talkRoutine = StartCoroutine(ContinueScript());
            }
        }
    }
}