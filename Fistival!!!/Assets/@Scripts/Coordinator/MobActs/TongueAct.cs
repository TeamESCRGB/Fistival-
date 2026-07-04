using System;
using UnityEngine;
using DG.Tweening;

namespace Coordinator.MobActs
{
    public class TongueAct : MobActBase
    {
        private Transform _tongueTransform;
        private float _tongDuration;
        private float _tongueStayDuration;
        private float _tongueLength;
        private Sequence _tongueSeq;

        private void Awake()
        {
            _tongueTransform = transform.Find("@Tongue");
        }

        public void Init(Action onEnd,Animator animator ,float tongueDuration, float tongueStayDuration, float tongueLength)
        {
            base.Init(onEnd,animator);
            if(_tongueSeq.IsActive())
            {
                _tongueSeq.Kill();
            }
            _tongDuration = tongueDuration;
            _tongueStayDuration = tongueStayDuration;
            _tongueLength = tongueLength;
        }

        public void StartTongueAct()
        {
            if (_tongueSeq.IsActive())
            {
                _tongueSeq.Kill();
            }
            _tongueSeq = DOTween.Sequence();
            _tongueSeq.Append(_tongueTransform.DOScaleX(_tongueLength, _tongDuration).From(0));
            _tongueSeq.AppendInterval(_tongueStayDuration);
            _tongueSeq.Append(_tongueTransform.DOScaleX(0, _tongDuration).OnComplete(OnTongueEnd));
            _tongueSeq.Play();
        }

        public override void Act()
        {
            _animator.SetTrigger("TongueLaunch");
        }

        private void OnTongueEnd()
        {
            _animator.SetTrigger("TongueRollBack");
            _onActEnd?.Invoke();
        }

        public override void StopAct()
        {
            if(_tongueSeq.IsActive())
            {
                _tongueSeq.Kill();
            }
            var a = _tongueTransform.localScale;
            a.x = 0;
            _tongueTransform.localScale = a;
            OnTongueEnd();
        }
    }
}
