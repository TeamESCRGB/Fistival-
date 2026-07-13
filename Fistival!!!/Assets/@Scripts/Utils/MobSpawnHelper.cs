using Coordinator;
using Manager;
using UnityEngine;

namespace Utils
{
    public static class MobSpawnHelper
    {
        public static MobCoordinatorBase SpawnBasicMob(int idx, Transform point)
        {
            if (Managers.Instance.DataManager.CommonMobDataDict.TryGetValue(idx, out var data) == false)
            {
                return null;
            }

            var go = Managers.Instance.ResourceManager.Instantiate(data.PrefabKey);
            if (go == null)
            {
                return null;
            }

            if (go.TryGetComponent<MobCoordinatorBase>(out var comp))
            {
                go.transform.SetPositionAndRotation(point.transform.position, point.transform.rotation);
                comp.Init(data);
                return comp;
            }
            else
            {
                Managers.Instance.ResourceManager.Destroy(go);
                return null;
            }
        }
    }
}
