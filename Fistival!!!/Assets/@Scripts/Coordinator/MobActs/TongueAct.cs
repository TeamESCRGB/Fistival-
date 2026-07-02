using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DG.Tweening;

namespace Coordinator.MobActs
{
    public class TongueAct : MobActBase
    {
        private float _tongDuration;
        private float _tongueStayDuration;
        private float _tongueLength;
        private Sequence _tongueSeq;

        public void Init(Action onEnd, float tongueDuration, float tongueStayDuration, float tongueLength)
        {
            base.Init(onEnd);
            if(_tongueSeq.IsActive())
            {
                _tongueSeq.Kill();
            }
            _tongDuration = tongueDuration;
            _tongueStayDuration = tongueStayDuration;
            _tongueLength = tongueLength;
        }

        public void StartAct()
        {
            if (_tongueSeq.IsActive())
            {
                _tongueSeq.Kill();
            }
            _tongueSeq = DOTween.Sequence();
            _tongueSeq.Append(transform.DOScaleX(_tongueLength, _tongDuration).From(0));
            _tongueSeq.AppendInterval(_tongueStayDuration);
            _tongueSeq.Append(transform.DOScaleX(0, _tongDuration).OnComplete(OnEnd));
            _tongueSeq.Play();
        }

        public override void Act()
        {
            StartAct();//애니메이션이 호출할거임 이제
        }

        private void OnEnd()
        {
            _onActEnd?.Invoke();
        }

        public override void StopAct()
        {
            if(_tongueSeq.IsActive())
            {
                _tongueSeq.Kill();
            }
            var a = transform.localScale;
            a.x = 0;
            transform.localScale = a;
            OnEnd();
        }
    }
}
