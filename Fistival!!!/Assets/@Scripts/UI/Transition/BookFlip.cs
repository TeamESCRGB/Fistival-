using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Manager;
using UnityEngine;

namespace UI.Transition
{
    public class BookFlip : MonoBehaviour
    {

        private TweenerCore<Quaternion, Vector3, QuaternionOptions> _tween;

        private void Flip(float angle, float duration)
        {
            if (_tween.IsActive())
            {
                _tween.Kill();
            }

            // 3. DOTween의 DORotate를 활용한 회전 연출
            // RotateMode.FastBeyond360을 사용해야 180도 회전 시 꼬임 현상을 방지할 수 있습니다.
            _tween = transform.DORotate(new Vector3(0, angle, 0), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad).SetUpdate(true); // 부드러운 가속/감속 효과
            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "BookFlipSFX", false, Managers.Instance.GameManager.SFXVolume);
        }


        public void Close(float duration)
        {
            Flip(0,duration);
        }

        public void Open(float duration)
        {
            Flip(180, duration);
        }
    }
}
