using Manager;
using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace UI.Popup
{
    public class StageClearedPopup : UIPopupBase
    {

        enum Images
        {
            BG,
            StageClearedImg,
            Character,
            Collection1,
            Collection2,
            Collection3,
            Collection4,
            Collection5
        }

        enum Texts
        {
            TotalDamageCounter,
            ClearTimeCounter
        }

        private int _idx=0;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindImage(typeof(Images));
            BindText(typeof(Texts));

            StartCoroutine(ShowRoutine());

            return true;
        }

        public void SetIdx(int idx)
        {
            _idx = idx;
        }

        private IEnumerator ShowRoutine()
        {
            var collectionData = Managers.Instance.DataManager.CollectionDataDict;
            var stageData = Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas[_idx];
            var waiter = new WaitForSeconds(0.25f);
            GetImage((int)Images.Collection1).sprite = Managers.Instance.ResourceManager.Load<Sprite>(collectionData[stageData.CollectedCollections[0]].Image);
            yield return waiter;
            GetImage((int)Images.Collection2).sprite = Managers.Instance.ResourceManager.Load<Sprite>(collectionData[stageData.CollectedCollections[1]].Image);
            yield return waiter;
            GetImage((int)Images.Collection3).sprite = Managers.Instance.ResourceManager.Load<Sprite>(collectionData[stageData.CollectedCollections[2]].Image);
            yield return waiter;
            GetImage((int)Images.Collection4).sprite = Managers.Instance.ResourceManager.Load<Sprite>(collectionData[stageData.CollectedCollections[3]].Image);
            yield return waiter;
            GetImage((int)Images.Collection5).sprite = Managers.Instance.ResourceManager.Load<Sprite>(collectionData[stageData.CollectedCollections[4]].Image);
            yield return new WaitForSeconds(5);

            GetText((int)Texts.ClearTimeCounter).text = TimeUtils.SecToTimeStr(stageData.ClearTimeWithPause);
            yield return waiter;
            GetText((int)Texts.TotalDamageCounter).text = stageData.TotalGainedDamage.ToString("N0");


        }



    }
}