using Defines;
using DG.Tweening;
using Manager;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI.Popup
{
    public class StageFailedPopup : UIPopupBase
    {
        enum Images
        {
            FailedImage,
            BG,
            Character
        }

        enum Buttons
        {
            Restart,
            Return
        }

        public override bool Init()
        {

            if(base.Init() == false)
            {
                return false;
            }

            BindImage(typeof(Images));
            BindButton(typeof(Buttons));


            GetImage((int)Images.FailedImage).gameObject.SetActive(false);
            GetImage((int)Images.BG).gameObject.SetActive(false);
            GetImage((int)Images.Character).gameObject.SetActive(false);

            GetButton((int)Buttons.Restart).gameObject.BindUIEvent(OnRestart);
            GetButton((int)Buttons.Return).gameObject.BindUIEvent(OnReturn);
            GetButton((int)Buttons.Restart).gameObject.SetActive(false);
            GetButton((int)Buttons.Return).gameObject.SetActive(false);


            StartCoroutine(ShowRoutine());

            Managers.Instance.GlobalSoundManager.Play(SoundChannel.BGM_0, "StageFailedBGM", true);

            return true;
        }

        private IEnumerator ShowRoutine()
        {

            GetImage((int)Images.BG).gameObject.SetActive(true);
            GetImage((int)Images.BG).GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).From(Vector3.zero);
            yield return new WaitForSeconds(0.3f);

            GetImage((int)Images.FailedImage).gameObject.SetActive(true);
            GetImage((int)Images.FailedImage).gameObject.GetComponent<Image>().DOFade(1f, 1).From(0f);
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "FailedTextIMGShowSFX", false);
            GetImage((int)Images.Character).gameObject.SetActive(true);
            GetImage((int)Images.Character).gameObject.GetComponent<Image>().DOFade(1f, 1).From(0f);

            yield return new WaitForSeconds(1.5f);
            GetButton((int)Buttons.Return).gameObject.SetActive(true);
            GetButton((int)Buttons.Return).gameObject.GetComponent<Image>().DOFade(1f,1f).From(0f);
            GetButton((int)Buttons.Restart).gameObject.SetActive(true);
            GetButton((int)Buttons.Restart).gameObject.GetComponent<Image>().DOFade(1f,1f).From(0f);
        }

        private void OnRestart(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
            int idx = Managers.Instance.StageManager.GetStageData().Idx;
            Managers.Instance.StageManager.Init();
            Managers.Instance.StageManager.SetStageIDX(idx);
            Managers.Instance.SceneManagerEx.LoadScene(Defines.SceneType.GameScene);
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "GameStartSFX", false);
        }

        private void OnReturn(PointerEventData _)
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.StageManager.ReturnToLobby();
            Managers.Instance.GlobalSoundManager.Play(SoundChannel.EFFECT_0, "BasicButtonClickSFX", false);
        }

    }
}