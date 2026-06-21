using Data;
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

        private int _idx=0;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            GetButton((int)Buttons.Next).gameObject.BindUIEvent(OnNext);
            StartCoroutine(ShowRoutine());

            return true;
        }

        public void SetIdx(int idx)
        {
            _idx = idx;
        }

        private void SetupCollection(int idx)
        {
            var data = Managers.Instance.DataManager.CollectionDataDict[idx];
            GetImage(data.SlotIDX).sprite = Managers.Instance.ResourceManager.Load<Sprite>(data.Image);
        }

        private IEnumerator ShowRoutine()
        {
            
            var stageData = Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas[_idx];
            var waiter = new WaitForSeconds(0.25f);


            for(int i = 0; i < stageData.CollectedCollections.Count; i++)
            {
                SetupCollection(stageData.CollectedCollections[i]);
                yield return waiter;
            }

            yield return new WaitForSeconds(1);

            GetText((int)Texts.ClearTimeCounter).text = TimeUtils.SecToTimeStr(stageData.ClearTimeWithPause);
            yield return waiter;
            GetText((int)Texts.TotalDamageCounter).text = stageData.TotalGainedDamage.ToString("N0");
        }


        private void OnNext(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnSaveYes, OnSaveNo).SetText("현 시점의 세이브를 저장하시겠습니까?");
        }

        private void OnSaveYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.SaveDataManager.SaveSaveData();
            Managers.Instance.StageManager.ReturnToLobby();
        }

        private void OnSaveNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.StageManager.ReturnToLobby();
        }
    }
}