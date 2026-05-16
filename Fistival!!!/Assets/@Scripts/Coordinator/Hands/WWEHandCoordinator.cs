using Coordinator.Skills;
using Coordinator.Victims;
using Defines;
using Manager;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Coordinator.Hands
{
    public class WWEHandCoordinator : HandCoordinatorBase
    {
        [SerializeField]
        private double _strongRdyThreshold = 0.5f;
        [SerializeField]
        private double _strongAttackThreshold = 1;
        [SerializeField] private AttackStatus _attackStatus = AttackStatus.NO_PRESSED;
        private double _pressedTime = 0;
        public Action<AttackStatus> OnAttackStatusChanged;
        private int _baseSmashDamage;

        [SerializeField]
        private float _comboThreshold=0.5f;
        private float _lastComboInput;
        [SerializeField]private WWESkillTypes _skillType;
        public event Action<WWESkillTypes> OnComboChanged;//기본이 아닌 같은 타입이 연속으로 들어오게 해서, 애니메이션 진행 상황을 다음으로 넘기는식으로 작동할 예정. 콤보 타이머는 거기서 이 클래스에서 직접 값을 뽑아간 다음, 거기서 자체 타이머 돌릴 예정. 아니면 타이머 콜백 만들거나

        [SerializeField]
        private int _maxEnergy;
        [SerializeField]private int _energy;

        private bool _isSkillActing = false;

        private FistSkill _normalSkill;//이거 나중에 리펙토링 하면서 SkillCoordinatorBase로 할 수 있으려나


        protected override void OnAwake()
        {
            base.OnAwake();
            var go = transform.Find("@FistSkill");
            _normalSkill = go?.GetComponent<FistSkill>();
#if UNITY_EDITOR
            Debug.Assert(_normalSkill != null, $"@FistSkill이 없거나 여기에 FistSkill이 없습니다.");
#endif
            if(_normalSkill != null )
            {
                _normalSkill.OnAttack += AddEnergy;
            }
        }

        protected override void OnUpdate()
        {
            //게임 일시정지 로직 나중에 추가
            base.OnUpdate();

            if (_attackStatus == AttackStatus.PRESSED)
            {
                if (Time.timeAsDouble - _pressedTime >= _strongRdyThreshold)
                {
                    _attackStatus = AttackStatus.STRONG_RDY;
                    OnAttackStatusChanged?.Invoke(AttackStatus.STRONG_RDY);
                }
            }

            if (_skillType != WWESkillTypes.NORMAL)
            {
                _lastComboInput -= Time.deltaTime;
                if (_lastComboInput <= 0)
                {
                    _skillType = WWESkillTypes.NORMAL;
                    OnComboChanged?.Invoke(WWESkillTypes.NORMAL);
                }
            }
        }

        public void Init(Rigidbody2D parentRb2d, int baseSmashDamage, LayerMask attackableFilter, LayerMask pickableObjectMask, float forcePerCharge, float chargeTimeInterval, float attackCooldwn)
        {
            InitCommonDatas(parentRb2d, attackableFilter, pickableObjectMask, forcePerCharge, chargeTimeInterval, attackCooldwn);
            _attackStatus = AttackStatus.NO_PRESSED;
            _skillType = WWESkillTypes.NORMAL;
            _pressedTime = 0;
            _lastComboInput = 0;
            _energy = 0;
            ResetEvents();
            _isSkillActing = false;
            _baseSmashDamage = baseSmashDamage;
            _normalSkill.Init(attackableFilter,baseSmashDamage);
        }

        public void AddEnergy(int amount)
        {
            _energy = math.clamp(_energy + amount, 0, _maxEnergy);
        }

        public void SetComboType(WWESkillTypes comboType)
        {
            if(_isSkillActing)
            {
                return;
            }
            _skillType = comboType;
            _lastComboInput = _comboThreshold;
            OnComboChanged?.Invoke(comboType);

            /*
             콤보 선택

            콤보가 바뀌면,
            지금 콤보 타입을 바꾼다,
            콤보 선택 시간을 초기화한다
            콜백 호출

            콤보가 바뀌면 안되는 상황(스턴걸린 상황은 위에서 거르니까 제외):
            이미 스킬 실행중인 상황
             */
        }

        public void StopAttack()
        {
            _lastComboInput = 0;
            _skillType = WWESkillTypes.NORMAL;
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
            OnComboChanged?.Invoke(WWESkillTypes.NORMAL);
        }

        protected override void Throw()
        {
            AddEnergy(_chargeCnt);
            base.Throw();
        }

        private void Attack()
        {
            int objDmg = 0;
            if (_grabbedObject != null)
            {
                objDmg = _grabbedObject.GetSharedData().Damage;
                if (_grabbedObject.Smash() == false)
                {
                    _grabbedObject = null;
                    _chargeCnt = 0;
                    InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
                    InvokeOnGrabbedObjectChanged(null);
                    _status = HandStatus.IDLE;
                }
            }
            
            _normalSkill.Attack(_attackStatus, objDmg);
            OnAttackSuccess();
        }

        private void DoWWESkill()
        {
            switch(_skillType)
            {
                case WWESkillTypes.WAVE:
                    _isSkillActing = true;
                    OnComboChanged?.Invoke(WWESkillTypes.WAVE);
                    _skillType = WWESkillTypes.NORMAL;
                    //스킬 실행 코드 추가
                    break;
                case WWESkillTypes.DRAGON:
                    _isSkillActing = true;
                    OnComboChanged?.Invoke(WWESkillTypes.DRAGON);
                    _skillType = WWESkillTypes.NORMAL;
                    //스킬 실행 코드 추가
                    break;
                case WWESkillTypes.TORNADO:
                    _isSkillActing = true;
                    OnComboChanged?.Invoke(WWESkillTypes.TORNADO);
                    _skillType = WWESkillTypes.NORMAL;
                    //스킬 실행 코드 추가
                    break;
            }
            OnAttackSuccess();//임시. 이거는 나중에 각 스킬에 end콜백 달아서 할거임
        }

        public override void OnLMBPressed()
        {
            if (_cooldownModule.IsCooldownEnded() == false)
            {
                return;
            }
            _attackStatus = AttackStatus.PRESSED;
            _pressedTime = Time.timeAsDouble;
            OnAttackStatusChanged?.Invoke(AttackStatus.PRESSED);
        }

        public override void OnLMBReleased()
        {
            if (_cooldownModule.IsCooldownEnded() == false || _attackStatus == AttackStatus.NO_PRESSED)
            {
                return;
            }

            if (_attackStatus == AttackStatus.STRONG_RDY && Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
            {
                _attackStatus = AttackStatus.STRONG;
            }

            if(_skillType == WWESkillTypes.NORMAL)
            {
                Attack();
            }
            else//이거 강공 한정으로만 해야되는데 안하는 버그 있음
            {
                DoWWESkill();
            }
        }

        private void OnAttackSuccess()
        {
            _isSkillActing = false;
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
            _cooldownModule.StartCooldown();
        }
    }
}
//(기본 공격 데미지*강공데미지 + 오브젝트 데미지)
/*


Update에서 하는건 콤보 체킹만 하고, 공격 땔 때 한번에 공격처리 하는거로 할까

지속기는 자체적인 업데이트를 가지고

일반기는 바로


*/
#if false
public virtual void __Attack()
{
    int totalDmg = _baseSmashDamage;

    if(_grabbedObject != null)
    {
        totalDmg += _grabbedObject.GetSharedData().Damage;
    }

    if (_grabbedObject.Smash() == false)
    {
        _grabbedObject = null;
        _chargeCnt = 0;
        InvokeOnChargeRateChanged(_chargeCnt, _maxChargeCnt);
        InvokeOnGrabbedObjectChanged(null);
        _status = HandStatus.IDLE;
    }
    윗부분까지가 호출부에서 처리할 일

    밑부분부터가 Skill쪽에서 처리할 일

    var enemies = Physics2D.OverlapBoxAll(_attackBox.position, _attackBox.localScale, 0, _attackableMask);

    if (enemies is null)
    {
        return;
    }

    for (int i = 0; i < enemies.Length; i++)
    {
        Collider2D enemy = enemies[i];
        if (enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
        {
            return;
        }

        if (_attackStatus == AttackStatus.STRONG)
        {
            totalDmg *= _strongDamageMultiplier;
        }
        Managers.Instance.AttackManager.RequestAttack(comp, _skillBase, totalDmg);
    }
}
#endif