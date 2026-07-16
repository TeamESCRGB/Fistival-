using Data;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
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
            _idx = 0;
            _talkRoutine = null;
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
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnTextTypeEnd()
        {
            if(_isTalking == false)
            {
                return;
            }
            _isTalking = false;
            _idx++;
        }

        private void CompleteTextType()
        {
            GetText((int)Texts.Script).text = _data.TalkData[_idx].script;
            OnTextTypeEnd();
        }

        private IEnumerator ContinueScript()
        {
            _isTalking = true;

            var charArr = _data.TalkData[_idx].script.ToCharArray();
            var text = GetText((int)Texts.Script);
            var sfx = _data.TalkData[_idx].typeSFX;

            GetText((int)Texts.Name).text = _data.TalkData[_idx].name;
            GetImage((int)Images.CharacterSprite).sprite = Managers.Instance.ResourceManager.Load<Sprite>(_data.TalkData[_idx].characterImg);
            text.text = "";

            for(int i = 0; i < charArr.Length; i++)
            {
                text.text += charArr[i];
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, sfx, false, Managers.Instance.GameManager.SFXVolume);
                yield return _waiter;
            }

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
                if(_talkRoutine != null)
                {
                    StopCoroutine(_talkRoutine);
                }
                CompleteTextType();
            }
            else
            {
                _talkRoutine = StartCoroutine(ContinueScript());
            }
        }
    }
}