using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class StageSelectUI : UIPopupBase
    {

        enum Buttons
        {
            NextStage,
            PrevStage,
            StageStart,
            Exit
        }

        enum Images
        {
            StagePreviewImg,
            Collection1,
            Collection2,
            Collection3,
            Collection4,
            Collection5
        }

        enum Texts
        {
            BestTimeCounter
        }

        private int _stageIdx = 0;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            BindImage(typeof(Images));
            BindText(typeof(Texts));


            UpdateUIState();

            return true;
        }

        private void UpdateUIState()
        {
            GetText((int)Texts.BestTimeCounter).text = TimeUtils.SecToTimeStr(Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas[_stageIdx].ClearTimeWithOutPause);
            GetButton((int)Buttons.Exit).gameObject.BindUIEvent(OnExitButton);

            var collectionList = Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas[_stageIdx].CollectedCollections;
            //GetImage((int)Images.Collection1).sprite; 이거 나중에 수집품 시스템 정리되면 그 때 세이브데이터에서 클리어 데이터 긁어와서 넣도록.
        }

        public void SetInitialStageIdx(int idx)
        {
            _stageIdx = idx;
        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}