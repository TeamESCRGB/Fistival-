using Manager.Core;
using UnityEngine;
using Utils;

namespace Manager
{
    public class Managers : MonoBehaviour
    {
        private static Managers _sInstance = null;

        public static Managers Instance { get { Init(); return _sInstance; } }

        #region core

        private GameObjectPoolManager _goPoolMgr = new GameObjectPoolManager();

        public GameObjectPoolManager GameObjectPoolManager { get { return Instance._goPoolMgr} }

        #endregion

        private static void Init()
        {
            if(_sInstance == null)
            {
                GameObject go = GameObject.Find("@Managers");

                if(go == null)
                {
                    go = new GameObject("@Managers");
                }

                _sInstance = go.GetOrAddComponent<Managers>();

                DontDestroyOnLoad(go);
            }
        }

        public void ResetManagers()
        {

        }
    }
}