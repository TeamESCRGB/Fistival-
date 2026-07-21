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
        
        public void CheckCheckpoint(bool isTriggeredByAttack)
        {
            if(_isCheckpointChecked)
            {
                return;
            }

            //이미지 바꾸고, 체크포인트 애니메이션 틀어야됨
            if(isTriggeredByAttack)
            {
                //맞아서 활성화될 때 바뀔 애니메이션,이미지
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "CheckpointActivationByAttackSFX", false, Managers.Instance.GameManager.SFXVolume);
                Debug.Log("공격으로 활성화");
            }
            else
            {
                //상호작용으로 호출될 떄 바뀔 애니메이션,이미지
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, "CheckpointActivationByInteractSFX", false, Managers.Instance.GameManager.SFXVolume);
                Debug.Log("상호작용으로 활성화");
            }

            DeActivateShader();
            FixHighlightState(false);
            _isCheckpointChecked = true;
            Managers.Instance.StageManager.SaveCheckpoint(transform.position);
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
            CheckCheckpoint(false);
        }
    }
}