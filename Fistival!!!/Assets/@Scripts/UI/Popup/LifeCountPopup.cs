using Manager;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Popup
{
    public class LifeCountPopup : UIPopupBase
    {

        enum Objects
        {
            LifeCounter,
            StagePreviewImg,
            Life
        }

        private int _life=0;
        private bool _canPause = true;
        private string _bgm;
        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindObject(typeof(Objects));
            GetObject((int)Objects.Life).GetComponent<TMP_Text>().text = $"x {_life}";
            GetObject((int)Objects.StagePreviewImg).GetComponent<Image>().sprite = Managers.Instance.ResourceManager.Load<Sprite>(Managers.Instance.StageManager.GetStageData().MapLockedIMG);

            GetObject((int)Objects.Life).SetActive(false);
            GetObject((int)Objects.LifeCounter).SetActive(false);
            Managers.Instance.NewInputSystemManager.UI_ESCInput -= OnESCInputBind;
            Managers.Instance.NewInputSystemManager.UI_ESCInput += OnESCInputBind;
            StartCoroutine(ShowRoutine());
            return true;
        }

        private void OnDisable()
        {
            Managers.Instance.NewInputSystemManager.UI_ESCInput -= OnESCInputBind;
        }

        private void OnESCInputBind(InputAction.CallbackContext ctx)
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

        private IEnumerator ShowRoutine()
        {
            var waiter = new WaitForSeconds(2);
            yield return waiter;
            GetObject((int)Objects.StagePreviewImg).SetActive(false);
            GetObject((int)Objects.Life).SetActive(true);
            GetObject((int)Objects.LifeCounter).SetActive(true);
            yield return waiter;
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.BGM_0, _bgm, true);
        }

        public void SetData(int life, string bgm)
        {
            _life = life;
            _bgm= bgm;
        }
    }
}