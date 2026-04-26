using Manager.Contents;
using Manager.Core;
using UnityEngine;
using Utils;

namespace Manager
{
    public class Managers : MonoBehaviour
    {
        private static Managers _sInstance = null;

        public static Managers Instance { get { Init(); return _sInstance; } }


        #region Contents
        private CooldownManager _cooldownMgr;
        private AttackManager _attackMgr;
        private RhythmModeManager _rhythmMgr;//나중에 특정 씬에서만 쓰는 매니저 Inject/Deinit가능하게 리펙토링 예정
        public CooldownManager CooldownManager { get { return Instance._cooldownMgr; } }
        public AttackManager AttackManager { get { return Instance._attackMgr; }  }
        public RhythmModeManager RhythmModeManager { get { return Instance._rhythmMgr; } }
        #endregion


        #region core

        private GameObjectPoolManager _goPoolMgr = new GameObjectPoolManager();
        private ResourceManager _resourceMgr = new ResourceManager();// 
        private GlobalSoundManager _gSoundMgr = new GlobalSoundManager();
        private SceneManagerEx _sceneMgr = new SceneManagerEx();
        private DataManager _dataMgr = new DataManager();
        private UIManager _uiMgr = new UIManager();

        public GameObjectPoolManager GameObjectPoolManager { get { return Instance._goPoolMgr; } }
        public ResourceManager ResourceManager { get { return Instance._resourceMgr; } }
        public GlobalSoundManager GlobalSoundManager { get { return Instance._gSoundMgr; } }
        public SceneManagerEx SceneManagerEx {  get { return Instance._sceneMgr; } }
        public DataManager DataManager {  get { return Instance._dataMgr; } }
        public UIManager UIManager {  get { return Instance._uiMgr; } }

        #endregion

        private void LateUpdate()
        {
            _resourceMgr.OnLateUpdate();
        }

        private void OnDestroy()
        {
            if (_sInstance == null)
            {
                return;
            }
            _sInstance._attackMgr.Clear();
            _sInstance._rhythmMgr.Clear();

            _sInstance._cooldownMgr = null;
            _sInstance._attackMgr = null;
            _sInstance._rhythmMgr = null;

            _goPoolMgr.Clear();
            _sInstance._resourceMgr.Clear();
            _sInstance._gSoundMgr.Clear();
            _sInstance._uiMgr.Clear();

            _sInstance._goPoolMgr = null;
            _sInstance._resourceMgr = null;
            _sInstance._gSoundMgr = null;
            _sInstance._sceneMgr = null;
            _sInstance._dataMgr = null;
            _sInstance._uiMgr = null;

            _sInstance = null;
        }

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

                _sInstance._cooldownMgr = go.GetOrAddComponent<CooldownManager>();
                _sInstance._attackMgr = go.GetOrAddComponent<AttackManager>();
                _sInstance._rhythmMgr = go.GetOrAddComponent<RhythmModeManager>();

                _sInstance._gSoundMgr.Init();

                DontDestroyOnLoad(go);
            }
        }

        public void ResetManagers()
        {
            _rhythmMgr.Clear();

            _attackMgr.Clear();
            _goPoolMgr.Clear();
            _gSoundMgr.Clear();
            _uiMgr.Clear();
            _resourceMgr.Clear();
        }
    }
}