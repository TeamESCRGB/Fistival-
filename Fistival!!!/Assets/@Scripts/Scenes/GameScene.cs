using Coordinator;
using Defines;
using Manager;
using UI;
using UI.Popup;
using UnityEngine;

namespace Scenes
{
    public class GameScene : SceneBase
    {
        public override SceneType NowSceneType => SceneType.GameScene;
        private PlayerCoordinator _player;
        private Transform _firstChunkPos;
        private PlayerHUD _hud;
        protected override void Init()
        {
            base.Init();
            _firstChunkPos = GameObject.Find("@FirstChunkSpawnPoint").transform;
            _player = FindAnyObjectByType<PlayerCoordinator>();
            Managers.Instance.NewInputSystemManager.SwitchActionMap(ActionMapTypes.PLAYER);
            _hud = FindAnyObjectByType<PlayerHUD>();
            _hud.gameObject.SetActive(false);
            Debug.Log($"{name} init complete");
        }

        private void Start()
        {
            _player.Init();
            var stageData = Managers.Instance.StageManager.GetStageData();
            _player.GetComponentInChildren<HPCoordinator>().SubscribeOnDead(Managers.Instance.StageManager.OnDead);
            Managers.Instance.StageManager.TrySpawnChunk(stageData.FirstStageSectionInstanceName, stageData.FirstStageLoadedDatasName, _firstChunkPos.position);
            Managers.Instance.StageManager.StartStage(_player.GetPlayerData().MaxLife);
            _hud.gameObject.SetActive(true);
            Managers.Instance.UIManager.ShowPopupUI<LifeCountPopup>("LifeCountPopup").SetData(_player.GetPlayerData().MaxLife);
        }
    }
}