using Coordinator;
using Coordinator.Objects;
using Data;
using Manager;
using Manager.Contents;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class PlayerHUD : UIBase
    {
        enum Images
        {
            Heart1=0,
            Heart2=1,
            Heart3=2,
            Heart4=3,
            Heart5=4,
            Heart6=5,
            Heart7=6,
            Heart8=7,
            Charge1=8,
            Charge2=9,
            Charge3=10,
            Charge4=11,

            GrabbedObjectImg
        }

        enum Objects
        {
            BossHPBarHUD
        }

        private Slider _bossHPSlider;

        private const int _chargeOffset = 8;
        private int _maxHP = 0;
        private int _bossHPMax = 1;

        private Animator _hpAnimator;


        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }
            BindObject(typeof(Objects));
            BindImage(typeof(Images));

            _bossHPSlider = GetObject((int)Objects.BossHPBarHUD).GetComponentInChildren<Slider>();

            InitUIDatas();

            _hpAnimator = gameObject.GetChild<Animator>("@HPHUD",true,true);
            _hpAnimator.ResetTrigger("Damaged");

            return true;
        }

        public void InitUIDatas()
        {
            var playerCoord = FindAnyObjectByType<PlayerCoordinator>();
            var modeManageCoord = FindAnyObjectByType<ModeManageCoordinator>();
            var playerData = playerCoord.GetPlayerData();
            _maxHP = playerData.MaxHP;

            modeManageCoord.OnModeChanged -= OnModeChanged;
            modeManageCoord.OnModeChanged += OnModeChanged;
            playerCoord.GetComponentInChildren<HPCoordinator>().SubscribeOnHPChanged(OnHPChanged);

            GetObject((int)Objects.BossHPBarHUD).SetActive(false);
            OnModeChanged(modeManageCoord.GetNowMode());

            GetImage((int)Images.GrabbedObjectImg).sprite = Managers.Instance.ResourceManager.Load<Sprite>("NullIg");

            //이쪽을 어떻게 해야되나
            for (int i = 0; i < 8; i++)
            {
                GetImage(i).gameObject.SetActive(i < playerData.MaxHP);
                GetImage(i).sprite = Managers.Instance.ResourceManager.Load<Sprite>("HPOn");
            }

            for (int i = _chargeOffset; i < 12; i++)
            {
                GetImage(i).gameObject.SetActive(i < playerData.MaxChargeCnt + _chargeOffset);
                GetImage(i).sprite = Managers.Instance.ResourceManager.Load<Sprite>("ChargeOff");
            }
        }


        public void SetBoss(HPCoordinator bossHPCoord, int maxHP)
        {
            _bossHPMax = maxHP;
            _bossHPSlider.value = 1;
            GetObject((int)Objects.BossHPBarHUD).SetActive(true);
            bossHPCoord.SubscribeOnHPChanged(OnBossHPChanged);
            bossHPCoord.SubscribeOnDead(OnBossDead);
        }

        private void OnBossDead()
        {
            GetObject((int)Objects.BossHPBarHUD).SetActive(false);
        }

        private void OnModeChanged(ModeBase mode)
        {
            mode.GetComponentInChildren<HandCoordinatorBase>().OnChargeRateChanged -= OnChargeRateChanged;
            mode.GetComponentInChildren<HandCoordinatorBase>().OnChargeRateChanged += OnChargeRateChanged;
            mode.GetComponentInChildren<HandCoordinatorBase>().OnGrabbedObjectChanged -= OnGrabbedObjectChanged;
            mode.GetComponentInChildren<HandCoordinatorBase>().OnGrabbedObjectChanged += OnGrabbedObjectChanged;
        }

        private void OnHPChanged(int old, int now, int delta)
        {
            if(now < old)
            {
                _hpAnimator.SetTrigger("Damaged");
            }
            for (int i = 0; i < _maxHP; i++)
            {
                if(now > i)
                {
                    GetImage(i).sprite = Managers.Instance.ResourceManager.Load<Sprite>("HPOn");
                }
                else
                {
                    GetImage(i).sprite = Managers.Instance.ResourceManager.Load<Sprite>("HPOff");
                }
            }
        }

        private void OnBossHPChanged(int old, int now, int delta)
        {
            if(_bossHPMax < 0)
            {
                _bossHPSlider.value = 0;
            }
            else
            {
                _bossHPSlider.value = (float)now / _bossHPMax;
            }
        }

        private void OnChargeRateChanged(int now, int max)
        {
            for(int i = _chargeOffset; i < _chargeOffset + max; i++)
            {
                if(now + _chargeOffset > i)
                {
                    GetImage(i).sprite = Managers.Instance.ResourceManager.Load<Sprite>("ChargeOn");
                }
                else
                {
                    GetImage(i).sprite = Managers.Instance.ResourceManager.Load<Sprite>("ChargeOff");
                }
            }
        }

        private void OnGrabbedObjectChanged(ObjectData obj)
        {
            if(obj is null)
            {
                GetImage((int)Images.GrabbedObjectImg).sprite = Managers.Instance.ResourceManager.Load<Sprite>("NullIMG");
            }
            else
            {
                GetImage((int)Images.GrabbedObjectImg).sprite = Managers.Instance.ResourceManager.Load<Sprite>(obj.SpriteName);
            }
        }
    }
}