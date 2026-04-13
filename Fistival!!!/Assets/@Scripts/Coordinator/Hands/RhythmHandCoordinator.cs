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
            /*
             판정에 따른 데미지 계수 곱하는거 빼면 로직은 기본가 똑같음
             */
        }

        private void ReflectDamage()
        {
            throw new System.NotImplementedException();
            /*
             마우스의 방향을 구한다
             _parryAttackBox의 크기와 길이만큼의 범위로 Boxcast를 날려서 범위에 닿은 모든 오브젝트를 가져온다
             그 오브젝트들에 공격 요청 내린다. 데미지는 들어온 데미지만큼 반사한다.
            */
        }


        public void OnLMBPressed()
        {
            _parryReflectionDamage = 0;

            var parryResult = Managers.Instance.RhythmModeManager.ClickParry(this);
            _parriedIdx = parryResult.nowIdx;
            _endIdx = parryResult.endIdx;
            _judgeType = parryResult.judgeType;
            _noteType = parryResult.noteType;

            if(_noteType == NoteTypes.LONG_PARRY_START)
            {
                return;
            }

            if((_judgeType & _missMask) == 0)
            {
                ReflectDamage();
            }

            Attack();
        }

        public void OnLMBReleased()
        {
            var parryResult = Managers.Instance.RhythmModeManager.ReleaseParry(_endIdx, this);
            if((parryResult.judgeType & _missMask) != 0)
            {
                return;
            }
            ReflectDamage();
            Attack();
        }

        public void AddParryDamage(int calculatedDamage)
        {
            _parryReflectionDamage += calculatedDamage;
        }
    }
}