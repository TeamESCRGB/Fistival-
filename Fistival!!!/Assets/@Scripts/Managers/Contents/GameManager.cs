using Coordinator;
using System.Collections.Generic;
using Defines;
using UnityEngine;

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

        public float Volume
        {
            get
            {
                return Managers.Instance.SaveDataManager.GetGameSettingRef().Volume;
            }
            set
            {
                AudioListener.volume = value;
                Managers.Instance.SaveDataManager.GetGameSettingRef().Volume = value;
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