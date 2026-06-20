using Coordinator;
using Defines;
using Manager;
using UnityEngine;

namespace Scenes
{
    public class GameScene : SceneBase
    {
        public override SceneType NowSceneType => SceneType.GameScene;
        private PlayerCoordinator _player;
        private Transform _firstChunkPos;
        protected override void Init()
        {
            base.Init();
            _firstChunkPos = GameObject.Find("@FirstChunkSpawnPoint").transform;
            _player = FindAnyObjectByType<PlayerCoordinator>();
            Managers.Instance.NewInputSystemManager.SwitchActionMap(ActionMapTypes.PLAYER);
            Debug.Log($"{name} init complete");
        }

        private void Start()
        {
            _player.Init();
            var stageData = Managers.Instance.StageManager.GetStageData();
            _player.GetComponentInChildren<HPCoordinator>().SubscribeOnDead(Managers.Instance.StageManager.OnDead);
            Managers.Instance.StageManager.TrySpawnChunk(stageData.FirstStageSectionInstanceName, stageData.FirstStageLoadedDatasName, _firstChunkPos.position);
            Managers.Instance.StageManager.StartStage(_player.GetPlayerData().MaxLife);
        }
    }
}