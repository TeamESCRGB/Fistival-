using Coordinator.Objects;
using Coordinator.Rhythm;
using Coordinator.Victims;
using Defines;
using Manager;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UIElements;
using static TreeEditor.TreeEditorHelper;
using static Utils.VectorUtils;

namespace Coordinator.Hands
{
    public class RhythmHandCoordinator : HandCoordinator, IParrableObject
    {
        private const JudgementTypes _missMask = JudgementTypes.EARLY_MISS | JudgementTypes.LATE_MISS;
        private const NoteTypes _noActionMask = NoteTypes.SHORT_PARRY_RDY | NoteTypes.LONG_PARRY_RDY | NoteTypes.LONG_PARRY_MIDDLE | NoteTypes.LONG_PARRY_START | NoteTypes.NO_ACTION;
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
            /*
             판정에 따른 데미지 계수 곱하는거 빼면 로직은 기본가 똑같음
             */

            var enemy = Physics2D.OverlapBox(_attackBox.position, _attackBox.localScale, 0, _attackableMask);

            if (enemy == null || enemy.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            //BoxOverlap에 필터링에 걸린것만 가져와서 수행.
            //없으면 실행 안함
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

            switch(_judgeType)
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

        private void ReflectDamage()
        {
            /*
             마우스의 방향을 구한다
             _parryAttackBox의 크기와 길이만큼의 범위로 Boxcast를 날려서 범위에 닿은 오브젝트를 가져온다
             그 오브젝트들에 공격 요청 내린다. 데미지는 들어온 데미지만큼 반사한다.
            */

            var dir = GetDirVec2(_mainCam.ScreenToWorldPoint(_mousePos), _parentRb2d.transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    //Physics2D.BoxCast(transform.position,             new Vector2(0.1f, box.localScale.y),             angle, dir, box.localScale.x, 1<<1);
            var hit = Physics2D.BoxCast(_parentRb2d.transform.position, new Vector2(0.1f, _parryAttackBox.localScale.y), angle, dir, _parryAttackBox.localScale.x, _attackableMask);

#if UNITY_EDITOR
            __DEBUG__angle = angle;
            __DEBUG__dir = dir;
            __DEBUG__hit = hit;
#endif
            if (hit.collider == null || hit.collider.gameObject.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            Managers.Instance.AttackManager.RequestAttack(comp,_skillBase,_parryReflectionDamage);
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

            if((_noteType & _noActionMask) == 0 && (_judgeType & _missMask) == 0)
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


#if UNITY_EDITOR
        RaycastHit2D __DEBUG__hit;
        float __DEBUG__angle;
        Vector2 __DEBUG__dir;
        private void OnDrawGizmos()
        {
            // 1. 박스의 시작 지점 (현재 위치)
            Gizmos.color = Color.yellow;
            __DEBUG__DrawGizmoBox(transform.position, new Vector2(0.1f, _parryAttackBox.localScale.y), __DEBUG__angle);

            if (__DEBUG__hit.collider != null)
            {
                // 2. 충돌 시: 발사 경로를 선으로 표시
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, __DEBUG__hit.centroid);

                // 3. 충돌 지점에서의 박스 모습
                __DEBUG__DrawGizmoBox(__DEBUG__hit.centroid, new Vector2(0.1f, _parryAttackBox.localScale.y), __DEBUG__angle);

                // 4. 충돌 지점(Point)과 법선(Normal) 표시
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(__DEBUG__hit.point, 0.05f);
                Gizmos.DrawRay(__DEBUG__hit.point, __DEBUG__hit.normal * 0.5f);
            }
            else
            {
                // 충돌 안 했을 때: 최대 거리만큼 가상의 경로 표시
                Gizmos.color = Color.green;
                Vector2 endPos = (Vector2)transform.position + (__DEBUG__dir.normalized * _parryAttackBox.localScale.x);
                Gizmos.DrawLine(transform.position, endPos);
                __DEBUG__DrawGizmoBox(endPos, new Vector2(0.1f, _parryAttackBox.localScale.y), __DEBUG__angle);
            }
        }

        // 회전된 박스를 그리기 위한 보조 메서드
        private void __DEBUG__DrawGizmoBox(Vector2 center, Vector2 size, float angle)
        {
            Matrix4x4 savedMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = savedMatrix;
        }
#endif
    }
}