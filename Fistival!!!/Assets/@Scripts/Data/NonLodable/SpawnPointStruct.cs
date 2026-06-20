using UnityEngine;

namespace Data.NonLodable
{
    [System.Serializable]
    public struct SpawnPointStruct
    {
        public Transform SpawnPoint;
        public string PrefabName;
        public int DataIdx;
    }
}
