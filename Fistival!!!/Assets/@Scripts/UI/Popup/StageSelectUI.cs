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

        private const int _stageCnt = 7;
        [SerializeField]private int _stageIdx = 0;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            GetButton((int)Buttons.Exit).gameObject.BindUIEvent(OnExitButton);
            GetButton((int)Buttons.NextStage).gameObject.BindUIEvent(OnNext);
            GetButton((int)Buttons.PrevStage).gameObject.BindUIEvent(OnPrev);

            UpdateUIState();

            return true;
        }

        private void UpdateUIState()
        {
            var stageData = Managers.Instance.DataManager.StageDataDict[_stageIdx];
            var save = Managers.Instance.SaveDataManager.GetSaveFileData();
            GetText((int)Texts.BestTimeCounter).text = TimeUtils.SecToTimeStr(save.StageSaveDatas[_stageIdx].ClearTimeWithOutPause);

            if (save.StageSaveDatas[_stageIdx].IsCleared)
            {
                GetImage((int)Images.StagePreviewImg).sprite = Managers.Instance.ResourceManager.Load<Sprite>(stageData.MapClearedIMG);
            }
            else
            {
                GetImage((int)Images.StagePreviewImg).sprite = Managers.Instance.ResourceManager.Load<Sprite>(stageData.MapLockedIMG);
            }

            var collectionList = save.StageSaveDatas[_stageIdx].CollectedCollections;
            //GetImage((int)Images.Collection1).sprite; 이거 나중에 수집품 시스템 정리되면 그 때 세이브데이터에서 클리어 데이터 긁어와서 넣도록.
        }

        private void OnNext(PointerEventData _)
        {
            _stageIdx = (_stageIdx + 1) % _stageCnt;
            UpdateUIState();
        }

        private void OnPrev(PointerEventData _)
        {
            _stageIdx--;
            if(_stageIdx < 0)
            {
                _stageIdx = _stageCnt - 1;
            }
            UpdateUIState();
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