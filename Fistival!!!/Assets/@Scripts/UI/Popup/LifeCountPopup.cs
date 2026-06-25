using Manager;
using System.Collections;
using TMPro;
using UnityEngine;
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

            StartCoroutine(ShowRoutine());
            return true;
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
        }

        public void SetData(int life)
        {
            _life = life;
        }
    }
}