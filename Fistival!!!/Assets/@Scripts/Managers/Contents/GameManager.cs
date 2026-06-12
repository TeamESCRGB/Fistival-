using Coordinator;
using System.Collections.Generic;
using Defines;
using UnityEngine;
using UI.Popup;

namespace Manager.Contents
{
    public class GameManager
    {
        private bool _isPaused;
        private ModeTypes _nowMode;
        private float _timeScale;

        private double _timeCheckOffset = 0;
        private double _totalPlayTime = 0;

        private Dictionary<int,bool> _clearedMap = new Dictionary<int, bool>(16);


        public void ChangeMode(ModeTypes nowMode)
        {
            _nowMode = nowMode;
        }

        public bool IsGamePaused()
        {
            return _isPaused;
        }

        public void PauseGame()
        {
            if(_isPaused == false)
            {
                _isPaused = true;
                _timeScale = Time.timeScale;
                Time.timeScale = 0;
                if(_nowMode == ModeTypes.RHYTHM)
                {
                    Managers.Instance.RhythmModeManager.PausePattern();
                }
                Managers.Instance.NewInputSystemManager.SwitchActionMap(ActionMapTypes.UI);
                Managers.Instance.UIManager.ShowPopupUI<PauseUI>("PauseUI");
            }
        }

        public void UnPauseGame()
        {
            if (_isPaused)
            {
                _isPaused = false;
                Time.timeScale = _timeScale;
                if (_nowMode == ModeTypes.RHYTHM)
                {
                    Managers.Instance.RhythmModeManager.UnPausePattern();
                }
                Managers.Instance.NewInputSystemManager.SwitchActionMap(ActionMapTypes.PLAYER);
            }
        }

        public void InitPauseState()
        {
            Time.timeScale = 1;
            _isPaused = false;
        }

        public float MasterVolume
        {
            get
            {
                return Managers.Instance.SaveDataManager.GetGameSettingRef().MasterVolume;
            }
            set
            {
                AudioListener.volume = value;
                Managers.Instance.SaveDataManager.GetGameSettingRef().MasterVolume = value;
            }
        }

        public float SFXVolume
        {
            get
            {
                return Managers.Instance.SaveDataManager.GetGameSettingRef().SFXVolume;
            }
            set
            {
                AudioListener.volume = value;
                Managers.Instance.SaveDataManager.GetGameSettingRef().SFXVolume = value;
            }
        }

        public float BGMVolume
        {
            get
            {
                return Managers.Instance.SaveDataManager.GetGameSettingRef().BGMVolume;
            }
            set
            {
                AudioListener.volume = value;
                Managers.Instance.SaveDataManager.GetGameSettingRef().BGMVolume = value;
            }
        }

        public double GetTotalPlayTime()
        {
            return _totalPlayTime + (Time.unscaledTimeAsDouble - _timeCheckOffset);
        }

        public void InitTotalPlayTimeChecker(double  totalPlayTime)
        {
            _totalPlayTime = totalPlayTime;
            _timeCheckOffset = Time.unscaledTimeAsDouble;
        }

        public IDictionary<int,bool> GetClearedMapDictRef()
        {
            return _clearedMap;
        }

        public void ClearClearedMapDict()
        {
            _clearedMap.Clear();
        }
    }
}