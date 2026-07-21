using Coordinator.Trigger;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class StageLoadTrigger : MonoBehaviour
    {
        [SerializeField]
        protected LayerMask _playerLayer;
        [SerializeField]
        protected string _chunkKey;
        [SerializeField]
        protected string _chunkResourceKey;
        [SerializeField]
        protected Transform _nextChunkSpawnPoint;
        protected bool _isLoaded;

        public Action<GameObject> OnStageLoadEnd;


        private void Start()
        {
            var trigger = GetComponent<TouchTrigger>();
            trigger.OnTriggerActivated -= LoadMap;
            trigger.OnTriggerActivated += LoadMap;
        }


        protected virtual void LoadMap(GameObject go)
        {
            if(_isLoaded)
            {
                return;
            }
            if ((1 << go.layer) != _playerLayer)
            {
                return;
            }

            Managers.Instance.ResourceManager.LoadAsyncAllIn(_chunkResourceKey, (_, now, max) =>
            {
                if(now < max)
                {
                    return;
                }
                var chunk = Managers.Instance.StageManager.TrySpawnChunk(_chunkKey, _chunkResourceKey, _nextChunkSpawnPoint.position);
                if (chunk == null)
                {
                    return;
                }
                OnStageLoadEnd?.Invoke(go);
                _isLoaded = true;
            });
        }
    }
}