using Coordinator;
using Defines;
using UnityEngine;

namespace Manager.Contents
{
    public class GameManager
    {
        private bool _isPaused;
        private ModeTypes _nowMode;
        private float _timeScale;

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
    }
}