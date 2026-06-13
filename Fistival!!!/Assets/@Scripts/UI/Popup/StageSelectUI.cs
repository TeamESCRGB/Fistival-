using Defines;
using DG.Tweening;
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

        enum Objects
        {
            LeftSide,
            RightSide
        }

        private const int _stageCnt = 7;
        private int _stageIdx = 0;//이거 나중에 StageManager만들면 거기에 넣어줘야됨


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
            GetButton((int)Buttons.Exit).gameObject.BindUIEvent(OnExitButton);
            GetButton((int)Buttons.NextStage).gameObject.BindUIEvent(OnNext);
            GetButton((int)Buttons.PrevStage).gameObject.BindUIEvent(OnPrev);
            GetButton((int)Buttons.StageStart).gameObject.BindUIEvent(OnGameStart);

            //UpdateUIState();

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

        private void OnGameStart(PointerEventData _)
        {
            GetObject((int)Objects.LeftSide).transform.DOLocalRotate(new Vector3(0, 0, 5), 0.5f)
                .SetEase(Ease.OutQuad);

            GetObject((int)Objects.LeftSide).GetComponent<RectTransform>().DOAnchorPosX(-25, 0.5f) // 왼쪽으로 100만큼 이동 (수치 조절 가능)
                .SetEase(Ease.OutQuad);


            // 2. 오른쪽 티켓: 오른쪽으로 회전하며 + 오른쪽(+X)으로 이동
            GetObject((int)Objects.RightSide).transform.DOLocalRotate(new Vector3(0, 0, -5), 0.5f)
                .SetEase(Ease.OutQuad);

            GetObject((int)Objects.RightSide).GetComponent<RectTransform>().DOAnchorPosX(25, 0.5f) // 오른쪽으로 100만큼 이동 (수치 조절 가능)
                .SetEase(Ease.OutQuad).onComplete += InternalLoadFunc;
        }

        private void InternalLoadFunc()
        {
            Managers.Instance.ResourceManager.LoadAsyncAllIn("GameSceneBasicLoaded", (_, gameNow, gameMax) =>
            {
                if (gameNow < gameMax)
                {
                    return;
                }

                Managers.Instance.ResourceManager.LoadAsyncAllIn(string.Format("Stage{0}Loaded_0", _stageIdx), (_, now, max) =>
                {
                    if (now == max)
                    {
                        Managers.Instance.ResourceManager.ReleaseIn("LobbySceneLoaded");
                        Managers.Instance.SceneManagerEx.LoadScene(SceneType.GameScene);
                    }
                });

            });
        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}