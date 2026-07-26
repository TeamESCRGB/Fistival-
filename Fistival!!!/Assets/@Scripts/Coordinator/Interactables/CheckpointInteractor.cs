using Coordinator.Victims;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Manager;

namespace Coordinator.Interactables
{
    public class CheckpointInteractor : InteractableObjectCoordinator
    {
        private bool _isCheckpointChecked = false;
        [SerializeField] 
        private Sprite _unusedSprite;
        [SerializeField]
        private Sprite _usedSprite;
        private SpriteRenderer _renderer;
        private ParticleSystem _ps;
        [SerializeField]
        private string _bgmKey;
        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _ps = GetComponentInChildren<ParticleSystem>();
        }

        public void CheckCheckpoint(bool isTriggeredByAttack)
        {
            if(_isCheckpointChecked)
            {
                return;
            }

            //이미지 바꾸고, 체크포인트 애니메이션 틀어야됨
            if(isTriggeredByAttack)
            {
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "MobHit", false, Managers.Instance.GameManager.SFXVolume);
                _ps.Stop();
                _ps.Play();
            }
            else
            {
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "Button", false, Managers.Instance.GameManager.SFXVolume);
            }
            _renderer.sprite = _usedSprite;
            DeActivateShader();
            FixHighlightState(true);
            _isCheckpointChecked = true;
            Managers.Instance.StageManager.SaveCheckpoint(transform.position, _bgmKey);
            var player = FindAnyObjectByType<PlayerCoordinator>();
            var hpCoord = player.GetComponentInChildren<HPCoordinator>();
            hpCoord.AddHP(player.GetPlayerData().MaxHP - hpCoord.GetHP());
            Debug.Log("CheckpointON");
        }

        public bool IsCheckpointChecked()
        {
            return _isCheckpointChecked;
        }

        public override void Interact()
        {
            _renderer.sprite = _unusedSprite;
            CheckCheckpoint(false);
        }
    }
}