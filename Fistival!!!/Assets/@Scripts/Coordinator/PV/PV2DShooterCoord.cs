using Data;
using Data.NonLodable;
using DG.Tweening;
using Manager;
using UnityEngine;

namespace Coordinator
{
    public class PV2DShooterCoord : MonoBehaviour
    {

        [SerializeField]
        private SpawnPointStruct[] _mobSpawnPoints;

        [SerializeField]private int _nowMobCnt;


        private TCoord Spawn<TCoord, TInitData>(in SpawnPointStruct point, TInitData spawnData) where TCoord : UnityEngine.Object
        {
            var go = Managers.Instance.ResourceManager.Instantiate("", null, false, true);
            if (go == null)
            {
                return null;
            }

            if (go.TryGetComponent<TCoord>(out var coord) == false)
            {
                Managers.Instance.ResourceManager.Destroy(go);
                return null;
            }
            go.transform.SetPositionAndRotation(point.SpawnPoint.position, point.SpawnPoint.rotation);
            return coord;
        }

        private void OnMobDead()
        {
            _nowMobCnt--;

            if(_nowMobCnt <= 0 )
            {
                var point = _mobSpawnPoints[_mobSpawnPoints.Length - 1];
                var go = Managers.Instance.ResourceManager.Instantiate("", null, true, false);
                var pos = point.SpawnPoint.position;
                pos.z = -6;
                go.transform.position = pos;
                go.transform.DOMoveX(-9.25f, 5).SetRelative(true).SetDelay(3);
                Debug.Log($"{pos} ==================");
            }
        }

        public void Init()
        {

            
            for (int i = 0; i < _mobSpawnPoints.Length-1; i++)
            {
                _nowMobCnt++;
                if (Managers.Instance.DataManager.CommonMobDataDict.TryGetValue(_mobSpawnPoints[i].DataIdx, out var data) == false)
                {
                    continue;
                }
                var coord = Spawn<MobCoordinatorBase, CommonMobData>(_mobSpawnPoints[i], data);
                if (coord != null)
                {
                    coord.Init(data);
                    coord.GetComponentInChildren<HPCoordinator>().SubscribeOnDead(OnMobDead);
                }
            }
        }


    }
}