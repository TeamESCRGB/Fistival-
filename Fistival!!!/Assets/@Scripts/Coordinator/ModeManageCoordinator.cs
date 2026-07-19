using Defines;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Coordinator
{
    public class ModeManageCoordinator : MonoBehaviour
    {
        public event Action<ModeTypes, bool> OnModeLockStatusChanged;
        public event Action<ModeBase> OnModeChanged;
        private Dictionary<ModeTypes,ModeBase> _modes;
        private ModeBase _nowMode;
        private void Awake()
        {
            _modes = new Dictionary<ModeTypes,ModeBase>(8);
            foreach(var mode in GetComponentsInChildren<ModeBase>(true))
            {
                _modes.Add(mode.ModeType, mode);
            }
            _nowMode = null;
        }

        public ModeBase[] GetModeList()
        {
            return _modes.Values.ToArray<ModeBase>();
        }

        public ModeTypes[] GetModeTypeList()
        {
            return _modes.Keys.ToArray<ModeTypes>();
        }

        public bool ChangeMode(ModeTypes type)
        {
            bool changedFromOther = false;
            if(IsModeUnlocked(type) == false)
            {
                return false;
            }
            if(_nowMode != null)
            {
                changedFromOther = true;
                _nowMode.PreDeInit();
                _nowMode.DeInit();
            }

            var modeData = Managers.Instance.DataManager.CommonModeDataDict[(int)type];
            _nowMode = _modes[type];
            _nowMode.PreInit(modeData,changedFromOther);
            
            if(changedFromOther==false)
            {
                _nowMode.Init(modeData);
            }

            Managers.Instance.GameManager.ChangeMode(type);
            OnModeChanged?.Invoke(_nowMode);
            return true;
        }

        public ModeBase GetNowMode()
        {
            return _nowMode;
        }

        public bool IsModeUnlocked(ModeTypes type)
        {
            if(_modes.TryGetValue(type,out var mode))
            {
                return mode.IsUnlocked;
            }
            return false;
        }

        public bool UnlockMode(ModeTypes type)
        {
            if(IsModeUnlocked(type) || (_modes.ContainsKey(type) == false))
            {
                return false;
            }
            OnModeLockStatusChanged?.Invoke(type,true);
            _modes[type].IsUnlocked = true;
            return true;
        }

        public bool LockMode(ModeTypes type)
        {
            if(IsModeUnlocked(type) == false)
            {
                return false;
            }
            OnModeLockStatusChanged?.Invoke(type, false);
            _modes[type].IsUnlocked = false;
            return true;
        }
    }
}
