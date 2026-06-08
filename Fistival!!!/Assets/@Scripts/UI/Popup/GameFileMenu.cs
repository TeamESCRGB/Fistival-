using Defines;
using Manager;
using TMPro;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Popup
{
    public class GameFileMenu : UIPopupBase
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
        private int _max=0;
        public override bool Init()
        {
            if(base.Init() == false)
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
            GetText((int)Text.InfoText).text = _nowMode == SaveFileAccessMode.LOAD ? "LOAD GAME" : "NEW GAME";
            RefreshMoveButtonState();
            RefreshButtonState();
        }

        private void OnLeftButton(PointerEventData data)
        {
            if(_selectedIdx > 0)
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
            if(_init == false)
            {
                return;
            }
            if (Managers.Instance.SaveDataManager.IsGameFileEmpty(_selectedIdx))
            {
                GetButton((int)Buttons.SelectButton).GetComponentInChildren<TextMeshProUGUI>().text = "EMPTY";
            }
            else
            {
                GetButton((int)Buttons.SelectButton).GetComponentInChildren<TextMeshProUGUI>().text = "Saved";
            }
        }

        private void OnFileClicked(PointerEventData data)
        {
            if(_nowMode == SaveFileAccessMode.LOAD)
            {
                if(Managers.Instance.SaveDataManager.IsGameFileEmpty(_selectedIdx))
                {
                    Managers.Instance.UIManager.ShowPopupUI<BasicPopupAlert>("BasicPopupAlert").SetText("game file empty");
                }
                else
                {
                    Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnLoadYes, OnConfirmNo);
                }
            }
            else if(_nowMode == SaveFileAccessMode.OVERWRITE)
            {
                Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnOverwriteYes, OnConfirmNo);
            }
        }

        private void OnOverwriteYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            if (Managers.Instance.SaveDataManager.SelectGameFile(_selectedIdx) == false)
            {
                return;
            }

            Managers.Instance.SaveDataManager.ClearSelectedGameFile();
            Managers.Instance.SaveDataManager.SelectGameFile(_selectedIdx);
            Managers.Instance.SaveDataManager.SelectSaveFile(0);
            Managers.Instance.SaveDataManager.SaveSaveData();

            Managers.Instance.ResourceManager.LoadAsyncAllIn("LobbySceneLoaded", (_, now, end) =>
            {
                if(now == end)
                {
                    Managers.Instance.ResourceManager.ReleaseIn("MainSceneLoaded");
                    Managers.Instance.SceneManagerEx.LoadScene(SceneType.LobbyScene);
                }
            });
            //컷씬만화 띄우고, 로비화면으로 넘어가도록 하기.
            //지금은 바로 로비화면으로 넘어가도록 한다.
        }

        private void OnLoadYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.SaveDataManager.SelectGameFile(_selectedIdx);
            //세이브파일 선택 팝업 띄우기
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