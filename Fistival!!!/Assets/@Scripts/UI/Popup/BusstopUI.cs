using Data;
using Manager;
using UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

namespace UI.Popup
{
    public class BusstopUI : UIPopupBase
    {
        enum Buttons
        {
            Stage1=0,
            Stage2=1,
            Stage3=2,
            Stage4=3,
            Stage5=4,
            Stage6=5,
            Stage7=6,
            Stage7_Locker,
            Exit
        }

        enum Images
        {
            Stage1_ClearedMark,
            Stage2_ClearedMark,
            Stage3_ClearedMark,
            Stage4_ClearedMark,
            Stage5_ClearedMark,
            Stage6_ClearedMark,
            Stage7_ClearedMark
        }


        private bool _canPause = true;
        private void UpdateButton(int idx, ref int clearCnt)
        {
            var saveData = Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas[idx];//클리어 정보 띄울거 생각해서 일단 남겨둠
            var stageData = Managers.Instance.DataManager.StageDataDict[idx];
            if(saveData.IsCleared)
            {
                clearCnt++;
            }
        }

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            int clearCnt = 0;

            UpdateButton((int)Buttons.Stage1, ref clearCnt);
            UpdateButton((int)Buttons.Stage2, ref clearCnt);
            UpdateButton((int)Buttons.Stage3, ref clearCnt);
            UpdateButton((int)Buttons.Stage4, ref clearCnt);
            UpdateButton((int)Buttons.Stage5, ref clearCnt);
            UpdateButton((int)Buttons.Stage6, ref clearCnt);
            UpdateButton((int)Buttons.Stage7, ref clearCnt);

            if(clearCnt < 6)
            {
                GetButton((int)Buttons.Stage7).interactable = false;
            }
            else
            {
                GetButton((int)Buttons.Stage7_Locker).gameObject.SetActive(false);
            }

            GetButton((int)Buttons.Exit).gameObject.BindUIEvent(OnExitButton);
            GetButton((int)Buttons.Stage1).gameObject.BindUIEvent(OnStage1Pressed);
            GetButton((int)Buttons.Stage2).gameObject.BindUIEvent(OnStage2Pressed);
            GetButton((int)Buttons.Stage3).gameObject.BindUIEvent(OnStage3Pressed);
            GetButton((int)Buttons.Stage4).gameObject.BindUIEvent(OnStage4Pressed);
            GetButton((int)Buttons.Stage5).gameObject.BindUIEvent(OnStage5Pressed);
            GetButton((int)Buttons.Stage6).gameObject.BindUIEvent(OnStage6Pressed);
            GetButton((int)Buttons.Stage7).gameObject.BindUIEvent(OnStage7Pressed);
            GetButton((int)Buttons.Stage7_Locker).gameObject.BindUIEvent(OnStage7LockedPressed);

            var stageClearedData = Managers.Instance.GameManager.GetClearedMapDictRef();
            GetImage((int)Images.Stage1_ClearedMark).gameObject.SetActive(stageClearedData[(int)Buttons.Stage1]);
            GetImage((int)Images.Stage2_ClearedMark).gameObject.SetActive(stageClearedData[(int)Buttons.Stage2]);
            GetImage((int)Images.Stage3_ClearedMark).gameObject.SetActive(stageClearedData[(int)Buttons.Stage3]);
            GetImage((int)Images.Stage4_ClearedMark).gameObject.SetActive(stageClearedData[(int)Buttons.Stage4]);
            GetImage((int)Images.Stage5_ClearedMark).gameObject.SetActive(stageClearedData[(int)Buttons.Stage5]);
            GetImage((int)Images.Stage6_ClearedMark).gameObject.SetActive(stageClearedData[(int)Buttons.Stage6]);
            GetImage((int)Images.Stage7_ClearedMark).gameObject.SetActive(stageClearedData[(int)Buttons.Stage7]);

            Managers.Instance.NewInputSystemManager.UI_ESCInput -= PauseOpenBind;
            Managers.Instance.NewInputSystemManager.UI_ESCInput += PauseOpenBind;

            return true;
        }
        private void OnDisable()
        {
            Managers.Instance.NewInputSystemManager.UI_ESCInput -= PauseOpenBind;
        }

        private void PauseOpenBind(InputAction.CallbackContext ctx)
        {
            if (ctx.performed == false || _canPause == false)
            {
                return;
            }

            Managers.Instance.GameManager.PauseGame();
            _canPause = false;
        }

        private void LateUpdate()
        {
            if (_canPause)
            {
                return;
            }
            _canPause = Managers.Instance.GameManager.IsGamePaused() == false;
        }

        private void OnStage1Pressed(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<StageSelectUI>("StageSelectUI").SetInitialStageIdx(0);
        }

        private void OnStage2Pressed(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<StageSelectUI>("StageSelectUI").SetInitialStageIdx(1);
        }

        private void OnStage3Pressed(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<StageSelectUI>("StageSelectUI").SetInitialStageIdx(2);
        }

        private void OnStage4Pressed(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<StageSelectUI>("StageSelectUI").SetInitialStageIdx(3);
        }

        private void OnStage5Pressed(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<StageSelectUI>("StageSelectUI").SetInitialStageIdx(4);
        }

        private void OnStage6Pressed(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<StageSelectUI>("StageSelectUI").SetInitialStageIdx(5);
        }

        private void OnStage7Pressed(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<StageSelectUI>("StageSelectUI").SetInitialStageIdx(6);
        }

        private void OnStage7LockedPressed(PointerEventData _)
        {
            Debug.Log("locked");
        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}