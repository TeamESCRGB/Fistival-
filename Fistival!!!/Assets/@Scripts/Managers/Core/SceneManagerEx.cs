using Defines;
using Scenes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Manager.Core
{
    public class SceneManagerEx
    {
        private SceneBase _currentScene;
        public SceneBase CurrentScene { get { return _currentScene; } }

        public SceneManagerEx()
        {
            //https://highfence.tistory.com/18
            //씬 로드가 끝난 후 호출되는 콜백
            //씬이 로드되고, 거기의 OnEnable들이 호출된 후 호출되는 콜백
            //이런게 있었구나
            SceneManager.sceneLoaded += RefreshNowSceneInstance;
        }
        //에셋 로드 위한 비동기 전환 구현하기
        public void LoadScene(SceneType name)
        {
            if (name == SceneType.UNKNOWN)
            {
#if UNITY_EDITOR
                Debug.LogWarning("UNKNOWN scene");
#endif
                return;
            }
            SceneManager.LoadScene(GetSceneName(name));
            Managers.Instance.ResetManagers();
        }

        void RefreshNowSceneInstance(Scene scene, LoadSceneMode mode)
        {
            _currentScene = GameObject.FindFirstObjectByType<SceneBase>();
            Managers.Instance.UIManager.Init();
        }

        string GetSceneName(SceneType type)
        {
            return System.Enum.GetName(typeof(SceneType), type);
        }

        public void SubscribeSceneUnloadedEvent(UnityAction<Scene> callback)
        {
            SceneManager.sceneUnloaded -= callback;
            SceneManager.sceneUnloaded += callback;
        }

        public void UnsubscribeSceneUnloadedEvent(UnityAction<Scene> callback)
        {
            SceneManager.sceneUnloaded -= callback;
        }
    }
}