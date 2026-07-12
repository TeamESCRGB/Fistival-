using Coordinator.FallingObjectCoordinator;
using Manager;
using UnityEngine;

namespace Assets._Scripts.Utils
{
    public static class FallingObjectSpawnHelper
    {
        public static bool SpawnFallingObject(int idx, Vector3 point)
        {
            if(Managers.Instance.DataManager.FallingObjectDataDict.TryGetValue(idx, out var data) == false)
            {
                return false;
            }

            var go = Managers.Instance.ResourceManager.Instantiate(data.PrefabKey);
            if(go == null)
            {
                return false;
            }

            if(go.TryGetComponent<FallingObjectCoordinator>(out var comp))
            {
                go.transform.position = point;
                comp.Init(data);
                return true;
            }
            else
            {
                Managers.Instance.ResourceManager.Destroy(go);
                return false;
            }
        }
    }
}
