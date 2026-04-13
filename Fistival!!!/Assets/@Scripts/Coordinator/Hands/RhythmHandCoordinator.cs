using Coordinator.Objects;
using Coordinator.Rhythm;
using Defines;
using Manager;
using Unity.Mathematics;
using UnityEngine;
using static Utils.VectorUtils;

namespace Coordinator.Hands
{
    public class RhythmHandCoordinator : HandCoordinator, IParrableObject
    {
        private const JudgementTypes _missMask = JudgementTypes.EARLY_MISS | JudgementTypes.LATE_MISS;
        private int _parriedIdx = -1;
        private int _endIdx = -1;
        private JudgementTypes _judgeType;
        private NoteTypes _noteType;
        private Transform _parryAttackBox;

        private int _parryReflectionDamage;

        protected override void OnAwake()
        {
            base.OnAwake();
            _parryAttackBox = transform.Find("@ParryAttackBox");
#if UNITY_EDITOR
            if (_parryAttackBox == null)
            {
                Debug.LogError($"@ParryAttackBox 가 {gameObject.name}의 자식중에 없습니다.");
            }
#endif
        }

        public override void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter)
        {
            base.Init(parentRb2d, baseSmashDamage, attackableFilter);
            _parriedIdx = -1;
            _endIdx = -1;
            _judgeType = _missMask;
            _noteType = NoteTypes.NO_ACTION;
            _parryReflectionDamage = 0;
        }

        public override void Attack()
        {
            throw new System.NotImplementedException();
        }

        public void OnLMBPressed()
        {
            throw new System.NotImplementedException();
        }

        public void OnLMBReleased()
        {
            throw new System.NotImplementedException();
        }

        public void AddParryDamage(int calculatedDamage)
        {
            _parryReflectionDamage += calculatedDamage;
        }
    }
}