using UnityEngine;
using UnityEngine.SceneManagement;
using Defines;
using System;
using UnityEngine.Events;
using Scenes;

namespace Manager.Core
{
    public class SceneManagerEx
    {
        public SceneBase NowSceneInstance { get; private set; } = null;
        public SceneManagerEx()
        {
            SceneManager.sceneLoaded += RefreshNowSceneInstance;
        }
        public void LoadScene(SceneType sceneName)
        {
            if(sceneName == SceneType.UNKNOWN)
            {
                return;
            }

            string name = Enum.GetName(typeof(SceneType), sceneName);

            if(string.IsNullOrEmpty(name))
            {
                return;
            }

            Managers.Instance.ResetManagers();
            SceneManager.LoadScene(name);
        }

        private void RefreshNowSceneInstance(Scene scene, LoadSceneMode mode)
        {
            NowSceneInstance = GameObject.FindFirstObjectByType<SceneBase>();
            Managers.Instance.UIManager.Init();
            Managers.Instance.GlobalSoundManager.Init();
        }


        public void SubscribeOnSceneUnloaded(UnityAction<Scene> callback)
        {
            SceneManager.sceneUnloaded -= callback;
            SceneManager.sceneUnloaded += callback;
        }

        public void UnSubscribeOnSceneUnloaded(UnityAction<Scene> callback)
        {
            SceneManager.sceneUnloaded -= callback;
        }
    }
}