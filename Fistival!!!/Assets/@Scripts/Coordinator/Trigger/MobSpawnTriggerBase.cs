using Assets._Scripts.Coordinator;
using UnityEngine;

namespace Coordinator.Trigger
{
    /// <summary>
    /// MobIdxVarType : DataManager에서 가져오거나, 직접 인스펙터에 할당해줄 스폰할 몹의 데이터 타입. 가능하다면 프리펩 말고, DataManager에서 가져오는 형태로 짤 것. List까지는 될듯
    /// </summary>
    /// <typeparam name="MobIdxVarType"></typeparam>
    public abstract class MobSpawnTriggerBase<MobIdxVarType> : MonoBehaviour, BasicInitializer
    {
        [SerializeField]
        protected MobIdxVarType _mobDataIdx;
        [SerializeField]
        protected Transform _mobSpawnPoint;

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
            var trigger = GetComponent<TouchTrigger>();
            trigger.OnTriggerActivated -= OnTrigger;
            trigger.OnTriggerActivated += OnTrigger;
        }

        protected abstract void OnTrigger(GameObject triggeredObject);
        public abstract void Init();
    }
}