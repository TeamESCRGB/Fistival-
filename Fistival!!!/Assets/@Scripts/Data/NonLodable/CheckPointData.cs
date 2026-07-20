using UnityEngine;

namespace Data.NonLodable
{
    public struct CheckPointSaveData
    {
        public Vector3 Pos { get; set; }
        public bool CameraFollowState { get; set; }
        public Vector2 CameraFollowDeadZoneWidth { get; set; }
        public Vector2 CameraFollowDeadZoneHeight { get; set; }
    }
}