using Defines;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class SaveFileMenu : UIPopupBase
    {
        enum Buttons
        {
            SelectButton,
            LeftButton,
            RightButton,
            ExitGameFileMenu
        }

        enum Text
        {
            InfoText,
            PageMaxText,
            PageNowText
        }

        private SaveFileAccessMode _nowMode = SaveFileAccessMode.LOAD;
        private int _selectedIdx = 0;
        private int _max = 0;

        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }
            _max = Managers.Instance.SaveDataManager.GetGameSlotCnt();
            BindText(typeof(Text));
            BindButton(typeof(Buttons));
            GetButton((int)Buttons.SelectButton).gameObject.BindUIEvent(OnFileClicked);
            GetButton((int)Buttons.ExitGameFileMenu).gameObject.BindUIEvent(OnExitButton);
            GetButton((int)Buttons.LeftButton).gameObject.BindUIEvent(OnLeftButton);
            GetButton((int)Buttons.RightButton).gameObject.BindUIEvent(OnRightButton);
            GetText((int)Text.PageMaxText).text = _max.ToString();

            return true;
        }

        protected override void Start()
        {
            base.Start();
            GetText((int)Text.InfoText).text = _nowMode == SaveFileAccessMode.LOAD ? "LOAD SAVE" : "SAVE";
            RefreshMoveButtonState();
            RefreshButtonState();
        }

        private void OnLeftButton(PointerEventData data)
        {
            if (_selectedIdx > 0)
            {
                _selectedIdx--;
            }

            RefreshButtonState();
            RefreshMoveButtonState();
        }

        private void OnRightButton(PointerEventData data)
        {
            if (_selectedIdx < _max - 1)
            {
                _selectedIdx++;
            }

            RefreshButtonState();
            RefreshMoveButtonState();
        }

        private void RefreshMoveButtonState()
        {
            GetText((int)Text.PageNowText).text = (_selectedIdx + 1).ToString();
            GetButton((int)Buttons.LeftButton).gameObject.SetActive(_selectedIdx > 0);
            GetButton((int)Buttons.RightButton).gameObject.SetActive(_selectedIdx < _max - 1);
        }

        public void SetMenuType(SaveFileAccessMode mode)
        {
            _nowMode = mode;
        }

        public void RefreshButtonState()
        {
            if (_init == false)
            {
                return;
            }
            if (Managers.Instance.SaveDataManager.IsGameFileEmpty(_selectedIdx))
            {
                GetButton((int)Buttons.SelectButton).GetComponentInChildren<TextMeshProUGUI>().text = "EMPTY";
                //여기에 이미지 보여주고 그런 로직 추가하기
            }
            else
            {
                GetButton((int)Buttons.SelectButton).GetComponentInChildren<TextMeshProUGUI>().text = "Saved";
                //여기에 이미지 보여주고 그런 로직 추가하기
            }
        }

        private void OnFileClicked(PointerEventData data)
        {
            if (_nowMode == SaveFileAccessMode.LOAD)
            {
                if (Managers.Instance.SaveDataManager.IsSaveFileEmpty(_selectedIdx))
                {
                    Managers.Instance.UIManager.ShowPopupUI<BasicPopupAlert>("BasicPopupAlert").SetText("save file empty");
                }
                else
                {
                    Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnLoadYes, OnConfirmNo);
                }
            }
            else if (_nowMode == SaveFileAccessMode.OVERWRITE)
            {
                Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnOverwriteYes, OnConfirmNo);
            }
        }

        private void OnOverwriteYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnLoadYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnConfirmNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }

        private void OnExitButton(PointerEventData data)
        {
            Managers.Instance.UIManager.ClosePopupUI();
        }
    }
}