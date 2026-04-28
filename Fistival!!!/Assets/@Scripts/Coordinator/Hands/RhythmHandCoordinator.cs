using Coordinator.Rhythm;
using Coordinator.Victims;
using Defines;
using Manager;
using UnityEngine;
namespace Coordinator.Hands
{
    public class RhythmHandCoordinator : HandCoordinator, IParrableObject
    {
        [Header("RhythmHandCoordinator Field")]
        [SerializeField]
        private float _shortParryReflectRadius = 2;
        [SerializeField]
        private float _longParryReflectRadius = 4.5f;
        private const JudgementTypes _missMask = JudgementTypes.EARLY_MISS | JudgementTypes.LATE_MISS;
        private const NoteTypes _noActionMask = NoteTypes.SHORT_PARRY_RDY | NoteTypes.LONG_PARRY_RDY | NoteTypes.LONG_PARRY_MIDDLE | NoteTypes.LONG_PARRY_START | NoteTypes.NO_ACTION;
        private int _endIdx = -1;
        private JudgementTypes _judgeType;
        private NoteTypes _noteType;

        private int _parryReflectionDamage;

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        public override void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter)
        {
            base.Init(parentRb2d, baseSmashDamage, attackableFilter);
            _endIdx = -1;
            _judgeType = _missMask;
            _noteType = NoteTypes.NO_ACTION;
            _parryReflectionDamage = 0;
        }

        public override void Attack()
        {
            /*
             판정에 따른 데미지 계수 곱하는거 빼면 로직은 기본가 똑같음
             */
            var enemies = Physics2D.OverlapCircleAll(_attackBox.position, _attackBox.localScale.x / 2, _attackableMask);
            if(enemies is null)
            {
                return;
            }

            for(int i = 0;  i < enemies.Length; i++)
            {
                Collider2D enemy = enemies[i];
                if (enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
                {
                    return;
                }

                int totalDmg = _baseSmashDamage;
                if (_grabbedObject != null)
                {

                    totalDmg += _grabbedObject.GetSharedData().Damage;

                    if (_grabbedObject.Smash() == false)
                    {
                        _grabbedObject = null;
                        _chargeCnt = 0;
                        InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
                        InvokeOnGrabbedObjectChanged(null);
                        _status = HandStatus.IDLE;
                    }
                }

                float damageMultiplier = 1;

                switch (_judgeType)
                {
                    case JudgementTypes.PERFECT:
                        damageMultiplier = 2;
                        break;
                    case JudgementTypes.GOOD:
                        damageMultiplier = 1.5f;
                        break;
                }

                Managers.Instance.AttackManager.RequestAttack(comp, _skillBase, (int)(totalDmg * damageMultiplier));
            }
        }

        private void ReflectDamage(NoteTypes noteType)
        {
            if(_parryReflectionDamage <= 0)
            {
                return;
            }

            float attackboxRange = 1;
            switch (noteType)
            {
                case NoteTypes.SHORT_PARRY:
                    attackboxRange = _shortParryReflectRadius;
                    break;
                case NoteTypes.LONG_PARRY_END:
                    attackboxRange = _longParryReflectRadius;
                    break;
            }

            var hit = Physics2D.OverlapCircleAll(_attackBox.position, _attackBox.localScale.x * attackboxRange / 2, _attackableMask);

            if(hit == null)
            {
                return;
            }

            for(int i = 0; i < hit.Length; i++)
            {
                Collider2D enemy = hit[i];
                if (enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
                {
                    return;
                }

                Managers.Instance.AttackManager.RequestAttack(comp, _skillBase, _parryReflectionDamage);
            }
        }


        public override void OnLMBPressed()
        {
            _parryReflectionDamage = 0;

            var parryResult = Managers.Instance.RhythmModeManager.ClickParry(this);
            _endIdx = parryResult.endIdx;
            _judgeType = parryResult.judgeType;
            _noteType = parryResult.noteType;

            if(_noteType == NoteTypes.LONG_PARRY_START)
            {
                return;
            }

            if((_noteType & _noActionMask) == 0 && (_judgeType & _missMask) == 0)
            {
                ReflectDamage(_noteType);
            }

            Attack();
        }

        public override void OnLMBReleased()
        {
            var parryResult = Managers.Instance.RhythmModeManager.ReleaseParry(_endIdx, this);
            _parryReflectionDamage = 0;
            if((parryResult.judgeType & _missMask) != 0)
            {
                return;
            }
            ReflectDamage(parryResult.noteType);
            Attack();
        }

        public void AddParryDamage(int calculatedDamage)
        {
            _parryReflectionDamage += calculatedDamage;
        }
    }
}