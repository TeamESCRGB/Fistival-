using Manager;
using UI.Popup;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
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


        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));
            int clearCnt = 0;
            foreach(int i in System.Enum.GetValues(typeof(Buttons)))
            {
                var saveData = Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas[i];
                var stageData = Managers.Instance.DataManager.StageDataDict[i];

                if(saveData.IsCleared)
                {
                    GetButton(i).GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(stageData.MapClearedIMG);
                    clearCnt++;
                }
                else
                {
                    GetButton(i).GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(stageData.MapLockedIMG);
                }
            }

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



            return true;
        }

        private void OnStage1Pressed(PointerEventData _)
        {
            Debug.Log("s1");
        }

        private void OnStage2Pressed(PointerEventData _)
        {
            Debug.Log("s2");
        }

        private void OnStage3Pressed(PointerEventData _)
        {
            Debug.Log("s3");
        }

        private void OnStage4Pressed(PointerEventData _)
        {
            Debug.Log("s4");
        }

        private void OnStage5Pressed(PointerEventData _)
        {
            Debug.Log("s5");
        }

        private void OnStage6Pressed(PointerEventData _)
        {
            Debug.Log("s6");
        }

        private void OnStage7Pressed(PointerEventData _)
        {
            Debug.Log("s7");
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