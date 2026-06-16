using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utils;

namespace UI.Popup
{
    public class HomeUI : UIPopupBase
    {
        enum Buttons
        {
            EquipmentButton,
            DataButton,
            SaveButton,
            ExitButton
        }
        enum Images
        {
            Stage1=0,
            Stage2=1,
            Stage3=2,
            Stage4=3,
            Stage5=4,
            Stage6=5,
            Stage7=6
        }

        private bool _canPause = true;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            BindImage(typeof(Images));

            GetButton((int)Buttons.ExitButton).gameObject.BindUIEvent(OnExitButton);
            GetButton((int)Buttons.EquipmentButton).gameObject.BindUIEvent(OnEquipmentButton);
            GetButton((int)Buttons.DataButton).gameObject.BindUIEvent(OnDataButton);
            GetButton((int)Buttons.SaveButton).gameObject.BindUIEvent(OnSaveButton);

            Managers.Instance.NewInputSystemManager.UI_ESCInput -= PauseOpenBind;
            Managers.Instance.NewInputSystemManager.UI_ESCInput += PauseOpenBind;

            foreach (var data in Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas)
            {
                if(data.Value.IsCleared)
                {
                    //이거 이미지 로드할까 생각해봤는데, 그냥 직접 배치해두고 껐다켰다하는게 더 좋을듯
                    GetImage(data.Key).gameObject.SetActive(true);
                }
                else
                {
                    GetImage(data.Key).gameObject.SetActive(false);
                }
            }

            return true;
        }

        private void OnDisable()
        {
            Managers.Instance.NewInputSystemManager.UI_ESCInput -= PauseOpenBind;
        }

        private void PauseOpenBind(InputAction.CallbackContext ctx)
        {
            if(ctx.performed == false || _canPause == false)
            {
                return;
            }

            Managers.Instance.GameManager.PauseGame();
            _canPause = false;
        }

        private void LateUpdate()
        {
            if(_canPause)
            {
                return;
            }
            _canPause = Managers.Instance.GameManager.IsGamePaused() == false;
        }

        private void OnEquipmentButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<EquipmentUI>("EquipmentUI");
        }
        private void OnSaveButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<SaveFileMenu>("SaveFileMenu").SetMenuType(Defines.SaveFileAccessMode.OVERWRITE);
        }
        private void OnDataButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ShowPopupUI<DataUI>("DataUI");
        }

        private void OnExitButton(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}