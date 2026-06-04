using Coordinator.Movements;
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
        private int _strongDamageMultiplier = 2;
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
        private Hadouken _hadouken;
        private Syouryuuken _syouryuuken;
        private Tatsumakisenpukyaku _tatsumakisenpukyaku;

        private Action<int, int> _onThrownObjectAttacked;//attackCnt, chargerate
        private Action<int, int> _onFistAttacked;
        protected override void OnAwake()
        {
            base.OnAwake();
            _onThrownObjectAttacked = OnThrownObjectAttacked;
            var go = transform.Find("@FistSkill");
            _normalSkill = go.GetComponent<FistSkill>();
            _hadouken = _handAnchor.Find("@Hadouken").GetComponent<Hadouken>();
            _syouryuuken = transform.Find("@Syouryuuken").GetComponent<Syouryuuken>();
            _tatsumakisenpukyaku = transform.Find("@Tatsumakisenpukyaku").GetComponent<Tatsumakisenpukyaku>();
#if UNITY_EDITOR
            Debug.Assert(_normalSkill != null, $"@FistSkill이 없거나 여기에 FistSkill이 없습니다.");
            Debug.Assert(_hadouken != null, $"@HandAnchor의 자식에 @Hadouken이 없거나 여기에 Hadouken이 없습니다.");
            Debug.Assert(_syouryuuken != null, $"@Syouryuuken이 없거나 여기에 Syouryuuken이 없습니다.");
            Debug.Assert(_tatsumakisenpukyaku != null, $"@Tatsumakisenpukyaku이 없거나 여기에 Tatsumakisenpukyaku가 없습니다.");
#endif
            _onFistAttacked = (_, dmg) => { AddEnergy(dmg); };
            
            if (_hadouken != null)
            {
                _hadouken.OnAttackEnd += OnAttackSuccess;
            }
            if (_syouryuuken != null)
            {
                _syouryuuken.OnAttackEnd += OnAttackSuccess;
            }
            if(_tatsumakisenpukyaku != null)
            {
                _tatsumakisenpukyaku.OnAttackEnd += OnAttackSuccess;
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
            _hadouken.Init(attackableFilter, -1);
            var attackable = transform.parent.parent.parent.Find("@Hitbox").GetComponent<IAttackable>();
            _syouryuuken.Init(attackableFilter, baseSmashDamage * _strongDamageMultiplier, GetComponentInParent<IPushable>(), attackable);
            _tatsumakisenpukyaku.Init(attackableFilter,baseSmashDamage, parentRb2d, attackable);
            if (_normalSkill != null)
            {
                _normalSkill.RegisterOnAttack(_onFistAttacked);
            }
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

            if(comboType == WWESkillTypes.HADOUKEN && _hadouken.GetDemendedCost() > _energy)
            {
                return;
            }
            else if(comboType == WWESkillTypes.SYOURYUUKEN && _syouryuuken.GetDemendedCost() > _energy)
            {
                return;
            }
            else if(comboType == WWESkillTypes.TATSUMAKISENPUKYAKU && _tatsumakisenpukyaku.GetDemendedCost() > _energy)
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
            필요 코스트보다 지금 코스트가 낮을 때<-이거는 나중에 스킬에 필요 코스트값 채우도록 한다
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
            _grabbedObject.RegisterOnAttack(_onThrownObjectAttacked);
            base.Throw();
            if (_attackStatus == AttackStatus.WEAPON)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
            }
        }

        private void Attack()
        {
            if(Physics2D.OverlapBoxAll(_normalSkill.transform.position, _normalSkill.transform.localScale, 0, _attackableMask).Length <= 0)
            {
                return;
            }
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

        private void OnThrownObjectAttacked(int attackCnt, int chargeRate)
        {
            if(attackCnt < 2)
            {
                AddEnergy(chargeRate);
            }
        }

        private void DoWWESkill()
        {
            switch(_skillType)
            {
                case WWESkillTypes.HADOUKEN:
                    _isSkillActing = true;
                    OnComboChanged?.Invoke(WWESkillTypes.ACTIVATION);
                    _skillType = WWESkillTypes.NORMAL;
                    _energy -= _hadouken.GetDemendedCost();
                    _hadouken.Attack(new Vector2(transform.forward.z,0));
                    return;
                case WWESkillTypes.SYOURYUUKEN:
                    _isSkillActing = true;
                    OnComboChanged?.Invoke(WWESkillTypes.ACTIVATION);
                    _skillType = WWESkillTypes.NORMAL;
                    _energy -= _syouryuuken.GetDemendedCost();
                    _syouryuuken.Attack(transform.forward.z < 0 ? -1 : 1);
                    break;
                case WWESkillTypes.TATSUMAKISENPUKYAKU:
                    _isSkillActing = true;
                    OnComboChanged?.Invoke(WWESkillTypes.ACTIVATION);
                    _skillType = WWESkillTypes.NORMAL;
                    _energy -= _tatsumakisenpukyaku.GetDemendedCost();
                    _tatsumakisenpukyaku.Attack();
                    break;
            }
        }

        public override void OnLMBPressed()
        {
            if (_isSkillActing || _cooldownModule.IsCooldownEnded() == false)
            {
                return;
            }

            if(CanUseWeapon())
            {
                _pressedTime = 0;
                _attackStatus = AttackStatus.WEAPON;
                _cooldownModule.StopCooldown();
                base.OnLMBPressed();
                return;
            }
            _attackStatus = AttackStatus.PRESSED;
            _pressedTime = Time.timeAsDouble;
            OnAttackStatusChanged?.Invoke(AttackStatus.PRESSED);
        }

        public override void OnLMBReleased()
        {
            if(_isSkillActing)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
                return;
            }

            if (CanUseWeapon())
            {
                if (_attackStatus == AttackStatus.WEAPON)
                {
                    _cooldownModule.StartCooldown();
                    _weapon.RegisterOnAttack(_onThrownObjectAttacked);
                }
                _attackStatus = AttackStatus.NO_PRESSED;
                _pressedTime = 0;
                base.OnLMBReleased();
                return;
            }

            if (_cooldownModule.IsCooldownEnded() == false || _attackStatus == AttackStatus.NO_PRESSED)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
                return;
            }

            if (_attackStatus == AttackStatus.STRONG_RDY && Time.timeAsDouble - _pressedTime >= _strongAttackThreshold)
            {
                _attackStatus = AttackStatus.STRONG;
            }

            if(_skillType != WWESkillTypes.NORMAL && _attackStatus == AttackStatus.STRONG)
            {
                DoWWESkill();
            }
            else
            {
                Attack();
            }
        }

        private void OnAttackSuccess()
        {
            _isSkillActing = false;//이거가 켜져있으면 공격,행동 이런거 못하게 해야함
            _attackStatus = AttackStatus.NO_PRESSED;
            OnAttackStatusChanged?.Invoke(AttackStatus.NO_PRESSED);
            _cooldownModule.StartCooldown();
        }
        public override void Drop()
        {
            base.Drop();
            if (_attackStatus == AttackStatus.WEAPON)
            {
                _attackStatus = AttackStatus.NO_PRESSED;
            }
        }
    }
}