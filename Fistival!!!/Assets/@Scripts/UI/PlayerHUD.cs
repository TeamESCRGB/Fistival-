using Coordinator;
using Coordinator.Objects;
using Data;
using Manager;
using Manager.Contents;
using System.Collections;
using UnityEngine;

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

        private const int _chargeOffset = 8;
        private int _maxHP = 0;

        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }
            BindObject(typeof(Objects));
            BindImage(typeof(Images));


            var playerCoord = FindAnyObjectByType<PlayerCoordinator>();
            var modeManageCoord = FindAnyObjectByType<ModeManageCoordinator>();
            var playerData = playerCoord.GetPlayerData();
            _maxHP = playerData.MaxHP;
            modeManageCoord.OnModeChanged += OnModeChanged;
            playerCoord.GetComponentInChildren<HPCoordinator>().SubscribeOnHPChanged(OnHPChanged);
            GetObject((int)Objects.BossHPBarHUD).SetActive(false);
            OnModeChanged(modeManageCoord.GetNowMode());

            GetImage((int)Images.GrabbedObjectImg).sprite = Managers.Instance.ResourceManager.Load<Sprite>("NullIg");

            for(int i = 0; i < 8; i++)
            {
                GetImage(i).gameObject.SetActive(i < playerData.MaxHP);
                GetImage(i).sprite = Managers.Instance.ResourceManager.Load<Sprite>("HPOn");
            }

            for(int i = _chargeOffset; i < 12; i++)
            {
                GetImage(i).gameObject.SetActive(i < playerData.MaxChargeCnt + _chargeOffset);
                GetImage(i).sprite = Managers.Instance.ResourceManager.Load<Sprite>("ChargeOff");
            }

            return true;
        }

        private void OnModeChanged(ModeBase mode)
        {
            mode.GetComponentInChildren<HandCoordinatorBase>().OnChargeRateChanged += OnChargeRateChanged;
            mode.GetComponentInChildren<HandCoordinatorBase>().OnGrabbedObjectChanged += OnGrabbedObjectChanged;
        }

        private void OnHPChanged(int old, int now, int delta)
        {
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
                GetImage((int)Images.GrabbedObjectImg).sprite = Managers.Instance.ResourceManager.Load<Sprite>("NullIg");
            }
            else
            {
                GetImage((int)Images.GrabbedObjectImg).sprite = Managers.Instance.ResourceManager.Load<Sprite>(obj.SpriteName);
            }
        }
    }
}