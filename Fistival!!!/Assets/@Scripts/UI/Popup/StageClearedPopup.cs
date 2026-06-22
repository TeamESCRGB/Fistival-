using Data;
using Defines;
using DG.Tweening;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class StageClearedPopup : UIPopupBase
    {

        enum Images
        {
            Collection1=0,
            Collection2=1,
            Collection3=2,
            Collection4=3,
            Collection5=4,
            BG,
            StageClearedImg,
            Character
        }

        enum Texts
        {
            TotalDamageCounter,
            ClearTimeCounter
        }

        enum Buttons
        {
            Next
        }

        enum Objects
        {
            Damage,
            ClearTime
        }

        private int _idx=0;
        private double _clearTimeWithPause = 0;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindObject(typeof(Objects));
            BindButton(typeof(Buttons));
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            GetButton((int)Buttons.Next).gameObject.BindUIEvent(OnNext);
            

            GetImage((int)Images.Collection1).gameObject.SetActive(false);
            GetImage((int)Images.Collection2).gameObject.SetActive(false);
            GetImage((int)Images.Collection3).gameObject.SetActive(false);
            GetImage((int)Images.Collection4).gameObject.SetActive(false);
            GetImage((int)Images.Collection5).gameObject.SetActive(false);
            GetImage((int)Images.StageClearedImg).gameObject.SetActive(false);
            GetImage((int)Images.Character).gameObject.SetActive(false);

            GetObject((int)Objects.ClearTime).gameObject.SetActive(false);
            GetObject((int)Objects.Damage).gameObject.SetActive(false);
            StartCoroutine(ShowRoutine());

            Managers.Instance.GlobalSoundManager.Play(SoundChannel.BGM_0, "StageClearedBGM", true, Managers.Instance.GameManager.BGMVolume);

            return true;
        }

        public void SetupData(int idx,double clearTimeWithPause)
        {
            _idx = idx;
            _clearTimeWithPause = clearTimeWithPause;
        }

        private void SetupCollection(int idx)
        {
            var data = Managers.Instance.DataManager.CollectionDataDict[idx];
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "CollectionResultSFX", false, Managers.Instance.GameManager.SFXVolume);
            GetImage(data.SlotIDX).gameObject.SetActive(true);
            GetImage(data.SlotIDX).sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.Image);
            GetImage(data.SlotIDX).GetComponent<RectTransform>().DOPunchScale(new Vector3(0.5f, 0.5f, 0f), 0.25f, vibrato: 1, elasticity: 0.5f);
        }

        private IEnumerator ShowRoutine()
        {
            var stageData = Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas[_idx];
            var waiter = new WaitForSeconds(0.25f);

            GetImage((int)Images.BG).gameObject.SetActive(true);
            GetImage((int)Images.BG).GetComponent<RectTransform>().localScale = Vector3.zero;
            GetImage((int)Images.BG).GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.3f);

            var charImg = GetImage((int)Images.Character);
            charImg.gameObject.SetActive(true); // 캐릭터 활성화

            RectTransform rect = charImg.GetComponent<RectTransform>();
            rect.DOKill(); // 기존 트윈 찌꺼기 제거

            // 1. 현재 기기 해상도에 맞는 캐릭터 높이(또는 화면 높이)를 가져옵니다.
            float bottomPos = rect.rect.height;

            // 2. 시작 위치를 화면 하단 바깥으로 강제 지정 (Y축을 마이너스 높이만큼 내림)
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -bottomPos);

            // 3. 0.4초 동안 원래 위치(0)로 툭 튀어나오며 올라옵니다.
            rect.DOAnchorPosY(0f, 0.4f).SetEase(Ease.OutBack);
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "CharacterPopupSFX", false, Managers.Instance.GameManager.SFXVolume);

            yield return new WaitForSeconds(0.4f);

            GetImage((int)Images.StageClearedImg).gameObject.SetActive(true);
            GetImage((int)Images.StageClearedImg).GetComponent<RectTransform>().DOPunchScale(new Vector3(1f, 1f, 0f), 0.25f, vibrato: 1, elasticity: 0.5f);
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "ClearTextIMGShowSFX", false, Managers.Instance.GameManager.SFXVolume);
            yield return new WaitForSeconds(0.25f);


            for (int i = 0; i < stageData.CollectedCollections.Count; i++)
            {
                SetupCollection(stageData.CollectedCollections[i]);
                yield return waiter;
            }

            yield return new WaitForSeconds(1);

            GetObject((int)Objects.ClearTime).gameObject.SetActive(true);
            GetText((int)Texts.ClearTimeCounter).text = TimeUtils.SecToTimeStr(_clearTimeWithPause);
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicResultPunchSFX", false, Managers.Instance.GameManager.SFXVolume);
            yield return waiter;
            GetObject((int)Objects.Damage).gameObject.SetActive(true);
            GetText((int)Texts.TotalDamageCounter).text = stageData.TotalGainedDamage.ToString("N0");
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicResultPunchSFX", false, Managers.Instance.GameManager.SFXVolume);

            yield return waiter;

        }


        private void OnNext(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnSaveYes, OnSaveNo).SetText("현 시점의 세이브를 저장하시겠습니까?");
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", true, Managers.Instance.GameManager.SFXVolume);
        }

        private void OnSaveYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.SaveDataManager.SaveSaveData();
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "OnSaveFileSavedSFX", true, Managers.Instance.GameManager.SFXVolume);
            Managers.Instance.StageManager.ReturnToLobby();
        }

        private void OnSaveNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.StageManager.ReturnToLobby();
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", true, Managers.Instance.GameManager.SFXVolume);
        }
    }
}