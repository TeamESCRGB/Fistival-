using Defines;
using DG.Tweening;
using Manager;
using System;
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
            Collection1=0,
            Collection2=1,
            Collection3=2,
            Collection4=3,
            Collection5=4,
            StagePreviewImg
        }

        enum Texts
        {
            BestTimeCounter
        }

        enum Objects
        {
            LeftSide,
            RightSide,
            Locker
        }

        private const int _stageCnt = 7;
        private int _stageIdx = 0;//이거 나중에 StageManager만들면 거기에 넣어줘야됨
        private bool _isLocked;

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

            if(_stageIdx == 6)
            {
                var clearDict = Managers.Instance.GameManager.GetClearedMapDictRef();

                int cnt = 0;

                for (int i = 0; i < 6; i++)
                {
                    if (clearDict[i])
                    {
                        cnt++;
                    }
                }

                GetObject((int)Objects.Locker).SetActive(cnt < 6);
            }
            else
            {
                GetObject((int)Objects.Locker).SetActive(false);
            }

            Span<int> collectionSlots = stackalloc int[] {(int)Images.Collection1, (int)Images.Collection2, (int)Images.Collection3, (int)Images.Collection4, (int)Images.Collection5};
            var collectionList = save.StageSaveDatas[_stageIdx].CollectedCollections;

            for(int i =0; i < collectionList.Count;i++)
            {
                var collectionData = Managers.Instance.DataManager.CollectionDataDict[collectionList[i]];
                GetImage(collectionData.SlotIDX).gameObject.SetActive(true);
                GetImage(collectionData.SlotIDX).sprite = Managers.Instance.ResourceManager.Load<Sprite>(collectionData.Image);
                collectionSlots[collectionData.SlotIDX] = -1;
            }

            for(int i = 0; i < collectionSlots.Length; i++)
            {
                if (collectionSlots[i] >= 0)
                {
                    GetImage(collectionSlots[i]).gameObject.SetActive(false);
                }
            }


        }

        private void OnNext(PointerEventData _)
        {
            _stageIdx = (_stageIdx + 1) % _stageCnt;
            UpdateUIState();
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "StageChangeSFX", false, Managers.Instance.GameManager.SFXVolume);
        }

        private void OnPrev(PointerEventData _)
        {
            _stageIdx--;
            if(_stageIdx < 0)
            {
                _stageIdx = _stageCnt - 1;
            }
            UpdateUIState();
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "StageChangeSFX", false, Managers.Instance.GameManager.SFXVolume);
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


            GetButton((int)Buttons.NextStage).GetComponent<RectTransform>().DOAnchorPosX(100, 0.25f) 
                .SetEase(Ease.OutQuad);
            GetButton((int)Buttons.PrevStage).GetComponent<RectTransform>().DOAnchorPosX(-100, 0.25f)
                .SetEase(Ease.OutQuad);

            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "GameStartSFX", false, Managers.Instance.GameManager.SFXVolume);
        }

        private void InternalLoadFunc()
        {
            Managers.Instance.ResourceManager.LoadAsyncAllIn("GameSceneBasicLoaded", (_, gameNow, gameMax) =>
            {
                if (gameNow < gameMax)
                {
                    return;
                }

                Managers.Instance.StageManager.SetStageIDX(_stageIdx);

                string loadKey = Managers.Instance.StageManager.GetStageData()?.FirstStageLoadedDatasName;
                loadKey = loadKey is null ? "" : loadKey;

                Managers.Instance.ResourceManager.LoadAsyncAllIn(loadKey, (_, now, max) =>
                {
                    if(now==max)
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
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "StageSelectExitSFX", false, Managers.Instance.GameManager.SFXVolume);
        }
    }
}